using FluentValidation;
using Microsoft.Extensions.Logging;
using NilArea.Contracts.Dto;
using NilArea.Grains.Repositories;
using NilArea.Interfaces.Exceptions;
using NilArea.Interfaces.IGrains;
using Orleans.Concurrency;

namespace NilArea.Grains.ImplGrains;

[StatelessWorker]
public sealed class AccountGrain(
    ILogger<AccountGrain> logger,
    IAccountRepository accountRepository,
    IValidator<RegisterRequest> registerRequestValidator,
    IValidator<LoginRequest> loginRequestValidator
) : Grain, IAccountGrain
{
    public async ValueTask<bool> ExistEmailAsync(string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        return await accountRepository.ExistsAccountAsync(email);
    }

    public async ValueTask<RegisterResponse> RegisterUserAsync(RegisterRequest request)
    {
        var validate = await registerRequestValidator.ValidateAsync(request);
        if (!validate.IsValid)
            throw new AccountException(validate.ToString(), AccountAction.Register);
        if (await accountRepository.ExistsAccountAsync(request.Email))
            throw new AccountException("Email already registered", AccountAction.Register);
        return await accountRepository.InsertAccountAsync(request);
    }

    public async ValueTask<LoginResponse> LoginAsync(LoginRequest request)
    {
        var validate = await loginRequestValidator.ValidateAsync(request);
        if (!validate.IsValid)
            throw new AccountException(validate.ToString(), AccountAction.Login);
        if (!await accountRepository.ExistsAccountAsync(request.Email))
            throw new AccountException("Email is not registered", AccountAction.Login);
        return await accountRepository.VerifyLoginInfoAsync(request);
    }
}
