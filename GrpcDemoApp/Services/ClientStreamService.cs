using Grpc.Core;

namespace GRPCService.Services;

public class ClientStreamService : ClientStream.ClientStreamBase
{
    private readonly ILogger<ClientStreamService> _logger;
    private Random _random;
    public ClientStreamService(ILogger<ClientStreamService> logger)
    {
        _logger = logger;
        _random = new Random();
    }
    public override async Task<StreamMessage> SendClientStream(IAsyncStreamReader<StreamMessage> requestStream, ServerCallContext context)
    {
        while(await requestStream.MoveNext())
        {
            Console.WriteLine(requestStream.Current.TestMessage);
        }
        Console.WriteLine("Client Streaming Is Completed");
        return new StreamMessage { TestMessage = "All Data Recivied" };
    }
}
