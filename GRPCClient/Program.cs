using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using GRPCService;
using System;

namespace GRPCClient;

public class Program
{
    private static Random _random;
    public static async Task Main(string[] args)
    {
        _random = new Random();
        //await GetUnraryResponse();
        //await GetServerStream();
        //await SendClientStream();
        await BiDirectionalStream();
        Console.ReadKey();
    }

    #region Unrary (Single Request - Single Response)
        private static async Task GetUnraryResponse()
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
            foreach (var product in getproductsListResponse.Products)
            {
                Console.WriteLine($"{product.ProductCode} | {product.ProductName} | {product.Price} | {product.StockDate.ToDateTime()}");
            }

            await channel.ShutdownAsync();
            Console.ReadKey();
        }
    #endregion

    #region ServerStream (Single Request - Multiple Response)
        private static async Task GetServerStream()
        {
            var channel = GrpcChannel.ForAddress("http://localhost:5000");
            var streamClient = new ServerStream.ServerStreamClient(channel);
            var response = streamClient.GetServerStream(new Test() { TestMessage = "Test" });
            while(await response.ResponseStream.MoveNext(CancellationToken.None))
            {
                Console.WriteLine(response.ResponseStream.Current.TestMessage);
            }
            Console.WriteLine("Server Streaming Completed");
            await channel.ShutdownAsync();
        }
    #endregion

    #region ClientStream (Multiple Request - Single Response)
        private static async Task SendClientStream()
        {
            var channel = GrpcChannel.ForAddress("http://localhost:5000");
            var streamClient = new ClientStream.ClientStreamClient(channel);
            var stream = streamClient.SendClientStream();
            for(int i = 1; i <= 10; i++)
            {
                await stream.RequestStream.WriteAsync(new StreamMessage() 
                {
                     TestMessage = $"Test {i}"
                });
                var randomNumber = _random.Next(1, 10);
                await Task.Delay(randomNumber * 1000);
            }
            await stream.RequestStream.CompleteAsync();
            Console.WriteLine("Client Stream Completed");
            await channel.ShutdownAsync();
        }
    #endregion

    #region BiDirectional Stream
        private static async Task BiDirectionalStream()
        {
            var channel = GrpcChannel.ForAddress("http://localhost:5000");
            var biDirectionalStreamClient = new BiDirectionalStream.BiDirectionalStreamClient(channel);
            var stream = biDirectionalStreamClient.BiDirectionalStream();
            var requestTasks = Task.Run(async () => 
            {
                for(int i = 1; i <= 10; i++)
                {
                    var randomNumber = _random.Next(1, 10);
                    await Task.Delay(randomNumber * 1000);
                    await stream.RequestStream.WriteAsync(new BiDirectionalMessage() 
                    {
                        Message = $"Message {i}"
                    });
                    Console.WriteLine($"Send Request: Message {i}");
                }
                await stream.RequestStream.CompleteAsync();
            });
            var responseTasks = Task.Run(async () => 
            {
                while (await stream.ResponseStream.MoveNext(CancellationToken.None))
                {
                    Console.WriteLine($"Received Response : {stream.ResponseStream.Current.Message}");
                }
                Console.WriteLine("Response Completed");
            });
            await Task.WhenAll(requestTasks, responseTasks);
            Console.WriteLine("BiDirectional Stream Completed");
            await channel.ShutdownAsync();
        }
    #endregion
}