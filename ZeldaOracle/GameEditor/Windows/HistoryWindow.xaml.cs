using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZeldaEditor.Control;
using ZeldaEditor.Controls;

namespace ZeldaEditor.Windows {
	/// <summary>
	/// Interaction logic for HistoryWindow.xaml
	/// </summary>
	public partial class HistoryWindow : Window {

		private static Size lastWindowSize =  Size.Empty; 
		
		private EditorControl editorControl;
		private bool loaded = false;

		public HistoryWindow(EditorControl editorControl, EventHandler onClosed) {
			InitializeComponent();
			this.editorControl = editorControl;
			listView.ItemsSource = editorControl.UndoActions;
			Closed += onClosed;

			if (lastWindowSize != Size.Empty) {
				Width = lastWindowSize.Width;
				Height = lastWindowSize.Height;
			}

			loaded = true;
		}

		public static HistoryWindow Show(Window owner, EditorControl editorControl, EventHandler onClosed) {
			HistoryWindow window = new HistoryWindow(editorControl, onClosed);
			window.Owner = owner;
			window.Show();
			return window;
		}

		private void OnListViewItemPreviewMouseLeftButtonDown(object sender, RoutedEventArgs e) {
			editorControl.GotoAction(listView.Items.IndexOf(sender as HistoryListViewItem));
		}

		private void OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (listView.SelectedItem != null)
				listView.ScrollIntoView(listView.SelectedItem);
		}

		public void SelectItem(HistoryListViewItem item) {
			listView.SelectedItem = item;
		}

		private void OnWindowLoaded(object sender, RoutedEventArgs e) {
			if (listView.SelectedItem != null)
				listView.ScrollIntoView(listView.SelectedItem);
		}

		private void CanExecuteUndo(object sender, CanExecuteRoutedEventArgs e) {
			if (!loaded) return;
			e.CanExecute = editorControl.CanUndo;
		}

		private void OnUndoCommand(object sender, ExecutedRoutedEventArgs e) {
			editorControl.Undo();
		}

		private void CanExecuteRedo(object sender, CanExecuteRoutedEventArgs e) {
			if (!loaded) return;
			e.CanExecute = editorControl.CanRedo;
		}

		private void OnRedoCommand(object sender, ExecutedRoutedEventArgs e) {
			editorControl.Redo();
		}

		private void OnWindowClosing(object sender, CancelEventArgs e) {
			lastWindowSize = new Size(Width, Height);
		}
	}
}
