using System.Collections.Immutable;
using Elastic.Clients.Elasticsearch;
using ElasticSearch.API.Dtos;
using ElasticSearch.API.Models;

namespace ElasticSearch.API.Repository;

public class ProductRepository
{
    private readonly ElasticsearchClient _client;

    public ProductRepository(ElasticsearchClient client)
    {
        _client = client;
    }

    public async Task<Product?> SaveAsync(Product product)
    {
        product.Created = DateTime.Now;

        var response = await _client.IndexAsync(product, x => x.Index("products").Id(Guid.NewGuid().ToString()));
        if (!response.IsValidResponse)
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
            if (hits.Source != null) hits.Source.Id = hits.Id;
        }

        return searchResponse.Documents.ToImmutableList();
    }

    public async Task<Product?> GetById(string id)
    {
        var response = await _client.GetAsync<Product>(id, x => x.Index("products"));

        if (!response.Found)
        {
            return null;
        }

        if (response.Source != null) response.Source.Id = response.Id;
        return response.Source;
    }

    public async Task<bool> UpdateAsync(ProductUpdateDto product)
    {
        var response =
            await _client.UpdateAsync<Product, ProductUpdateDto>("products", product.Id, x => x.Doc(product));
        return response.IsValidResponse;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var response = await _client.DeleteAsync<Product>(id, x => x.Index("products"));
        return response.IsValidResponse;
    }
}