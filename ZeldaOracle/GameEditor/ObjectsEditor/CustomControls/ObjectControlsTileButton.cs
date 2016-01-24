using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaOracle.Common.Scripting;


namespace ZeldaEditor.ObjectsEditor.CustomControls {
	public class ObjectControlsTileButton : CustomObjectEditorControl {

		private CheckBox checkBoxPressed;
		private CheckBox checkBoxStayPressed;
		private CheckBox checkBoxRememberState;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ObjectControlsTileButton() {
			checkBoxPressed			= AddCheckBox("Pressed");
			checkBoxStayPressed		= AddCheckBox("Stay Pressed");
			checkBoxRememberState	= AddCheckBox("Remember State");
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void SetupObject(IPropertyObject obj) {
			checkBoxPressed.Checked			= obj.Properties.Get("pressed", false);
			checkBoxStayPressed.Checked		= !obj.Properties.Get("releasable", true);
			checkBoxRememberState.Checked	= obj.Properties.Get("remember_state", false);
		}

		public override void ApplyChanges(IPropertyObject obj) {
			obj.Properties.Set("pressed",			checkBoxPressed.Checked);
			obj.Properties.Set("releasable",		!checkBoxStayPressed.Checked);
			obj.Properties.Set("remember_state",	checkBoxRememberState.Checked);
		}
	}
}
