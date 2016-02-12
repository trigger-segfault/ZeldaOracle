using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.Custom;


namespace ZeldaEditor.ObjectsEditor.CustomControls {
	public class ObjectControlsTileColorCube : CustomObjectEditorControl {

		private ComboBox comboBoxColor;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ObjectControlsTileColorCube() {
			
			comboBoxColor = AddComboBox("Orientation",
				"Blue-Yellow",
				"Blue-Red",
				"Yellow-Red",
				"Yellow-Blue",
				"Red-Blue",
				"Red-Yellow");
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void SetupObject(IPropertyObject obj) {
			comboBoxColor.SelectedIndex = obj.Properties.Get("orientation", 0);
		}

		public override void ApplyChanges(IPropertyObject obj) {
			obj.Properties.Set("orientation", comboBoxColor.SelectedIndex);
		}
	}
}
