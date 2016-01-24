namespace ZeldaEditor.ObjectsEditor {
	partial class ObjectEditorEventsTab {
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
			this.listBoxEvents = new System.Windows.Forms.ListBox();
			this.panelEventProperties = new System.Windows.Forms.Panel();
			this.labelEventDescription = new System.Windows.Forms.Label();
			this.labelEventName = new System.Windows.Forms.Label();
			this.radioButtonEventScript = new System.Windows.Forms.RadioButton();
			this.radioButtonEventCustomCode = new System.Windows.Forms.RadioButton();
			this.groupBoxScript = new System.Windows.Forms.GroupBox();
			this.buttonEditScript = new System.Windows.Forms.Button();
			this.comboBoxScript = new System.Windows.Forms.ComboBox();
			this.groupBoxCustomCode = new System.Windows.Forms.GroupBox();
			this.buttonEditCode = new System.Windows.Forms.Button();
			this.radioButtonEventNone = new System.Windows.Forms.RadioButton();
			this.panelEventProperties.SuspendLayout();
			this.groupBoxScript.SuspendLayout();
			this.groupBoxCustomCode.SuspendLayout();
			this.SuspendLayout();
			// 
			// listBoxEvents
			// 
			this.listBoxEvents.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBoxEvents.FormattingEnabled = true;
			this.listBoxEvents.Location = new System.Drawing.Point(0, 0);
			this.listBoxEvents.Name = "listBoxEvents";
			this.listBoxEvents.Size = new System.Drawing.Size(323, 440);
			this.listBoxEvents.TabIndex = 0;
			this.listBoxEvents.SelectedIndexChanged += new System.EventHandler(this.listBoxEvents_SelectedIndexChanged);
			// 
			// panelEventProperties
			// 
			this.panelEventProperties.Controls.Add(this.labelEventDescription);
			this.panelEventProperties.Controls.Add(this.labelEventName);
			this.panelEventProperties.Controls.Add(this.radioButtonEventScript);
			this.panelEventProperties.Controls.Add(this.radioButtonEventCustomCode);
			this.panelEventProperties.Controls.Add(this.groupBoxScript);
			this.panelEventProperties.Controls.Add(this.groupBoxCustomCode);
			this.panelEventProperties.Controls.Add(this.radioButtonEventNone);
			this.panelEventProperties.Dock = System.Windows.Forms.DockStyle.Right;
			this.panelEventProperties.Location = new System.Drawing.Point(323, 0);
			this.panelEventProperties.Name = "panelEventProperties";
			this.panelEventProperties.Size = new System.Drawing.Size(306, 440);
			this.panelEventProperties.TabIndex = 5;
			// 
			// labelEventDescription
			// 
			this.labelEventDescription.AutoSize = true;
			this.labelEventDescription.Location = new System.Drawing.Point(12, 40);
			this.labelEventDescription.Name = "labelEventDescription";
			this.labelEventDescription.Size = new System.Drawing.Size(91, 13);
			this.labelEventDescription.TabIndex = 1;
			this.labelEventDescription.Text = "Event Description";
			// 
			// labelEventName
			// 
			this.labelEventName.AutoSize = true;
			this.labelEventName.Location = new System.Drawing.Point(12, 14);
			this.labelEventName.Name = "labelEventName";
			this.labelEventName.Size = new System.Drawing.Size(66, 13);
			this.labelEventName.TabIndex = 1;
			this.labelEventName.Text = "Event Name";
			// 
			// radioButtonEventScript
			// 
			this.radioButtonEventScript.AutoSize = true;
			this.radioButtonEventScript.Location = new System.Drawing.Point(159, 80);
			this.radioButtonEventScript.Name = "radioButtonEventScript";
			this.radioButtonEventScript.Size = new System.Drawing.Size(52, 17);
			this.radioButtonEventScript.TabIndex = 4;
			this.radioButtonEventScript.TabStop = true;
			this.radioButtonEventScript.Text = "Script";
			this.radioButtonEventScript.UseVisualStyleBackColor = true;
			this.radioButtonEventScript.CheckedChanged += new System.EventHandler(this.radioButtonEventScript_CheckedChanged);
			// 
			// radioButtonEventCustomCode
			// 
			this.radioButtonEventCustomCode.AutoSize = true;
			this.radioButtonEventCustomCode.Location = new System.Drawing.Point(65, 80);
			this.radioButtonEventCustomCode.Name = "radioButtonEventCustomCode";
			this.radioButtonEventCustomCode.Size = new System.Drawing.Size(88, 17);
			this.radioButtonEventCustomCode.TabIndex = 4;
			this.radioButtonEventCustomCode.TabStop = true;
			this.radioButtonEventCustomCode.Text = "Custom Code";
			this.radioButtonEventCustomCode.UseVisualStyleBackColor = true;
			this.radioButtonEventCustomCode.CheckedChanged += new System.EventHandler(this.radioButtonEventCustomCode_CheckedChanged);
			// 
			// groupBoxScript
			// 
			this.groupBoxScript.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxScript.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.groupBoxScript.Controls.Add(this.buttonEditScript);
			this.groupBoxScript.Controls.Add(this.comboBoxScript);
			this.groupBoxScript.Location = new System.Drawing.Point(8, 170);
			this.groupBoxScript.Name = "groupBoxScript";
			this.groupBoxScript.Size = new System.Drawing.Size(288, 84);
			this.groupBoxScript.TabIndex = 0;
			this.groupBoxScript.TabStop = false;
			// 
			// buttonEditScript
			// 
			this.buttonEditScript.Location = new System.Drawing.Point(6, 50);
			this.buttonEditScript.Name = "buttonEditScript";
			this.buttonEditScript.Size = new System.Drawing.Size(75, 23);
			this.buttonEditScript.TabIndex = 3;
			this.buttonEditScript.Text = "Edit Script";
			this.buttonEditScript.UseVisualStyleBackColor = true;
			this.buttonEditScript.Click += new System.EventHandler(this.buttonEditScript_Click);
			// 
			// comboBoxScript
			// 
			this.comboBoxScript.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxScript.FormattingEnabled = true;
			this.comboBoxScript.Location = new System.Drawing.Point(6, 23);
			this.comboBoxScript.Name = "comboBoxScript";
			this.comboBoxScript.Size = new System.Drawing.Size(276, 21);
			this.comboBoxScript.TabIndex = 3;
			this.comboBoxScript.TextChanged += new System.EventHandler(this.comboBoxScript_TextChanged);
			// 
			// groupBoxCustomCode
			// 
			this.groupBoxCustomCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxCustomCode.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.groupBoxCustomCode.Controls.Add(this.buttonEditCode);
			this.groupBoxCustomCode.Location = new System.Drawing.Point(9, 103);
			this.groupBoxCustomCode.Name = "groupBoxCustomCode";
			this.groupBoxCustomCode.Size = new System.Drawing.Size(287, 61);
			this.groupBoxCustomCode.TabIndex = 0;
			this.groupBoxCustomCode.TabStop = false;
			// 
			// buttonEditCode
			// 
			this.buttonEditCode.Location = new System.Drawing.Point(6, 25);
			this.buttonEditCode.Name = "buttonEditCode";
			this.buttonEditCode.Size = new System.Drawing.Size(75, 23);
			this.buttonEditCode.TabIndex = 2;
			this.buttonEditCode.Text = "Edit Code";
			this.buttonEditCode.UseVisualStyleBackColor = true;
			this.buttonEditCode.Click += new System.EventHandler(this.buttonEditCode_Click);
			// 
			// radioButtonEventNone
			// 
			this.radioButtonEventNone.AutoSize = true;
			this.radioButtonEventNone.Location = new System.Drawing.Point(8, 80);
			this.radioButtonEventNone.Name = "radioButtonEventNone";
			this.radioButtonEventNone.Size = new System.Drawing.Size(51, 17);
			this.radioButtonEventNone.TabIndex = 4;
			this.radioButtonEventNone.TabStop = true;
			this.radioButtonEventNone.Text = "None";
			this.radioButtonEventNone.UseVisualStyleBackColor = true;
			this.radioButtonEventNone.CheckedChanged += new System.EventHandler(this.radioButtonEventNone_CheckedChanged);
			// 
			// ObjectEditorEventsTab
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.listBoxEvents);
			this.Controls.Add(this.panelEventProperties);
			this.Name = "ObjectEditorEventsTab";
			this.Size = new System.Drawing.Size(629, 440);
			this.panelEventProperties.ResumeLayout(false);
			this.panelEventProperties.PerformLayout();
			this.groupBoxScript.ResumeLayout(false);
			this.groupBoxCustomCode.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListBox listBoxEvents;
		private System.Windows.Forms.Panel panelEventProperties;
		private System.Windows.Forms.Label labelEventDescription;
		private System.Windows.Forms.Label labelEventName;
		private System.Windows.Forms.RadioButton radioButtonEventScript;
		private System.Windows.Forms.RadioButton radioButtonEventCustomCode;
		private System.Windows.Forms.GroupBox groupBoxScript;
		private System.Windows.Forms.Button buttonEditScript;
		private System.Windows.Forms.ComboBox comboBoxScript;
		private System.Windows.Forms.GroupBox groupBoxCustomCode;
		private System.Windows.Forms.Button buttonEditCode;
		private System.Windows.Forms.RadioButton radioButtonEventNone;

	}
}
