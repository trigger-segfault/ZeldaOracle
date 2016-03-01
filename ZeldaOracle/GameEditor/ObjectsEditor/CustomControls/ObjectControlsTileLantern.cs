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


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ObjectControlsTileLantern() {
			checkBoxLit = AddCheckBox("Lit");
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void SetupObject(IPropertyObject obj) {
			checkBoxLit.Checked = obj.Properties.Get("lit", true);
		}

		public override void ApplyChanges(IPropertyObject obj) {
			obj.Properties.Set("lit", checkBoxLit.Checked);
		}
	}
}
