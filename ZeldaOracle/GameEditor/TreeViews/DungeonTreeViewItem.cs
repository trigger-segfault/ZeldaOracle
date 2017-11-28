using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ZeldaEditor.Control;
//using ZeldaEditor.PropertiesEditor.CustomEditors;
using ZeldaEditor.Scripting;
using ZeldaEditor.Undo;
using ZeldaEditor.Util;
using ZeldaEditor.Windows;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Worlds;

namespace ZeldaEditor.TreeViews {
	
	public class DungeonTreeViewItem : IWorldTreeViewItem {

		private Dungeon dungeon;
		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		public DungeonTreeViewItem(Dungeon dungeon) {
			this.dungeon = dungeon;
			Source  = EditorImages.Dungeon;
			Header				= dungeon.ID;
			Tag				= "script";
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Open(EditorControl editorControl) {
			// Open the dungeon's properties.
			editorControl.OpenProperties(dungeon);
		}

		public override void Delete(EditorControl editorControl) {
			MessageBoxResult result = TriggerMessageBox.Show(editorControl.EditorWindow, MessageIcon.Warning,
				"Are you sure you want to delete the dungeon '" + dungeon.ID + "'?", "Confirm",
				MessageBoxButton.YesNo);

			if (result == MessageBoxResult.Yes) {
				ActionDeleteDungeon action = new ActionDeleteDungeon(dungeon);
				editorControl.PushAction(action, ActionExecution.Execute);
				/*editorControl.World.RemoveDungeon(dungeon);
				editorControl.EditorWindow.TreeViewWorld.RefreshDungeons();
				editorControl.IsModified = true;*/
			}
		}

		public override void Rename(EditorControl editorControl, string name) {
			editorControl.World.RenameDungeon(dungeon, name);
			Header = dungeon.ID;
			editorControl.IsModified = true;
		}

		public override void Duplicate(EditorControl editorControl) {
			// Dummy dungeon for the rename window
			Dungeon duplicate = new Dungeon();
			string newName = RenameWindow.Show(Window.GetWindow(this), editorControl.World, duplicate);
			if (newName != null) {
				EditorAction action = new ActionDuplicateDungeon(dungeon, newName);
				editorControl.PushAction(action, ActionExecution.Execute);
			}
		}

		public Dungeon Dungeon {
			get { return dungeon; }
		}

		public override IIDObject IDObject { get { return dungeon; } }
	}
}
