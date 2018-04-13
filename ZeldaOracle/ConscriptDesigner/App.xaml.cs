using System.Diagnostics;
using ZeldaWpf;
using ZeldaWpf.Util;
using ZeldaWpf.Windows;

namespace ConscriptDesigner {
	/// <summary>The application class.</summary>
	public partial class App : ZeldaWpfApp {

		/// <summary>Constructs the app to do root initialization.</summary>
		public App() {
			// Prevent System.Windows.Data Error: 4
			PresentationTraceSources.DataBindingSource.Switch.Level =
				SourceLevels.Critical;

			// Setup the Error Message Box settings
			ErrorMessageBox.ProgramName = "Conscript Designer";
			ErrorMessageBox.ErrorIcon =
				BitmapFactory.FromResource("Resources/Icons/AppError.ico");
		}
	}
}
