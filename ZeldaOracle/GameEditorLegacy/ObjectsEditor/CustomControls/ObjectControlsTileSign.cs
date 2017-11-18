using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaOracle.Common.Scripting;


namespace ZeldaEditor.ObjectsEditor.CustomControls {
	public class ObjectControlsTileSign : CustomObjectEditorControl {
		private TextBox textBox1;
		private Label label1;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ObjectControlsTileSign() {
			InitializeComponent();
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
		}

		public override void SetupObject(IPropertyObject obj) {
			textBox1.Text = obj.Properties.Get("text", "");
		}

		public override void ApplyChanges(IPropertyObject obj) {
			obj.Properties.Set("text", textBox1.Text);
		}


		//-----------------------------------------------------------------------------
		// Initialize Component (Auto-Generated)
		//-----------------------------------------------------------------------------

		private void InitializeComponent() {
			this.label1 = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(31, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Text:";
			// 
			// textBox1
			// 
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1.Location = new System.Drawing.Point(3, 26);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(264, 126);
			this.textBox1.TabIndex = 3;
			// 
			// ObjectControlsTileSign
			// 
			this.AutoSize = false;
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.label1);
			this.Name = "ObjectControlsTileSign";
			this.Size = new System.Drawing.Size(273, 242);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
	}
}
