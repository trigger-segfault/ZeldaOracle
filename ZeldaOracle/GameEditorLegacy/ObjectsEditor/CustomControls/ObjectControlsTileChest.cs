using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaOracle.Common.Scripting;


namespace ZeldaEditor.ObjectsEditor.CustomControls {
	public class ObjectControlsTileChest : CustomObjectEditorControl {

		private RewardChooser rewardChooser;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ObjectControlsTileChest() {
			rewardChooser = new RewardChooser();
			rewardChooser.Dock = DockStyle.Fill;
			Controls.Add(rewardChooser);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			rewardChooser.Setup(EditorControl.RewardManager);
		}

		public override void SetupObject(IPropertyObject obj) {
			rewardChooser.RewardName = obj.Properties.Get("reward", "");
		}

		public override void ApplyChanges(IPropertyObject obj) {
			obj.Properties.Set("reward", rewardChooser.RewardName);
		}
	}
}
