using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using ZeldaEditor.Control;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Tiles.ActionTiles;
using ZeldaOracle.Game.Worlds;

namespace ZeldaEditor.PropertiesEditor.CustomEditors {
	public class WarpPropertyEditor : ComboBoxEditor {

		protected EditorControl EditorControl {
			get { return PropertyDescriptor.EditorControl; }
		}
		protected CustomPropertyDescriptor PropertyDescriptor {
			get { return PropertyItem.PropertyDescriptor as CustomPropertyDescriptor; }
		}

		protected override IValueConverter CreateValueConverter() {
			return new NoneStringConverter();
		}
		protected override IEnumerable CreateItemsSource(PropertyItem item) {
			Editor.DropDownOpened += OnDropDownOpened;
			return CreateItemsSource();
		}

		private void OnDropDownOpened(object sender, EventArgs e) {
			Editor.ItemsSource = CreateItemsSource();
		}

		private IEnumerable CreateItemsSource() {
			Property property = PropertyDescriptor.Property;
			World world = EditorControl.World;
			string levelID = PropertyDescriptor.PropertyObject.Properties.GetString(property.Documentation.EditorSubType, "");
			Level level = world.GetLevel(levelID);
			List<string> warps = new List<string>();
			warps.Add("(none)");
			if (level != null) {
				foreach (Room room in level.Rooms) {
					foreach (var actionTile in room.GetActionTiles(true)) {
						string id = actionTile.ID;
						if (actionTile.Type == typeof(WarpAction) && id != "" && !warps.Contains(id)) {
							warps.Add(id);
						}
					}
				}
			}
			return warps;
		}
	}
}
