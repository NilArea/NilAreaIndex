using FluentValidation;
using Microsoft.Extensions.Logging;
using NilArea.Common.Utils;
using NilArea.Contracts.Dto;
using Nilarea.Database.Abstract.Services;
using NilArea.Grains.Services;
using NilArea.Interfaces.Exceptions;
using NilArea.Interfaces.IGrains;
using Orleans.Concurrency;

namespace NilArea.Grains.ImplGrains;

[Reentrant]
[StatelessWorker]
public class AccountGrain(
    ILogger<AccountGrain> logger,
    IAccountRepository accountRepository,
    IEmailServices emailServices,
    IValidator<AccountRegisterRequest> registerRequestValidator,
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

    public async ValueTask CallConfirmKey(string email, ConfirmKey keyCode = ConfirmKey.Default)
    {
        var validate = await EmailValidator.ValidateAsync(email);
        if (!validate.IsValid)
            throw new AccountException(validate.ToString());
        var confirmKey = Random.Shared.GetHexString(6);
        await accountRepository.CacheConfirmKeyAsync(email, confirmKey, keyCode);
        await emailServices.SendConfirmKeyAsync(email, confirmKey, keyCode);
    }

    public async ValueTask<AccountRegisterResponse> RegisterUserAsync(AccountRegisterRequest request)
    {
        var validate = await registerRequestValidator.ValidateAsync(request);
        if (!validate.IsValid)
            throw new AccountException(validate.ToString(), AccountAction.Register);
        if (await accountRepository.ExistsAccountAsync(request.Email))
            throw new AccountException("Email already registered", AccountAction.Register);
        if (!await accountRepository.CheckConfirmKeyAsync(request.Email, request.ConfirmKey))
            throw new AccountException("Confirm key not valid", AccountAction.Register);
        var add = await accountRepository.InsertAccountAsync(
            request.Email,
            passwordHasher.SaltedHash(request.Password),
            request.Username);
        return new AccountRegisterResponse
        {
            UserId = add.UserId,
            Email = add.Email,
            Username = add.UserName,
            CreatedAt = add.CreatedAt
        };
    }

    public async ValueTask DeleteAccountAsync(DeleteAccountRequest request)
    {
        throw new NotImplementedException();
    }

    public async ValueTask ChangePasswd(ChangePasswdRequest request)
    {
        throw new NotImplementedException();
    }
}
