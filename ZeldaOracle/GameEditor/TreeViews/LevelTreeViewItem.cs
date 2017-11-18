using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Worlds;
using ZeldaEditor.Control;
//using ZeldaEditor.PropertiesEditor.CustomEditors;
using ZeldaEditor.Util;
using System.Windows.Media;
using ZeldaEditor.Windows;
using System.Windows;
using ZeldaOracle.Common.Scripting;

namespace ZeldaEditor.TreeViews {
	
	public class LevelTreeViewItem : IWorldTreeViewItem {
		private Level level;
		
		public LevelTreeViewItem(Level level) {
			this.level = level;
			Source  = EditorImages.Level;
			Header				= level.ID;
			Tag             = "level";
			
		}

		public override void Open(EditorControl editorControl) {
			editorControl.OpenLevel(level);
			editorControl.OpenObjectProperties(level);
		}

		public override void Delete(EditorControl editorControl) {
			MessageBoxResult result = TriggerMessageBox.Show(editorControl.EditorWindow, MessageIcon.Info,
				"You are about to delete the level '" + level.ID + "'. This will be permanent. Continue?", "Confirm",
				MessageBoxButton.YesNo);

			if (result == MessageBoxResult.Yes) {
				int levelIndex = editorControl.World.Levels.IndexOf(level);
				editorControl.World.RemoveLevel(level);
				editorControl.RefreshWorldTreeView();
				editorControl.IsModified = true;

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

		public override void Rename(World world, string name) {
			world.RenameLevel(level, name);
			Header = level.ID;
		}

		public override void Duplicate(EditorControl editorControl, string suffix) {

		}

		public override IIDObject IDObject { get { return level; } }
	}
}
