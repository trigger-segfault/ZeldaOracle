using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaOracle.Common.Scripting;


namespace ZeldaEditor.ObjectsEditor.CustomControls {
	public class ObjectControlsTileReward : CustomObjectEditorControl {
		private RewardChooser rewardChooser;
		private Label label1;
		private CheckBox checkBoxSpawnFromCeiling;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ObjectControlsTileReward() {
			InitializeComponent();
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			rewardChooser.Setup(EditorControl.RewardManager);
		}

		public override void SetupObject(IPropertyObject obj) {
			checkBoxSpawnFromCeiling.Checked = obj.Properties.Get("spawn_from_ceiling", false);
			rewardChooser.RewardName = obj.Properties.Get("reward", "");
		}

		public override void ApplyChanges(IPropertyObject obj) {
			obj.Properties.Set("spawn_from_ceiling", checkBoxSpawnFromCeiling.Checked);
			obj.Properties.Set("reward", rewardChooser.RewardName);
		}


		//-----------------------------------------------------------------------------
		// Initialize Component (Auto-Generated)
		//-----------------------------------------------------------------------------

		private void InitializeComponent() {
			this.checkBoxSpawnFromCeiling = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.rewardChooser = new ZeldaEditor.ObjectsEditor.CustomControls.RewardChooser();
			this.SuspendLayout();
			// 
			// checkBoxSpawnFromCeiling
			// 
			this.checkBoxSpawnFromCeiling.AutoSize = true;
			this.checkBoxSpawnFromCeiling.Location = new System.Drawing.Point(6, 3);
			this.checkBoxSpawnFromCeiling.Name = "checkBoxSpawnFromCeiling";
			this.checkBoxSpawnFromCeiling.Size = new System.Drawing.Size(200, 17);
			this.checkBoxSpawnFromCeiling.TabIndex = 2;
			this.checkBoxSpawnFromCeiling.Text = "Drop from ceiling";
			this.checkBoxSpawnFromCeiling.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(47, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Reward:";
			// 
			// rewardChooser
			// 
			this.rewardChooser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.rewardChooser.Location = new System.Drawing.Point(3, 48);
			this.rewardChooser.Name = "rewardChooser";
			this.rewardChooser.RewardName = "";
			this.rewardChooser.Size = new System.Drawing.Size(267, 191);
			this.rewardChooser.TabIndex = 0;
			// 
			// ObjectControlsTileReward
			// 
			this.AutoSize = false;
			this.Controls.Add(this.checkBoxSpawnFromCeiling);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.rewardChooser);
			this.Name = "ObjectControlsTileReward";
			this.Size = new System.Drawing.Size(273, 242);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
	}
}
