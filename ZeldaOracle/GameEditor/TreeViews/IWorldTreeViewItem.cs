using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ZeldaEditor.Control;
using ZeldaEditor.Controls;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Worlds;

namespace ZeldaEditor.TreeViews {
			
	// Interface for all nodes in the tree view.
	public abstract class IWorldTreeViewItem : ImageTreeViewItem {
		public virtual void Open(EditorControl editorControl) {}
		public virtual void Delete(EditorControl editorControl) {}
		public virtual void Rename(World world, string name) {}
		public virtual void Duplicate(EditorControl editorControl, string suffix) {}
		public abstract IIDObject IDObject { get; }
	}
}
