using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaOracle.Common.Scripting;


namespace ZeldaEditor.ObjectsEditor.CustomControls {
	public class ObjectControlsTileCrossingGate : CustomObjectEditorControl {

		private CheckBox checkBoxRaised;
		private ComboBox comboBoxFaceDirection;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ObjectControlsTileCrossingGate() {
			checkBoxRaised			= AddCheckBox("Raised");
			comboBoxFaceDirection	= AddComboBox("Facing Direction", "Right", "Left");
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void SetupObject(IPropertyObject obj) {
			checkBoxRaised.Checked				= obj.Properties.Get("raised", false);
			comboBoxFaceDirection.SelectedIndex	= (obj.Properties.Get("face_left", false) ? 1 : 0);
		}

		public override void ApplyChanges(IPropertyObject obj) {
			obj.Properties.Set("raised",	checkBoxRaised.Checked);
			obj.Properties.Set("face_left",	comboBoxFaceDirection.SelectedIndex == 1);
		}
	}
}
