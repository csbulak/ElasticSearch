using System.Collections.Immutable;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using ElasticSearch.API.Models.ECommerceModel;

namespace ElasticSearch.API.Repository
{
    public class ECommerceRepository
    {
        private readonly ElasticsearchClient _client;
        private const string indexName = "kibana_sample_data_ecommerce";

        public ECommerceRepository(ElasticsearchClient client)
        {
            _client = client;
        }

        public async Task<ImmutableList<ECommerce>> TermQuery(string customerFirstName)
        {
            var result = await _client.SearchAsync<ECommerce>(x =>
                x.Index(indexName)
                    .Query(query =>
                        query.Term(t =>
                            t.Field("customer_first_name.keyword").Value(customerFirstName))));

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

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> TermsQuery(List<string> customerFirstNames)
        {
            List<FieldValue> terms = new List<FieldValue>();
            customerFirstNames.ForEach(x => { terms.Add(x); });
            // 1. yol
            // var termsQuery = new TermsQuery()
            // {
            //     Field = "customer_first_name.keyword",
            //     Terms = new TermsQueryField(terms.AsReadOnly())
            // };
            //
            // var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(termsQuery));

            // 2.yol
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
                .Size(100)
                .Query(q => q
                    .Terms(f => f
                        .Field("customer_first_name_keyword")
                        .Terms(new TermsQueryField(terms.AsReadOnly())))));

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

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> PrefixQuery(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
                .Query(q => q
                    .Prefix(p => p
                        .Field("customer_full_name.keyword")
                        .Value(customerFullName))));

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

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> RangeQuery(double? fromPrice, double? toPrice)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s
                .Index(indexName)
                .Query(q => q
                    .Range(r => r
                        .NumberRange(nr => nr
                            .Field(f => f.TaxFullTotalPrice)
                            .Gte(fromPrice)
                            .Lte(toPrice)))));
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

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MatchAllQuery()
        {
            var result = await _client.SearchAsync<ECommerce>(s => s
                .Index(indexName)
                .Size(100)
                .Query(q => q.MatchAll()));

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

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MatchAllPaginationQuery(int page, int pageSize)
        {
            if (page <= 0)
                page = 1;

            var pageFrom = (page - 1) * pageSize;

            var result = await _client.SearchAsync<ECommerce>(s => s
                .Index(indexName)
                .From(pageFrom)
                .Size(pageSize)
                .Query(q => q.MatchAll()));

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

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> WildCardQuery(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
                .Query(q => q
                    .Wildcard(w => w
                        .Field(f => f
                            .CustomerFullName.Suffix("keyword"))
                        .Wildcard(customerFullName))));

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

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> FuzzyQuery(string customerName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
                .Query(q => q
                    .Fuzzy(t => t
                        .Field(f => f.CustomerFirstName.Suffix("keyword"))
                        .Value(customerName)
                        .Fuzziness(new Fuzziness(1))))
                .Sort(sort => sort.Field(f => f.TaxFullTotalPrice, new FieldSort()
                {
                    Order = SortOrder.Desc
                })));

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

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MatchQueryFullText(string categoryName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s
                .Index(indexName)
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Category)
                        .Query(categoryName)))
                .Size(1000));

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

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MatchBoolPrefixQuery(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
                .Size(100)
                .Query(q => q
                    .MatchBoolPrefix(m => m
                        .Field(f => f.CustomerFullName)
                        .Query(customerFullName))));

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

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MatchPhraseQuery(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
                .Size(100)
                .Query(q => q
                    .MatchPhrase(m => m
                        .Field(f => f.CustomerFullName)
                        .Query(customerFullName))));

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

            return result.Documents.ToImmutableList();
        }
    }
}