using Microsoft.SPOT;
using Microsoft.SPOT.Net.NetworkInformation;
using System;
using System.IO;
using System.Net;
using System.Threading;

namespace Common.Helpers
{
    public static class NetworkHelper
    {
        public static String MakeWebRequest(String url, String method = "GET")
        {
            using (var httpWebRequest = WebRequest.Create(url))
            {
                httpWebRequest.Method = method;
                using (var httpResponse = httpWebRequest.GetResponse())
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
        }
        public static Boolean TryObtainIpAddress(NetworkInterface net, Int32 timeOut = 10000)
        {
            if (net.IPAddress == IPAddress.Any.ToString())
            {
                Debug.Print("No IP Address");

                if (net.IsDhcpEnabled)
                {
                    Debug.Print("DHCP is enabled, attempting to get an IP Address");

                    var sleepInterval = 10;
                    var maxIntervalCount = timeOut / sleepInterval;
                    var count = 0;
                    while (IPAddress.GetDefaultLocalAddress() == IPAddress.Any && count < maxIntervalCount)
                    {
                        Debug.Print("Sleep while obtaining an IP");
                        Thread.Sleep(10);
                        count++;
                    }

                    // if we got here, we either timed out or got an address, so let's find out.
                    if (net.IPAddress == IPAddress.Any.ToString())
                    {
                        Debug.Print("Failed to get an IP Address in the alotted time.");
                        return false;
                    }

                    Debug.Print("Got IP Address: " + net.IPAddress);
                    return true;
                }

                Debug.Print("DHCP is not enabled, and no IP address is configured, bailing out.");
                return false;
            }

            Debug.Print("Already had IP Address: " + net.IPAddress);
            return true;
        }
        public static void ShowNetworkInterfaces(NetworkInterface[] interfaces)
        {
            foreach (var net in interfaces)
            {
                switch (net.NetworkInterfaceType)
                {
                    case (NetworkInterfaceType.Ethernet):
                        Debug.Print("Found Ethernet Interface");
                        break;
                    case (NetworkInterfaceType.Wireless80211):
                        Debug.Print("Found 802.11 WiFi Interface");
                        break;
                    case (NetworkInterfaceType.Unknown):
                        Debug.Print("Found Unknown Interface");
                        break;
                }
            }
        }
        public static void ShowNetworkInterfaceInfo(NetworkInterface net)
        {
            Debug.Print("MAC Address: " + BytesToHexString(net.PhysicalAddress));
            Debug.Print("DHCP enabled: " + net.IsDhcpEnabled);
            Debug.Print("Dynamic DNS enabled: " + net.IsDynamicDnsEnabled);
            Debug.Print("IP Address: " + net.IPAddress);
            Debug.Print("Subnet Mask: " + net.SubnetMask);
            Debug.Print("Gateway: " + net.GatewayAddress);

            var wifi = net as Wireless80211;
            if (wifi != null)
            {
                Debug.Print("SSID:" + wifi.Ssid);
            }
        }
        private static String BytesToHexString(Byte[] bytes)
        {
            const String hexChars = "0123456789ABCDEF"; // Create a character array for hexadecimal conversion.
            var hexString = String.Empty;

            for (Byte b = 0; b < bytes.Length; b++)
            {
                if (b > 0)
                    hexString += "-";

                hexString += hexChars[bytes[b] >> 4];   // Grab the top 4 bits and append the hex equivalent to the return string.
                hexString += hexChars[bytes[b] & 0x0F]; // Mask off the upper 4 bits to get the rest of it.
            }

            return hexString;
        }
    }
}