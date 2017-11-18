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
				"You are about to delete the script '" + dungeon.ID + "'. This will be permanent. Continue?", "Confirm",
				MessageBoxButton.YesNo);

			if (result == MessageBoxResult.Yes) {
				editorControl.World.RemoveDungeon(dungeon);
				editorControl.RefreshWorldTreeView();
				editorControl.IsModified = true;
			}
		}

		public override void Rename(World world, string name) {
			world.RenameDungeon(dungeon, name);
			Header = dungeon.ID;
		}

		public override void Duplicate(EditorControl editorControl, string suffix) {
			Dungeon duplicate = new Dungeon(dungeon);
			duplicate.Name += suffix;
			editorControl.World.AddDungeon(dungeon);
			editorControl.RefreshWorldTreeView();
		}

		public override IIDObject IDObject { get { return dungeon; } }
	}
}
