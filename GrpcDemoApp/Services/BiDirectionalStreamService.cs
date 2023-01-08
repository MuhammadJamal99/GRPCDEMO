using Grpc.Core;

namespace GRPCService.Services;

public class BiDirectionalStreamService : BiDirectionalStream.BiDirectionalStreamBase
{
    private readonly ILogger<BiDirectionalStreamService> _logger;
    private Random _random;
    public BiDirectionalStreamService(ILogger<BiDirectionalStreamService> logger)
    {
        _logger = logger;
        _random = new Random();
    }
    public override async Task BiDirectionalStream(IAsyncStreamReader<BiDirectionalMessage> requestStream, IServerStreamWriter<BiDirectionalMessage> responseStream, ServerCallContext context)
    {
        var tasks = new List<Task>();
        while (await requestStream.MoveNext())
        {
            Console.WriteLine($"Received Request {requestStream.Current.Message}");
            tasks.Add(Task.Run(async () =>
            {
                var message = requestStream.Current.Message;
                var randomNumber = _random.Next(1, 100);
                await Task.Delay(randomNumber * 1000);
                await responseStream.WriteAsync(new BiDirectionalMessage()
                {
                    Message = message
                });
                Console.WriteLine($"Sent Response: {message}");
            }));
        }
        await Task.WhenAll(tasks);
        Console.WriteLine("Bidirectional Streaming Completed");

    }
}
