using System;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using ZeldaEditor.Control;
using ZeldaEditor.Controls;
using ZeldaWpf.Util;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaWpf.Controls;
using ZeldaWpf.Windows;

namespace ZeldaEditor.Windows {
	/// <summary>
	/// Interaction logic for ScriptEditor.xaml
	/// </summary>
	public partial class ScriptEditor : TimersWindow {

		private EditorControl               editorControl;
		private ScriptTextEditor			editor;
		private Script                      script;
		private bool                        newScript;
		private bool                        internalScript;
		private bool                        needsRecompiling;   // Has the code changed and needs to be recompiled?
		private CompileTask					compileTask;
		private ScriptCompileError          displayedError;
		private string                      previousName;       // The name of the script when the editor was opened.
		private string                      previousCode;       // The code of the script when the editor was opened.


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public ScriptEditor(Script script, EditorControl editorControl,
			bool newScript, bool internalScript)
		{
			InitializeComponent();
			
			// Create the script text editor
			editor = new ScriptTextEditor();
			Grid.SetRow(editor, 1);
			dockPanel.Children.Add(editor);

			Loaded += OnLoaded;

			this.script = script;
			this.editorControl = editorControl;
			this.newScript = newScript;
			this.internalScript = internalScript;

			previousName = script.ID;
			previousCode = script.Code;
			needsRecompiling = false;
			compileTask = null;
		}


		//-----------------------------------------------------------------------------
		// UI Callbacks
		//-----------------------------------------------------------------------------

		private void OnLoaded(object sender, RoutedEventArgs e) {
			if (internalScript) {
				Title = "Script Editor: __internal_script__";
				textBoxName.Text = "__internal_script__";
				textBoxName.IsEnabled = false;
			}
			else {
				Title = "Script Editor: " + script.ID;
				textBoxName.Text = script.ID;
			}

			ContinuousEvents.Start(0.5, TimerPriority.Low, OnTimerUpdate);
			
			editor.Script = script;
			editor.ScriptCodeChanged += OnTextChanged;
			needsRecompiling = true;

			UpdateStatusBar();
			UpdateDisplayedError();

			editor.Focus();
		}

		/// <summary>Checks periodically if we need to recompile.</summary>
		private void OnTimerUpdate() {
			// Check if we need to compile
			if (needsRecompiling && !IsCompiling)
				BeginCompilingScript();
		}

		private void OnCaretPositionChanged(object sender, EventArgs e) {
			if (IsLoaded && editor.IsLoaded)
				UpdateStatusBar();
		}

		private void OnTextChanged(object sender, EventArgs e) {
			needsRecompiling = true;
			UpdateStatusBar();
			CommandManager.InvalidateRequerySuggested();
		}

		private void OnRedoCommand(object sender, ExecutedRoutedEventArgs e) {
			editor.Redo();
		}

		private void CanExecuteRedo(object sender, CanExecuteRoutedEventArgs e) {
			if (IsLoaded && editor.IsLoaded)
				e.CanExecute = editor.CanRedo;
		}

		private void OnFinished(object sender, RoutedEventArgs e) {
			if (UpdateScript()) {
				DialogResult = true;
				Close();
			}
		}

		private void CanSaveScript(object sender, CanExecuteRoutedEventArgs e) {
			if (IsLoaded && editor.IsLoaded)
				e.CanExecute = (editor.IsModified || script.ID != textBoxName.Text);
		}

		private void SaveScript(object sender, ExecutedRoutedEventArgs e) {
			UpdateScript();
		}

		private void OnWindowClosing(object sender, CancelEventArgs e) {
			if ((!DialogResult.HasValue || !DialogResult.Value) && editor.IsModified) {
				var result = TriggerMessageBox.Show(this, MessageIcon.Warning,
					"Save changes to the code?", "Warning",
					MessageBoxButton.YesNoCancel);

				if (result == MessageBoxResult.Yes) {
					if (!UpdateScript())
						e.Cancel = true;
				}
				else if (result == MessageBoxResult.No) {
					DialogResult = false;
					script.ID = previousName;
					script.Code = previousCode;
				}
				else {
					e.Cancel = true;
				}
			}

			if (DialogResult.HasValue && DialogResult.Value) {
				editorControl.EditorWindow.WorldTreeView.RefreshScripts(true, false);
				editorControl.NeedsRecompiling = true;
			}
		}

