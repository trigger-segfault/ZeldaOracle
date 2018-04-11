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
using System.Windows;
using ZeldaWpf.Controls;

namespace ZeldaEditor.PropertiesEditor.CustomEditors {
	public class AreaPropertyEditor : ComboBoxEditor {

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
			World world = EditorControl.World;
			string[] areaIDs = new string[world.AreaCount + 1];
			areaIDs[0] = "(none)";
			for (int i = 0; i < world.AreaCount; i++) {
				areaIDs[i + 1] = world.GetAreaAt(i).ID;
			}
			return areaIDs;
		}
	}
}
