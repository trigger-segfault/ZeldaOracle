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
	
	/// <summary>An action that can be done and undone.</summary>
	/// <typeparam name="Context">Type of the context which is passed into Undo/Redo
	/// functions</typeparam>
	public abstract class UndoAction<Context> : HistoryListViewItem {
		
		//-----------------------------------------------------------------------------
		// Virtual Execution
		//-----------------------------------------------------------------------------

		/// <summary>Execute and initialize the action.</summary>
		public virtual void Execute(Context context) {
			Redo(context);
		}

		/// <summary>Finalize an action after it was executed manually.</summary>
		public virtual void PostExecute(Context context) { }
		

		//-----------------------------------------------------------------------------
		// Abstract Undo/Redo
		//-----------------------------------------------------------------------------

		/// <summary>Undo the action.</summary>
		public abstract void Undo(Context context);
		/// <summary>Redo the action.</summary>
		public abstract void Redo(Context context);
		

		//-----------------------------------------------------------------------------
		// Virtual Properties
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if the action should not be pushed onto the stack
		/// because nothing happened.</summary>
		public virtual bool IgnoreAction { get { return false; } }
	}
}
