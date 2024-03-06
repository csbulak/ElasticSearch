using ElasticSearch.API.Models;
using Nest;

namespace ElasticSearch.API.Repository;

public class ProductRepository
{
    private readonly IElasticClient _client;

    public ProductRepository(IElasticClient client)
    {
        _client = client;
    }

    public async Task<Product?> SaveAsync(Product product)
    {
        product.Created = DateTime.Now;

        var response = await _client.IndexAsync(product, x => x.Index("products").Id(Guid.NewGuid().ToString()));
        if (!response.IsValid)
        {
            return null;
        }

        product.Id = response.Id;
        return product;
    }
}