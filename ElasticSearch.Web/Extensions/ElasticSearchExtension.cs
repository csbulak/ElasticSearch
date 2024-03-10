using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

namespace ElasticSearch.Web.Extensions
{
    public static class ElasticSearchExtension
    {
        public static void AddElastic(this IServiceCollection services, IConfiguration configuration)
        {
            var username = configuration.GetSection("Elastic")["Username"] ?? string.Empty;
            var password = configuration.GetSection("Elastic")["Password"] ?? string.Empty;
            var settings = new ElasticsearchClientSettings(new Uri(configuration.GetSection("Elastic")["Url"] ??
                                                                   string.Empty))
                .Authentication(new BasicAuthentication(username, password));

            var client = new ElasticsearchClient(settings);
            services.AddSingleton(client);
        }
    }
}