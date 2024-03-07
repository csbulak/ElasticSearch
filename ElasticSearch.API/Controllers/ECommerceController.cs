using ElasticSearch.API.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearch.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ECommerceController : ControllerBase
    {
        private readonly ECommerceRepository _eCommerceRepository;

        public ECommerceController(ECommerceRepository eCommerceRepository)
        {
            _eCommerceRepository = eCommerceRepository;
        }

        [HttpGet]
        public async Task<IActionResult> TermQuery(string customerFirstName)
        {
            var result = await _eCommerceRepository.TermQuery(customerFirstName);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> TermsQuery(List<string> customerFirstName)
        {
            var result = await _eCommerceRepository.TermsQuery(customerFirstName);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> PrefixQuery(string customerFullName)
        {
            var result = await _eCommerceRepository.PrefixQuery(customerFullName);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> RangeQuery(double fromPrice, double toPrice)
        {
            return Ok(await _eCommerceRepository.RangeQuery(fromPrice, toPrice));
        }

        [HttpGet]
        public async Task<IActionResult> MatchAllQuery()
        {
            return Ok(await _eCommerceRepository.MatchAllQuery());
        }

        [HttpGet]
        public async Task<IActionResult> MatchAllPaginationQuery(int page, int pageSize)
        {
            return Ok(await _eCommerceRepository.MatchAllPaginationQuery(page, pageSize));
        }

        [HttpGet]
        public async Task<IActionResult> WildCardQuery(string customerFullName)
        {
            return Ok(await _eCommerceRepository.WildCardQuery(customerFullName));
        }
    }
}