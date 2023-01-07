using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using GRPCService;

namespace GRPCClient;

public class Program
{
    static async Task Main(string[] args)
    {
        var channel = GrpcChannel.ForAddress("http://localhost:5000");
        
        var sampleClient = new Sample.SampleClient(channel);
        var sampleResponse = await sampleClient.GetFullNameAsync(new SampleRequest() { FirstName = "Muhammad", LastName = "Punk" });
        Console.WriteLine(sampleResponse.FullName);
        
        var productClient = new Product.ProductClient(channel);
  
        var stockDate = DateTime.SpecifyKind(new DateTime(2022, 11, 14), DateTimeKind.Utc);
        var productResponse = await productClient.CreateProductAsync(new CreateProductRequest()
        {
            ProductName = "HeadPhone",
            ProductCode = "1",
            Price = 2000,
            StockDate = Timestamp.FromDateTime(stockDate)
        });

        Console.WriteLine($"{productResponse.StatusCode} | {productResponse.IsSuccessful}");

        var getproductsListResponse = await productClient.GetProductsAsync(new Google.Protobuf.WellKnownTypes.Empty());
        foreach(var product in getproductsListResponse.Products)
        {
            Console.WriteLine($"{product.ProductCode} | {product.ProductName} | {product.Price} | {product.StockDate.ToDateTime()}");
        }

        Console.ReadKey();
        await channel.ShutdownAsync();
    }
}