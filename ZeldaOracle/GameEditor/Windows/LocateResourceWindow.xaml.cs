using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ZeldaOracle.Game.Worlds;
using ZeldaResources = ZeldaOracle.Common.Content.Resources;

namespace ZeldaEditor.Windows {
	/// <summary>
	/// Interaction logic for LocateResourceWindow.xaml
	/// </summary>
	public partial class LocateResourceWindow : Window {

		private LocateResourceEventArgs args;
		private List<string> orderedList;
		private string filter = "";
		private List<string> filteredList;

		public LocateResourceWindow(LocateResourceEventArgs args) {
			InitializeComponent();
			this.args = args;

			this.textBlockLocate.Text = "Locate " + args.Type.Name + ": " + args.OldName;

			orderedList = new List<string>();
			orderedList.AddRange(ZeldaResources.GetDictionaryKeys(args.Type));
			orderedList.Sort();

			filter = "";
			filteredList = orderedList;
			listView.ItemsSource = filteredList;

			textBoxSearch.Focus();
		}

		private void OnSearchTextChanged(object sender, TextChangedEventArgs e) {
			filter = textBoxSearch.Text;
			if (string.IsNullOrEmpty(filter)) {
				filteredList = orderedList;
			}
			else {
				filteredList = new List<string>();
				foreach (string key in orderedList) {
					if (key.Contains(filter))
						filteredList.Add(key);
				}
			}
			listView.ItemsSource = filteredList;
		}


		public static void Show(Window owner, LocateResourceEventArgs args) {
			LocateResourceWindow window = new LocateResourceWindow(args);
			window.Owner = owner;
			window.ShowDialog();
		}

		private void OnChoose(object sender, RoutedEventArgs e) {
			args.NewName = (string) listView.SelectedItem;
			DialogResult = true;
			Close();
		}

		private void OnRemove(object sender, RoutedEventArgs e) {
			args.NewName = null;
			DialogResult = true;
			Close();
		}

		private void OnRemoveRemaining(object sender, RoutedEventArgs e) {
			args.SkipRemaining = true;
			args.NewName = null;
			DialogResult = true;
			Close();
		}

		private void OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
			buttonChoose.IsEnabled = (listView.SelectedIndex != -1);
		}
	}
}
