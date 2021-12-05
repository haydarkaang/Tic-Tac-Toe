using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Service
{
    public class Network
    {
        /// <summary>
        /// Client type does not matter. The need is any available port to use.
        /// </summary>
        /// <returns>Any available port</returns>
        public static int GetAvailablePort()
        {
            var udp = new UdpClient(0, AddressFamily.InterNetwork);
            return ((IPEndPoint)udp.Client.LocalEndPoint).Port;
        }

        public static IPAddress GetNetworkInfo()
        {
            var interfaces = new NetworkInterfaceType[] {
                NetworkInterfaceType.Wireless80211,
                NetworkInterfaceType.Ethernet
            };
            
            IPAddress address = null;
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (interfaces.Contains(item.NetworkInterfaceType) && item.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties adapterProperties = item.GetIPProperties();
                    if (adapterProperties.GatewayAddresses.FirstOrDefault() != null)
                    {   
                        foreach (UnicastIPAddressInformation ip in adapterProperties.UnicastAddresses)
                        {   
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                address = ip.Address;
                                break;
                            }
                        }
                    }
                }
                
                if (address != null) { break; }
            }
            
            return address;
        }
    }
}
