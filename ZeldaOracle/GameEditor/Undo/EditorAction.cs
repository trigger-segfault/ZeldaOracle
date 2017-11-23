using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Game.Worlds;
using ZeldaEditor.Control;
using System.Windows.Media;
using ZeldaEditor.Controls;

namespace ZeldaEditor.Undo {
	public enum ActionExecution {
		None,
		Execute,
		PostExecute
	}
	public abstract class EditorAction : HistoryListViewItem {
		
		public virtual void Execute(EditorControl editorControl) {
			Redo(editorControl);
		}
		public virtual void PostExecute(EditorControl editorControl) {
			PostRedo(editorControl);
		}

		public abstract void Undo(EditorControl editorControl);
		public abstract void Redo(EditorControl editorControl);

		public virtual void PostUndo(EditorControl editorControl) { }
		public virtual void PostRedo(EditorControl editorControl) { }
		
		public virtual bool IgnoreAction { get { return false; } }
		
	}
}
