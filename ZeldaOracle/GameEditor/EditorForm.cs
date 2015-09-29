using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaEditor.Control;
using ZeldaOracle.Common.Properties;

namespace ZeldaEditor {

	public partial class EditorForm : Form {
		
		private LevelDisplay levelDisplay;
		private TileDisplay tileDisplay;
		private EditorControl editorControl;
		private PropertiesContainer propertiesContainer;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public EditorForm() {
			InitializeComponent();

			editorControl		= new EditorControl();
			editorControl.EditorForm	= this;

			// Create the level display.
			levelDisplay		= new LevelDisplay();
			levelDisplay.EditorControl	= editorControl;
			levelDisplay.Name	= "levelDisplay";
			levelDisplay.Dock	= DockStyle.Fill;
			levelDisplay.EditorForm = this;
			panelWorld.Controls.Add(this.levelDisplay);

			tileDisplay			= new TileDisplay();
			tileDisplay.EditorControl	= editorControl;
			tileDisplay.Name	= "tileDisplay";
			tileDisplay.Dock	= DockStyle.Fill;
			tileDisplay.EditorForm = this;
			panelTiles2.Controls.Add(tileDisplay);

			treeViewLevels.ExpandAll();
			treeViewLevels.NodeMouseDoubleClick += delegate(object sender, TreeNodeMouseClickEventArgs e) {
				editorControl.OpenLevel(e.Node.Index);
			};
			treeViewLevels.AfterLabelEdit += delegate(object sender, NodeLabelEditEventArgs e) {
				Console.WriteLine("Renamed level to " + e.Label);
				// Editing the label renames the level.
			};

			// Create test properties.
			Properties properties = new Properties();
			properties.Set("text",		"Hello, World!");
			properties.Set("direction",	3);
			properties.Set("pi",		3.14f);
			properties.Set("enabled",		true);

			/*
			Properties properties2 = new Properties();
			properties.Set("text",		"Hello, World!");
			properties.Set("direction",	3);
			properties.Set("pi",		3.14f);
			properties.Set("enabled",		true);
			*/

			propertiesContainer = new PropertiesContainer();
			propertiesContainer.Set(null, properties);
			propertyGrid.SelectedObject			= propertiesContainer;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		public void CloseProperties() {
			propertiesContainer.Clear();
			propertyGrid.Refresh();
		}

		public void OpenProperties(Properties properties, Properties baseProperties) {
			propertiesContainer.Set(properties, baseProperties);
			propertyGrid.Refresh();
		}


		//-----------------------------------------------------------------------------
		// Event handlers
		//-----------------------------------------------------------------------------

		// Open a file.
		private void buttonLoad_Click(object sender, EventArgs e) {
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.DereferenceLinks = true;
			openFileDialog.Filter = "Zelda world files (*.zwd)|*.zwd";

			openFileDialog.ShowDialog();

			if (openFileDialog.FileName != String.Empty) {
				Console.WriteLine("Opened file " + openFileDialog.FileName + ".");
				editorControl.OpenFile(openFileDialog.FileName);
			}
		}

		// Save the file as.
		private void buttonSave_Click(object sender, EventArgs e) {
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "Zelda world files (*.zwd)|*.zwd";
			saveFileDialog.ValidateNames = true;

			saveFileDialog.ShowDialog();

			if (saveFileDialog.FileName != String.Empty) {
				Console.WriteLine("Saving file as " + saveFileDialog.FileName + ".");
				editorControl.SaveFile(saveFileDialog.FileName);
			}
		}

		// Add a new level to the world.
		private void buttonAddLevel_Click(object sender, EventArgs e) {
			LevelAddForm form = new LevelAddForm();
			form.ShowDialog(this);
		}

		private void buttonAnimations_Click(object sender, EventArgs e) {
			editorControl.PlayAnimations = (sender as ToolStripButton).Checked;
		}

		private void comboBoxTilesets_SelectedIndexChanged(object sender, EventArgs e) {
			if ((string)(sender as ToolStripComboBox).Items[(sender as ToolStripComboBox).SelectedIndex] != "")
				editorControl.ChangeTileset((string)(sender as ToolStripComboBox).Items[(sender as ToolStripComboBox).SelectedIndex]);
		}

		private void comboBoxZone_SelectedIndexChanged(object sender, EventArgs e) {
			if ((string)(sender as ToolStripComboBox).Items[(sender as ToolStripComboBox).SelectedIndex] != "")
				editorControl.ChangeZone((string)(sender as ToolStripComboBox).Items[(sender as ToolStripComboBox).SelectedIndex]);
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public TreeView LevelTreeView {
			get { return treeViewLevels; }
		}

		public ContextMenuStrip ContextMenuLevelSelect {
			get { return contextMenuLevelSelect; }
		}

		public bool PlayAnimations {
			get { return buttonAnimations.Checked; }
		}

		public ToolStripStatusLabel StatusBarLabelTileLoc {
			get { return statusBarLabelTileLoc; }
		}

		public ToolStripStatusLabel StatusBarLabelRoomLoc {
			get { return statusBarLabelRoomLoc; }
		}

		public LevelDisplay LevelDisplay {
			get { return levelDisplay; }
		}

		public TileDisplay TileDisplay {
			get { return tileDisplay; }
		}

		public ToolStripComboBox ComboBoxTilesets {
			get { return comboBoxTilesets; }
		}

		public ToolStripComboBox ComboBoxZones {
			get { return comboBoxZones; }
		}
	}

}
