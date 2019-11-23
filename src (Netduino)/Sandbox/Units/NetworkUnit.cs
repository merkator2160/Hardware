using Microsoft.SPOT;
using Microsoft.SPOT.Net.NetworkInformation;
using System.Threading;

namespace Sandbox.Units
{
    internal static class NetworkUnit
    {
        public static void Run()
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            if (NetworkHelper.TryObtainIpAddress(interfaces[0]))
            {
                var responce = NetworkHelper.MakeWebRequest("http://google.com");
                Debug.Print(responce);
            }

            Thread.Sleep(Timeout.Infinite);
        }
    }
}