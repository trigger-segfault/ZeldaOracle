using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using ZeldaEditor.Control;
using ZeldaEditor.Util;
using ZeldaOracle.Common.Scripting;

namespace ZeldaEditor.Windows {
	/// <summary>The type of information to refactor.</summary>
	public enum RefactorType {
		Properties,
		Events
	}

	/// <summary>A window for renaming properties or events in the world that have
	/// been modified as the game engine develops.</summary>
	public partial class RefactorWindow : Window {

		//-----------------------------------------------------------------------------
		// Internal Classes
		//-----------------------------------------------------------------------------

		/// <summary>The scope of information being refactored.</summary>
		private enum RefactorScope {
			World,
			Level,
			Selection
		}


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		// Static ---------------------------------------------------------------------

		private static bool lastNobase = true;
		private static RefactorScope lastScope = RefactorScope.World;
		
		// Instance -------------------------------------------------------------------

		private RefactorType refactorType;
		private RefactorScope refactorScope;
		private EditorControl editorControl;
		private Task<int> searchTask;
		private CancellationTokenSource cancellationToken;
		private StoppableTimer updateTimer;
		private bool needsToSearch;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the refactor window.</summary>
		private RefactorWindow(EditorControl editorControl, RefactorType refactorType, EventHandler onClosed) {
			InitializeComponent();

			this.Closed += onClosed;
			this.editorControl = editorControl;
			this.needsToSearch = true;
			this.refactorType = refactorType;
			this.updateTimer = StoppableTimer.StartNew(
				TimeSpan.FromMilliseconds(100),
				DispatcherPriority.ApplicationIdle,
				UpdateSearch);
			/*this.updateTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(100),
				DispatcherPriority.ApplicationIdle, delegate { UpdateSearch(); },
				Application.Current.Dispatcher);*/

			textBoxFind.Focus();

			buttonReplaceAll.IsEnabled = false;
			buttonReplaceAll.Content = "Remove All";
			labelCount.Content = "Searching...";
			Title = "Refactor " + GetPluralName(2);
			labelFind.Content = "Find " + GetPluralName(2).ToLower() +
				" with name:";
			labelReplace.Content = "Replace " + GetPluralName(1).ToLower() +
				" names with:";
			if (refactorType == RefactorType.Properties) {
				Icon = EditorImages.PropertyRefactor;
				checkBoxNoBase.Content = "Only if no base property exists";
			}
			else {
				Icon = EditorImages.EventRefactor;
				checkBoxNoBase.Content = "Only if no event documentation exists";
			}

			AddRefactorScope("Entire World", RefactorScope.World);
			AddRefactorScope("Current Level", RefactorScope.Level);
			AddRefactorScope("Current Selection", RefactorScope.Selection);
			if (comboBoxScope.SelectedIndex == -1)
				comboBoxScope.SelectedIndex = 0;
		}


		//-----------------------------------------------------------------------------
		// Show Window
		//-----------------------------------------------------------------------------

		/// <summary>Shows and returns the refactor window.</summary>
		public static RefactorWindow Show(Window owner, EditorControl editorControl, RefactorType refactorType, EventHandler onClosed) {
			RefactorWindow window = new RefactorWindow(editorControl, refactorType, onClosed);
			window.Owner = owner;
			window.Show();
			return window;
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------
		
		private void OnWindowClosing(object sender, CancelEventArgs e) {
			lastScope = refactorScope;
			lastNobase = checkBoxNoBase.IsChecked.Value;
		}

		private void OnPreviewKeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Escape) {
				Close();
			}
		}

		private void OnFindTextChanged(object sender, TextChangedEventArgs e) {
			needsToSearch = true;
			buttonReplaceAll.IsEnabled = !string.IsNullOrWhiteSpace(textBoxFind.Text);
		}

		private void OnReplaceTextChanged(object sender, TextChangedEventArgs e) {
			bool remove = string.IsNullOrWhiteSpace(textBoxReplace.Text);
			if (remove)
				buttonReplaceAll.Content = "Remove All";
			else
				buttonReplaceAll.Content = "Replace All";
		}

		private void OnScopeChanged(object sender, SelectionChangedEventArgs e) {
			refactorScope = (RefactorScope) ((ComboBoxItem) comboBoxScope.SelectedItem).Tag;
			needsToSearch = true;
		}

		private void OnNoBaseChecked(object sender, RoutedEventArgs e) {
			needsToSearch = true;
		}

		private void OnReplaceAll(object sender, RoutedEventArgs e) {
			if (!CheckCanReplace())
				return;

			string findName = textBoxFind.Text;
			string replaceName = textBoxReplace.Text;
			bool noBase = checkBoxNoBase.IsChecked.Value;
			int count = 0;
			bool remove = string.IsNullOrWhiteSpace(replaceName);
			
			if (refactorType == RefactorType.Properties) {
				IPropertyObjectContainer scope = GetPropertyScope();
				foreach (IPropertyObject propertyObject in scope.GetPropertyObjects()) {
					if (!remove && propertyObject.Properties.RenameProperty(findName, replaceName, noBase))
						count++;
					else if (remove && propertyObject.Properties.RemoveProperty(findName, noBase))
						count++;
				}
			}
			else {
				IEventObjectContainer scope = GetEventScope();
				foreach (IEventObject eventObject in scope.GetEventObjects()) {
					if (!remove && eventObject.Events.RenameEvent(findName, replaceName, noBase))
						count++;
					else if (remove && eventObject.Events.RemoveEvent(findName, noBase))
						count++;
				}
			}

			string renameString = (remove ? "removed" : "renamed");

			TriggerMessageBox.Show(this, MessageIcon.Info, count + " " +
				GetPluralName(count).ToLower() + " " + renameString + "!",
				"Refactor " + GetPluralName(2));

			if (count > 0) {
				editorControl.UndoHistory.PopToOriginalAction();
				editorControl.PropertyGrid.RefreshProperties();
				editorControl.IsModified = true;
				needsToSearch = true;
			}
		}

