using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace GRPCService.Services;

public class ProductService : Product.ProductBase
{
    private readonly ILogger<ProductService> _logger;
    public ProductService(ILogger<ProductService> logger)
    {
        _logger = logger;
    }
    public override Task<CreateProductResponse> CreateProduct(CreateProductRequest request, ServerCallContext context)
    {
        Console.WriteLine($"{request.ProductName} | {request.ProductCode} | {request.Price} | {request.StockDate}");
        return Task.FromResult(new CreateProductResponse()
        {
            StatusCode = 200,
            IsSuccessful = true
        });
    }
    public override Task<ProductList> GetProducts(Empty request, ServerCallContext context)
    {
        ProductList productsList = new();
        var stockDate = DateTime.SpecifyKind(new DateTime(2022, 11, 14), DateTimeKind.Utc);
        for (int i = 1; i <= 100; i++) 
        {
            productsList.Products.Add(new CreateProductRequest() {
                 ProductName = $"Product00{i}",
                 ProductCode = $"P00{i}",
                 Price = i * 100,
                 StockDate = Timestamp.FromDateTime(stockDate)
            });
        }
        return Task.FromResult(productsList);
    }
}
