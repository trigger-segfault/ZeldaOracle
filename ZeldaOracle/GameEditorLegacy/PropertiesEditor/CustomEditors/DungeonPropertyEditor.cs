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

namespace ZeldaEditor.PropertiesEditor.CustomEditors {
	public class DungeonPropertyEditor : DropDownPropertyEditor {


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public DungeonPropertyEditor() {

		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void CreateList(ListBox listBox, object value) {
			listBox.Items.Add("(none)");
			foreach (Area dungeon in EditorControl.World.GetAreas())
				listBox.Items.Add(dungeon.ID);
		}

		public override object OnItemSelected(ListBox listBox, int index, object value) {
			Area dungeon = null;
			if (index > 0)
				dungeon = EditorControl.World.GetArea((string) listBox.Items[index]);
			return (dungeon != null ? dungeon.ID : "");
		}
	}
}
