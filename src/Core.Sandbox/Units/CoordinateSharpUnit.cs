using CoordinateSharp;
using System;

namespace Core.Sandbox.Units
{
    internal class CoordinateSharpUnit
    {
        public static void Run()
        {
            var cel = Celestial.CalculateCelestialTimes(56.3287, 44.002, DateTime.UtcNow);
            if (!cel.SunRise.HasValue)
            {
                Console.WriteLine("N/A");
            }
            else
            {
                Console.WriteLine(cel.SunRise.Value.ToLocalTime());
            }

            if (!cel.SunSet.HasValue)
            {
                Console.WriteLine("N/A");
            }
            else
            {
                Console.WriteLine(cel.SunSet.Value.ToLocalTime());
            }

            Console.ReadKey();
        }
    }
}