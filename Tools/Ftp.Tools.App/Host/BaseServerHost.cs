namespace Ftp.Tools.App.Host;

public interface IServerHost
{
    public abstract string Name { get; }
    public abstract Task Run(CancellationToken token);
}

public abstract class BaseServerHost : IServerHost
{
    public abstract string Name { get; }
    public abstract Task Run(CancellationToken token);
}