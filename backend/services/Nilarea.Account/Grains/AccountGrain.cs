using FluentValidation;
using Microsoft.Extensions.Logging;
using NilArea.Account.Infrastructure.ExternalServices;
using NilArea.Account.Infrastructure.Repositories;
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
    AccountRepository accountRepository,
    IEmailServices emailServices,
    IValidator<RegisterAccountCommand> registerRequestValidator,
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
            throw new AccountException(validate.ToString());
        return await accountRepository.ExistsAccountAsync(email);
    }

    public async ValueTask CallConfirmKey(string email, ConfirmType typeCode = ConfirmType.Default)
    {
        var validate = await EmailValidator.ValidateAsync(email);
        if (!validate.IsValid) throw new AccountException(validate.ToString());
        var confirmCode = Random.Shared.GetHexString(6);
        if (!await accountRepository.CacheConfirmCodeAsync(email, confirmCode, typeCode))
            throw new AccountException("Failed to cache confirm code");
        if (!await emailServices.SendConfirmKeyAsync(email, confirmCode, typeCode))
        {
            await accountRepository.CheckConfirmCodeAsync(email, confirmCode);
            throw new AccountException("Failed to send confirm key");
        }
    }

    public async ValueTask<RegisterAccountResponse> RegisterUserAsync(RegisterAccountCommand command)
    {
        var validate = await registerRequestValidator.ValidateAsync(command);
        if (!validate.IsValid)
            throw new AccountException(validate.ToString(), AccountAction.Register);
        if (await accountRepository.ExistsAccountAsync(command.Email))
            throw new AccountException("Email already registered", AccountAction.Register);
        if (!await accountRepository.CheckConfirmCodeAsync(command.Email, command.ConfirmCode))
            throw new AccountException("Confirm Code is invalid", AccountAction.Register);
        var add = await accountRepository.InsertAccountAsync(
            command.Email,
            passwordHasher.SaltedHash(command.Password),
            command.Username);
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
        throw new NotImplementedException();
    }

    public async ValueTask ChangePassword(ChangePasswordCommand command)
    {
        throw new NotImplementedException();
    }
}