		private void OnClose(object sender, RoutedEventArgs e) {
			Close();
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		private bool CheckCanReplace() {
			if (editorControl.UndoPosition > 0) {
				var result = TriggerMessageBox.Show(this, MessageIcon.Warning, "Refactoring " +
					"is not supported as an undo action. If you continue with refactoring, " +
					"all previous undo actions will be purged. Are you sure you want to continue?",
					"Continue Refactor", MessageBoxButton.YesNo);
				if (result == MessageBoxResult.No)
					return false;
			}
			if (!editorControl.IsWorldOpen) {
				TriggerMessageBox.Show(this, MessageIcon.Warning, "Cannot refactor without an open world!", "Can't Refactor");
				return false;
			}
			else {
				switch (refactorScope) {
				case RefactorScope.Selection:
					if (!editorControl.IsLevelOpen) {
						TriggerMessageBox.Show(this, MessageIcon.Warning, "Cannot refactor selection when no level is open", "Can't Refactor");
						return false;
					}
					if (editorControl.CurrentTool != editorControl.ToolSelection) {
						TriggerMessageBox.Show(this, MessageIcon.Warning, "Cannot refactor selection when the selection tool is inactive!", "Can't Refactor");
						return false;
					}
					else if (!editorControl.ToolSelection.HasSelection) {
						TriggerMessageBox.Show(this, MessageIcon.Warning, "Cannot refactor selection when there is no active selection!", "Can't Refactor");
						return false;
					}
					break;
				case RefactorScope.Level:
					if (!editorControl.IsLevelOpen) {
						TriggerMessageBox.Show(this, MessageIcon.Warning, "Cannot refactor level when no level is open", "Can't Refactor");
						return false;
					}
					break;
				}
			}
			return true;
		}

		private void AddRefactorScope(string name, RefactorScope scope) {
			ComboBoxItem item = new ComboBoxItem();
			item.Content = name;
			item.Tag = scope;
			comboBoxScope.Items.Add(item);
			if (scope == lastScope)
				comboBoxScope.SelectedItem = item;
		}

		private void UpdateSearch() {
			if (searchTask != null) {
				if (searchTask.IsCompleted) {
					labelCount.Content = searchTask.Result + " " +
						GetPluralName(searchTask.Result).ToLower() + " found";
					cancellationToken = null;
					searchTask = null;
				}
			}
			if (needsToSearch) {
				needsToSearch = false;
				if (searchTask != null) {
					cancellationToken.Cancel();
					cancellationToken = null;
					searchTask = null;
				}
				labelCount.Content = "Searching...";
				cancellationToken = new CancellationTokenSource();
				string findName = textBoxFind.Text;
				string replaceName = textBoxReplace.Text;
				bool noBase = checkBoxNoBase.IsChecked.Value;
				searchTask = Task.Run(() => Search(findName, replaceName, noBase),
					cancellationToken.Token);
			}
		}

		private int Search(string findName, string replaceName, bool noBase) {
			int count = 0;
			bool remove = string.IsNullOrWhiteSpace(replaceName);

			if (string.IsNullOrWhiteSpace(findName)) {

			}
			else if (refactorType == RefactorType.Properties) {
				IPropertyObjectContainer scope = GetPropertyScope();
				foreach (IPropertyObject propertyObject in scope.GetPropertyObjects()) {
					if (!noBase && propertyObject.Properties.Contains(findName, false))
						count++;
					else if (noBase && propertyObject.Properties.ContainsWithNoBase(findName))
						count++;
				}
			}
			else {
				IEventObjectContainer scope = GetEventScope();
				foreach (IEventObject eventObject in scope.GetEventObjects()) {
					if (!noBase) {
						if (!remove && eventObject.Events.CanRenameEvent(findName, replaceName))
							count++;
						else if (remove && eventObject.Events.ContainsEvent(findName))
							count++;
					}
					else if (eventObject.Events.ContainsWithNoDocumentation(findName)) {
						count++;
					}
				}
			}

			return count;
		}

		private IPropertyObjectContainer GetPropertyScope() {
			IPropertyObjectContainer scope = editorControl.World;
			switch (refactorScope) {
			case RefactorScope.World: return editorControl.World;
			case RefactorScope.Level: return editorControl.Level;
			case RefactorScope.Selection: return editorControl.ToolSelection;
			}
			return null;
		}

		private IEventObjectContainer GetEventScope() {
			IPropertyObjectContainer scope = GetPropertyScope();
			if (scope is IEventObjectContainer)
				return (IEventObjectContainer) scope;
			return null;
		}

		private string GetPluralName(int count) {
			if (count == 1) {
				if (refactorType == RefactorType.Properties)
					return "Property";
				else
					return "Event";
			}
			else {
				if (refactorType == RefactorType.Properties)
					return "Properties";
				else
					return "Events";
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the type of information this window is refactoring.</summary>
		public RefactorType RefactorType {
			get { return refactorType; }
		}
	}
}
