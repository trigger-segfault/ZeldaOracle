using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Worlds;
using ZeldaEditor.Control;
using ZeldaEditor.PropertiesEditor.CustomEditors;

namespace ZeldaEditor.TreeViews {
	
	public class LevelTreeNode : IWorldTreeViewNode {
		private Level level;

		public LevelTreeNode(Level level) {
			this.level = level;
			ImageIndex			= 2;
			SelectedImageIndex	= 2;
			Text				= level.ID;
			Name				= "level";
			
		}

		public override void Open(EditorControl editorControl) {
			editorControl.OpenLevel(level);
			editorControl.OpenObjectProperties(level);
		}

		public override void Delete(EditorControl editorControl) {
			DialogResult result = MessageBox.Show(editorControl.EditorForm,
				"You are about to delete the level '" + level.ID + "'. This will be permanent. Continue?", "Confirm",
				MessageBoxButtons.YesNo, MessageBoxIcon.Information,
				MessageBoxDefaultButton.Button2);

			if (result == DialogResult.Yes) {
				int levelIndex = editorControl.World.IndexOfLevel(level);
				editorControl.World.RemoveLevel(level);
				editorControl.RefreshWorldTreeView();

				if (editorControl.World.LevelCount == 0) {
					editorControl.CloseLevel();
					//worldTreeView.SelectedNode = worldTreeView.Nodes[0].Nodes[0];
					//treeViewLevels_AfterSelect(null, new TreeViewEventArgs(worldTreeView.SelectedNode));
				}
				else {
					editorControl.OpenLevel(Math.Max(0, levelIndex - 1));
					//worldTreeView.SelectedNode = worldTreeView.Nodes[0].Nodes[0].Nodes[GMath.Max(0, index - 1)];
				}
			}
		}

		public override void Rename(string name) {
			level.ID = name;
		}

		public override void Duplicate(EditorControl editorControl, string suffix) {

		}
	}
}
