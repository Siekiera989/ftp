namespace Ftp.Core.Interfaces;

public interface IServiceInstanceContext
{
    /// <summary>
    /// Current machine context name, on SF case it's the Name of the NodeContext
    /// Within VM-scale set it can be VM local ip address
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Returns name of the project instance like media / signal server
    /// </summary>
    string ProjectName { get; }

    /// <summary>
    /// Current partition high key, will return only when partition is of type Int64RangePartitionInformation on SF case.
    /// </summary>
    /// <returns></returns>
    long GetPartitionHighKey();
}