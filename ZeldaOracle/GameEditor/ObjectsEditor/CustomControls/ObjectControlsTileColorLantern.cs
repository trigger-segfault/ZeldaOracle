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
	public class ObjectControlsTileColorLantern : CustomObjectEditorControl {

		private ComboBox comboBoxColor;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ObjectControlsTileColorLantern() {
			
			comboBoxColor = AddComboBox("Flame Color",
				new ComboBoxItem<PuzzleColor>(PuzzleColor.None,		"None"),
				new ComboBoxItem<PuzzleColor>(PuzzleColor.Red,		"Red"),
				new ComboBoxItem<PuzzleColor>(PuzzleColor.Yellow,	"Yellow"),
				new ComboBoxItem<PuzzleColor>(PuzzleColor.Blue,		"Blue"));
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void SetupObject(IPropertyObject obj) {
			ComboBoxItem<PuzzleColor>.SelectComboBoxItem(comboBoxColor, 
				(PuzzleColor) obj.Properties.Get("color", (int) PuzzleColor.None));
		}

		public override void ApplyChanges(IPropertyObject obj) {
			obj.Properties.Set("color", (int) ComboBoxItem<PuzzleColor>.GetComboBoxValue(comboBoxColor));
		}
	}
}
