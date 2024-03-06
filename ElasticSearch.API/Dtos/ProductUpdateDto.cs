using ElasticSearch.API.Models;

namespace ElasticSearch.API.Dtos;

public record ProductUpdateDto(string Id,string Name, decimal Price, int Stock, ProductFeatureDto Feature);