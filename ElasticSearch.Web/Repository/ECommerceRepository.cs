using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using ElasticSearch.Web.Models;
using ElasticSearch.Web.ViewModel;

namespace ElasticSearch.Web.Repository
{
    /// <summary>
    /// The ECommerceRepository class provides functionality to perform eCommerce search operations.
    /// </summary>
    public class ECommerceRepository(ElasticsearchClient client)
    {
        /// <summary>
        /// Represents the name of the index in Elasticsearch for the eCommerce data.
        /// </summary>
        private const string indexName = "kibana_sample_data_ecommerce";

        /// <summary>
        /// Performs a search for ECommerce objects asynchronously based on the provided search criteria.
        /// </summary>
        /// <param name="searchViewModel">The search criteria.</param>
        /// <param name="page">The page number of the search results.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>
        /// A tuple containing the list of ECommerce objects that match the search criteria,
        /// the total count of matching items, and the number of page links generated.
        /// </returns>
        public async Task<(List<ECommerce> list, long count)> SearchAsync(ECommerceSearchViewModel? searchViewModel, int page, int pageSize)
        {
            List<Action<QueryDescriptor<ECommerce>>> listQuery = [];

            if (searchViewModel is null)
            {
                listQuery.Add((q) => q.MatchAll());
                return await CalculateResultSet(page, pageSize, listQuery);
            }

            if (!string.IsNullOrWhiteSpace(searchViewModel.Category))
            {
                listQuery.Add((q) => q.Match(m => m
                    .Field(f => f.Category)
                    .Query(searchViewModel.Category)));
            }

            if (!string.IsNullOrWhiteSpace(searchViewModel.CustomerFullName))
            {
                listQuery.Add((q) => q.Match(m => m
                    .Field(f => f.CustomerFullName)
                    .Query(searchViewModel.CustomerFullName)));
            }

            if (searchViewModel is
                {
                    OrderDateStart: not null
                })
            {
                listQuery.Add((q) => q
                    .Range(r => r
                        .DateRange(dr => dr
                            .Field(f => f.OrderDate)
                            .Gte(searchViewModel.OrderDateStart.Value))));
            }

            if (searchViewModel is
                {
                    OrderDateEnd: not null
                })
            {
                listQuery.Add((q) => q
                    .Range(r => r
                        .DateRange(dr => dr
                            .Field(f => f.OrderDate)
                            .Lte(searchViewModel.OrderDateEnd.Value))));
            }

            if (!string.IsNullOrWhiteSpace(searchViewModel.Gender))
            {
                listQuery.Add((q) => q.Term(m => m
                    .Field(f => f.Gender)
                    .Value(searchViewModel.Gender).CaseInsensitive()));
            }

            if (listQuery.Count == 0)
            {
                listQuery.Add((q) => q.MatchAll());
            }

            return await CalculateResultSet(page, pageSize, listQuery);
        }

        /// <summary>
        /// Calculates the result set based on the given page, page size, and list of query actions.
        /// </summary>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="listQuery">The list of query actions.</param>
        /// <returns>
        /// A tuple containing the list of ECommerce objects and the total count of documents.
        /// </returns>
        private async Task<(List<ECommerce> list, long count)> CalculateResultSet(int page, int pageSize, List<Action<QueryDescriptor<ECommerce>>> listQuery)
        {
            var pageFrom = (page - 1) * pageSize;

            var result = await client.SearchAsync<ECommerce>(s => s.Index(indexName)
                .Size(pageSize)
                .From(pageFrom)
                .Query(q => q
                    .Bool(b => b
                        .Must(listQuery.ToArray()))));

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

            return (list: result.Documents.ToList(), count: result.Total);
        }
    }
}