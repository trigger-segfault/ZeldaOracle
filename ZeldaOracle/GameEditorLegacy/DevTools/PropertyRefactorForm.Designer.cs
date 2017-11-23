namespace ZeldaEditor.DevTools {
	partial class PropertyRefactorForm {
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
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxReplaceName = new System.Windows.Forms.TextBox();
			this.textBoxFindName = new System.Windows.Forms.TextBox();
			this.comboBoxLookIn = new System.Windows.Forms.ComboBox();
			this.buttonReplaceAll = new System.Windows.Forms.Button();
			this.buttonFindAll = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(5, 87);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(45, 13);
			this.label3.TabIndex = 3;
			this.label3.Text = "Look in:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(5, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(147, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Replace property names with:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(5, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(130, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Find properties with name:";
			// 
			// textBoxReplaceName
			// 
			this.textBoxReplaceName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxReplaceName.Location = new System.Drawing.Point(8, 64);
			this.textBoxReplaceName.Name = "textBoxReplaceName";
			this.textBoxReplaceName.Size = new System.Drawing.Size(223, 20);
			this.textBoxReplaceName.TabIndex = 1;
			// 
			// textBoxFindName
			// 
			this.textBoxFindName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxFindName.Location = new System.Drawing.Point(8, 25);
			this.textBoxFindName.Name = "textBoxFindName";
			this.textBoxFindName.Size = new System.Drawing.Size(223, 20);
			this.textBoxFindName.TabIndex = 0;
			// 
			// comboBoxLookIn
			// 
			this.comboBoxLookIn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxLookIn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxLookIn.FormattingEnabled = true;
			this.comboBoxLookIn.Location = new System.Drawing.Point(8, 103);
			this.comboBoxLookIn.Name = "comboBoxLookIn";
			this.comboBoxLookIn.Size = new System.Drawing.Size(223, 21);
			this.comboBoxLookIn.TabIndex = 2;
			// 
			// buttonReplaceAll
			// 
			this.buttonReplaceAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonReplaceAll.Location = new System.Drawing.Point(156, 186);
			this.buttonReplaceAll.Name = "buttonReplaceAll";
			this.buttonReplaceAll.Size = new System.Drawing.Size(75, 23);
			this.buttonReplaceAll.TabIndex = 4;
			this.buttonReplaceAll.Text = "Replace All";
			this.buttonReplaceAll.UseVisualStyleBackColor = true;
			this.buttonReplaceAll.Click += new System.EventHandler(this.buttonReplaceAll_Click);
			// 
			// buttonFindAll
			// 
			this.buttonFindAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonFindAll.Location = new System.Drawing.Point(75, 186);
			this.buttonFindAll.Name = "buttonFindAll";
			this.buttonFindAll.Size = new System.Drawing.Size(75, 23);
			this.buttonFindAll.TabIndex = 12;
			this.buttonFindAll.Text = "Find All";
			this.buttonFindAll.UseVisualStyleBackColor = true;
			this.buttonFindAll.Click += new System.EventHandler(this.buttonFindAll_Click);
			// 
			// PropertyRefactorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(243, 221);
			this.Controls.Add(this.buttonFindAll);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBoxReplaceName);
			this.Controls.Add(this.textBoxFindName);
			this.Controls.Add(this.comboBoxLookIn);
			this.Controls.Add(this.buttonReplaceAll);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "PropertyRefactorForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Refactor Properties";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PropertyRefactorForm_KeyDown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBoxReplaceName;
		private System.Windows.Forms.TextBox textBoxFindName;
		private System.Windows.Forms.ComboBox comboBoxLookIn;
		private System.Windows.Forms.Button buttonReplaceAll;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonFindAll;
	}
}