using System.Net;
using ElasticSearch.API.Dtos;
using ElasticSearch.API.Models;
using ElasticSearch.API.Repository;

namespace ElasticSearch.API.Services;

public class ProductService
{
    private readonly ProductRepository _productRepository;

    public ProductService(ProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ResponseDto<ProductDto>> SaveProductAsync(ProductCreateDto request)
    {
        var response = await _productRepository.SaveAsync(request.CreateProduct());

        if (response == null)
        {
            return ResponseDto<ProductDto>.Fail(new List<string>() { "hata meydana geldi" },
                HttpStatusCode.InternalServerError);
        }

        return ResponseDto<ProductDto>.Success(response.CreateDto(), HttpStatusCode.Created);
    }
}