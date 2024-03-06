using System.Collections.Immutable;
using ElasticSearch.API.Dtos;
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

    public async Task<ImmutableList<Product>> GetAllAsync()
    {
        var searchResponse = await _client.SearchAsync<Product>(s => s.Index("products")
            .Query(q => q.MatchAll()));

        foreach (var hits in searchResponse.Hits)
        {
            hits.Source.Id = hits.Id;
        }

        return searchResponse.Documents.ToImmutableList();
    }

    public async Task<Product?> GetById(string id)
    {
        var response = await _client.GetAsync<Product>(id, x => x.Index("products"));
        response.Source.Id = response.Id;
        return response.Source;
    }

    public async Task<bool> UpdateAsync(ProductUpdateDto product)
    {
        var response = await _client.UpdateAsync<Product, ProductUpdateDto>(product.Id, x => x.Index("products")
            .Doc(product));
        return response.IsValid;
    }
}