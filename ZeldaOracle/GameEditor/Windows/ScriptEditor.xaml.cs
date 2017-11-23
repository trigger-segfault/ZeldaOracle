using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Indentation;
using ICSharpCode.CodeCompletion;
using ZeldaEditor.Control;
using ZeldaEditor.Scripting;
using ZeldaOracle.Game.Control.Scripting;

namespace ZeldaEditor.Windows {
	/// <summary>
	/// Interaction logic for ScriptEditor.xaml
	/// </summary>
	public partial class ScriptEditor : Window {

		private EditorControl               editorControl;
		private Script                      script;
		private bool                        newScript;
		private Task<ScriptCompileResult>   compileTask;        // The async task that compiles the code as it changes.
		private bool                        needsRecompiling;   // Has the code changed and needs to be recompiled?
		private ScriptCompileError          displayedError;
		private String                      previousName;       // The name of the script when the editor was opened.
		private String                      previousCode;       // The code of the script when the editor was opened.
		private bool                        autoCompile;
		private bool                        compileOnClose;
		private DispatcherTimer             timer;
		private bool                        loaded;

		private static CSharpCompletion     completion;

		public static void Initialize() {
			Assembly[] assemblies = new Assembly[] {
				typeof(ZeldaAPI.Tile).Assembly,
				typeof(object).Assembly,
				typeof(Uri).Assembly,
				typeof(Enumerable).Assembly
			};
			completion = new CSharpCompletion(new ScriptProvider(), assemblies);
		}

		public ScriptEditor(Script script, EditorControl editorControl, bool newScript) {
			InitializeComponent();
			this.loaded = false;
			this.script = script;
			this.editorControl = editorControl;
			this.newScript = newScript;
			this.previousName = script.ID;
			this.previousCode = script.Code;
			autoCompile = true;
			compileOnClose = true;
			needsRecompiling = false;
			compileTask = null;

			if (script.HasErrors) {
				displayedError = script.Errors[0];
				statusErrorMessage.Text  = displayedError.ToString();
				statusError.Visibility = Visibility.Visible;
			}
			else {
				displayedError = null;
				statusErrorMessage.Text  = "";
				statusError.Visibility = Visibility.Hidden;
			}

			Title = "Script Editor: " + script.ID;
			textBoxName.Text = script.ID;

			editor.Completion = completion;
			editor.Document.FileName = "dummyFileName.cs";
			editor.Script = script;
			editor.EditorControl = editorControl;
			editor.Text = script.Code;
			editor.TextArea.Margin = new Thickness(4, 4, 0, 4);
			editor.TextArea.TextView.Options.AllowScrollBelowDocument = true;
			editor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");

			// Selection Style
			editor.TextArea.SelectionCornerRadius = 0;
			editor.TextArea.SelectionBorder = null;
			editor.FontFamily = new FontFamily("Lucida Console");
			editor.FontSize = 12.667;
			editor.TextChanged += OnTextChanged;
			editor.TextArea.Caret.PositionChanged += OnCaretPositionChanged;
			timer = new DispatcherTimer(TimeSpan.FromMilliseconds(500), DispatcherPriority.ApplicationIdle, delegate { RecompileUpdate(); }, Dispatcher);
			TextOptions.SetTextFormattingMode(editor, TextFormattingMode.Display);
			editor.IsModified = false;
			editor.Focus();
			editor.TextArea.IndentationStrategy = new DefaultIndentationStrategy();
			UpdateStatusBar();
			loaded = true;
		}


		private void OnCaretPositionChanged(object sender, EventArgs e) {
			if (!loaded) return;
			UpdateStatusBar();
		}
		private void OnTextChanged(object sender, EventArgs e) {
			needsRecompiling = true;
			UpdateStatusBar();
			CommandManager.InvalidateRequerySuggested();
		}

		private void OnRecompileCheck(object sender, ElapsedEventArgs e) {
			RecompileUpdate();
		}

		private void OnFinished(object sender, RoutedEventArgs e) {
			if (UpdateScript()) {
				DialogResult = true;
				Close();
			}
		}

		public static bool Show(Window owner, Script script, EditorControl editorControl, bool newScript) {
			ScriptEditor editor = new ScriptEditor(script, editorControl, newScript);
			editor.Owner = owner;
			var result = editor.ShowDialog();
			return result.HasValue && result.Value;
		}

