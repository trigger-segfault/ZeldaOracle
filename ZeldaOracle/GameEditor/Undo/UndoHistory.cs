using System;
using System.Collections.ObjectModel;
using ZeldaEditor.Control;

namespace ZeldaEditor.Undo {

	public class UndoHistory<Context> {

		private ObservableCollection<UndoAction<Context>> undoActions;
		private int undoPosition;
		private Context context;
		private int maxUndos;
		//public event Action ActionRedone;
		//public event Action ActionUndone;
		//public event Action UndoPositionChanged;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public UndoHistory(Context context, int maxUndos) {
			this.context = context;
			this.maxUndos = maxUndos;
			undoPosition = -1;
			undoActions = new ObservableCollection<UndoAction<Context>>();
		}


		//-----------------------------------------------------------------------------
		// Action Interface
		//-----------------------------------------------------------------------------

		/// <summary>Pushes the action to the list and performs the specified
		/// execution.</summary>
		public void PushAction(UndoAction<Context> action, ActionExecution execution) {
			// Ignore if nothing occurred during this action
			if (action.IgnoreAction)
				return;

			// Execute the action if requested
			if (execution == ActionExecution.Execute)
				action.Execute(context);
			else if (execution == ActionExecution.PostExecute)
				action.PostExecute(context);

			// Deselect the previous action
			if (undoPosition >= 0)
				undoActions[undoPosition].IsSelected = false;

			// Clear any redone actions
			while (undoPosition + 1 < undoActions.Count)
				undoActions.RemoveAt(undoPosition + 1);

			// Remove old actions if we will exceed our maximum count
			while (undoActions.Count + 1 > maxUndos)
				undoActions.RemoveAt(0);

			// Push the new action onto the stack
			undoActions.Add(action);
			undoPosition = undoActions.Count - 1;
		}

		/// <summary>Pops the last action from the list.</summary>
		public void PopAction() {
			while (undoPosition < undoActions.Count)
				undoActions.RemoveAt(undoPosition);
			undoPosition = undoActions.Count - 1;
		}

		/// <summary>Undos the last executed action.</summary>
		public void Undo(int count = 1) {
			if (undoPosition <= 0)
				return;
			for (int i = 0; i < count && undoPosition > 0; i++) {
				undoActions[undoPosition].IsSelected = false;
				undoPosition--;
				undoActions[undoPosition].IsUndone = true;
				undoActions[undoPosition].Undo(context);
			}
		}

		/// <summary>Redos the next undone action.</summary>
		public void Redo(int count = 1) {
			if (undoPosition + 1 >= undoActions.Count)
				return;
			for (int i = 0; i < count && undoPosition + 1 < undoActions.Count; i++) {
				undoActions[undoPosition].IsSelected = false;
				undoPosition++;
				undoActions[undoPosition].IsUndone = false;
				undoActions[undoPosition].Redo(context);
			}
		}

		/// <summary>Navigates to the action at the specified position in the history.
		/// </summary>
		public void GoToAction(int position) {
			if (position == -1)
				return;

			// Undo if the position is behind, redo if the position is ahead
			if (position < undoPosition)
				Undo(undoPosition - position);
			else if (position > undoPosition)
				Redo(position - undoPosition);
		}
		

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the complete list of undo actions.</summary>
		public ObservableCollection<UndoAction<Context>> UndoActions {
			get { return undoActions; }
		}

		/// <summary>The current action to be undone.</summary>
		public UndoAction<Context> LastAction {
			get { return undoActions[undoPosition]; }
		}

		/// <summary>Gets the current position in the undo history. Actions before this
		/// position can be undone while actions after this position can be redone.
		/// </summary>
		public int UndoPosition {
			get { return undoPosition; }
		}

		/// <summary>Returns true if there are any actions to undo.</summary>
		public bool CanUndo {
			get { return (undoPosition > 0); }
		}

		/// <summary>Returns true if there are any actions to redo.</summary>
		public bool CanRedo {
			get { return (undoPosition + 1 < undoActions.Count); }
		}
	}
}
