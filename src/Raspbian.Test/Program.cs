using System;
using System.Diagnostics;
using System.Threading;

namespace Raspbian.Test
{
	class Program
	{
		static void Main(String[] args)
		{
			Console.WriteLine("Hello world!");
#if DEBUG
			WaitForDebuggerAttach();
#endif
			Console.WriteLine("Program body");
		}

		// FUNCTIONS ////////////////////////////////////////////////////////////////////////////////////
		private static void WaitForDebuggerAttach()
		{
			Console.WriteLine("waiting for debugger attach");
			while(!Debugger.IsAttached)
			{
				Thread.Sleep(100);
			}

			Console.WriteLine("Debugger attached");
		}
	}
}