		private void OnNameTextChanged(object sender, TextChangedEventArgs e) {
			editor.UpdateMethodName(textBoxName.Text);
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		public static bool ShowRegularEditor(Window owner, Script script,
			EditorControl editorControl, bool newScript)
		{
			ScriptEditor editor = new ScriptEditor(
				script, editorControl, newScript, false);
			editor.Owner = owner;
			var result = editor.ShowDialog();
			return result.HasValue && result.Value;
		}

		public static bool ShowCustomEditor(Window owner, Script script,
			EditorControl editorControl, bool newScript)
		{
			ScriptEditor editor = new ScriptEditor(
				script, editorControl, newScript, true);
			editor.Owner = owner;
			var result = editor.ShowDialog();
			return result.HasValue && result.Value;
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		private bool UpdateScript() {
			string newID = textBoxName.Text;
			Script containedID = null;
			if (!internalScript && string.IsNullOrWhiteSpace(newID)) {
				TriggerMessageBox.Show(this, MessageIcon.Warning,
					"Script ID cannot be empty or whitespace!", "Invalid ID");
				return false;
			}
			else if (!internalScript && script.ID != newID) {
				if (!ScriptManager.IsValidScriptName(newID)) {
					TriggerMessageBox.Show(this, MessageIcon.Warning,
						"Script ID must start with a letter or underscore and can " +
						"only contain letters, digits, and underscores!",
						"Invalid Script ID");
					return false;
				}
				containedID = editorControl.World.GetScript(newID);
			}
			if (containedID != null) {
				TriggerMessageBox.Show(this, MessageIcon.Warning,
						"A script with that ID already exists!", "ID Taken");
				return false;
			}
			else {
				if (!internalScript) {
					if (newScript)
						script.ID = newID;
					else
						editorControl.World.RenameScript(script, newID);
				}
				previousName = newID;
				previousCode = editor.ScriptCode;
				script.Code = editor.ScriptCode;
				editor.IsModified = false;
				return true;
			}
		}

		private void UpdateStatusBar() {
			var position = editor.CaretPosition;
			if (position.Line == -1) {
				statusLine.Content = "Line -";
				statusColumn.Content = "Col -";
				statusChar.Content = "Char -";
			}
			else {
				statusLine.Content = "Line " + position.Line;
				statusColumn.Content = "Col " + position.VisualColumn;
				statusChar.Content = "Char " + position.Column;
			}
		}
		
		private void UpdateDisplayedError() {
			// Update the error message status-strip
			if (script.HasErrors) {
				displayedError = script.Errors[0];
				statusErrorMessage.Text = displayedError.ToString();
				statusError.Visibility = Visibility.Visible;
			}
			else {
				displayedError = null;
				statusErrorMessage.Text  = "";
				statusError.Visibility = Visibility.Hidden;
			}
		}

		/// <summary>Begin compiling the script asyncronously.</summary>
		private void BeginCompilingScript() {
			script.Code = editor.ScriptCode;

			CompileTask compileTask =
				editorControl.ScriptCompileService.CompileSingleScript(
					script, editorControl.World);
			compileTask.Completed += OnCompileComplete;
			needsRecompiling = false;
		}

		/// <summary>Called once an asyncronous compiling task has completed.</summary>
		private void OnCompileComplete(ScriptCompileResult results,
			GeneratedScriptCode generatedCode)
		{
			Dispatcher.Invoke(() => {
				// Update the script object
				script.Code		= editor.ScriptCode;
				script.Errors	= results.Errors;
				script.Warnings	= results.Warnings;

				// Update the error message status-strip
				UpdateDisplayedError();
			});
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if the compile task is running.</summary>
		public bool IsCompiling {
			get { return (compileTask != null && !compileTask.IsCompleted); }
		}
	}
}
