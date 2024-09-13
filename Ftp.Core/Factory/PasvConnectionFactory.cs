using System.Net;
using System.Net.Sockets;
using Ftp.Core.Connection;
using Ftp.Core.Interfaces.Settings;
using Ftp.Core.Models.Options;
using Ftp.Core.Services;
using Serilog;

namespace Ftp.Core.Factory;

public interface IPasvConnectionFactory
{
    PassiveDataConnector CreatePassiveDataConnector(IPAddress localAddress);
}

internal class PasvConnectionFactory : IPasvConnectionFactory
{
    private readonly IAddressResolver _addressResolver;
    private readonly IPassiveConnectionSettings _passiveConnectionSettings;
    private readonly ILogger _logger;
    private readonly Random _prng = new();
    private readonly object _listenerLock = new();

    public PasvConnectionFactory(IAddressResolver addressResolver, IPassiveConnectionSettings passiveConnectionSettings, ILogger logger)
    {
        _addressResolver = addressResolver;
        _passiveConnectionSettings = passiveConnectionSettings;
        _logger = logger;
    }

    public PassiveDataConnector CreatePassiveDataConnector(IPAddress localAddress)
    {
        var pasvOptions = _addressResolver.Resolve(_passiveConnectionSettings);

        PassiveDataConnector listener = CreateListenerInRange(localAddress, pasvOptions);

        return listener;
    }

    private PassiveDataConnector CreateListenerInRange(IPAddress localAddress, PasvOptions pasvOptions)
    {
        lock (_listenerLock)
        {
            // randomize ports so we don't always get the ports in the same order
            foreach (var port in GetPorts(pasvOptions))
            {
                try
                {
                    return new PassiveDataConnector(localAddress, port);
                }
                catch (SocketException se)
                {
                    // retry if the socket is already in use, else throw the underlying exception
                    if (se.SocketErrorCode != SocketError.AddressAlreadyInUse)
                    {
                        _logger.Error(se, "Could not create listener");
                        throw;
                    }
                }
            }

            // if we reach this point, we have not been able to create a listener within range
            _logger.Warning("No free ports available for data connection");
            throw new SocketException((int)SocketError.AddressAlreadyInUse);
        }
    }

    private IEnumerable<int> GetPorts(PasvOptions options)
    {
        var portRangeCount = (options.MaxPort - options.MinPort + 1) * 2;
        while (portRangeCount-- != 0)
        {
            yield return _prng.Next(options.MinPort, options.MaxPort + 1);
        }
    }
}

