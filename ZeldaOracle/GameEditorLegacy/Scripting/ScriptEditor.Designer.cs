namespace ZeldaEditor.Scripting {
	partial class ScriptEditor {
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.buttonDone = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.buttonUndo = new System.Windows.Forms.ToolStripButton();
			this.buttonRedo = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.buttonCut = new System.Windows.Forms.ToolStripButton();
			this.buttonCopy = new System.Windows.Forms.ToolStripButton();
			this.buttonPaste = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
			this.textBoxName = new System.Windows.Forms.ToolStripTextBox();
			this.panelCode = new System.Windows.Forms.Panel();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.labelLineNumber = new System.Windows.Forms.ToolStripStatusLabel();
			this.labelColumnNumber = new System.Windows.Forms.ToolStripStatusLabel();
			this.statusStripErrorMessage = new System.Windows.Forms.StatusStrip();
			this.labelErrorMessage = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStrip1.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.statusStripErrorMessage.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.BackColor = System.Drawing.SystemColors.Control;
			this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonDone,
            this.toolStripSeparator1,
            this.buttonUndo,
            this.buttonRedo,
            this.toolStripSeparator3,
            this.buttonCut,
            this.buttonCopy,
            this.buttonPaste,
            this.toolStripSeparator2,
            this.toolStripLabel1,
            this.textBoxName});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(715, 33);
			this.toolStrip1.TabIndex = 4;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// buttonDone
			// 
			this.buttonDone.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonDone.Image = global::ZeldaEditor.ResourceProperties.Resources.tick;
			this.buttonDone.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonDone.Name = "buttonDone";
			this.buttonDone.Size = new System.Drawing.Size(23, 30);
			this.buttonDone.Text = "buttonDone";
			this.buttonDone.ToolTipText = "OK, Save changes";
			this.buttonDone.Click += new System.EventHandler(this.buttonDone_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 33);
			// 
			// buttonUndo
			// 
			this.buttonUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonUndo.Image = global::ZeldaEditor.ResourceProperties.Resources.arrow_circle_225_left;
			this.buttonUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonUndo.Name = "buttonUndo";
			this.buttonUndo.Size = new System.Drawing.Size(23, 30);
			this.buttonUndo.Text = "buttonUndo";
			this.buttonUndo.Click += new System.EventHandler(this.buttonUndo_Click);
			// 
			// buttonRedo
			// 
			this.buttonRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonRedo.Image = global::ZeldaEditor.ResourceProperties.Resources.arrow_circle_315;
			this.buttonRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonRedo.Name = "buttonRedo";
			this.buttonRedo.Size = new System.Drawing.Size(23, 30);
			this.buttonRedo.Text = "buttonRedo";
			this.buttonRedo.Click += new System.EventHandler(this.buttonRedo_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 33);
			// 
			// buttonCut
			// 
			this.buttonCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonCut.Image = global::ZeldaEditor.ResourceProperties.Resources.scissors;
			this.buttonCut.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonCut.Name = "buttonCut";
			this.buttonCut.Size = new System.Drawing.Size(23, 30);
			this.buttonCut.Text = "buttonCut";
			this.buttonCut.Click += new System.EventHandler(this.buttonCut_Click);
			// 
			// buttonCopy
			// 
			this.buttonCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonCopy.Image = global::ZeldaEditor.ResourceProperties.Resources.document_copy;
			this.buttonCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonCopy.Name = "buttonCopy";
			this.buttonCopy.Size = new System.Drawing.Size(23, 30);
			this.buttonCopy.Text = "buttonCopy";
			this.buttonCopy.Click += new System.EventHandler(this.buttonCopy_Click);
			// 
			// buttonPaste
			// 
			this.buttonPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonPaste.Image = global::ZeldaEditor.ResourceProperties.Resources.clipboard_paste;
			this.buttonPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonPaste.Name = "buttonPaste";
			this.buttonPaste.Size = new System.Drawing.Size(23, 30);
			this.buttonPaste.Text = "buttonPaste";
			this.buttonPaste.Click += new System.EventHandler(this.buttonPaste_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 33);
			// 
			// toolStripLabel1
			// 
			this.toolStripLabel1.Name = "toolStripLabel1";
			this.toolStripLabel1.Size = new System.Drawing.Size(42, 30);
			this.toolStripLabel1.Text = "Name:";
			// 
			// textBoxName
			// 
			this.textBoxName.Margin = new System.Windows.Forms.Padding(1, 5, 1, 5);
			this.textBoxName.Name = "textBoxName";
			this.textBoxName.Size = new System.Drawing.Size(150, 23);
			this.textBoxName.ToolTipText = "The name of the script";
			this.textBoxName.TextChanged += new System.EventHandler(this.textBoxName_TextChanged);
			// 
			// panelCode
			// 
			this.panelCode.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panelCode.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelCode.Location = new System.Drawing.Point(0, 33);
			this.panelCode.Name = "panelCode";
			this.panelCode.Size = new System.Drawing.Size(715, 369);
			this.panelCode.TabIndex = 5;
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.labelLineNumber,
            this.labelColumnNumber});
			this.statusStrip1.Location = new System.Drawing.Point(0, 424);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(715, 22);
			this.statusStrip1.TabIndex = 1;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// labelLineNumber
			// 
			this.labelLineNumber.Name = "labelLineNumber";
			this.labelLineNumber.Size = new System.Drawing.Size(38, 17);
			this.labelLineNumber.Text = "Line 4";
			// 
			// labelColumnNumber
			// 
			this.labelColumnNumber.Name = "labelColumnNumber";
			this.labelColumnNumber.Size = new System.Drawing.Size(34, 17);
			this.labelColumnNumber.Text = "Col 4";
			// 
			// statusStripErrorMessage
			// 
			this.statusStripErrorMessage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.labelErrorMessage});
			this.statusStripErrorMessage.Location = new System.Drawing.Point(0, 402);
			this.statusStripErrorMessage.Name = "statusStripErrorMessage";
			this.statusStripErrorMessage.Size = new System.Drawing.Size(715, 22);
			this.statusStripErrorMessage.SizingGrip = false;
			this.statusStripErrorMessage.TabIndex = 2;
			this.statusStripErrorMessage.Text = "statusStrip2";
			this.statusStripErrorMessage.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.statusStripErrorMessage_ItemClicked);
			// 
			// labelErrorMessage
			// 
			this.labelErrorMessage.Image = global::ZeldaEditor.ResourceProperties.Resources.exclamation;
			this.labelErrorMessage.Name = "labelErrorMessage";
			this.labelErrorMessage.Size = new System.Drawing.Size(76, 17);
			this.labelErrorMessage.Text = "Error label";
			// 
			// ScriptEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(715, 446);
			this.Controls.Add(this.panelCode);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.statusStripErrorMessage);
			this.Controls.Add(this.statusStrip1);
			this.MinimumSize = new System.Drawing.Size(300, 200);
			this.Name = "ScriptEditor";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Script Editor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ScriptEditor_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ScriptEditor_FormClosed);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.statusStripErrorMessage.ResumeLayout(false);
			this.statusStripErrorMessage.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.Panel panelCode;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripLabel toolStripLabel1;
		private System.Windows.Forms.ToolStripTextBox textBoxName;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel labelLineNumber;
		private System.Windows.Forms.ToolStripStatusLabel labelColumnNumber;
		private System.Windows.Forms.StatusStrip statusStripErrorMessage;
		private System.Windows.Forms.ToolStripStatusLabel labelErrorMessage;
		private System.Windows.Forms.ToolStripButton buttonDone;
		private System.Windows.Forms.ToolStripButton buttonPaste;
		private System.Windows.Forms.ToolStripButton buttonCopy;
		private System.Windows.Forms.ToolStripButton buttonCut;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripButton buttonUndo;
		private System.Windows.Forms.ToolStripButton buttonRedo;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;

	}
}