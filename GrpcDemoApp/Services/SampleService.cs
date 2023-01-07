using Grpc.Core;

namespace GRPCService.Services;

public class SampleService: Sample.SampleBase
{
    private readonly ILogger<SampleService> _logger;
    public SampleService(ILogger<SampleService> logger) 
    {
        _logger = logger;
    }
    public override Task<SampleResponse> GetFullName(SampleRequest request, ServerCallContext context)
    {
        var result = $"{request.FirstName} {request.LastName}";
        return Task.FromResult(new SampleResponse() { FullName = result});
    }
}
