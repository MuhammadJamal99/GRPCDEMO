using Grpc.Core;

namespace GRPCService.Services;

public class ServerStreamService : ServerStream.ServerStreamBase
{
    private readonly ILogger<ServerStreamService> _logger;
    private Random _random;
    public ServerStreamService(ILogger<ServerStreamService> logger)
    {
        _logger = logger;
        _random = new Random();
    }
    public override async Task GetServerStream(Test request, IServerStreamWriter<Test> responseStream, ServerCallContext context)
    {
        for(int i = 1; i <= 20; i++) 
        {
            await responseStream.WriteAsync(new Test()
            {
                TestMessage = $"Message {i}",
                RandomNumber = _random.Next(1,100)
            });
            var randomNumber = _random.Next(1, 10);
            await Task.Delay(randomNumber * 1000);
            Console.WriteLine(randomNumber);
        }
    }
}
