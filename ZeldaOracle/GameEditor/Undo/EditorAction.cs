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
	/// <summary>How an action should be executed when it's pushed onto the stack.</summary>
	public enum ActionExecution {
		/// <summary>No execution, the action was executed manually.</summary>
		None,
		/// <summary>Fully execute the action.</summary>
		Execute,
		/// <summary>Finalize the action after it was executed manually.</summary>
		PostExecute
	}

	/// <summary>An action in the editor that makes changes to the world file.
	///<para>All changes to the world file must be represented as an action.</para></summary>
	public abstract class EditorAction : HistoryListViewItem {

		//-----------------------------------------------------------------------------
		// Virtual Execution
		//-----------------------------------------------------------------------------

		/// <summary>Execute and initialize the action.</summary>
		public virtual void Execute(EditorControl editorControl) {
			Redo(editorControl);
		}
		/// <summary>Finalize an action after it was executed manually.</summary>
		public virtual void PostExecute(EditorControl editorControl) { }


		//-----------------------------------------------------------------------------
		// Abstract Undo/Redo
		//-----------------------------------------------------------------------------

		/// <summary>Undo the action.</summary>
		public abstract void Undo(EditorControl editorControl);
		/// <summary>Redo the action.</summary>
		public abstract void Redo(EditorControl editorControl);


		//-----------------------------------------------------------------------------
		// Virtual Properties
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if the action should not be pushed onto the stack
		/// because nothing happened.</summary>
		public virtual bool IgnoreAction { get { return false; } }
	}
}
