using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsGraphicsDevice;
//using ZeldaOracle.Common.Content;

namespace ZeldaEditor {
	public partial class EditorForm : Form {
		
		private LevelDisplay levelDisplay;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public EditorForm() {
			InitializeComponent();

			// Create the level display.
			levelDisplay		= new LevelDisplay();
			levelDisplay.Name	= "levelDisplay";
			levelDisplay.Dock	= DockStyle.Fill;
			splitContainer2.Panel1.Controls.Add(this.levelDisplay);
			levelDisplay.EditorForm = this;

			treeView1.ExpandAll();
			treeView1.NodeMouseDoubleClick += delegate(object sender, TreeNodeMouseClickEventArgs e) {
				levelDisplay.OpenLevel(e.Node.Index);
			};
			treeView1.AfterLabelEdit += delegate(object sender, NodeLabelEditEventArgs e) {
				Console.WriteLine("Renamed level to " + e.Label);
				// Editing the label renames the level.
			};
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
				levelDisplay.OpenFile(openFileDialog.FileName);
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
				levelDisplay.SaveFile(saveFileDialog.FileName);
			}
		}

		// Add a new level to the world.
		private void buttonAddLevel_Click(object sender, EventArgs e) {
			LevelAddForm form = new LevelAddForm();
			form.ShowDialog(this);
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public TreeView LevelTreeView {
			get { return treeView1; }
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
	}
}
