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
	
	public class WorldTreeNode : IWorldTreeViewNode {
		private World world;

		public WorldTreeNode(World world) {
			this.world = world;
			ImageIndex			= 0;
			SelectedImageIndex	= 0;
			Text				= world.ID;
			Name				= "world";
			
		}

		public override void Open(EditorControl editorControl) {
			editorControl.OpenObjectProperties(world);
		}

		public override void Rename(string name) {
			world.ID = name;
		}
	}
}
