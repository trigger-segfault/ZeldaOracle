namespace ZeldaEditor {
	partial class EditorForm {
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorForm));
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.buttonNew = new System.Windows.Forms.ToolStripButton();
			this.buttonLoad = new System.Windows.Forms.ToolStripButton();
			this.buttonSave = new System.Windows.Forms.ToolStripButton();
			this.buttonSaveAs = new System.Windows.Forms.ToolStripButton();
			this.buttonAddLevel = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStrip2 = new System.Windows.Forms.ToolStrip();
			this.buttonToolPointer = new System.Windows.Forms.ToolStripButton();
			this.buttonToolPlace = new System.Windows.Forms.ToolStripButton();
			this.buttonToolSelection = new System.Windows.Forms.ToolStripButton();
			this.buttonToolEyedropper = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.buttonToolCopy = new System.Windows.Forms.ToolStripButton();
			this.buttonToolCut = new System.Windows.Forms.ToolStripButton();
			this.buttonToolPaste = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.comboBoxWorldLayer = new System.Windows.Forms.ToolStripComboBox();
			this.dropDownButtonVisuals = new System.Windows.Forms.ToolStripDropDownButton();
			this.hideBelowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.fadeBelowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showBelowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.hideAboveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.fadeAboveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showAboveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.statusBarLabelRoomLoc = new System.Windows.Forms.ToolStripStatusLabel();
			this.statusBarLabelTileLoc = new System.Windows.Forms.ToolStripStatusLabel();
			this.splitContainerLevelsAndWorld = new System.Windows.Forms.SplitContainer();
			this.panelLevels = new System.Windows.Forms.Panel();
			this.treeViewLevels = new System.Windows.Forms.TreeView();
			this.splitContainerWorldAndTiles = new System.Windows.Forms.SplitContainer();
			this.panelWorld = new System.Windows.Forms.Panel();
			this.splitContainerTilesAndProperties = new System.Windows.Forms.SplitContainer();
			this.panelTiles1 = new System.Windows.Forms.Panel();
			this.panelTiles2 = new System.Windows.Forms.Panel();
			this.toolStrip3 = new System.Windows.Forms.ToolStrip();
			this.comboBoxTilesets = new System.Windows.Forms.ToolStripComboBox();
			this.comboBoxZones = new System.Windows.Forms.ToolStripComboBox();
			this.panelProperties = new System.Windows.Forms.Panel();
			this.propertyGrid = new System.Windows.Forms.PropertyGrid();
			this.contextMenuLevelSelect = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.buttonWorldGrid = new System.Windows.Forms.ToolStripButton();
			this.showRewardsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showRoomBordersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.buttonAnimations = new System.Windows.Forms.ToolStripButton();
			this.menuStrip1.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.toolStrip2.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerLevelsAndWorld)).BeginInit();
			this.splitContainerLevelsAndWorld.Panel1.SuspendLayout();
			this.splitContainerLevelsAndWorld.Panel2.SuspendLayout();
			this.splitContainerLevelsAndWorld.SuspendLayout();
			this.panelLevels.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerWorldAndTiles)).BeginInit();
			this.splitContainerWorldAndTiles.Panel1.SuspendLayout();
			this.splitContainerWorldAndTiles.Panel2.SuspendLayout();
			this.splitContainerWorldAndTiles.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerTilesAndProperties)).BeginInit();
			this.splitContainerTilesAndProperties.Panel1.SuspendLayout();
			this.splitContainerTilesAndProperties.Panel2.SuspendLayout();
			this.splitContainerTilesAndProperties.SuspendLayout();
			this.panelTiles1.SuspendLayout();
			this.toolStrip3.SuspendLayout();
			this.panelProperties.SuspendLayout();
			this.contextMenuLevelSelect.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(875, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
			this.editToolStripMenuItem.Text = "Edit";
			// 
			// viewToolStripMenuItem
			// 
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.viewToolStripMenuItem.Text = "View";
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonNew,
            this.buttonLoad,
            this.buttonSave,
            this.buttonSaveAs,
            this.buttonAddLevel,
            this.toolStripSeparator1});
			this.toolStrip1.Location = new System.Drawing.Point(0, 24);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(875, 25);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// buttonNew
			// 
			this.buttonNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonNew.Image = ((System.Drawing.Image)(resources.GetObject("buttonNew.Image")));
			this.buttonNew.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonNew.Name = "buttonNew";
			this.buttonNew.Size = new System.Drawing.Size(23, 22);
			this.buttonNew.Text = "New World";
			// 
			// buttonLoad
			// 
			this.buttonLoad.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonLoad.Image = ((System.Drawing.Image)(resources.GetObject("buttonLoad.Image")));
			this.buttonLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonLoad.Name = "buttonLoad";
			this.buttonLoad.Size = new System.Drawing.Size(23, 22);
			this.buttonLoad.Text = "Open World";
			this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
			// 
			// buttonSave
			// 
			this.buttonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonSave.Image = ((System.Drawing.Image)(resources.GetObject("buttonSave.Image")));
			this.buttonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.Size = new System.Drawing.Size(23, 22);
			this.buttonSave.Text = "Save World";
			this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
			// 
			// buttonSaveAs
			// 
			this.buttonSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonSaveAs.Image = ((System.Drawing.Image)(resources.GetObject("buttonSaveAs.Image")));
			this.buttonSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonSaveAs.Name = "buttonSaveAs";
			this.buttonSaveAs.Size = new System.Drawing.Size(23, 22);
			this.buttonSaveAs.Text = "Save As World";
			// 
			// buttonAddLevel
			// 
			this.buttonAddLevel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonAddLevel.Image = ((System.Drawing.Image)(resources.GetObject("buttonAddLevel.Image")));
			this.buttonAddLevel.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonAddLevel.Name = "buttonAddLevel";
			this.buttonAddLevel.Size = new System.Drawing.Size(23, 22);
			this.buttonAddLevel.Text = "Add New Level";
			this.buttonAddLevel.Click += new System.EventHandler(this.buttonAddLevel_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStrip2
			// 
			this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonToolPointer,
            this.buttonToolPlace,
            this.buttonToolSelection,
            this.buttonToolEyedropper,
            this.toolStripSeparator2,
            this.buttonToolCopy,
            this.buttonToolCut,
            this.buttonToolPaste,
            this.toolStripSeparator3,
            this.comboBoxWorldLayer,
            this.dropDownButtonVisuals,
            this.buttonWorldGrid,
            this.buttonAnimations});
			this.toolStrip2.Location = new System.Drawing.Point(0, 49);
			this.toolStrip2.Name = "toolStrip2";
			this.toolStrip2.Size = new System.Drawing.Size(875, 25);
			this.toolStrip2.TabIndex = 2;
			this.toolStrip2.Text = "toolStrip2";
			// 
			// buttonToolPointer
			// 
			this.buttonToolPointer.Checked = true;
			this.buttonToolPointer.CheckOnClick = true;
			this.buttonToolPointer.CheckState = System.Windows.Forms.CheckState.Checked;
			this.buttonToolPointer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonToolPointer.Image = ((System.Drawing.Image)(resources.GetObject("buttonToolPointer.Image")));
			this.buttonToolPointer.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonToolPointer.Name = "buttonToolPointer";
			this.buttonToolPointer.Size = new System.Drawing.Size(23, 22);
			this.buttonToolPointer.Text = "Pointer Tool";
			this.buttonToolPointer.Click += new System.EventHandler(this.buttonTool_Click);
			// 
			// buttonToolPlace
			// 
			this.buttonToolPlace.CheckOnClick = true;
			this.buttonToolPlace.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonToolPlace.Image = ((System.Drawing.Image)(resources.GetObject("buttonToolPlace.Image")));
			this.buttonToolPlace.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonToolPlace.Name = "buttonToolPlace";
			this.buttonToolPlace.Size = new System.Drawing.Size(23, 22);
			this.buttonToolPlace.Text = "Place Tool";
			this.buttonToolPlace.Click += new System.EventHandler(this.buttonTool_Click);
			// 
			// buttonToolSelection
			// 
			this.buttonToolSelection.CheckOnClick = true;
			this.buttonToolSelection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonToolSelection.Image = ((System.Drawing.Image)(resources.GetObject("buttonToolSelection.Image")));
			this.buttonToolSelection.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonToolSelection.Name = "buttonToolSelection";
			this.buttonToolSelection.Size = new System.Drawing.Size(23, 22);
			this.buttonToolSelection.Text = "Selection Tool";
			this.buttonToolSelection.Click += new System.EventHandler(this.buttonTool_Click);
			// 
			// buttonToolEyedropper
			// 
			this.buttonToolEyedropper.CheckOnClick = true;
			this.buttonToolEyedropper.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonToolEyedropper.Image = ((System.Drawing.Image)(resources.GetObject("buttonToolEyedropper.Image")));
			this.buttonToolEyedropper.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonToolEyedropper.Name = "buttonToolEyedropper";
			this.buttonToolEyedropper.Size = new System.Drawing.Size(23, 22);
			this.buttonToolEyedropper.Text = "Eyedropper Tool";
			this.buttonToolEyedropper.Click += new System.EventHandler(this.buttonTool_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// buttonToolCopy
			// 
			this.buttonToolCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonToolCopy.Image = ((System.Drawing.Image)(resources.GetObject("buttonToolCopy.Image")));
			this.buttonToolCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonToolCopy.Name = "buttonToolCopy";
			this.buttonToolCopy.Size = new System.Drawing.Size(23, 22);
			this.buttonToolCopy.Text = "toolStripButton1";
			// 
			// buttonToolCut
			// 
			this.buttonToolCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonToolCut.Image = ((System.Drawing.Image)(resources.GetObject("buttonToolCut.Image")));
			this.buttonToolCut.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonToolCut.Name = "buttonToolCut";
			this.buttonToolCut.Size = new System.Drawing.Size(23, 22);
			this.buttonToolCut.Text = "toolStripButton1";
			// 
			// buttonToolPaste
			// 
			this.buttonToolPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonToolPaste.Image = ((System.Drawing.Image)(resources.GetObject("buttonToolPaste.Image")));
			this.buttonToolPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonToolPaste.Name = "buttonToolPaste";
			this.buttonToolPaste.Size = new System.Drawing.Size(23, 22);
			this.buttonToolPaste.Text = "toolStripButton1";
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
			// 
			// comboBoxWorldLayer
			// 
			this.comboBoxWorldLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxWorldLayer.Name = "comboBoxWorldLayer";
			this.comboBoxWorldLayer.Size = new System.Drawing.Size(75, 25);
			this.comboBoxWorldLayer.SelectedIndexChanged += new System.EventHandler(this.comboBoxWorldLayer_SelectedIndexChanged);
			// 
			// dropDownButtonVisuals
			// 
			this.dropDownButtonVisuals.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.dropDownButtonVisuals.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hideBelowToolStripMenuItem,
            this.fadeBelowToolStripMenuItem,
            this.showBelowToolStripMenuItem,
            this.toolStripSeparator4,
            this.hideAboveToolStripMenuItem,
            this.fadeAboveToolStripMenuItem,
            this.showAboveToolStripMenuItem,
            this.toolStripSeparator5,
            this.showRewardsToolStripMenuItem,
            this.showRoomBordersToolStripMenuItem});
			this.dropDownButtonVisuals.Image = ((System.Drawing.Image)(resources.GetObject("dropDownButtonVisuals.Image")));
			this.dropDownButtonVisuals.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.dropDownButtonVisuals.Name = "dropDownButtonVisuals";
			this.dropDownButtonVisuals.Size = new System.Drawing.Size(29, 22);
			this.dropDownButtonVisuals.Text = "Layer Visuals";
			// 
			// hideBelowToolStripMenuItem
			// 
			this.hideBelowToolStripMenuItem.CheckOnClick = true;
			this.hideBelowToolStripMenuItem.Name = "hideBelowToolStripMenuItem";
			this.hideBelowToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			this.hideBelowToolStripMenuItem.Text = "Hide Below";
			this.hideBelowToolStripMenuItem.Click += new System.EventHandler(this.hideBelowToolStripMenuItem_Click);
			// 
			// fadeBelowToolStripMenuItem
			// 
			this.fadeBelowToolStripMenuItem.Checked = true;
			this.fadeBelowToolStripMenuItem.CheckOnClick = true;
			this.fadeBelowToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.fadeBelowToolStripMenuItem.Name = "fadeBelowToolStripMenuItem";
			this.fadeBelowToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			this.fadeBelowToolStripMenuItem.Text = "Fade Below";
			this.fadeBelowToolStripMenuItem.Click += new System.EventHandler(this.fadeBelowToolStripMenuItem_Click);
			// 
			// showBelowToolStripMenuItem
			// 
			this.showBelowToolStripMenuItem.CheckOnClick = true;
			this.showBelowToolStripMenuItem.Name = "showBelowToolStripMenuItem";
			this.showBelowToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			this.showBelowToolStripMenuItem.Text = "Show Below";
			this.showBelowToolStripMenuItem.Click += new System.EventHandler(this.showBelowToolStripMenuItem_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(178, 6);
			// 
			// hideAboveToolStripMenuItem
			// 
			this.hideAboveToolStripMenuItem.CheckOnClick = true;
			this.hideAboveToolStripMenuItem.Name = "hideAboveToolStripMenuItem";
			this.hideAboveToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			this.hideAboveToolStripMenuItem.Text = "Hide Above";
			this.hideAboveToolStripMenuItem.Click += new System.EventHandler(this.hideAboveToolStripMenuItem_Click);
			// 
			// fadeAboveToolStripMenuItem
			// 
			this.fadeAboveToolStripMenuItem.Checked = true;
			this.fadeAboveToolStripMenuItem.CheckOnClick = true;
			this.fadeAboveToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.fadeAboveToolStripMenuItem.Name = "fadeAboveToolStripMenuItem";
			this.fadeAboveToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			this.fadeAboveToolStripMenuItem.Text = "Fade Above";
			this.fadeAboveToolStripMenuItem.Click += new System.EventHandler(this.fadeAboveToolStripMenuItem_Click);
			// 
			// showAboveToolStripMenuItem
			// 
			this.showAboveToolStripMenuItem.CheckOnClick = true;
			this.showAboveToolStripMenuItem.Name = "showAboveToolStripMenuItem";
			this.showAboveToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			this.showAboveToolStripMenuItem.Text = "Show Above";
			this.showAboveToolStripMenuItem.Click += new System.EventHandler(this.showAboveToolStripMenuItem_Click);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(178, 6);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusBarLabelRoomLoc,
            this.statusBarLabelTileLoc});
			this.statusStrip1.Location = new System.Drawing.Point(0, 498);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(875, 22);
			this.statusStrip1.TabIndex = 3;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// statusBarLabelRoomLoc
			// 
			this.statusBarLabelRoomLoc.Name = "statusBarLabelRoomLoc";
			this.statusBarLabelRoomLoc.Size = new System.Drawing.Size(134, 17);
			this.statusBarLabelRoomLoc.Text = "statusBarLabelRoomLoc";
			// 
			// statusBarLabelTileLoc
			// 
			this.statusBarLabelTileLoc.Name = "statusBarLabelTileLoc";
			this.statusBarLabelTileLoc.Size = new System.Drawing.Size(121, 17);
			this.statusBarLabelTileLoc.Text = "statusBarLabelTileLoc";
			// 
			// splitContainerLevelsAndWorld
			// 
			this.splitContainerLevelsAndWorld.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerLevelsAndWorld.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainerLevelsAndWorld.Location = new System.Drawing.Point(0, 74);
			this.splitContainerLevelsAndWorld.Name = "splitContainerLevelsAndWorld";
			// 
			// splitContainerLevelsAndWorld.Panel1
			// 
			this.splitContainerLevelsAndWorld.Panel1.Controls.Add(this.panelLevels);
			// 
			// splitContainerLevelsAndWorld.Panel2
			// 
			this.splitContainerLevelsAndWorld.Panel2.Controls.Add(this.splitContainerWorldAndTiles);
			this.splitContainerLevelsAndWorld.Size = new System.Drawing.Size(875, 424);
			this.splitContainerLevelsAndWorld.SplitterDistance = 167;
			this.splitContainerLevelsAndWorld.TabIndex = 4;
			// 
			// panelLevels
			// 
			this.panelLevels.AutoScroll = true;
			this.panelLevels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelLevels.Controls.Add(this.treeViewLevels);
			this.panelLevels.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelLevels.Location = new System.Drawing.Point(0, 0);
			this.panelLevels.Name = "panelLevels";
			this.panelLevels.Size = new System.Drawing.Size(167, 424);
			this.panelLevels.TabIndex = 0;
			// 
			// treeViewLevels
			// 
			this.treeViewLevels.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.treeViewLevels.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeViewLevels.Location = new System.Drawing.Point(0, 0);
			this.treeViewLevels.Name = "treeViewLevels";
			this.treeViewLevels.Size = new System.Drawing.Size(165, 422);
			this.treeViewLevels.TabIndex = 0;
			// 
			// splitContainerWorldAndTiles
			// 
			this.splitContainerWorldAndTiles.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerWorldAndTiles.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainerWorldAndTiles.Location = new System.Drawing.Point(0, 0);
			this.splitContainerWorldAndTiles.Name = "splitContainerWorldAndTiles";
			// 
			// splitContainerWorldAndTiles.Panel1
			// 
			this.splitContainerWorldAndTiles.Panel1.Controls.Add(this.panelWorld);
			// 
			// splitContainerWorldAndTiles.Panel2
			// 
			this.splitContainerWorldAndTiles.Panel2.Controls.Add(this.splitContainerTilesAndProperties);
			this.splitContainerWorldAndTiles.Size = new System.Drawing.Size(704, 424);
			this.splitContainerWorldAndTiles.SplitterDistance = 412;
			this.splitContainerWorldAndTiles.TabIndex = 0;
			// 
			// panelWorld
			// 
			this.panelWorld.AutoScroll = true;
			this.panelWorld.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelWorld.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelWorld.Location = new System.Drawing.Point(0, 0);
			this.panelWorld.Name = "panelWorld";
			this.panelWorld.Size = new System.Drawing.Size(412, 424);
			this.panelWorld.TabIndex = 0;
			// 
			// splitContainerTilesAndProperties
			// 
			this.splitContainerTilesAndProperties.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerTilesAndProperties.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainerTilesAndProperties.Location = new System.Drawing.Point(0, 0);
			this.splitContainerTilesAndProperties.Name = "splitContainerTilesAndProperties";
			this.splitContainerTilesAndProperties.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainerTilesAndProperties.Panel1
			// 
			this.splitContainerTilesAndProperties.Panel1.Controls.Add(this.panelTiles1);
			// 
			// splitContainerTilesAndProperties.Panel2
			// 
			this.splitContainerTilesAndProperties.Panel2.Controls.Add(this.panelProperties);
			this.splitContainerTilesAndProperties.Size = new System.Drawing.Size(288, 424);
			this.splitContainerTilesAndProperties.SplitterDistance = 234;
			this.splitContainerTilesAndProperties.TabIndex = 0;
			// 
			// panelTiles1
			// 
			this.panelTiles1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelTiles1.Controls.Add(this.panelTiles2);
			this.panelTiles1.Controls.Add(this.toolStrip3);
			this.panelTiles1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelTiles1.Location = new System.Drawing.Point(0, 0);
			this.panelTiles1.Name = "panelTiles1";
			this.panelTiles1.Size = new System.Drawing.Size(288, 234);
			this.panelTiles1.TabIndex = 0;
			// 
			// panelTiles2
			// 
			this.panelTiles2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelTiles2.Location = new System.Drawing.Point(0, 25);
			this.panelTiles2.Name = "panelTiles2";
			this.panelTiles2.Size = new System.Drawing.Size(286, 207);
			this.panelTiles2.TabIndex = 1;
			// 
			// toolStrip3
			// 
			this.toolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.comboBoxTilesets,
            this.comboBoxZones});
			this.toolStrip3.Location = new System.Drawing.Point(0, 0);
			this.toolStrip3.Name = "toolStrip3";
			this.toolStrip3.Size = new System.Drawing.Size(286, 25);
			this.toolStrip3.TabIndex = 0;
			this.toolStrip3.Text = "toolStrip3";
			// 
			// comboBoxTilesets
			// 
			this.comboBoxTilesets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxTilesets.Name = "comboBoxTilesets";
			this.comboBoxTilesets.Size = new System.Drawing.Size(121, 25);
			this.comboBoxTilesets.SelectedIndexChanged += new System.EventHandler(this.comboBoxTilesets_SelectedIndexChanged);
			// 
			// comboBoxZones
			// 
			this.comboBoxZones.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxZones.Name = "comboBoxZones";
			this.comboBoxZones.Size = new System.Drawing.Size(121, 25);
			this.comboBoxZones.SelectedIndexChanged += new System.EventHandler(this.comboBoxZone_SelectedIndexChanged);
			// 
			// panelProperties
			// 
			this.panelProperties.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelProperties.Controls.Add(this.propertyGrid);
			this.panelProperties.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelProperties.Location = new System.Drawing.Point(0, 0);
			this.panelProperties.Name = "panelProperties";
			this.panelProperties.Size = new System.Drawing.Size(288, 186);
			this.panelProperties.TabIndex = 0;
			// 
			// propertyGrid
			// 
			this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid.Location = new System.Drawing.Point(0, 0);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.Size = new System.Drawing.Size(286, 184);
			this.propertyGrid.TabIndex = 0;
			// 
			// contextMenuLevelSelect
			// 
			this.contextMenuLevelSelect.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem,
            this.renameToolStripMenuItem,
            this.duplicateToolStripMenuItem});
			this.contextMenuLevelSelect.Name = "contextMenuLevelSelect";
			this.contextMenuLevelSelect.Size = new System.Drawing.Size(125, 70);
			// 
			// deleteToolStripMenuItem
			// 
			this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			this.deleteToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
			this.deleteToolStripMenuItem.Text = "Delete";
			// 
			// renameToolStripMenuItem
			// 
			this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
			this.renameToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
			this.renameToolStripMenuItem.Text = "Rename";
			// 
			// duplicateToolStripMenuItem
			// 
			this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
			this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
			this.duplicateToolStripMenuItem.Text = "Duplicate";
			// 
			// buttonWorldGrid
			// 
			this.buttonWorldGrid.CheckOnClick = true;
			this.buttonWorldGrid.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonWorldGrid.Image = ((System.Drawing.Image)(resources.GetObject("buttonWorldGrid.Image")));
			this.buttonWorldGrid.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonWorldGrid.Name = "buttonWorldGrid";
			this.buttonWorldGrid.Size = new System.Drawing.Size(23, 22);
			this.buttonWorldGrid.Text = "Show Grid";
			this.buttonWorldGrid.Click += new System.EventHandler(this.buttonWorldGrid_Click);
			// 
			// showRewardsToolStripMenuItem
			// 
			this.showRewardsToolStripMenuItem.Checked = true;
			this.showRewardsToolStripMenuItem.CheckOnClick = true;
			this.showRewardsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.showRewardsToolStripMenuItem.Name = "showRewardsToolStripMenuItem";
			this.showRewardsToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			this.showRewardsToolStripMenuItem.Text = "Show Rewards";
			this.showRewardsToolStripMenuItem.Click += new System.EventHandler(this.showRewardsToolStripMenuItem_Click);
			// 
			// showRoomBordersToolStripMenuItem
			// 
			this.showRoomBordersToolStripMenuItem.Checked = true;
			this.showRoomBordersToolStripMenuItem.CheckOnClick = true;
			this.showRoomBordersToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.showRoomBordersToolStripMenuItem.Name = "showRoomBordersToolStripMenuItem";
			this.showRoomBordersToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			this.showRoomBordersToolStripMenuItem.Text = "Show Room Borders";
			this.showRoomBordersToolStripMenuItem.Click += new System.EventHandler(this.showRoomBordersToolStripMenuItem_Click);
			// 
			// buttonAnimations
			// 
			this.buttonAnimations.CheckOnClick = true;
			this.buttonAnimations.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonAnimations.Image = ((System.Drawing.Image)(resources.GetObject("buttonAnimations.Image")));
			this.buttonAnimations.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonAnimations.Name = "buttonAnimations";
			this.buttonAnimations.Size = new System.Drawing.Size(23, 22);
			this.buttonAnimations.Text = "Play Animations";
			this.buttonAnimations.Click += new System.EventHandler(this.buttonAnimations_Click);
			// 
			// EditorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(875, 520);
			this.Controls.Add(this.splitContainerLevelsAndWorld);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.toolStrip2);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "EditorForm";
			this.Text = "Oracle Engine Editor";
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.toolStrip2.ResumeLayout(false);
			this.toolStrip2.PerformLayout();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.splitContainerLevelsAndWorld.Panel1.ResumeLayout(false);
			this.splitContainerLevelsAndWorld.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerLevelsAndWorld)).EndInit();
			this.splitContainerLevelsAndWorld.ResumeLayout(false);
			this.panelLevels.ResumeLayout(false);
			this.splitContainerWorldAndTiles.Panel1.ResumeLayout(false);
			this.splitContainerWorldAndTiles.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerWorldAndTiles)).EndInit();
			this.splitContainerWorldAndTiles.ResumeLayout(false);
			this.splitContainerTilesAndProperties.Panel1.ResumeLayout(false);
			this.splitContainerTilesAndProperties.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerTilesAndProperties)).EndInit();
			this.splitContainerTilesAndProperties.ResumeLayout(false);
			this.panelTiles1.ResumeLayout(false);
			this.panelTiles1.PerformLayout();
			this.toolStrip3.ResumeLayout(false);
			this.toolStrip3.PerformLayout();
			this.panelProperties.ResumeLayout(false);
			this.contextMenuLevelSelect.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton buttonLoad;
		private System.Windows.Forms.ToolStripButton buttonSave;
		private System.Windows.Forms.ToolStrip toolStrip2;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.SplitContainer splitContainerLevelsAndWorld;
		private System.Windows.Forms.SplitContainer splitContainerWorldAndTiles;
		private System.Windows.Forms.SplitContainer splitContainerTilesAndProperties;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
		private System.Windows.Forms.ToolStripButton buttonAddLevel;
		private System.Windows.Forms.Panel panelLevels;
		private System.Windows.Forms.Panel panelWorld;
		private System.Windows.Forms.ToolStripStatusLabel statusBarLabelRoomLoc;
		private System.Windows.Forms.ToolStripStatusLabel statusBarLabelTileLoc;
		private System.Windows.Forms.ContextMenuStrip contextMenuLevelSelect;
		private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem duplicateToolStripMenuItem;
		private System.Windows.Forms.Panel panelTiles1;
		private System.Windows.Forms.Panel panelProperties;
		private System.Windows.Forms.PropertyGrid propertyGrid;
		private System.Windows.Forms.ToolStripButton buttonNew;
		private System.Windows.Forms.ToolStripButton buttonSaveAs;
		private System.Windows.Forms.ToolStrip toolStrip3;
		private System.Windows.Forms.ToolStripComboBox comboBoxTilesets;
		private System.Windows.Forms.Panel panelTiles2;
		private System.Windows.Forms.ToolStripComboBox comboBoxZones;
		private System.Windows.Forms.TreeView treeViewLevels;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton buttonToolPointer;
		private System.Windows.Forms.ToolStripButton buttonToolPlace;
		private System.Windows.Forms.ToolStripButton buttonToolSelection;
		private System.Windows.Forms.ToolStripButton buttonToolEyedropper;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripButton buttonToolCopy;
		private System.Windows.Forms.ToolStripButton buttonToolCut;
		private System.Windows.Forms.ToolStripButton buttonToolPaste;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripComboBox comboBoxWorldLayer;
		private System.Windows.Forms.ToolStripDropDownButton dropDownButtonVisuals;
		private System.Windows.Forms.ToolStripMenuItem showAboveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem showBelowToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fadeAboveToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem hideAboveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem hideBelowToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fadeBelowToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripMenuItem showRewardsToolStripMenuItem;
		private System.Windows.Forms.ToolStripButton buttonWorldGrid;
		private System.Windows.Forms.ToolStripMenuItem showRoomBordersToolStripMenuItem;
		private System.Windows.Forms.ToolStripButton buttonAnimations;
	}
}