using System.Collections.Immutable;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using ElasticSearch.API.Models.ECommerceModel;

namespace ElasticSearch.API.Repository
{
    /// <summary>
    /// Represents a repository for accessing ECommerce data in Elasticsearch.
    /// </summary>
    public class ECommerceRepository
    {
        /// <summary>
        /// The client used to interact with Elasticsearch for the ECommerceRepository.
        /// </summary>
        private readonly ElasticsearchClient _client;
        /// <summary>
        /// The name of the Elasticsearch index used by the ECommerceRepository class.
        /// </summary>
        private const string indexName = "kibana_sample_data_ecommerce";

        /// <summary>
        /// Repository class for interacting with the ECommerce model in Elasticsearch.
        /// </summary>
        public ECommerceRepository(ElasticsearchClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Performs a term query on the "customer_first_name.keyword" field in the Elasticsearch index "kibana_sample_data_ecommerce"
        /// to retrieve a list of ECommerce documents that match the given customer first name.
        /// </summary>
        /// <param name="customerFirstName">The customer's first name.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an ImmutableList of ECommerce documents.</returns>
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

        /// <summary>
        /// Performs a terms query on the "customer_first_name.keyword" field in the "kibana_sample_data_ecommerce" index.
        /// </summary>
        /// <param name="customerFirstNames">A list of customer first names to search for.</param>
        /// <returns>A list of ECommerce objects that match the given customer first names.</returns>
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

        /// <summary>
        /// Performs a prefix search on the "customer_full_name.keyword" field in the "kibana_sample_data_ecommerce" index.
        /// </summary>
        /// <param name="customerFullName">The customer's full name to search for.</param>
        /// <returns>A list of ECommerce objects that match the prefix search.</returns>
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

        /// <summary>
        /// Executes a range query to retrieve a list of eCommerce objects with taxful total price within a specified range.
        /// </summary>
        /// <param name="fromPrice">The lower bound of the price range (inclusive).</param>
        /// <param name="toPrice">The upper bound of the price range (inclusive).</param>
        /// <returns>A list of eCommerce objects with taxful total price within the specified range.</returns>
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

        /// <summary>
        /// Executes a match all query to retrieve all documents from the Elasticsearch index.
        /// </summary>
        /// <returns>An immutable list of ECommerce objects representing the retrieved documents.</returns>
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

        /// <summary>
        /// Executes a paginated MatchAll query on the Elasticsearch index.
        /// </summary>
        /// <param name="page">The page number to retrieve. Must be greater than or equal to 1.</param>
        /// <param name="pageSize">The number of documents to retrieve per page. Must be greater than or equal to 1.</param>
        /// <returns>The paginated list of ECommerce documents matching the MatchAll query.</returns>
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

        /// <summary>
        /// Perform a wildcard query to search for documents with a matching field value using wildcard patterns.
        /// </summary>
        /// <param name="customerFullName">The full name of the customer to search for.</param>
        /// <returns>A list of ECommerce documents that match the given wildcard pattern on the CustomerFullName field.</returns>
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

        /// <summary>
        /// Execute a fuzzy query to search for documents in the Elasticsearch index based on a fuzzy match on the customer name.
        /// </summary>
        /// <param name="customerName">The customer name to match.</param>
        /// <returns>A list of ECommerce documents that match the fuzzy query.</returns>
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

        /// <summary>
        /// Performs a full-text match query on the "Category" field of the ECommerce documents.
        /// </summary>
        /// <param name="categoryName">The category name to match with.</param>
        /// <returns>An immutable list of ECommerce documents that match the given category name.</returns>
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

        /// <summary>
        /// Performs a match phrase prefix query on the "CustomerFullName" field in the Elasticsearch index.
        /// </summary>
        /// <param name="customerFullName">The customer's full name to search for.</param>
        /// <returns>An immutable list of ECommerce objects that match the query criteria.</returns>
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

        /// <summary>
        /// Executes a match phrase query based on the given customer full name.
        /// </summary>
        /// <param name="customerFullName">The customer's full name.</param>
        /// <returns>An immutable list of ECommerce objects that match the query.</returns>
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

        /// <summary>
        /// Executes a compound query to search for ecommerce data based on various criteria.
        /// </summary>
        /// <param name="cityName">The city name to search for.</param>
        /// <param name="taxFullTotalPrice">The maximum value for taxful total price.</param>
        /// <param name="categoryName">The category name to search for.</param>
        /// <param name="menufacturer">The manufacturer name to search for.</param>
        /// <returns>A list of ecommerce data that matches the search criteria.</returns>
        public async Task<ImmutableList<ECommerce>> CompoundQueryExampleOne(string cityName, double taxFullTotalPrice, string categoryName, string menufacturer)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
                .Size(20)
                .Query(q => q
                    .Bool(b => b
                        .Must(m => m
                            .Term(t => t
                                .Field("geoip.city_name")
                                .Value(cityName)))
                        .MustNot(mn => mn
                            .Range(r => r
                                .NumberRange(nr => nr
                                    .Field(f => f.TaxFullTotalPrice)
                                    .Lte(taxFullTotalPrice))))
                        .Should(should => should
                            .Term(t => t
                                .Field(f => f.Category.Suffix("keyword"))
                                .Value(categoryName)))
                        .Filter(f => f
                            .Term(t => t
                                .Field("manufacturer.keyword")
                                .Value(menufacturer))))));

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

        public async Task<ImmutableList<ECommerce>> CompoundQueryExampleTwo(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
                .Size(20)
                .Query(q => q
                    .Bool(b => b
                        .Should(m => m
                            .Match(matchQueryDescriptor => matchQueryDescriptor
                                .Field(f => f.CustomerFullName)
                                .Query(customerFullName))
                            .Prefix(p => p
                                .Field(f => f.CustomerFullName.Suffix("keyword"))
                                .Value(customerFullName))))));

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