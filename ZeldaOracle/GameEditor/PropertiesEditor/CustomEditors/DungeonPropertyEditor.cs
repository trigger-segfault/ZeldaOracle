using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ZeldaEditor.Control;
using ZeldaOracle.Game;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Worlds;
using System.Collections;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using System.Windows.Data;

namespace ZeldaEditor.PropertiesEditor.CustomEditors {
	public class DungeonPropertyEditor : ComboBoxEditor {

		protected override IValueConverter CreateValueConverter() {
			return new NoneStringConverter();
		}
		protected override IEnumerable CreateItemsSource(PropertyItem item) {
			World world = EditorControl.Instance.World;
			string[] dungeonIds = new string[world.DungeonCount + 1];
			dungeonIds[0] = "(none)";
			for (int i = 0; i < world.DungeonCount; i++) {
				dungeonIds[i + 1] = world.GetDungeonAt(i).ID;
			}
			return dungeonIds;
		}
	}
}
