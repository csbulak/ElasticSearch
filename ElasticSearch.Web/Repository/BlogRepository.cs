using Elastic.Clients.Elasticsearch;
using ElasticSearch.Web.Models;

namespace ElasticSearch.Web.Repository
{
    public class BlogRepository(ElasticsearchClient client)
    {
        /// <summary>
        /// The name of the Elasticsearch index used by the ECommerceRepository class.
        /// </summary>
        private const string indexName = "blog";

        public async Task<Blog?> SaveAsync(Blog blog)
        {
            blog.Created = DateTime.Now;
            var response = await client.IndexAsync<Blog>(blog, idx => idx.Index(indexName));

            if (!response.IsValidResponse)
            {
                return null;
            }

            blog.Id = response.Id;
            return blog;
        }
    }
}