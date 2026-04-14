using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NilArea.Common.Services;

namespace NilArea.Account;

public class ServiceInitializer(
    IServiceProvider serviceProvider
) : IHostedService
{
    private List<IAsyncLifetime> Services { get; } = serviceProvider.GetServices<IAsyncLifetime>().ToList();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Task.WhenAll(Services.Select(x => x.InitializeAsync()));
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.WhenAll(Services.Select(x => x.DisposeAsync()));
    }
}