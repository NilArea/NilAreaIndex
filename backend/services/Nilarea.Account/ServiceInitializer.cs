using Microsoft.Extensions.Hosting;
using NilArea.Account.Infrastructure.Repositories;
using NilArea.Account.Infrastructure.Services;

namespace NilArea.Account;

public class ServiceInitializer(
    IAccountRepository accountRepository,
    IEmailServices emailServices
) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await accountRepository.InitializeAsync();
        await emailServices.InitializeAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await emailServices.DisposeAsync();
        await accountRepository.DisposeAsync();
    }
}