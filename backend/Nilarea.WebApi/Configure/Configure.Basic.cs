using NilArea.Common;
using OpenSearch.Client;
using OpenSearch.Net;
using Orleans.Configuration;
using Orleans.Dashboard;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace NilArea.Web.Configure;

internal static partial class Configure
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureNilareaOrleans()
        {
            builder.UseOrleansClient(clientBuilder =>
            {
                clientBuilder
                    .AddDashboard();
#if DEBUG
                clientBuilder
                    .Configure<ClusterOptions>(options =>
                    {
                        var configuration = builder.Configuration;
                        options.ClusterId = configuration.SafeGetConfigureValue("ClusterOptions:ClusterId");
                        options.ServiceId = configuration.SafeGetConfigureValue("ClusterOptions:ServiceId");
                    })
                    .UseLocalhostClustering();
#endif
            });
            return builder;
        }

        public IHostApplicationBuilder ConfigureRedis()
        {
            builder.Services
                .AddStackExchangeRedisExtensions<NewtonsoftSerializer>(sp =>
                {
                    var configuration = builder.Configuration;
                    var conf = new RedisConfiguration
                    {
                        ConnectionString = configuration.SafeGetConnectionString("REDIS_CLUSTER")
                    };
                    return [conf];
                });
            return builder;
        }

        public IHostApplicationBuilder ConfigureOpenSearch()
        {
            var config = builder.Configuration.GetSection("OpenSearch");
            var singleNodePool = new SingleNodeConnectionPool(new Uri(config.SafeGetConfigureValue("Uri")));
            var connectionSettings = new ConnectionSettings(singleNodePool)
                .BasicAuthentication(config.SafeGetConfigureValue("User"), config.SafeGetConfigureValue("Password"))
                .SniffOnStartup(false)
                .SniffOnConnectionFault(false)
                .SniffLifeSpan(null);
            if (builder.Environment.IsDevelopment())
                connectionSettings.ServerCertificateValidationCallback((sender, cert, chain, errors) => true);
            builder.Services.AddSingleton<IOpenSearchClient>(new OpenSearchClient(connectionSettings));
            return builder;
        }
    }
}
