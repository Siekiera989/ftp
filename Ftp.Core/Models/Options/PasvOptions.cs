namespace Ftp.Core.Models.Options;

public class PasvOptions
{
    public PasvOptions(int minPort, int maxPort)
    {
        MinPort = minPort;
        MaxPort = maxPort;
    }

    public bool HasPortRange => MinPort > 0 && MaxPort > MinPort;
    public int MinPort { get; }
    public int MaxPort { get; }
}
