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
using ZeldaEditor.Undo;

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
				ActionDeleteLevel action = new ActionDeleteLevel(level);
				editorControl.PushAction(action, ActionExecution.Execute);
				/*int levelIndex = editorControl.World.Levels.IndexOf(level);
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
				}*/
			}
		}

		public override void Rename(EditorControl editorControl, string name) {
			editorControl.World.RenameLevel(level, name);
			Header = level.ID;
			editorControl.IsModified = true;
		}

		public override void Duplicate(EditorControl editorControl) {
			/*Level duplicate = new Level(level);
			duplicate.ID = "";
			string newName = RenameWindow.Show(Window.GetWindow(this), editorControl.World, duplicate);
			if (newName != null) {
				duplicate.ID = newName;
				editorControl.AddLevel(duplicate, true);
			}*/
		}

		public Level Level {
			get { return level; }
		}

		public override IIDObject IDObject { get { return level; } }
	}
}
