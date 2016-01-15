using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Tiles;


namespace ZeldaEditor.ObjectsEditor.CustomControls {
	public class ObjectControlsTileColorJumpPad : CustomObjectEditorControl {

		private CheckBox checkBoxRememberState;
		private ComboBox comboBoxColor;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ObjectControlsTileColorJumpPad() {
			checkBoxRememberState	= AddCheckBox("Remember State");
			comboBoxColor			= AddComboBox("Color",
				new ComboBoxItem<PuzzleColor>(PuzzleColor.Red,		"Red"),
				new ComboBoxItem<PuzzleColor>(PuzzleColor.Yellow,	"Yellow"),
				new ComboBoxItem<PuzzleColor>(PuzzleColor.Blue,		"Blue"));
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void SetupObject(IPropertyObject obj) {
			checkBoxRememberState.Checked = obj.Properties.Get("remember_state", false);
			ComboBoxItem<PuzzleColor>.SelectComboBoxItem(comboBoxColor, 
				(PuzzleColor) obj.Properties.Get("color", (int) PuzzleColor.None));
		}

		public override void ApplyChanges(IPropertyObject obj) {
			obj.Properties.Set("remember_state", checkBoxRememberState.Checked);
			obj.Properties.Set("color", (int) ComboBoxItem<PuzzleColor>.GetComboBoxValue(comboBoxColor));
		}
	}
}
