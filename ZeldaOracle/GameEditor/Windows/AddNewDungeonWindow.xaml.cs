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
using ZeldaOracle.Game.Worlds;

namespace ZeldaEditor.Windows {
	/// <summary>
	/// Interaction logic for AddNewDungeonWindow.xaml
	/// </summary>
	public partial class AddNewDungeonWindow : Window {
		private Dungeon dungeon;
		private EditorControl editorControl;

		public AddNewDungeonWindow(EditorControl editorControl) {
			InitializeComponent();

			this.editorControl = editorControl;
		}


		private void OnAdd(object sender, RoutedEventArgs e) {
			string newID = textBoxDungeonID.Text;
			if (string.IsNullOrWhiteSpace(newID)) {
				TriggerMessageBox.Show(this, MessageIcon.Warning,
					"Dungeon ID cannot be empty or whitespace!", "Invalid ID");
				return;
			}
			else if (editorControl.World.ContainsDungeon(newID)) {
				TriggerMessageBox.Show(this, MessageIcon.Warning,
					"A dungeon with that ID already exists!", "ID Taken");
				return;
			}
			else {
				string name = textBoxDungeonName.Text;
				dungeon = new Dungeon(newID, name);
				DialogResult = true;
				Close();
			}
		}

		public static Dungeon Show(Window owner, EditorControl editorControl) {
			AddNewDungeonWindow window = new AddNewDungeonWindow(editorControl);
			window.Owner = owner;
			var result = window.ShowDialog();
			if (result.HasValue && result.Value) {
				return window.dungeon;
			}
			return null;
		}
	}

}
