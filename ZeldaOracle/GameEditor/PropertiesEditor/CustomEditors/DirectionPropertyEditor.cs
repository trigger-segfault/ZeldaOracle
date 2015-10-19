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

namespace ZeldaEditor.PropertiesEditor.CustomEditors {
	public class DirectionPropertyEditor : DropDownPropertyEditor {

		private static string[] DIRECTION_NAMES = new string[] {"Right", "Up", "Left", "Down"};


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public DirectionPropertyEditor() {

		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void CreateList(ListBox listBox, object value) {
			for (int i = 0; i < DIRECTION_NAMES.Length; i++)
				listBox.Items.Add(DIRECTION_NAMES[i]);
		}

		public override object OnItemSelected(ListBox listBox, int index, object value) {
			return index;
		}
	}
}
