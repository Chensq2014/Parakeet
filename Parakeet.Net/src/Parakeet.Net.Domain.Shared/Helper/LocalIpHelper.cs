using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Parakeet.Net.Helper
{
    /// <summary>
    /// 获取当前机器的本机ipv4
    /// </summary>
    public class LocalIpHelper
    {
        static string _ipv4 = null;
        public static string GetLocalIpv4()
        {
            if (_ipv4 != null)
            {
                return _ipv4;
            }
            return _ipv4 = NetworkInterface
                .GetAllNetworkInterfaces()
                .Select(p => p.GetIPProperties())
                .SelectMany(p => p.UnicastAddresses)
                .FirstOrDefault(p => p.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(p.Address))?.Address.ToString();
        }

        public static string GetLocalIpv6()
        {
            return  NetworkInterface
                .GetAllNetworkInterfaces()
                .Select(p => p.GetIPProperties())
                .SelectMany(p => p.UnicastAddresses)
                .FirstOrDefault(p => p.Address.AddressFamily == AddressFamily.InterNetworkV6 && !IPAddress.IsLoopback(p.Address))?.Address.ToString();
        }

        public static string GetIp()
        {
            var hostName = Dns.GetHostName();//本机名
            var addressList = Dns.GetHostAddresses(hostName);//会返回所有地址，包括IPv4和IPv6
            //if (addressList.Any())
            //{
            //    return addressList[0].ToString();
            //}
            return string.Join(",",addressList.ToList());
        }

    }
}
