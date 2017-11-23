using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaEditor.Control;

namespace ZeldaEditor.TreeViews {
			
	// Interface for all nodes in the tree view.
	public abstract class IWorldTreeViewNode : TreeNode {
		public virtual void Open(EditorControl editorControl) {}
		public virtual void Delete(EditorControl editorControl) {}
		public virtual void Rename(string name) {}
		public virtual void Duplicate(EditorControl editorControl, string suffix) {}
	}
}
