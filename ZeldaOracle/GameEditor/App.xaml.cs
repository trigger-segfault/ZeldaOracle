using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ZeldaEditor.Windows;
using WinFormsApplication = System.Windows.Forms.Application;

namespace ZeldaEditor {
	/// <summary>The application class.</summary>
	public partial class App : Application {
		/// <summary>The last exception. Used to prevent multiple error windows for the same error.</summary>
		private static object lastException = null;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the app and sets up embedded assembly resolving.</summary>
		public App() {
			AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssemblies;
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		/// <summary>Resolves assemblies that may be embedded in the executable.</summary>
		private Assembly OnResolveAssemblies(object sender, ResolveEventArgs args) {
			var executingAssembly = Assembly.GetExecutingAssembly();
			var assemblyName = new AssemblyName(args.Name);

			string path = assemblyName.Name + ".dll";
			if (assemblyName.CultureInfo.Equals(CultureInfo.InvariantCulture) == false) {
				path = string.Format(@"{0}\{1}", assemblyName.CultureInfo, path);
			}

			using (Stream stream = executingAssembly.GetManifestResourceStream(path)) {
				if (stream == null)
					return null;

				byte[] assemblyRawBytes = new byte[stream.Length];
				stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);
				return Assembly.Load(assemblyRawBytes);
			}
		}

		/// <summary>Hook the AppDomain and TaskScheduler unhandled exception events.</summary>
		private void OnAppStartup(object sender, StartupEventArgs e) {
			// Catch exceptions not in a UI thread
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnAppDomainUnhandledException);
			TaskScheduler.UnobservedTaskException += OnTaskSchedulerUnobservedTaskException;
			WinFormsApplication.ThreadException += OnWinFormsThreadException;
		}

		/// <summary>Show an exception window for an exception that occurred in a dispatcher thread.</summary>
		private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
			if (e.Exception != lastException) {
				lastException = e.Exception;
				if (ErrorMessageBox.Show(e.Exception))
					Environment.Exit(0);
				e.Handled = true;
			}
		}

		/// <summary>Show an exception window for an exception that occurred in the AppDomain.</summary>
		private void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e) {
			if (e.ExceptionObject != lastException) {
				lastException = e.ExceptionObject;
				Dispatcher.Invoke(() => {
					if (ErrorMessageBox.Show(e.ExceptionObject))
						Environment.Exit(0);
				});
			}
		}

		/// <summary>Show an exception window for an exception that occurred in a task.</summary>
		private void OnTaskSchedulerUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e) {
			if (e.Exception != lastException) {
				lastException = e.Exception;
				Dispatcher.Invoke(() => {
					if (ErrorMessageBox.Show(e.Exception))
						Environment.Exit(0);
				});
			}
		}

		/// <summary>Show an exception window for an exception that occurred in winforms.</summary>
		private void OnWinFormsThreadException(object sender, ThreadExceptionEventArgs e) {
			if (e.Exception != lastException) {
				lastException = e.Exception;
				Dispatcher.Invoke(() => {
					if (ErrorMessageBox.Show(e.Exception))
						Environment.Exit(0);
				});
			}
		}

	}
}
