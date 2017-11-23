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
			editorControl.OpenObjectProperties(dungeon);
		}

		public override void Delete(EditorControl editorControl) {
			MessageBoxResult result = TriggerMessageBox.Show(editorControl.EditorWindow, MessageIcon.Info,
				"You are about to delete the dungeon '" + dungeon.ID + "'. This will be permanent. Continue?", "Confirm",
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
			Dungeon duplicate = new Dungeon(dungeon);
			duplicate.ID = "";
			string newName = RenameWindow.Show(Window.GetWindow(this), editorControl.World, duplicate);
			if (newName != null) {
				duplicate.ID = newName;
				editorControl.AddDungeon(duplicate, true);
				editorControl.EditorWindow.TreeViewWorld.RefreshDungeons();
			}
		}

		public Dungeon Dungeon {
			get { return dungeon; }
		}

		public override IIDObject IDObject { get { return dungeon; } }
	}
}
