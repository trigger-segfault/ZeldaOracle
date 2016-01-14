using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaOracle.Common.Scripting;


namespace ZeldaEditor.ObjectsEditor.CustomControls {
	public class ObjectControlsTileLever : CustomObjectEditorControl {

		private CheckBox checkBoxSwitchOnce;
		private CheckBox checkBoxRememberState;
		private ComboBox comboBoxColor;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ObjectControlsTileLever() {
			checkBoxSwitchOnce		= AddCheckBox("Switch Once");
			checkBoxRememberState	= AddCheckBox("Remember State");
			comboBoxColor			= AddComboBox("Switch State", "Left", "Right");
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void SetupObject(IPropertyObject obj) {
			checkBoxSwitchOnce.Checked		= obj.Properties.Get("switch_once", false);
			checkBoxRememberState.Checked	= obj.Properties.Get("remember_state", false);
			comboBoxColor.SelectedIndex		= obj.Properties.Get("switch_state", false) ? 1 : 0;
		}

		public override void ApplyChanges(IPropertyObject obj) {
			obj.Properties.Set("switch_once",		checkBoxSwitchOnce.Checked);
			obj.Properties.Set("remember_state",	checkBoxRememberState.Checked);
			obj.Properties.Set("switch_state",		comboBoxColor.SelectedIndex == 1);
		}
	}
}
