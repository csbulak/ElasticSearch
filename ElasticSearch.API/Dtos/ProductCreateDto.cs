using ElasticSearch.API.Models;

namespace ElasticSearch.API.Dtos;

public record ProductCreateDto(string Name, decimal Price, int Stock, ProductFeatureDto ProductFeature)
{
    public Product CreateProduct()
    {
        return new Product
        {
            Name = Name,
            Price = Price,
            Stock = Stock,
            Feature = new ProductFeature()
            {
                Width = ProductFeature.Width,
                Height = ProductFeature.Height,
                Color = (EColor)int.Parse(ProductFeature.EColor)
            },
         };
    }
}