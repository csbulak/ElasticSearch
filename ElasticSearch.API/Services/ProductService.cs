using System.Net;
using ElasticSearch.API.Dtos;
using ElasticSearch.API.Repository;
using Microsoft.AspNetCore.Http.HttpResults;

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

    public async Task<ResponseDto<List<ProductDto>>> GetAllAsync()
    {
        var result = await _productRepository.GetAllAsync();
        var productListDto = result.Select(p => p.CreateDto()).ToList();
        return ResponseDto<List<ProductDto>>.Success(productListDto, HttpStatusCode.OK);
    }

    public async Task<ResponseDto<ProductDto>> GetById(string id)
    {
        var result = await _productRepository.GetById(id);
        if (result is null)
        {
            return ResponseDto<ProductDto>.Fail("Product not found", HttpStatusCode.NotFound);
        }

        var productDto = result.CreateDto();
        return ResponseDto<ProductDto>.Success(productDto, HttpStatusCode.OK);
    }

    public async Task<ResponseDto<bool>> UpdateAsync(ProductUpdateDto product)
    {
        var success = await _productRepository.UpdateAsync(product);
        return success
            ? ResponseDto<bool>.Success(true, HttpStatusCode.OK)
            : ResponseDto<bool>.Fail("Update operation failed", HttpStatusCode.InternalServerError);
    }
}