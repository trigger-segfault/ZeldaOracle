using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaOracle.Common.Scripting;


namespace ZeldaEditor.ObjectsEditor.CustomControls {
	public class ObjectControlsTileDoor : CustomObjectEditorControl {

		private CheckBox checkBoxOpen;
		private CheckBox checkBoxRememberState;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ObjectControlsTileDoor() {
			checkBoxOpen			= AddCheckBox("Open");
			checkBoxRememberState	= AddCheckBox("Remember State");
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void SetupObject(IPropertyObject obj) {
			checkBoxOpen.Checked			= obj.Properties.Get("open", false);
			checkBoxRememberState.Checked	= obj.Properties.Get("remember_state", false);
		}

		public override void ApplyChanges(IPropertyObject obj) {
			obj.Properties.Set("open",				checkBoxOpen.Checked);
			obj.Properties.Set("remember_state",	checkBoxRememberState.Checked);
		}
	}
}
