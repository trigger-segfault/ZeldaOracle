namespace ZeldaEditor.ObjectsEditor {
	partial class ObjectEditor {
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
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.comboBoxSolidType = new System.Windows.Forms.ComboBox();
			this.comboBoxMovementType = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.comboBoxLedgeDirection = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.comboBoxCollisionModel = new System.Windows.Forms.ComboBox();
			this.label7 = new System.Windows.Forms.Label();
			this.labelLedgeDirection = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.checkBoxSecretSound = new System.Windows.Forms.CheckBox();
			this.checkBoxPoofEffect = new System.Windows.Forms.CheckBox();
			this.checkBoxDropFromCeiling = new System.Windows.Forms.CheckBox();
			this.checkBoxStartDisabled = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxId = new System.Windows.Forms.TextBox();
			this.tabPage5 = new System.Windows.Forms.TabPage();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.labelMoveDirection = new System.Windows.Forms.Label();
			this.checkBox10 = new System.Windows.Forms.CheckBox();
			this.labelBraceletLevel = new System.Windows.Forms.Label();
			this.checkBoxBurnable = new System.Windows.Forms.CheckBox();
			this.checkBoxBombable = new System.Windows.Forms.CheckBox();
			this.labelSwordLevel = new System.Windows.Forms.Label();
			this.checkBoxDigable = new System.Windows.Forms.CheckBox();
			this.comboBoxMoveDirection = new System.Windows.Forms.ComboBox();
			this.comboBoxBraceletLevel = new System.Windows.Forms.ComboBox();
			this.comboBoxSwordLevel = new System.Windows.Forms.ComboBox();
			this.checkBoxMoveOnce = new System.Windows.Forms.CheckBox();
			this.checkBoxBreakOnSwitch = new System.Windows.Forms.CheckBox();
			this.checkBoxSwitchable = new System.Windows.Forms.CheckBox();
			this.checkBoxBoomerangable = new System.Windows.Forms.CheckBox();
			this.checkBoxPickupable = new System.Windows.Forms.CheckBox();
			this.checkBoxMovable = new System.Windows.Forms.CheckBox();
			this.checkBoxCuttable = new System.Windows.Forms.CheckBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.panel1 = new System.Windows.Forms.Panel();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonDone = new System.Windows.Forms.Button();
			this.numberBoxHeight = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.numberBoxWidth = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tabPage5.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.tabPage3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numberBoxHeight)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numberBoxWidth)).BeginInit();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage5);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(567, 394);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.groupBox2);
			this.tabPage1.Controls.Add(this.groupBox1);
			this.tabPage1.Controls.Add(this.checkBoxStartDisabled);
			this.tabPage1.Controls.Add(this.label1);
			this.tabPage1.Controls.Add(this.textBoxId);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(559, 368);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "General";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.numberBoxWidth);
			this.groupBox2.Controls.Add(this.comboBoxSolidType);
			this.groupBox2.Controls.Add(this.numberBoxHeight);
			this.groupBox2.Controls.Add(this.comboBoxMovementType);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.comboBoxLedgeDirection);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.comboBoxCollisionModel);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Controls.Add(this.labelLedgeDirection);
			this.groupBox2.Location = new System.Drawing.Point(11, 65);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(298, 198);
			this.groupBox2.TabIndex = 11;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "General";
			// 
			// comboBoxSolidType
			// 
			this.comboBoxSolidType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxSolidType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxSolidType.FormattingEnabled = true;
			this.comboBoxSolidType.Location = new System.Drawing.Point(147, 71);
			this.comboBoxSolidType.Name = "comboBoxSolidType";
			this.comboBoxSolidType.Size = new System.Drawing.Size(145, 21);
			this.comboBoxSolidType.TabIndex = 10;
			this.comboBoxSolidType.SelectedIndexChanged += new System.EventHandler(this.comboBoxSolidType_SelectedIndexChanged);
			// 
			// comboBoxMovementType
			// 
			this.comboBoxMovementType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxMovementType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxMovementType.FormattingEnabled = true;
			this.comboBoxMovementType.Location = new System.Drawing.Point(147, 152);
			this.comboBoxMovementType.Name = "comboBoxMovementType";
			this.comboBoxMovementType.Size = new System.Drawing.Size(145, 21);
			this.comboBoxMovementType.TabIndex = 10;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(6, 74);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(60, 13);
			this.label5.TabIndex = 1;
			this.label5.Text = "Solid Type:";
			// 
			// comboBoxLedgeDirection
			// 
			this.comboBoxLedgeDirection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxLedgeDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxLedgeDirection.FormattingEnabled = true;
			this.comboBoxLedgeDirection.Location = new System.Drawing.Point(147, 98);
			this.comboBoxLedgeDirection.Name = "comboBoxLedgeDirection";
			this.comboBoxLedgeDirection.Size = new System.Drawing.Size(145, 21);
			this.comboBoxLedgeDirection.TabIndex = 10;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(8, 155);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(96, 13);
			this.label6.TabIndex = 1;
			this.label6.Text = "Environment Type:";
			// 
			// comboBoxCollisionModel
			// 
			this.comboBoxCollisionModel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxCollisionModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxCollisionModel.FormattingEnabled = true;
			this.comboBoxCollisionModel.Location = new System.Drawing.Point(147, 125);
			this.comboBoxCollisionModel.Name = "comboBoxCollisionModel";
			this.comboBoxCollisionModel.Size = new System.Drawing.Size(145, 21);
			this.comboBoxCollisionModel.TabIndex = 10;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(8, 128);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(80, 13);
			this.label7.TabIndex = 1;
			this.label7.Text = "Collision Model:";
			// 
			// labelLedgeDirection
			// 
			this.labelLedgeDirection.AutoSize = true;
			this.labelLedgeDirection.Location = new System.Drawing.Point(6, 101);
			this.labelLedgeDirection.Name = "labelLedgeDirection";
			this.labelLedgeDirection.Size = new System.Drawing.Size(82, 13);
			this.labelLedgeDirection.TabIndex = 1;
			this.labelLedgeDirection.Text = "Ledge Direction";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.checkBoxSecretSound);
			this.groupBox1.Controls.Add(this.checkBoxPoofEffect);
			this.groupBox1.Controls.Add(this.checkBoxDropFromCeiling);
			this.groupBox1.Location = new System.Drawing.Point(11, 269);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(298, 90);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Spawn Type";
			// 
			// checkBoxSecretSound
			// 
			this.checkBoxSecretSound.AutoSize = true;
			this.checkBoxSecretSound.Location = new System.Drawing.Point(6, 65);
			this.checkBoxSecretSound.Name = "checkBoxSecretSound";
			this.checkBoxSecretSound.Size = new System.Drawing.Size(125, 17);
			this.checkBoxSecretSound.TabIndex = 0;
			this.checkBoxSecretSound.Text = "Secret Sound TODO";
			this.checkBoxSecretSound.UseVisualStyleBackColor = true;
			// 
			// checkBoxPoofEffect
			// 
			this.checkBoxPoofEffect.AutoSize = true;
			this.checkBoxPoofEffect.Location = new System.Drawing.Point(6, 42);
			this.checkBoxPoofEffect.Name = "checkBoxPoofEffect";
			this.checkBoxPoofEffect.Size = new System.Drawing.Size(79, 17);
			this.checkBoxPoofEffect.TabIndex = 0;
			this.checkBoxPoofEffect.Text = "Poof Effect";
			this.checkBoxPoofEffect.UseVisualStyleBackColor = true;
			// 
			// checkBoxDropFromCeiling
			// 
			this.checkBoxDropFromCeiling.AutoSize = true;
			this.checkBoxDropFromCeiling.Location = new System.Drawing.Point(6, 19);
			this.checkBoxDropFromCeiling.Name = "checkBoxDropFromCeiling";
			this.checkBoxDropFromCeiling.Size = new System.Drawing.Size(106, 17);
			this.checkBoxDropFromCeiling.TabIndex = 0;
			this.checkBoxDropFromCeiling.Text = "Drop from Ceiling";
			this.checkBoxDropFromCeiling.UseVisualStyleBackColor = true;
			// 
			// checkBoxStartDisabled
			// 
			this.checkBoxStartDisabled.AutoSize = true;
			this.checkBoxStartDisabled.Location = new System.Drawing.Point(17, 42);
			this.checkBoxStartDisabled.Name = "checkBoxStartDisabled";
			this.checkBoxStartDisabled.Size = new System.Drawing.Size(97, 17);
			this.checkBoxStartDisabled.TabIndex = 4;
			this.checkBoxStartDisabled.Text = "Starts Disabled";
			this.checkBoxStartDisabled.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(21, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "ID:";
			// 
			// textBoxId
			// 
			this.textBoxId.Location = new System.Drawing.Point(35, 6);
			this.textBoxId.Name = "textBoxId";
			this.textBoxId.Size = new System.Drawing.Size(132, 20);
			this.textBoxId.TabIndex = 0;
			this.textBoxId.Text = "id";
			// 
			// tabPage5
			// 
			this.tabPage5.Controls.Add(this.groupBox3);
			this.tabPage5.Location = new System.Drawing.Point(4, 22);
			this.tabPage5.Name = "tabPage5";
			this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage5.Size = new System.Drawing.Size(559, 368);
			this.tabPage5.TabIndex = 4;
			this.tabPage5.Text = "Interactions";
			this.tabPage5.UseVisualStyleBackColor = true;
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Controls.Add(this.labelMoveDirection);
			this.groupBox3.Controls.Add(this.checkBox10);
			this.groupBox3.Controls.Add(this.labelBraceletLevel);
			this.groupBox3.Controls.Add(this.checkBoxBurnable);
			this.groupBox3.Controls.Add(this.checkBoxBombable);
			this.groupBox3.Controls.Add(this.labelSwordLevel);
			this.groupBox3.Controls.Add(this.checkBoxDigable);
			this.groupBox3.Controls.Add(this.comboBoxMoveDirection);
			this.groupBox3.Controls.Add(this.comboBoxBraceletLevel);
			this.groupBox3.Controls.Add(this.comboBoxSwordLevel);
			this.groupBox3.Controls.Add(this.checkBoxMoveOnce);
			this.groupBox3.Controls.Add(this.checkBoxBreakOnSwitch);
			this.groupBox3.Controls.Add(this.checkBoxSwitchable);
			this.groupBox3.Controls.Add(this.checkBoxBoomerangable);
			this.groupBox3.Controls.Add(this.checkBoxPickupable);
			this.groupBox3.Controls.Add(this.checkBoxMovable);
			this.groupBox3.Controls.Add(this.checkBoxCuttable);
			this.groupBox3.Location = new System.Drawing.Point(256, 6);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(297, 356);
			this.groupBox3.TabIndex = 12;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Player Interactions";
			// 
			// labelMoveDirection
			// 
			this.labelMoveDirection.AutoSize = true;
			this.labelMoveDirection.Location = new System.Drawing.Point(119, 167);
			this.labelMoveDirection.Name = "labelMoveDirection";
			this.labelMoveDirection.Size = new System.Drawing.Size(79, 13);
			this.labelMoveDirection.TabIndex = 10;
			this.labelMoveDirection.Text = "Move Direction";
			// 
			// checkBox10
			// 
			this.checkBox10.AutoSize = true;
			this.checkBox10.Location = new System.Drawing.Point(6, 329);
			this.checkBox10.Name = "checkBox10";
			this.checkBox10.Size = new System.Drawing.Size(98, 17);
			this.checkBox10.TabIndex = 0;
			this.checkBox10.Text = "Stay Destroyed";
			this.checkBox10.UseVisualStyleBackColor = true;
			// 
			// labelBraceletLevel
			// 
			this.labelBraceletLevel.AutoSize = true;
			this.labelBraceletLevel.Location = new System.Drawing.Point(159, 94);
			this.labelBraceletLevel.Name = "labelBraceletLevel";
			this.labelBraceletLevel.Size = new System.Drawing.Size(119, 13);
			this.labelBraceletLevel.TabIndex = 10;
			this.labelBraceletLevel.Text = "Minimum Bracelet Level";
			// 
			// checkBoxBurnable
			// 
			this.checkBoxBurnable.AutoSize = true;
			this.checkBoxBurnable.Location = new System.Drawing.Point(6, 191);
			this.checkBoxBurnable.Name = "checkBoxBurnable";
			this.checkBoxBurnable.Size = new System.Drawing.Size(68, 17);
			this.checkBoxBurnable.TabIndex = 0;
			this.checkBoxBurnable.Text = "Burnable";
			this.checkBoxBurnable.UseVisualStyleBackColor = true;
			// 
			// checkBoxBombable
			// 
			this.checkBoxBombable.AutoSize = true;
			this.checkBoxBombable.Location = new System.Drawing.Point(6, 214);
			this.checkBoxBombable.Name = "checkBoxBombable";
			this.checkBoxBombable.Size = new System.Drawing.Size(73, 17);
			this.checkBoxBombable.TabIndex = 0;
			this.checkBoxBombable.Text = "Bombable";
			this.checkBoxBombable.UseVisualStyleBackColor = true;
			// 
			// labelSwordLevel
			// 
			this.labelSwordLevel.AutoSize = true;
			this.labelSwordLevel.Location = new System.Drawing.Point(159, 44);
			this.labelSwordLevel.Name = "labelSwordLevel";
			this.labelSwordLevel.Size = new System.Drawing.Size(110, 13);
			this.labelSwordLevel.TabIndex = 10;
			this.labelSwordLevel.Text = "Minimum Sword Level";
			// 
			// checkBoxDigable
			// 
			this.checkBoxDigable.AutoSize = true;
			this.checkBoxDigable.Location = new System.Drawing.Point(6, 237);
			this.checkBoxDigable.Name = "checkBoxDigable";
			this.checkBoxDigable.Size = new System.Drawing.Size(62, 17);
			this.checkBoxDigable.TabIndex = 0;
			this.checkBoxDigable.Text = "Digable";
			this.checkBoxDigable.UseVisualStyleBackColor = true;
			// 
			// comboBoxMoveDirection
			// 
			this.comboBoxMoveDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxMoveDirection.FormattingEnabled = true;
			this.comboBoxMoveDirection.Location = new System.Drawing.Point(28, 164);
			this.comboBoxMoveDirection.Name = "comboBoxMoveDirection";
			this.comboBoxMoveDirection.Size = new System.Drawing.Size(85, 21);
			this.comboBoxMoveDirection.TabIndex = 9;
			// 
			// comboBoxBraceletLevel
			// 
			this.comboBoxBraceletLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxBraceletLevel.FormattingEnabled = true;
			this.comboBoxBraceletLevel.Location = new System.Drawing.Point(28, 91);
			this.comboBoxBraceletLevel.Name = "comboBoxBraceletLevel";
			this.comboBoxBraceletLevel.Size = new System.Drawing.Size(125, 21);
			this.comboBoxBraceletLevel.TabIndex = 9;
			// 
			// comboBoxSwordLevel
			// 
			this.comboBoxSwordLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxSwordLevel.FormattingEnabled = true;
			this.comboBoxSwordLevel.Location = new System.Drawing.Point(28, 41);
			this.comboBoxSwordLevel.Name = "comboBoxSwordLevel";
			this.comboBoxSwordLevel.Size = new System.Drawing.Size(125, 21);
			this.comboBoxSwordLevel.TabIndex = 9;
			// 
			// checkBoxMoveOnce
			// 
			this.checkBoxMoveOnce.AutoSize = true;
			this.checkBoxMoveOnce.Location = new System.Drawing.Point(28, 141);
			this.checkBoxMoveOnce.Name = "checkBoxMoveOnce";
			this.checkBoxMoveOnce.Size = new System.Drawing.Size(106, 17);
			this.checkBoxMoveOnce.TabIndex = 0;
			this.checkBoxMoveOnce.Text = "Only Move Once";
			this.checkBoxMoveOnce.UseVisualStyleBackColor = true;
			// 
			// checkBoxBreakOnSwitch
			// 
			this.checkBoxBreakOnSwitch.AutoSize = true;
			this.checkBoxBreakOnSwitch.Location = new System.Drawing.Point(28, 306);
			this.checkBoxBreakOnSwitch.Name = "checkBoxBreakOnSwitch";
			this.checkBoxBreakOnSwitch.Size = new System.Drawing.Size(104, 17);
			this.checkBoxBreakOnSwitch.TabIndex = 0;
			this.checkBoxBreakOnSwitch.Text = "Break on Switch";
			this.checkBoxBreakOnSwitch.UseVisualStyleBackColor = true;
			// 
			// checkBoxSwitchable
			// 
			this.checkBoxSwitchable.AutoSize = true;
			this.checkBoxSwitchable.Checked = true;
			this.checkBoxSwitchable.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxSwitchable.Location = new System.Drawing.Point(6, 283);
			this.checkBoxSwitchable.Name = "checkBoxSwitchable";
			this.checkBoxSwitchable.Size = new System.Drawing.Size(107, 17);
			this.checkBoxSwitchable.TabIndex = 0;
			this.checkBoxSwitchable.Text = "Switch Hookable";
			this.checkBoxSwitchable.UseVisualStyleBackColor = true;
			this.checkBoxSwitchable.CheckedChanged += new System.EventHandler(this.checkBoxSwitchable_CheckedChanged);
			// 
			// checkBoxBoomerangable
			// 
			this.checkBoxBoomerangable.AutoSize = true;
			this.checkBoxBoomerangable.Location = new System.Drawing.Point(6, 260);
			this.checkBoxBoomerangable.Name = "checkBoxBoomerangable";
			this.checkBoxBoomerangable.Size = new System.Drawing.Size(132, 17);
			this.checkBoxBoomerangable.TabIndex = 0;
			this.checkBoxBoomerangable.Text = "Magic Boomerangable";
			this.checkBoxBoomerangable.UseVisualStyleBackColor = true;
			// 
			// checkBoxPickupable
			// 
			this.checkBoxPickupable.AutoSize = true;
			this.checkBoxPickupable.Checked = true;
			this.checkBoxPickupable.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxPickupable.Location = new System.Drawing.Point(6, 68);
			this.checkBoxPickupable.Name = "checkBoxPickupable";
			this.checkBoxPickupable.Size = new System.Drawing.Size(79, 17);
			this.checkBoxPickupable.TabIndex = 0;
			this.checkBoxPickupable.Text = "Pickupable";
			this.checkBoxPickupable.UseVisualStyleBackColor = true;
			this.checkBoxPickupable.CheckedChanged += new System.EventHandler(this.checkBoxPickupable_CheckedChanged);
			// 
			// checkBoxMovable
			// 
			this.checkBoxMovable.AutoSize = true;
			this.checkBoxMovable.Checked = true;
			this.checkBoxMovable.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxMovable.Location = new System.Drawing.Point(6, 118);
			this.checkBoxMovable.Name = "checkBoxMovable";
			this.checkBoxMovable.Size = new System.Drawing.Size(67, 17);
			this.checkBoxMovable.TabIndex = 0;
			this.checkBoxMovable.Text = "Movable";
			this.checkBoxMovable.UseVisualStyleBackColor = true;
			this.checkBoxMovable.CheckedChanged += new System.EventHandler(this.checkBoxMovable_CheckedChanged);
			// 
			// checkBoxCuttable
			// 
			this.checkBoxCuttable.AutoSize = true;
			this.checkBoxCuttable.Checked = true;
			this.checkBoxCuttable.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxCuttable.Location = new System.Drawing.Point(6, 19);
			this.checkBoxCuttable.Name = "checkBoxCuttable";
			this.checkBoxCuttable.Size = new System.Drawing.Size(112, 17);
			this.checkBoxCuttable.TabIndex = 0;
			this.checkBoxCuttable.Text = "Cuttable by Sword";
			this.checkBoxCuttable.UseVisualStyleBackColor = true;
			this.checkBoxCuttable.CheckedChanged += new System.EventHandler(this.checkBoxCuttable_CheckedChanged);
			// 
			// tabPage2
			// 
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(559, 368);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Events";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.dataGridView1);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage3.Size = new System.Drawing.Size(559, 368);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Raw Properties";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// dataGridView1
			// 
			this.dataGridView1.AllowUserToAddRows = false;
			this.dataGridView1.AllowUserToDeleteRows = false;
			this.dataGridView1.AllowUserToResizeRows = false;
			this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
			this.dataGridView1.Location = new System.Drawing.Point(8, 6);
			this.dataGridView1.MultiSelect = false;
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.RowHeadersVisible = false;
			this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataGridView1.Size = new System.Drawing.Size(543, 356);
			this.dataGridView1.TabIndex = 3;
			// 
			// Column1
			// 
			this.Column1.HeaderText = "Property Name";
			this.Column1.Name = "Column1";
			// 
			// Column2
			// 
			this.Column2.HeaderText = "Value";
			this.Column2.Name = "Column2";
			// 
			// panel1
			// 
			this.panel1.AutoSize = true;
			this.panel1.Controls.Add(this.buttonCancel);
			this.panel1.Controls.Add(this.buttonDone);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 394);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(567, 29);
			this.panel1.TabIndex = 1;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(488, 3);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 1;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonDone
			// 
			this.buttonDone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonDone.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonDone.Location = new System.Drawing.Point(407, 3);
			this.buttonDone.Name = "buttonDone";
			this.buttonDone.Size = new System.Drawing.Size(75, 23);
			this.buttonDone.TabIndex = 0;
			this.buttonDone.Text = "Done";
			this.buttonDone.UseVisualStyleBackColor = true;
			this.buttonDone.Click += new System.EventHandler(this.buttonDone_Click);
			// 
			// numberBoxHeight
			// 
			this.numberBoxHeight.Location = new System.Drawing.Point(147, 45);
			this.numberBoxHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numberBoxHeight.Name = "numberBoxHeight";
			this.numberBoxHeight.Size = new System.Drawing.Size(145, 20);
			this.numberBoxHeight.TabIndex = 12;
			this.numberBoxHeight.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 21);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(38, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Width:";
			// 
			// numberBoxWidth
			// 
			this.numberBoxWidth.Location = new System.Drawing.Point(147, 19);
			this.numberBoxWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numberBoxWidth.Name = "numberBoxWidth";
			this.numberBoxWidth.Size = new System.Drawing.Size(145, 20);
			this.numberBoxWidth.TabIndex = 12;
			this.numberBoxWidth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 47);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(38, 13);
			this.label3.TabIndex = 1;
			this.label3.Text = "Height";
			// 
			// ObjectEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(567, 423);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.panel1);
			this.Name = "ObjectEditor";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ObjectEditor";
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.tabPage5.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.tabPage3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numberBoxHeight)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numberBoxWidth)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxId;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonDone;
		private System.Windows.Forms.CheckBox checkBoxStartDisabled;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox checkBoxSecretSound;
		private System.Windows.Forms.CheckBox checkBoxPoofEffect;
		private System.Windows.Forms.CheckBox checkBoxDropFromCeiling;
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
		private System.Windows.Forms.ComboBox comboBoxSolidType;
		private System.Windows.Forms.TabPage tabPage5;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label labelMoveDirection;
		private System.Windows.Forms.CheckBox checkBox10;
		private System.Windows.Forms.Label labelBraceletLevel;
		private System.Windows.Forms.CheckBox checkBoxBurnable;
		private System.Windows.Forms.CheckBox checkBoxBombable;
		private System.Windows.Forms.Label labelSwordLevel;
		private System.Windows.Forms.CheckBox checkBoxDigable;
		private System.Windows.Forms.ComboBox comboBoxMoveDirection;
		private System.Windows.Forms.ComboBox comboBoxBraceletLevel;
		private System.Windows.Forms.ComboBox comboBoxSwordLevel;
		private System.Windows.Forms.CheckBox checkBoxMoveOnce;
		private System.Windows.Forms.CheckBox checkBoxBreakOnSwitch;
		private System.Windows.Forms.CheckBox checkBoxSwitchable;
		private System.Windows.Forms.CheckBox checkBoxBoomerangable;
		private System.Windows.Forms.CheckBox checkBoxPickupable;
		private System.Windows.Forms.CheckBox checkBoxMovable;
		private System.Windows.Forms.CheckBox checkBoxCuttable;
		private System.Windows.Forms.ComboBox comboBoxMovementType;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox comboBoxCollisionModel;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ComboBox comboBoxLedgeDirection;
		private System.Windows.Forms.Label labelLedgeDirection;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.NumericUpDown numberBoxWidth;
		private System.Windows.Forms.NumericUpDown numberBoxHeight;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
	}
}