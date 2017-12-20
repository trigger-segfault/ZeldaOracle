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
using ConscriptDesigner.Anchorables;

namespace ConscriptDesigner.Windows {
	/// <summary>
	/// Interaction logic for SaveChangesWindow.xaml
	/// </summary>
	public partial class SaveChangesWindow : Window {
		private MessageBoxResult result;

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

		private void OnButtonClicked(object sender, RoutedEventArgs e) {
			result = (MessageBoxResult)Enum.Parse(typeof(MessageBoxResult), ((Button) sender).Tag as string);
			DialogResult = true;
			Close();
		}

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
