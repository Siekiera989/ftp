using Ftp.Core.Interfaces.Settings;
using Ftp.Core.Models.Options;

namespace Ftp.Core.Services;

public interface IAddressResolver
{
    PasvOptions Resolve(IPassiveConnectionSettings options);
}

public class AddressResolver : IAddressResolver
{
    private static readonly char[] allowedRangeSeparators = [':', '-'];

    public PasvOptions Resolve(IPassiveConnectionSettings options)
    {
        var portRange = GetPasvPortRange(options);

        var minPort = portRange.HasValue ? portRange.Value.minPort : 0;

        if (minPort > 0 && minPort < 1024)
        {
            minPort = 1024;
        }

        var maxPort = Math.Max(portRange.HasValue ? portRange.Value.maxPort : 0, minPort);

        return new PasvOptions(minPort, maxPort);
    }


    internal static (int minPort, int maxPort)? GetPasvPortRange(IPassiveConnectionSettings options)
    {
        var portRange = options.PassivePortsRange!.Split(allowedRangeSeparators, StringSplitOptions.RemoveEmptyEntries);

        if (portRange.Length != 2)
        {
            throw new ApplicationException("Need exactly two ports for PASV port range");
        }

        var iPorts = portRange.Select(s => Convert.ToInt32(s)).ToArray();

        if (iPorts[1] < iPorts[0])
        {
            throw new ApplicationException("PASV start port must be smaller than end port");
        }

        return (iPorts[0], iPorts[1]);
    }
}


