using Microsoft.Extensions.Hosting;

namespace AccountManagement.Batch
{
    public interface IBatchJob : IHostedService
    {
        string Name { get; }
    }
}
