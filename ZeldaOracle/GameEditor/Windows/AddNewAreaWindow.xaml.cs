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
using ZeldaEditor.Control;
using ZeldaEditor.Undo;
using ZeldaOracle.Game.Worlds;
using ZeldaWpf.Windows;

namespace ZeldaEditor.Windows {
	/// <summary>
	/// Interaction logic for AddNewAreaWindow.xaml
	/// </summary>
	public partial class AddNewAreaWindow : Window {
		private ActionCreateArea action;
		private EditorControl editorControl;

		public AddNewAreaWindow(EditorControl editorControl) {
			InitializeComponent();

			this.editorControl = editorControl;
			textBoxAreaID.Focus();
		}


		private void OnAdd(object sender, RoutedEventArgs e) {
			string newID = textBoxAreaID.Text;
			if (string.IsNullOrWhiteSpace(newID)) {
				TriggerMessageBox.Show(this, MessageIcon.Warning,
					"Area ID cannot be empty or whitespace!", "Invalid ID");
				return;
			}
			else if (editorControl.World.ContainsArea(newID)) {
				TriggerMessageBox.Show(this, MessageIcon.Warning,
					"An area with that ID already exists!", "ID Taken");
				return;
			}
			else {
				string name = textBoxAreaName.Text;
				Area area = new Area(newID, name);
				action = new ActionCreateArea(area);
				DialogResult = true;
				Close();
			}
		}

		public static ActionCreateArea Show(Window owner, EditorControl editorControl) {
			AddNewAreaWindow window = new AddNewAreaWindow(editorControl);
			window.Owner = owner;
			var result = window.ShowDialog();
			if (result.HasValue && result.Value) {
				return window.action;
			}
			return null;
		}
	}

}
