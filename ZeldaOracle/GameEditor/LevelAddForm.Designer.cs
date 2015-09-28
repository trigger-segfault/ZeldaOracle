namespace ZeldaEditor {
	partial class LevelAddForm {
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
			this.textBoxLevelName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.numericLevelWidth = new System.Windows.Forms.NumericUpDown();
			this.numericLevelHeight = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.comboBoxRoomSize = new System.Windows.Forms.ComboBox();
			this.comboBoxZone = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonAdd = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.numericLevelWidth)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericLevelHeight)).BeginInit();
			this.SuspendLayout();
			// 
			// textBoxLevelName
			// 
			this.textBoxLevelName.Location = new System.Drawing.Point(85, 12);
			this.textBoxLevelName.Name = "textBoxLevelName";
			this.textBoxLevelName.Size = new System.Drawing.Size(145, 20);
			this.textBoxLevelName.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(65, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Level name:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(38, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(38, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Width:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// numericLevelWidth
			// 
			this.numericLevelWidth.Location = new System.Drawing.Point(85, 38);
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
			// 
			// numericLevelHeight
			// 
			this.numericLevelHeight.Location = new System.Drawing.Point(85, 64);
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
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(17, 94);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(59, 13);
			this.label3.TabIndex = 1;
			this.label3.Text = "Room size:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(38, 66);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(41, 13);
			this.label4.TabIndex = 1;
			this.label4.Text = "Height:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboBoxRoomSize
			// 
			this.comboBoxRoomSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxRoomSize.FormattingEnabled = true;
			this.comboBoxRoomSize.Items.AddRange(new object[] {
            "Small (10 x 8)",
            "Large (15 x 11)"});
			this.comboBoxRoomSize.Location = new System.Drawing.Point(85, 91);
			this.comboBoxRoomSize.Name = "comboBoxRoomSize";
			this.comboBoxRoomSize.Size = new System.Drawing.Size(145, 21);
			this.comboBoxRoomSize.TabIndex = 3;
			// 
			// comboBoxZone
			// 
			this.comboBoxZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxZone.FormattingEnabled = true;
			this.comboBoxZone.Items.AddRange(new object[] {
            "Summer",
            "Forest",
            "Graveyard",
            "Interior"});
			this.comboBoxZone.Location = new System.Drawing.Point(85, 118);
			this.comboBoxZone.Name = "comboBoxZone";
			this.comboBoxZone.Size = new System.Drawing.Size(145, 21);
			this.comboBoxZone.TabIndex = 4;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(38, 121);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(35, 13);
			this.label5.TabIndex = 1;
			this.label5.Text = "Zone:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(155, 165);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 6;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonAdd
			// 
			this.buttonAdd.Location = new System.Drawing.Point(74, 165);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Size = new System.Drawing.Size(75, 23);
			this.buttonAdd.TabIndex = 5;
			this.buttonAdd.Text = "Add";
			this.buttonAdd.UseVisualStyleBackColor = true;
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			// 
			// LevelAddForm
			// 
			this.AcceptButton = this.buttonAdd;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(242, 200);
			this.Controls.Add(this.buttonAdd);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.comboBoxZone);
			this.Controls.Add(this.comboBoxRoomSize);
			this.Controls.Add(this.numericLevelHeight);
			this.Controls.Add(this.numericLevelWidth);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBoxLevelName);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LevelAddForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Add New Level";
			((System.ComponentModel.ISupportInitialize)(this.numericLevelWidth)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericLevelHeight)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBoxLevelName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown numericLevelWidth;
		private System.Windows.Forms.NumericUpDown numericLevelHeight;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox comboBoxRoomSize;
		private System.Windows.Forms.ComboBox comboBoxZone;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonAdd;
	}
}