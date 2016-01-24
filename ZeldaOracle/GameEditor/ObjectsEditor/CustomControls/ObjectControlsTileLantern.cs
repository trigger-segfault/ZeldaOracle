using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaOracle.Common.Scripting;


namespace ZeldaEditor.ObjectsEditor.CustomControls {
	public class ObjectControlsTileLantern : CustomObjectEditorControl {

		private CheckBox checkBoxLit;
		private CheckBox checkBoxRememberState;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ObjectControlsTileLantern() {
			checkBoxLit				= AddCheckBox("Lit");
			checkBoxRememberState	= AddCheckBox("Remember State");
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void SetupObject(IPropertyObject obj) {
			checkBoxLit.Checked = obj.Properties.Get("lit", true);
			checkBoxRememberState.Checked = obj.Properties.Get("remember_state", false);
		}

		public override void ApplyChanges(IPropertyObject obj) {
			obj.Properties.Set("lit", checkBoxLit.Checked);
			obj.Properties.Set("remember_state", checkBoxRememberState.Checked);
		}
	}
}
