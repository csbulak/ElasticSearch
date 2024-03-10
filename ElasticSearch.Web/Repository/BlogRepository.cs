using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
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

        public async Task<List<Blog>> SearchAsync(string searchText)
        {
            List<Action<QueryDescriptor<Blog>>> listQuery = [];

            Action<QueryDescriptor<Blog>> matchAll = (q) => q.MatchAll();
            Action<QueryDescriptor<Blog>> matchContent = (q) => q.Match(m => m
                .Field(f => f.Content)
                .Query(searchText));
            Action<QueryDescriptor<Blog>> matchBoolPrefixTitle = (q) => q.MatchBoolPrefix(m => m
                .Field(f => f.Content)
                .Query(searchText));

            if (string.IsNullOrWhiteSpace(searchText))
            {
                listQuery.Add(matchAll);
            }
            else
            {
                listQuery.Add(matchContent);
                listQuery.Add(matchBoolPrefixTitle);
            }

            var result = await client.SearchAsync<Blog>(s => s.Index(indexName)
                .Query(q => q
                    .Bool(b => b
                        .Should(listQuery.ToArray()))));

            if (!result.IsValidResponse)
            {
                throw new Exception("Error occurred while executing the search query.");
            }

            if (result.Documents == null)
            {
                throw new Exception("No documents found.");
            }

            foreach (var hit in result.Hits)
            {
                if (hit.Source != null) hit.Source.Id = hit.Id;
            }

            return result.Documents.ToList();
        }
    }
}