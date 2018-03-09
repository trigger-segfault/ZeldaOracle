using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaEditor.Control;
using ZeldaEditor.PropertiesEditor.CustomEditors;
using ZeldaEditor.Scripting;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Worlds;

namespace ZeldaEditor.TreeViews {
	
	public class DungeonTreeNode : IWorldTreeViewNode {

		private Area dungeon;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public DungeonTreeNode(Area dungeon) {
			this.dungeon = dungeon;
			ImageIndex			= 6;
			SelectedImageIndex	= 6;
			Text				= dungeon.ID;
			Name				= "script";
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Open(EditorControl editorControl) {
			// Open the dungeon's properties.
			editorControl.OpenObjectProperties(dungeon);
		}

		public override void Delete(EditorControl editorControl) {
			DialogResult result = MessageBox.Show(editorControl.EditorForm,
				"You are about to delete the script '" + dungeon.ID + "'. This will be permanent. Continue?", "Confirm",
				MessageBoxButtons.YesNo, MessageBoxIcon.Information,
				MessageBoxDefaultButton.Button2);

			if (result == DialogResult.Yes) {
				editorControl.World.RemoveArea(dungeon);
				editorControl.RefreshWorldTreeView();
			}
		}

		public override void Rename(string name) {
			dungeon.ID = name;
		}

		public override void Duplicate(EditorControl editorControl, string suffix) {
			Area duplicate = new Area(dungeon);
			duplicate.Name += suffix;
			editorControl.World.AddArea(dungeon);
			editorControl.RefreshWorldTreeView();
		}
	}
}
