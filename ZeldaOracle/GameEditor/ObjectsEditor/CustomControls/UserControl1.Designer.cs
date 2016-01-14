namespace ZeldaEditor.ObjectsEditor.CustomControls {
	partial class UserControl1 {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.rewardChooser1 = new ZeldaEditor.ObjectsEditor.CustomControls.RewardChooser();
			this.SuspendLayout();
			// 
			// rewardChooser1
			// 
			this.rewardChooser1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rewardChooser1.Location = new System.Drawing.Point(70, 44);
			this.rewardChooser1.Name = "rewardChooser1";
			this.rewardChooser1.RewardName = "";
			this.rewardChooser1.Size = new System.Drawing.Size(234, 200);
			this.rewardChooser1.TabIndex = 0;
			// 
			// UserControl1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = false;
			this.Controls.Add(this.rewardChooser1);
			this.Name = "UserControl1";
			this.Size = new System.Drawing.Size(354, 306);
			this.ResumeLayout(false);

		}

		#endregion

		private RewardChooser rewardChooser1;
	}
}
