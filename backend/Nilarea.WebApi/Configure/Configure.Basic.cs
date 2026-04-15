using NilArea.Common;
using NilArea.Contracts.Annotation;
using OpenSearch.Client;
using OpenSearch.Net;
using Orleans.Configuration;
using Orleans.Dashboard;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace NilArea.Web.Configure;

internal static partial class Configure
{
    extension(IHostApplicationBuilder builder)
    {
        [RequireEnvironmentVariable("CLUSTER_ID", DefaultValue = "nilarea")]
        [RequireEnvironmentVariable("SERVICE_ID", DefaultValue = "default")]
        public IHostApplicationBuilder ConfigureNilareaOrleans()
        {
            builder.UseOrleansClient(clientBuilder =>
            {
                clientBuilder
                    .ConfigureStorage()
                    .AddDashboard();
#if DEBUG
                clientBuilder
                    .Configure<ClusterOptions>(options =>
                    {
                        var configuration = builder.Configuration;
                        options.ClusterId = configuration.SafeGetConfigureValue("CLUSTER_ID");
                        options.ServiceId = configuration.SafeGetConfigureValue("SERVICE_ID");
                    });
#endif
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

    extension(IClientBuilder clientBuilder)
    {
        [RequireEnvironmentVariable("REDIS_CLUSTER")]
        [EnvironmentVariableNameFormat(Suffix = "_FILE")]
        public IClientBuilder ConfigureStorage()
        {
            var connectionString = clientBuilder.Configuration.GetSecretFromFile("REDIS_CLUSTER");
            clientBuilder.Services
                .AddStackExchangeRedisExtensions<NewtonsoftSerializer>(sp =>
                {
                    var conf = new RedisConfiguration
                    {
                        ConnectionString = connectionString
                    };
                    return [conf];
                });
#if DEBUG
            clientBuilder.UseRedisClustering(options =>
            {
                options.ConfigurationOptions = ConfigurationOptions.Parse(connectionString);
            });
#endif
            return clientBuilder;
        }
    }
}