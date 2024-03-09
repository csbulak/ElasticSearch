using ElasticSearch.API.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearch.API.Controllers
{
    /// <summary>
    /// Controller for managing ECommerce operations.
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ECommerceController : ControllerBase
    {
        /// data.
        private readonly ECommerceRepository _eCommerceRepository;

        /// <summary>
        /// Initializes a new instance of the ECommerceController class.
        /// </summary>
        /// <param name="eCommerceRepository">The instance of ECommerceRepository to be injected.</param>
        public ECommerceController(ECommerceRepository eCommerceRepository)
        {
            _eCommerceRepository = eCommerceRepository;
        }

        /// <summary>
        /// Executes a term query on the "customer_first_name.keyword" field in the "kibana_sample_data_ecommerce" index.
        /// </summary>
        /// <param name="customerFirstName">The value to search for in the "customer_first_name.keyword" field.</param>
        /// <returns>
        /// An <see cref="ImmutableList{T}"/> of <see cref="ECommerce"/> objects that match the term query.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> TermQuery(string customerFirstName)
        {
            var result = await _eCommerceRepository.TermQuery(customerFirstName);
            return Ok(result);
        }

        /// <summary>
        /// Executes a TermsQuery against the Elasticsearch index to search for documents that match the provided customer first names.
        /// </summary>
        /// <param name="customerFirstNames">A list of customer first names to search for.</param>
        /// <returns>An immutable list of ECommerce documents that match the search criteria.</returns>
        [HttpPost]
        public async Task<IActionResult> TermsQuery(List<string> customerFirstName)
        {
            var result = await _eCommerceRepository.TermsQuery(customerFirstName);
            return Ok(result);
        }

        /// <summary>
        /// Executes a prefix query on the "customer_full_name.keyword" field in the "kibana_sample_data_ecommerce" index.
        /// </summary>
        /// <param name="customerFullName">The value to search for in the "customer_full_name.keyword" field.</param>
        /// <returns>
        /// An <see cref="ImmutableList{ECommerce}"/> containing the documents that match the prefix query.
        /// </returns>
        /// <exception cref="Exception">Thrown when an error occurs while executing the search query or no documents are found.</exception>
        [HttpGet]
        public async Task<IActionResult> PrefixQuery(string customerFullName)
        {
            var result = await _eCommerceRepository.PrefixQuery(customerFullName);
            return Ok(result);
        }

        /// <summary>
        /// Executes a range query on the "TaxFullTotalPrice" field of the ECommerce index.
        /// Retrieves documents where the "TaxFullTotalPrice" field value is between the specified "fromPrice" and "toPrice" values, inclusive.
        /// </summary>
        /// <param name="fromPrice">The minimum value of the "TaxFullTotalPrice" field.</param>
        /// <param name="toPrice">The maximum value of the "TaxFullTotalPrice" field.</param>
        /// <returns>An asynchronous task that represents the operation. The task result contains an ImmutableList of ECommerce documents that match the range query.</returns>
        [HttpGet]
        public async Task<IActionResult> RangeQuery(double fromPrice, double toPrice)
        {
            return Ok(await _eCommerceRepository.RangeQuery(fromPrice, toPrice));
        }

        /// Retrieves all documents from the Elasticsearch index.
        /// @return A list of ECommerce objects representing the retrieved documents.
        /// @throws Exception If an error occurs while executing the search query or if no documents are found.
        /// /
        [HttpGet]
        public async Task<IActionResult> MatchAllQuery()
        {
            return Ok(await _eCommerceRepository.MatchAllQuery());
        }

        /// <summary>
        /// Performs a pagination query by retrieving a specified page of documents from the Elasticsearch index.
        /// </summary>
        /// <param name="page">The page number to retrieve. Must be greater than or equal to 1.</param>
        /// <param name="pageSize">The number of documents to retrieve per page. Must be greater than 0.</param>
        /// <returns>
        /// An ImmutableList of ECommerce objects representing the documents retrieved from the Elasticsearch index.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> MatchAllPaginationQuery(int page, int pageSize)
        {
            return Ok(await _eCommerceRepository.MatchAllPaginationQuery(page, pageSize));
        }

        /// <summary>
        /// Executes a wildcard query on the specified field in the Elasticsearch index.
        /// </summary>
        /// <param name="customerFullName">The value to be used for the wildcard query.</param>
        /// <returns>An ImmutableList of ECommerce objects representing the search results.</returns>
        [HttpGet]
        public async Task<IActionResult> WildCardQuery(string customerFullName)
        {
            return Ok(await _eCommerceRepository.WildCardQuery(customerFullName));
        }

        /// <summary>
        /// Executes a fuzzy query to search for documents in the Elasticsearch index based on a fuzzy match on the customerName field.
        /// </summary>
        /// <param name="customerName">The customer name to perform a fuzzy search on.</param>
        /// <returns>A list of ECommerce documents that match the fuzzy search criteria.</returns>
        [HttpGet]
        public async Task<IActionResult> FuzzyQuery(string customerName)
        {
            return Ok(await _eCommerceRepository.FuzzyQuery(customerName));
        }

        /// <summary>
        /// Performs a full-text match query on the categoryName field in the Elasticsearch index "kibana_sample_data_ecommerce".
        /// </summary>
        /// <param name="categoryName">The category name to match against.</param>
        /// <returns>An ImmutableList of ECommerce objects that match the given categoryName.</returns>
        [HttpGet]
        public async Task<IActionResult> MatchQueryFullText(string categoryName)
        {
            return Ok(await _eCommerceRepository.MatchQueryFullText(categoryName));
        }

        /// <summary>
        /// Executes a Match Bool Prefix Query on the Elasticsearch index "kibana_sample_data_ecommerce".
        /// </summary>
        /// <param name="customerFullName">The customer's full name to search for.</param>
        /// <returns>
        /// Returns an immutable list of ECommerce documents that match the specified customer's full name.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> MatchBoolPrefixQuery(string customerFullName)
        {
            return Ok(await _eCommerceRepository.MatchBoolPrefixQuery(customerFullName));
        }

        /// <summary>
        /// Executes a Match Phrase Query on the Elasticsearch index "kibana_sample_data_ecommerce" to search for documents with a specific value in the "CustomerFullName" field.
        /// </summary>
        /// <param name="customerFullName">The value to search for in the "CustomerFullName" field.</param>
        /// <returns>An ImmutableList containing the matched documents of type ECommerce.</returns>
        [HttpGet]
        public async Task<IActionResult> MatchPhraseQuery(string customerFullName)
        {
            return Ok(await _eCommerceRepository.MatchPhraseQuery(customerFullName));
        }

        [HttpGet]
        public async Task<IActionResult> CompoundQueryExampleOne(string cityName, double taxFullTotalPrice, string categoryName, string menufacturer)
        {
            return Ok(await _eCommerceRepository.CompoundQueryExampleOne(cityName, taxFullTotalPrice, categoryName, menufacturer));
        }

        [HttpGet]
        public async Task<IActionResult> CompoundQueryExampleTwo(string customerFullName)
        {
            return Ok(await _eCommerceRepository.CompoundQueryExampleTwo(customerFullName));
        }
    }
}