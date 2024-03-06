using Elasticsearch.Net;
using Nest;

namespace ElasticSearch.API.Extensions;

public static class ElasticsearchExt
{
    public static void AddElastic(this IServiceCollection services, IConfiguration configuration)
    {
        var pool = new SingleNodeConnectionPool(new Uri(configuration.GetSection("Elastic")["Url"] ??
                                                        string.Empty));

        var settings = new ConnectionSettings(pool);
        var client = new ElasticClient(settings);
        services.AddSingleton<IElasticClient>(client);
    }
}