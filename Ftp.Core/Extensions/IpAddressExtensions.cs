using System.Net.Sockets;
using System.Net;
using Ftp.Core.Enums;

namespace Ftp.Core.Extensions;

public static class IpAddressExtensions
{
    public static string GetLocalIpAddress()
    {
        if (ServerMode.HostType == ServerHost.Console)
            return "127.0.0.1";

        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }

        throw new Exception("No network adapters with an IPv4 address in the system!");
    }
}