		private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e) {
			if ((!DialogResult.HasValue || !DialogResult.Value) && editor.IsModified) {
				var result = TriggerMessageBox.Show(this, MessageIcon.Warning, "Save changes to the code?", "Warning", MessageBoxButton.YesNoCancel);

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

			if (DialogResult.HasValue && DialogResult.Value && editor.IsModified) {
				editorControl.NeedsRecompiling = true;
			}
		}

		private bool UpdateScript() {
			string newID = textBoxName.Text;
			Script containedID = null;
			if (string.IsNullOrWhiteSpace(newID)) {
				TriggerMessageBox.Show(this, MessageIcon.Warning,
					"Script ID cannot be empty or whitespace!", "Invalid ID");
				return false;
			}
			else if (script.ID != newID) {
				if (!ScriptManager.IsValidScriptName(newID)) {
					TriggerMessageBox.Show(this, MessageIcon.Warning,
						"Script ID must start with a letter or underscore and can only contain letters, digits, and underscores!", "Invalid Script ID");
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
				if (newScript)
					script.ID = newID;
				else
					editorControl.World.RenameScript(script, newID);
				previousName = newID;
				previousCode = editor.Text;
				script.Code = editor.Text;
				editor.IsModified = false;
				return true;
			}
		}

		private void UpdateStatusBar() {
			statusLine.Content = "Line " + editor.TextArea.Caret.Position.Line;
			statusColumn.Content = "Col " + editor.TextArea.Caret.Position.Column;
			statusChar.Content = "Char " + editor.CaretOffset;
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsCompiling {
			get { return (compileTask != null); }
		}

		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		// Begin compiling the script asyncronously.
		private void BeginCompilingScript() {
			script.Code = editor.Text;

			editorControl.CompileScriptAsync(script, OnCompileComplete);

			//compileTask = ScriptEditorCompiler.CompileScriptAsync(script);
			needsRecompiling = false;
		}

		// Check when the compiling has finished.
		private void RecompileUpdate() {
			// Check if we have finished compiling.
			if (IsCompiling) {
				if (compileTask.IsCompleted) {
					ScriptCompileResult results = compileTask.Result;
					compileTask = null;
					OnCompileComplete(results);
				}
			}
			// Begin recompiling.
			else if (needsRecompiling && !editorControl.IsBusyCompiling) {
				BeginCompilingScript();
			}
		}

		// Called once an asyncronous compiling task has completed.
		private void OnCompileComplete(ScriptCompileResult results) {
			Dispatcher.Invoke(() => {
				// Update the script object.
				script.Code             = editor.Text;
				script.Errors           = results.Errors;
				script.Warnings         = results.Warnings;

				// Update the error message status-strip.
				if (script.HasErrors) {
					displayedError = script.Errors[0];
					statusErrorMessage.Text  = displayedError.ToString();
					statusError.Visibility = Visibility.Visible;
				}
				else {
					displayedError = null;
					statusErrorMessage.Text  = "";
					statusError.Visibility = Visibility.Hidden;
				}

				// Update the world-tree-view's scripts (if there is an error icon for a failed compile).
				if (!script.IsHidden)
					editorControl.EditorWindow.TreeViewWorld.RefreshScripts();
			});
		}

		private void CanSaveScript(object sender, CanExecuteRoutedEventArgs e) {
			if (!loaded) return;
			e.CanExecute = (editor.IsModified || script.ID != textBoxName.Text);
		}

		private void SaveScript(object sender, ExecutedRoutedEventArgs e) {
			UpdateScript();
		}

		private void OnDeleteScript(object sender, RoutedEventArgs e) {
			var result = MessageBoxResult.Yes;
			if (string.IsNullOrWhiteSpace(previousCode) && string.IsNullOrWhiteSpace(editor.Text))
				result = TriggerMessageBox.Show(this, MessageIcon.Warning, "Are you sure you want to delete this script?", "Delete Script", MessageBoxButton.YesNo);

			if (result == MessageBoxResult.Yes) {
				if (!newScript) {
					editorControl.World.ScriptManager.RemoveScript(script);
					script.ID = "";
				}
				DialogResult = true;
				Close();
			}
		}
	}
}
