using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZeldaEditor.Control;
using ZeldaEditor.Controls;
using ZeldaEditor.Undo;

namespace ZeldaEditor.Windows {
	/// <summary>
	/// Interaction logic for HistoryWindow.xaml
	/// </summary>
	public partial class HistoryWindow : Window {

		private static Size lastWindowSize =  Size.Empty; 
		
		private bool loaded = false;
		private IUndoHistory history;

		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public HistoryWindow(IUndoHistory history, EventHandler onClosed) {
			this.history = history;

			InitializeComponent();

			listView.ItemsSource = history.ActionsSource;
			Closed += onClosed;

			if (lastWindowSize != Size.Empty) {
				Width = lastWindowSize.Width;
				Height = lastWindowSize.Height;
			}

			loaded = true;
		}
		

		public void SelectItem(HistoryListViewItem item) {
			listView.SelectedItem = item;
		}


		//-----------------------------------------------------------------------------
		// Command CanExecute
		//-----------------------------------------------------------------------------

		private void CanExecuteUndo(object sender, CanExecuteRoutedEventArgs e) {
			if (!loaded) return;
			e.CanExecute = history.CanUndo;
		}

		private void CanExecuteRedo(object sender, CanExecuteRoutedEventArgs e) {
			if (!loaded) return;
			e.CanExecute = history.CanRedo;
		}


		//-----------------------------------------------------------------------------
		// Command Execute
		//-----------------------------------------------------------------------------
		
		private void OnUndoCommand(object sender, ExecutedRoutedEventArgs e) {
			history.Undo();
		}

		private void OnRedoCommand(object sender, ExecutedRoutedEventArgs e) {
			history.Redo();
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		private void OnSelectListItem(object sender, RoutedEventArgs e) {
			UndoAction<EditorControl> action = (UndoAction<EditorControl>) sender;
			int position = listView.Items.IndexOf(action);
			history.GoToAction(position);
		}

		private void OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (listView.SelectedItem != null)
				listView.ScrollIntoView(listView.SelectedItem);
		}

		private void OnWindowLoaded(object sender, RoutedEventArgs e) {
			if (listView.SelectedItem != null)
				listView.ScrollIntoView(listView.SelectedItem);
		}
		
		private void OnWindowClosing(object sender, CancelEventArgs e) {
			lastWindowSize = new Size(Width, Height);
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		public static HistoryWindow Show(Window owner,
			IUndoHistory history, EventHandler onClosed)
		{
			HistoryWindow window = new HistoryWindow(history, onClosed);
			window.Owner = owner;
			window.Show();
			return window;
		}
	}
}
