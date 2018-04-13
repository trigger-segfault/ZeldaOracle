using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ConscriptDesigner.Windows {
	/// <summary>
	/// Interaction logic for SaveChangesWindow.xaml
	/// </summary>
	public partial class SaveChangesWindow : Window {

		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The returned message box result.</summary>
		private MessageBoxResult result;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the save changes window.</summary>
		private SaveChangesWindow(IEnumerable<string> files, bool reload) {
			InitializeComponent();
			foreach (var file in files) {
				listView.Items.Add(file);
			}

			if (reload) {
				button3.Visibility = Visibility.Collapsed;
				Title = "Reload Files";
				textBlockMessage.Text = "The following files have been modified " +
					"outside of the program. Would you like to reload them?";
			}
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		private void OnButtonClicked(object sender, RoutedEventArgs e) {
			result = (MessageBoxResult)Enum.Parse(typeof(MessageBoxResult), ((Button) sender).Tag as string);
			DialogResult = true;
			Close();
		}


		//-----------------------------------------------------------------------------
		// Showing
		//-----------------------------------------------------------------------------

		/// <summary>Shows the save changes window.</summary>
		public static MessageBoxResult Show(Window owner, IEnumerable<string> files, bool reload) {
			SaveChangesWindow window = new SaveChangesWindow(files, reload);
			window.Owner = owner;
			var result = window.ShowDialog();
			if (result.HasValue && result.Value)
				return window.result;
			return MessageBoxResult.Cancel;
		}
	}
}
