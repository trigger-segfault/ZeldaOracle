using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using ZeldaEditor.Control;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Tiles.EventTiles;
using ZeldaOracle.Game.Worlds;

namespace ZeldaEditor.PropertiesEditor.CustomEditors {
	public class WarpPropertyEditor : ComboBoxEditor {

		PropertyItem propertyItem;

		protected override IValueConverter CreateValueConverter() {
			return new NoneStringConverter();
		}
		protected override IEnumerable CreateItemsSource(PropertyItem item) {
			this.propertyItem = item;
			Editor.DropDownOpened += OnDropDownOpened;
			return CreateItemsSource();
		}

		private void OnDropDownOpened(object sender, EventArgs e) {
			Editor.ItemsSource = CreateItemsSource();
		}

		private IEnumerable CreateItemsSource() {
			CustomPropertyDescriptor descriptor = (CustomPropertyDescriptor)propertyItem.PropertyDescriptor;
			Property property = descriptor.Property;
			World world = EditorControl.Instance.World;
			string levelID = descriptor.PropertyObject.Properties.GetString(property.Documentation.EditorSubType, "");
			Level level = world.GetLevel(levelID);
			List<string> warps = new List<string>();
			warps.Add("(none)");
			if (level != null) {
				foreach (Room room in level.Rooms) {
					foreach (var eventTile in room.EventData) {
						string id = eventTile.ID;
						if (eventTile.Type == typeof(WarpEvent) && id != "" && !warps.Contains(id)) {
							warps.Add(id);
						}
					}
				}
			}
			return warps;
		}
	}
}
