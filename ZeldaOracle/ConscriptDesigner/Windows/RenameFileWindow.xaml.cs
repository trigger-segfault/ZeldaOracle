using System.IO;
using System.Windows;
using ConscriptDesigner.Content;
using ZeldaOracle.Common.Util;
using ZeldaWpf.Windows;

namespace ConscriptDesigner.Windows {
	/// <summary>
	/// Interaction logic for RenameFileWindow.xaml
	/// </summary>
	public partial class RenameFileWindow : Window {

		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The directory of the file.</summary>
		private string directory;
		/// <summary>The extension of the file.</summary>
		private string extension;
		/// <summary>The original file name.</summary>
		private string name;
		/// <summary>The root content project.</summary>
		private ContentRoot root;


		//-----------------------------------------------------------------------------
		// Showing
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the rename window.</summary>
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


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

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


		//-----------------------------------------------------------------------------
		// Showing
		//-----------------------------------------------------------------------------

		/// <summary>Shows the rename window.</summary>
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
