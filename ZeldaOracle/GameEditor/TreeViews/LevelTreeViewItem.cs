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
		
		public LevelTreeViewItem(Level level, EditorControl editorControl) {
			this.level = level;
			Source  = EditorImages.Level;
			Header				= level.ID;
			Tag             = "level";
			if (level == editorControl.Level)
				FontWeight = FontWeights.Bold;
		}

		public override void Open(EditorControl editorControl) {
			editorControl.OpenLevel(level);
			editorControl.OpenProperties(level);
		}

		public override void Delete(EditorControl editorControl) {
			MessageBoxResult result = TriggerMessageBox.Show(editorControl.EditorWindow, MessageIcon.Warning,
				"Are you sure you want to delete the level '" + level.ID + "'?", "Confirm",
				MessageBoxButton.YesNo);


			if (result == MessageBoxResult.Yes) {
				ActionDeleteLevel action = new ActionDeleteLevel(level);
				editorControl.PushAction(action, ActionExecution.Execute);
			}
		}

		public override void Rename(EditorControl editorControl, string name) {
			editorControl.World.RenameLevel(level, name);
			Header = level.ID;
			editorControl.IsModified = true;
		}

		public override void Duplicate(EditorControl editorControl) {
			// Dummy level for the rename window
			Level duplicate = new Level();
			string newName = RenameWindow.Show(Window.GetWindow(this), editorControl.World, duplicate);
			if (newName != null) {
				duplicate = new Level(level);
				EditorAction action = new ActionDuplicateLevel(level, duplicate);
				editorControl.PushAction(action, ActionExecution.Execute);
			}
		}

		public Level Level {
			get { return level; }
		}

		public override IIDObject IDObject { get { return level; } }
	}
}
