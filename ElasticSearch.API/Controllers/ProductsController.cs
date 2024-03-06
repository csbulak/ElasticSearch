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
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return CreateActionResult(await _productService.GetAllAsync());
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            return CreateActionResult(await _productService.GetById(id));
        }
        
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ProductUpdateDto productUpdateDto)
        {
            return CreateActionResult(await _productService.UpdateAsync(productUpdateDto));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            return CreateActionResult(await _productService.DeleteAsync(id));
        }
    }
}
