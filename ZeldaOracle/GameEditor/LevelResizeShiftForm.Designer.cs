namespace ZeldaEditor {
	partial class LevelResizeShiftForm {
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
			this.labelWidth = new System.Windows.Forms.Label();
			this.numericLevelWidth = new System.Windows.Forms.NumericUpDown();
			this.numericLevelHeight = new System.Windows.Forms.NumericUpDown();
			this.labelHeight = new System.Windows.Forms.Label();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.numericLevelWidth)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericLevelHeight)).BeginInit();
			this.SuspendLayout();
			// 
			// labelWidth
			// 
			this.labelWidth.Location = new System.Drawing.Point(3, 16);
			this.labelWidth.Name = "labelWidth";
			this.labelWidth.Size = new System.Drawing.Size(74, 13);
			this.labelWidth.TabIndex = 1;
			this.labelWidth.Text = "Width:";
			this.labelWidth.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// numericLevelWidth
			// 
			this.numericLevelWidth.Location = new System.Drawing.Point(85, 14);
			this.numericLevelWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericLevelWidth.Name = "numericLevelWidth";
			this.numericLevelWidth.Size = new System.Drawing.Size(145, 20);
			this.numericLevelWidth.TabIndex = 1;
			this.numericLevelWidth.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
			this.numericLevelWidth.ValueChanged += new System.EventHandler(this.numericLevelWidth_ValueChanged);
			// 
			// numericLevelHeight
			// 
			this.numericLevelHeight.Location = new System.Drawing.Point(85, 40);
			this.numericLevelHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericLevelHeight.Name = "numericLevelHeight";
			this.numericLevelHeight.Size = new System.Drawing.Size(145, 20);
			this.numericLevelHeight.TabIndex = 2;
			this.numericLevelHeight.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
			this.numericLevelHeight.ValueChanged += new System.EventHandler(this.numericLevelHeight_ValueChanged);
			// 
			// labelHeight
			// 
			this.labelHeight.Location = new System.Drawing.Point(3, 42);
			this.labelHeight.Name = "labelHeight";
			this.labelHeight.Size = new System.Drawing.Size(74, 13);
			this.labelHeight.TabIndex = 1;
			this.labelHeight.Text = "Height:";
			this.labelHeight.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(155, 77);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 7;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.Location = new System.Drawing.Point(74, 77);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 6;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// LevelResizeShiftForm
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(242, 112);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.numericLevelHeight);
			this.Controls.Add(this.numericLevelWidth);
			this.Controls.Add(this.labelHeight);
			this.Controls.Add(this.labelWidth);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LevelResizeShiftForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Resize Level";
			((System.ComponentModel.ISupportInitialize)(this.numericLevelWidth)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericLevelHeight)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label labelWidth;
		private System.Windows.Forms.NumericUpDown numericLevelWidth;
		private System.Windows.Forms.NumericUpDown numericLevelHeight;
		private System.Windows.Forms.Label labelHeight;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
	}
}