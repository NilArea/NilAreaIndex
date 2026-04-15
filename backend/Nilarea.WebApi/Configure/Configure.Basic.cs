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
                clientBuilder
                    .Configure<ClusterOptions>(options =>
                    {
                        var configuration = builder.Configuration;
                        options.ClusterId = configuration.SafeGetConfigureValue("CLUSTER_ID");
                        options.ServiceId = configuration.SafeGetConfigureValue("SERVICE_ID");
                    });
            });
            return builder;
        }

        [RequireEnvironmentVariable("OPENSEARCH_CLUSTER")]
        [RequireEnvironmentVariable("OPENSEARCH_USER")]
        [RequireEnvironmentVariable("OPENSEARCH_PASSWORD")]
        [EnvironmentVariableNameFormat(Suffix = "_FILE")]
        public IHostApplicationBuilder ConfigureOpenSearch()
        {
            var singleNodePool =
                new SingleNodeConnectionPool(new Uri(builder.Configuration.GetSecretFromFile("OPENSEARCH_CLUSTER")));
            var connectionSettings = new ConnectionSettings(singleNodePool)
                .BasicAuthentication(builder.Configuration.GetSecretFromFile("OPENSEARCH_USER"),
                    builder.Configuration.GetSecretFromFile("OPENSEARCH_PASSWORD"))
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
            clientBuilder.UseRedisClustering(options =>
            {
                options.ConfigurationOptions = ConfigurationOptions.Parse(connectionString);
            });
            return clientBuilder;
        }
    }
}