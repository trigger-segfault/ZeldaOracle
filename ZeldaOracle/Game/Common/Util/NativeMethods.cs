using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Common.Util {
	public static class NativeMethods {

		/// <summary>The standard output device. Initially, this is the active
		/// console screen buffer, CONOUT$.</summary>
		public const uint StdOutputHandle = 0xFFFFFFF5;

		/// <summary>Allocates a new console for the calling process.</summary>
		[DllImport("kernel32.dll")]
		public static extern bool AllocConsole();

		/// <summary>Detaches the calling process from its console.</summary>
		[DllImport("kernel32.dll")]
		public static extern bool FreeConsole();

		/// <summary>Retrieves a handle to the specified standard device
		/// (standard input, standard output, or standard error).</summary>
		[DllImport("kernel32.dll")]
		public static extern IntPtr GetStdHandle(uint nStdHandle);

		/// <summary>Sets the handle for the specified standard device
		/// (standard input, standard output, or standard error).</summary>
		[DllImport("kernel32.dll")]
		public static extern void SetStdHandle(uint nStdHandle, IntPtr hHandle);

		/// <summary>Sets the title for the current console window.</summary>
		[DllImport("kernel32.dll")]
		public static extern bool SetConsoleTitle(string lpConsoleTitle);
	}
}
