using Microsoft.SPOT;
using Microsoft.SPOT.IO;
using System;
using System.IO;
using System.Text;

namespace Sandbox.Units
{
    internal static class SdCardUnit
    {
        public static void Run()
        {
            var volume = new VolumeInfo("SD");
            var infoStr = GetOutputSdInfo(volume);

            Debug.Print(infoStr);


            var path = Path.Combine("SD", "test.txt");
            File.WriteAllBytes(path, Encoding.UTF8.GetBytes(infoStr));

            volume.FlushAll();
        }
        private static String GetOutputSdInfo(VolumeInfo vInfo)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("Is Formatted: " + vInfo.IsFormatted);
            stringBuilder.AppendLine("Total Free Space: " + vInfo.TotalFreeSpace);
            stringBuilder.AppendLine("Total Size: " + vInfo.TotalSize);
            stringBuilder.AppendLine("File System: " + vInfo.FileSystem);

            return stringBuilder.ToString();
        }
    }
}