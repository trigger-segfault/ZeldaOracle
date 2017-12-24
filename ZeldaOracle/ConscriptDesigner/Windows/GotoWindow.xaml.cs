using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ConscriptDesigner.Content;
using ConscriptDesigner.Control;

namespace ConscriptDesigner.Windows {
	/// <summary>
	/// Interaction logic for GotoWindow.xaml
	/// </summary>
	public partial class GotoWindow : Window {

		//-----------------------------------------------------------------------------
		// Static Members
		//-----------------------------------------------------------------------------

		private static int lastGoto = 0;


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		private bool suppressEvents;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the goto window.</summary>
		public GotoWindow(int lastLine) {
			suppressEvents = true;
			InitializeComponent();

			this.spinnerLine.Maximum = lastLine;
			this.spinnerLine.Value = (lastGoto <= lastLine ? lastGoto : 0);
			this.spinnerLine.Focus();
			suppressEvents = false;
		}


		//-----------------------------------------------------------------------------
		// Commands Execution
		//-----------------------------------------------------------------------------
		
		private void OnEscapeCloseCommand(object sender, ExecutedRoutedEventArgs e) {
			Close();
		}


		//-----------------------------------------------------------------------------
		// Commands Can Execute
		//-----------------------------------------------------------------------------

		private void CanAlwaysExecute(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = true;
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e) {
			if (DialogResult.HasValue && DialogResult.Value && spinnerLine.Value.HasValue)
				lastGoto = this.spinnerLine.Value.Value;
		}

		private void OnOK(object sender, RoutedEventArgs e) {
			DialogResult = true;
			Close();
		}
		
		private void OnLineChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
			if (suppressEvents) return;
			buttonGoto.IsEnabled = spinnerLine.Value.HasValue;
		}


		//-----------------------------------------------------------------------------
		// Showing
		//-----------------------------------------------------------------------------

		/// <summary>Shows the goto window.</summary>
		public static void Show(Window owner) {
			ContentScript activeScript = DesignerControl.GetActiveContentFile() as ContentScript;
			int lastLine = 0;
			if (activeScript.IsOpen)
				lastLine = activeScript.TextEditor.Document.Lines.Count;
			GotoWindow window = new GotoWindow(lastLine);
			window.Owner = owner;
			var result = window.ShowDialog();
			if (result.HasValue && result.Value && activeScript.IsOpen) {
				activeScript.GotoLocation(window.spinnerLine.Value.Value, 0);
			}
		}
	}
}
