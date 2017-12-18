using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Path = System.IO.Path;
using System.IO;
using ConscriptDesigner.Util;

namespace ConscriptDesigner.Windows {
	/// <summary>
	/// Interaction logic for RenameFileWindow.xaml
	/// </summary>
	public partial class RenameFileWindow : Window {
		
		private string directory;
		private string extension;
		private string name;
		private ContentRoot root;

		public RenameFileWindow(string action, string title, string name, string directory, ContentRoot root) {
			InitializeComponent();

			this.directory = directory;
			this.root = root;
			this.extension = Path.GetExtension(name);
			this.name = Path.GetFileNameWithoutExtension(name);

			textBox.Text = this.name;
			textBox.Focus();
			textBox.SelectAll();

			buttonRename.Content = action;
			Title = title;
		}

		private void OnOK(object sender = null, RoutedEventArgs e = null) {
			string newName = textBox.Text + extension;
			string fullPath = Path.Combine(root.ProjectDirectory, directory, newName);
			// Ignore everything if we end up with the same name
			if (string.Compare(name, textBox.Text, true) == 0) {
				DialogResult = true;
				Close();
			}
			else if (string.IsNullOrWhiteSpace(newName)) {
				TriggerMessageBox.Show(this, MessageIcon.Warning,
					"Filename cannot be empty or whitespace!", "Invalid Filename");
				return;
			}
			else if (!PathHelper.IsValidName(newName)) {
				TriggerMessageBox.Show(this, MessageIcon.Warning,
					"Filename contains invalid characters!", "Invalid Filename");
				return;
			}
			else if (root.Contains(Path.Combine(directory, newName))) {
				TriggerMessageBox.Show(this, MessageIcon.Warning,
					"A project file with that name already exists!", "Invalid Filename");
				return;
			}
			else if (File.Exists(fullPath)) {
				TriggerMessageBox.Show(this, MessageIcon.Warning,
					"A file with that name already exists!", "Invalid Filename");
				return;
			}
			else if (Directory.Exists(fullPath)) {
				TriggerMessageBox.Show(this, MessageIcon.Warning,
					"A directory with that name already exists!", "Invalid Filename");
				return;
			}
			else {
				DialogResult = true;
				Close();
			}
		}

		public static string Show(Window owner, string action, string title, string name, string directory, ContentRoot root) {
			RenameFileWindow window = new RenameFileWindow(action, title, name, directory, root);
			window.Owner = owner;
			var result = window.ShowDialog();
			if (result.HasValue && result.Value) {
				return window.textBox.Text + window.extension;
			}
			return null;
		}
	}
}
