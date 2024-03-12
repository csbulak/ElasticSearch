using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using ElasticSearch.Web.Models;

namespace ElasticSearch.Web.Repository
{
    /// <summary>
    /// Represents a repository for managing Blog entities.
    /// </summary>
    public class BlogRepository(ElasticsearchClient client)
    {
        /// used by the `BlogRepository` class.
        private const string indexName = "blog";

        /// <summary>
        /// Saves a blog asynchronously.
        /// </summary>
        /// <param name="blog">The blog object to save.</param>
        /// <returns>A task representing the asynchronous save operation. The task will complete with a Blog object representing the saved blog if the save operation is successful, or null otherwise.</returns>
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

        /// <summary>
        /// Searches for blogs matching the specified search text.
        /// </summary>
        /// <param name="searchText">The text to search for.</param>
        /// <returns>A list of blogs that match the search text.</returns>
        public async Task<List<Blog>> SearchAsync(string searchText)
        {
            List<Action<QueryDescriptor<Blog>>> listQuery = [];

            if (string.IsNullOrWhiteSpace(searchText))
            {
                listQuery.Add(MatchAll);
            }
            else
            {
                listQuery.Add(MatchContent);
                listQuery.Add(MatchBoolPrefixTitle);
                listQuery.Add(TagTerm);
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

            void MatchAll(QueryDescriptor<Blog> q) => q.MatchAll();

            void MatchContent(QueryDescriptor<Blog> q) =>
                q.Match(m => m.Field(f => f.Content)
                    .Query(searchText));

            void MatchBoolPrefixTitle(QueryDescriptor<Blog> q) =>
                q.MatchBoolPrefix(m => m.Field(f => f.Content)
                    .Query(searchText));

            void TagTerm(QueryDescriptor<Blog> q) =>
                q.Term(m => m.Field(f => f.Tags)
                    .Value(searchText));
        }
    }
}