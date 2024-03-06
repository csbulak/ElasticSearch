namespace ElasticSearch.API.Dtos;

public record ProductDto(
    string Id,
    string Name,
    decimal Price,
    DateTime Created,
    DateTime? Updated,
    ProductFeatureDto? Feature);