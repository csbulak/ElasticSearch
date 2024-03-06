using ElasticSearch.API.Dtos;
using ElasticSearch.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearch.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : BaseController
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] ProductCreateDto productCreateDto)
        {
            return CreateActionResult(await _productService.SaveProductAsync(productCreateDto));
        }
    }
}
