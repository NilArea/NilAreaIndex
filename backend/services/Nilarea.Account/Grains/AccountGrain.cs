using FluentValidation;
using Microsoft.Extensions.Logging;
using NilArea.Account.Infrastructure.Repositories;
using NilArea.Account.Infrastructure.Services;
using NilArea.Common.Utils;
using NilArea.Contracts.Commands.Account;
using NilArea.Contracts.Enums;
using NilArea.Contracts.Exceptions;
using NilArea.Contracts.Grains.Account;
using NilArea.Contracts.Responses.Account;
using Orleans.Concurrency;

namespace NilArea.Account.Grains;

[Reentrant]
[StatelessWorker]
public class AccountGrain(
    ILogger<AccountGrain> logger,
    IAccountRepository accountRepository,
    IConfirmRepository confirmRepository,
    IEmailServices emailServices,
    IValidator<RegisterAccountCommand> registerRequestValidator,
    IValidator<ChangePasswordCommand> changePasswordValidator,
    IValidator<DeleteAccountCommand> deleteAccountValidator,
    IValidator<ChangeAccountEmailCommand> changeAccountEmailValidator,
    IValidator<ResetPasswordCommand> resetPasswordValidator,
    IPasswordHasher passwordHasher
) : Grain, IAccountGrain
{
    private static readonly InlineValidator<string> EmailValidator;

    static AccountGrain()
    {
        var validator = new InlineValidator<string>();
        validator.RuleFor(x => x)
            .NotEmpty()
            .EmailAddress();
        EmailValidator = validator;
    }

    public async ValueTask<bool> ExistAccountAsync(string email)
    {
        var validate = await EmailValidator.ValidateAsync(email);
        if (!validate.IsValid)
        {
            logger.LogWarning("Email validation failed: {Email}, errors: {Errors}", email, validate.ToString());
            throw new AccountException(validate.ToString());
        }

        return await accountRepository.ExistsAccountAsync(email);
    }

    public async ValueTask CallConfirmKeyAsync(string email, ConfirmType typeCode = ConfirmType.Default)
    {
        var validate = await EmailValidator.ValidateAsync(email);
        if (!validate.IsValid)
        {
            logger.LogWarning("Email validation failed: {Email}, errors: {Errors}", email, validate.ToString());
            throw new AccountException(validate.ToString());
        }
        logger.LogDebug("Sending confirm key to email: {Email}, type: {TypeCode}", email, typeCode);
        // 添加发送频率限制
        var rateLimitKey = $"rate_limit:confirm_key:{email}";
        if (await confirmRepository.ExistConfirmCodeAsync(rateLimitKey))
        {
            logger.LogWarning("Rate limit exceeded for email: {Email}", email);
            throw new AccountException("Please wait before requesting another verification code");
        }

        var confirmCode = Random.Shared.GetHexString(6);
        if (!await confirmRepository.CacheConfirmCodeAsync(email, confirmCode, typeCode))
        {
            logger.LogError("Failed to cache confirm code for email: {Email}", email);
            throw new AccountException("Failed to cache confirm code");
        }

        if (!await emailServices.SendConfirmKeyAsync(email, confirmCode, typeCode))
        {
            logger.LogError("Failed to send confirm key to email: {Email}", email);
            await confirmRepository.CheckConfirmCodeAsync(email, confirmCode);
            throw new AccountException("Failed to send confirm key");
        }

        // 设置频率限制，1分钟内不能再次发送
        await confirmRepository.CacheConfirmCodeAsync(rateLimitKey, "1", ConfirmType.Default);

        logger.LogDebug("Confirm key sent successfully to email: {Email}", email);
    }

    public async ValueTask<RegisterAccountResponse> RegisterUserAsync(RegisterAccountCommand command)
    {
        logger.LogDebug("Registering user with email: {Email}", command.Email);

        var validate = await registerRequestValidator.ValidateAsync(command);
        if (!validate.IsValid)
        {
            logger.LogWarning("Registration validation failed for email: {Email}, errors: {Errors}", command.Email,
                validate.ToString());
            throw new AccountException(validate.ToString(), AccountAction.Register);
        }

        if (await accountRepository.ExistsAccountAsync(command.Email))
        {
            logger.LogWarning("Email already registered: {Email}", command.Email);
            throw new AccountException("Email already registered", AccountAction.Register);
        }

        if (!await confirmRepository.CheckConfirmCodeAsync(command.Email, command.ConfirmKey))
        {
            logger.LogWarning("Invalid confirm code for email: {Email}", command.Email);
            throw new AccountException("Confirm Code is invalid", AccountAction.Register);
        }

        var add = await accountRepository.InsertAccountAsync(
            command.Email,
            passwordHasher.SaltedHash(command.Password),
            command.Username);

        logger.LogDebug("User registered successfully: {Email}, UserId: {UserId}", command.Email, add.UserId);

        return new RegisterAccountResponse
        {
            UserId = add.UserId,
            Email = add.Email,
            Username = add.UserName,
            CreatedAt = add.CreatedAt
        };
    }

    public async ValueTask DeleteAccountAsync(DeleteAccountCommand command)
    {
        logger.LogDebug("Deleting account with email: {Email}", command.Email);

        var validate = await deleteAccountValidator.ValidateAsync(command);
        if (!validate.IsValid)
        {
            logger.LogWarning("Delete account validation failed for email: {Email}, errors: {Errors}", command.Email,
                validate.ToString());
            throw new AccountException(validate.ToString());
        }

        // 验证当前密码
        var accountVerify = await confirmRepository.GetAccountVerifyAsync(command.Email);
        if (!passwordHasher.Verify(command.Password, accountVerify.PasswordSaltHash))
        {
            logger.LogWarning("Incorrect password for email: {Email}", command.Email);
            throw new AccountException("Current password is incorrect");
        }

        // 验证确认码
        if (!await confirmRepository.CheckConfirmCodeAsync(command.Email, command.ConfirmKey))
        {
            logger.LogWarning("Invalid confirm code for email: {Email}", command.Email);
            throw new AccountException("Confirm Code is invalid");
        }

        // 删除账号
        await accountRepository.DeleteAccountAsync(accountVerify.UserId);

        logger.LogDebug("Account deleted successfully: {Email}, UserId: {UserId}", command.Email, accountVerify.UserId);
    }

    public async ValueTask ChangePasswordAsync(ChangePasswordCommand command)
    {
        logger.LogDebug("Changing password for email: {Email}", command.Email);

        var validate = await changePasswordValidator.ValidateAsync(command);
        if (!validate.IsValid)
        {
            logger.LogWarning("Change password validation failed for email: {Email}, errors: {Errors}", command.Email,
                validate.ToString());
            throw new AccountException(validate.ToString());
        }

        // 验证当前密码
        var accountVerify = await confirmRepository.GetAccountVerifyAsync(command.Email);
        if (!passwordHasher.Verify(command.Password, accountVerify.PasswordSaltHash))
        {
            logger.LogWarning("Incorrect password for email: {Email}", command.Email);
            throw new AccountException("Current password is incorrect");
        }

        // 验证确认码
        if (!await confirmRepository.CheckConfirmCodeAsync(command.Email, command.ConfirmKey))
        {
            logger.LogWarning("Invalid confirm code for email: {Email}", command.Email);
            throw new AccountException("Confirm Code is invalid");
        }

        // 修改密码
        await accountRepository.ChangePasswordAsync(accountVerify.UserId,
            passwordHasher.SaltedHash(command.NewPassword));

        logger.LogDebug("Password changed successfully for email: {Email}, UserId: {UserId}", command.Email,
            accountVerify.UserId);
    }

    public async ValueTask<AccountInfoResponse> GetAccountInfoAsync(long userId)
    {
        logger.LogDebug("Getting account info for UserId: {UserId}", userId);

        var account = await accountRepository.GetAccountAsync(userId);
        if (account == null)
        {
            logger.LogWarning("Account not found for UserId: {UserId}", userId);
            throw new AccountException("Account not found");
        }

        logger.LogDebug("Account info retrieved successfully for UserId: {UserId}, Email: {Email}", userId,
            account.Email);

        return new AccountInfoResponse
        {
            UserId = account.UserId,
            Email = account.Email,
            Username = account.UserName,
            CreatedAt = account.CreatedAt,
            UpdatedAt = account.UpdatedAt
        };
    }

    public async ValueTask ChangeAccountEmailAsync(ChangeAccountEmailCommand command)
    {
        logger.LogDebug("Changing account email for user id: {UserId}", command.UserId);

        var validate = await changeAccountEmailValidator.ValidateAsync(command);
        if (!validate.IsValid)
        {
            logger.LogWarning("Change account email validation failed for user id: {UserId}, errors: {Errors}",
                command.UserId,
                validate.ToString());
            throw new AccountException(validate.ToString());
        }

        await accountRepository.ChangeEmailAsync(command.UserId, command.NewEmail);

        logger.LogDebug("Account email changed successfully for user id: {UserId}, new email: {NewEmail}",
            command.UserId, command.NewEmail);
    }

    public async ValueTask ResetPasswordAsync(ResetPasswordCommand command)
    {
        logger.LogDebug("Resetting password for email: {Email}", command.Email);

        var validate = await resetPasswordValidator.ValidateAsync(command);
        if (!validate.IsValid)
        {
            logger.LogWarning("Reset password validation failed for email: {Email}, errors: {Errors}", command.Email,
                validate.ToString());
            throw new AccountException(validate.ToString());
        }

        if (!await confirmRepository.CheckConfirmCodeAsync(command.Email, command.ConfirmKey))
        {
            logger.LogWarning("Invalid confirm code for email: {Email}", command.Email);
            throw new AccountException("Confirm Code is invalid");
        }

        var account = await accountRepository.GetAccountByEmailAsync(command.Email);
        if (account == null)
        {
            logger.LogWarning("Account not found for email: {Email}", command.Email);
            throw new AccountException("Account not found");
        }

        await accountRepository.ChangePasswordAsync(account.UserId, passwordHasher.SaltedHash(command.NewPassword));

        logger.LogDebug("Password reset successfully for email: {Email}, UserId: {UserId}", command.Email,
            account.UserId);
    }
}