using FluentValidation;
using Microsoft.Extensions.Logging;
using NilArea.Contracts.Dto;
using NilArea.Grains.Repositories;
using NilArea.Interfaces.Exceptions;
using NilArea.Interfaces.IGrains;

namespace NilArea.Grains.ImplGrains;

public class AccountGrain(
    ILogger<AccountGrain> logger,
    IAccountRepository accountRepository,
    IValidator<RegisterRequest> registerRequestValidator
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
            throw new AccountException(validate.ToString());
        if (await accountRepository.ExistsAccountAsync(request.Email))
            throw new AccountException("Email already registered", AccountAction.Register);
        var (uid, ca) = await accountRepository.InsertAccount(request);
        return new RegisterResponse
        {
            Email = request.Email,
            Username = request.Username,
            CreatedAt = ca,
            UserId = uid
        };
    }
}
