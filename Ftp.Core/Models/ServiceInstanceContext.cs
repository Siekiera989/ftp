using Ftp.Core.Interfaces;

namespace Ftp.Core.Models;

public class ServiceInstanceContext : IServiceInstanceContext
{
    public string Name { get; } = "InMemory";
    public string ProjectName { get; init; }
    public long GetPartitionHighKey() => int.MinValue;
}
