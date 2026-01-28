using Microsoft.Extensions.Hosting;
using NilArea.Grains.Services;

namespace NilArea.Grains;

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
    }
}
