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
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Common.Content;
using ZeldaEditor.Undo;
using ZeldaWpf.Windows;

namespace ZeldaEditor.Windows {
	/// <summary>
	/// Interaction logic for AddNewLevelWindow.xaml
	/// </summary>
	public partial class AddNewLevelWindow : Window {

		private string id;
		private Point2I dimensions;
		private int layerCount;
		private Point2I roomSize;
		private Zone zone;
		private ActionCreateLevel action;
		private EditorControl editorControl;

		public AddNewLevelWindow(EditorControl editorControl) {
			InitializeComponent();

			this.editorControl = editorControl;

			InitRoomSizes();
			InitZones();

			textBoxLevelID.Focus();
		}

		private void InitRoomSizes() {
			ComboBoxItem item = new ComboBoxItem();
			Point2I size = GameSettings.ROOM_SIZE_SMALL;
			item.Tag = size;
			item.Content = "Small (" + size.X + " x " + size.Y + ")";
			comboBoxRoomSizes.Items.Add(item);

			item = new ComboBoxItem();
			size = GameSettings.ROOM_SIZE_LARGE;
			item.Tag = size;
			item.Content = "Large (" + size.X + " x " + size.Y + ")";
			comboBoxRoomSizes.Items.Add(item);

			comboBoxRoomSizes.SelectedIndex = 0;
		}

		private void InitZones() {
			foreach (var pair in ZeldaOracle.Common.Content.Resources.GetDictionary<Zone>()) {
				comboBoxZones.Items.Add(pair.Key);
			}
			comboBoxZones.SelectedIndex = 0;
		}

		private void OnAdd(object sender, RoutedEventArgs e) {
			string newID = textBoxLevelID.Text;
			if (string.IsNullOrWhiteSpace(newID)) {
				TriggerMessageBox.Show(this, MessageIcon.Warning,
					"Level ID cannot be empty or whitespace!", "Invalid ID");
				return;
			}
			else if (editorControl.World.ContainsLevel(newID)) {
				TriggerMessageBox.Show(this, MessageIcon.Warning,
					"A level with that ID already exists!", "ID Taken");
				return;
			}
			else {
				id = newID;
				dimensions = new Point2I(spinnerWidth.Value.Value, spinnerHeight.Value.Value);
				layerCount = 3;
				roomSize = (Point2I)(comboBoxRoomSizes.SelectedItem as ComboBoxItem).Tag;
				zone = ZeldaOracle.Common.Content.Resources.Get<Zone>((string)comboBoxZones.SelectedItem);
				Level level = new Level(id, dimensions, layerCount, roomSize, zone);
				action = new ActionCreateLevel(level);
				DialogResult = true;
				Close();
			}
		}

		public static ActionCreateLevel Show(Window owner, EditorControl editorControl) {
			AddNewLevelWindow window = new AddNewLevelWindow(editorControl);
			window.Owner = owner;
			var result = window.ShowDialog();
			if (result.HasValue && result.Value) {
				return window.action;
			}
			return null;
		}
	}
}
