using System;
using System.Collections;
using System.Collections.ObjectModel;

namespace ZeldaEditor.Undo {

	public interface IUndoHistory {
		/// <summary>Undos the last 'count' number of executed action. Returns true if
		/// any actions where undone.</summary>
		bool Undo(int count = 1);
		/// <summary>Redos the next 'count' number of undone action. Returns true if
		/// any actions where redone.</summary>
		bool Redo(int count = 1);
		/// <summary>Navigates to the action at the specified position in the history.
		/// Returns true if any actions where undone or redone.</summary>
		bool GoToAction(int position);
		/// <summary>Gets the current position in the undo history. Actions before this
		/// position can be undone while actions after this position can be redone.
		/// </summary>
		int UndoPosition { get; }
		/// <summary>Returns true if there are any actions to undo.</summary>
		bool CanUndo { get; }
		/// <summary>Returns true if there are any actions to redo.</summary>
		bool CanRedo { get; }
		/// <summary>Returns the iterable list of actions.</summary>
		IEnumerable ActionsSource { get; }
	}

	public class UndoHistory<Context> : IUndoHistory {

		private event EventHandler positionChanged;
		private event EventHandler actionUndone;
		private event EventHandler actionRedone;
		private event EventHandler actionUndoing;
		private event EventHandler actionRedoing;

		private ObservableCollection<UndoAction<Context>> undoActions;
		private int undoPosition;
		private Context context;
		private int maxUndos;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public UndoHistory(Context context, int maxUndos) {
			this.context = context;
			this.maxUndos = maxUndos;
			undoPosition = -1;
			undoActions = new ObservableCollection<UndoAction<Context>>();
			positionChanged = null;
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
			positionChanged?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>Pops the last action from the list.</summary>
		public void PopAction() {
			while (undoPosition < undoActions.Count)
				undoActions.RemoveAt(undoPosition);
			undoPosition = undoActions.Count - 1;
			positionChanged?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>Undos the last executed action. Returns true if any actions where
		/// undone.</summary>
		public bool Undo(int count = 1) {
			if (undoPosition <= 0 || count <= 0)
				return false;
			actionUndoing?.Invoke(this, EventArgs.Empty);
			for (int i = 0; i < count && undoPosition > 0; i++) {
				undoActions[undoPosition].IsSelected = false;
				undoActions[undoPosition].IsUndone = true;
				undoActions[undoPosition].Undo(context);
				undoPosition--;
			}
			actionUndone?.Invoke(this, EventArgs.Empty);
			positionChanged?.Invoke(this, EventArgs.Empty);
			return true;
		}

		/// <summary>Redos the next undone action. Returns true if any actions where
		/// redone.</summary>
		public bool Redo(int count = 1) {
			if (undoPosition + 1 >= undoActions.Count || count <= 0)
				return false;
			actionRedoing?.Invoke(this, EventArgs.Empty);
			for (int i = 0; i < count && undoPosition + 1 < undoActions.Count; i++) {
				undoActions[undoPosition].IsSelected = false;
				undoPosition++;
				undoActions[undoPosition].IsUndone = false;
				undoActions[undoPosition].Redo(context);
			}
			actionRedone?.Invoke(this, EventArgs.Empty);
			positionChanged?.Invoke(this, EventArgs.Empty);
			return true;
		}

		/// <summary>Navigates to the specified action in the history. Returns true if
		/// any actions where undone or redone.</summary>
		public bool GoToAction(UndoAction<Context> action) {
			return GoToAction(undoActions.IndexOf(action));
		}

		/// <summary>Navigates to the action at the specified position in the history.
		/// Returns true if any actions where undone or redone.</summary>
		public bool GoToAction(int position) {
			if (position == -1)
				return false;

			// Undo if the position is behind, redo if the position is ahead
			if (position < undoPosition)
				return Undo(undoPosition - position);
			else if (position > undoPosition)
				return Redo(position - undoPosition);
			return false;
		}

		/// <summary>Clears all undo actions except the original action.
		/// AKA: Open World or New World.</summary>
		public void PopToOriginalAction() {
			while (undoActions.Count > 1)
				undoActions.RemoveAt(1);
			undoPosition = 0;
			positionChanged?.Invoke(this, EventArgs.Empty);
		}
		
		/// <summary>Removes all actions from the history.</summary>
		public void Clear() {
			undoPosition = -1;
			undoActions.Clear();
			positionChanged?.Invoke(this, EventArgs.Empty);
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
			get {
				if (undoPosition >= 0)
					return undoActions[undoPosition];
				return null;
			}
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

		/// <summary>Called after the position in the undo history changes, after any
		/// actions are undone or redone.</summary>
		public event EventHandler PositionChanged {
			add { positionChanged += value; }
			remove { positionChanged -= value; }
		}

		/// <summary>Called before an action is undone. If multiple actions are undone
		/// at once, then this is called after all of them are undone. This is not
		/// called when a new action is pushed onto the undo stack.</summary>
		public event EventHandler ActionUndoing {
			add { actionUndoing += value; }
			remove { actionUndoing -= value; }
		}

		/// <summary>Called after an action is undone. If multiple actions are undone
		/// at once, then this is called after all of them are undone. This is not
		/// called when a new action is pushed onto the undo stack.</summary>
		public event EventHandler ActionUndone {
			add { actionUndone += value; }
			remove { actionUndone -= value; }
		}

		/// <summary>Called after an action is undone. If multiple actions are redone
		/// at once, then this is called after all of them are redone.</summary>
		public event EventHandler ActionRedone {
			add { actionRedone += value; }
			remove { actionRedone -= value; }
		}

		/// <summary>Called after an action is undone. If multiple actions are redone
		/// at once, then this is called before any of them are redone.</summary>
		public event EventHandler ActionRedoing {
			add { actionRedoing += value; }
			remove { actionRedoing -= value; }
		}

		/// <summary>Returns the iterable list of actions.</summary>
		public IEnumerable ActionsSource {
			get { return undoActions; }
		}
	}
}
