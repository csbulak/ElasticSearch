using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using ElasticSearch.Web.Models;
using ElasticSearch.Web.ViewModel;

namespace ElasticSearch.Web.Repository
{
    public class ECommerceRepository(ElasticsearchClient client)
    {
        private const string indexName = "kibana_sample_data_ecommerce";

        public async Task<(List<ECommerce> list, long count)> SearchAsync(ECommerceSearchViewModel searchViewModel, int page, int pageSize)
        {
            List<Action<QueryDescriptor<ECommerce>>> listQuery = [];

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

            if (searchViewModel.OrderDateStart.HasValue)
            {
                listQuery.Add((q) => q
                    .Range(r => r
                        .DateRange(dr => dr
                            .Field(f => f.OrderDate)
                            .Gte(searchViewModel.OrderDateStart.Value))));
            }

            if (searchViewModel.OrderDateEnd.HasValue)
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
                    .Value(searchViewModel.Gender)));
            }

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