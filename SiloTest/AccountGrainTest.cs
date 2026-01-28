using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NilArea.Interfaces.IGrains;
using Orleans.Configuration;

namespace SiloTest;

public class AccountGrainTest : IAsyncLifetime
{
    private IClusterClient _clusterClient = null!;
    private IHost _host = null!;

    public async Task InitializeAsync()
    {
        _host = Host.CreateDefaultBuilder()
            .UseOrleansClient(clientBuilder =>
            {
                clientBuilder
                    .Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "nilarea-cluster";
                        options.ServiceId = "nilarea-service";
                    })
                    .UseLocalhostClustering();
            }).Build();
        await _host.StartAsync();
        _clusterClient = (_host.Services.GetRequiredService(typeof(IClusterClient)) as IClusterClient)!;
    }

    public async Task DisposeAsync()
    {
        await _host.StopAsync();
    }

    [Fact]
    public async Task TestExist1()
    {
        const int count = 100000;
        var ag = _clusterClient.GetGrain<IAccountGrain>(Guid.Empty);
        foreach (var i in Enumerable.Range(0, count)) await ag.ExistAccountAsync($"test{i}@silo.test");
    }

    [Fact]
    public async Task TestExist2()
    {
        const int count = 100000;
        var ag = _clusterClient.GetGrain<IAccountGrain>(Guid.Empty);
        await Task.WhenAll(Enumerable.Range(0, count).Select(async i =>
        {
            await ag.ExistAccountAsync($"test{i}@silo.test");
        }).ToArray());
    }
}
