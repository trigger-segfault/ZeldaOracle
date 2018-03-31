using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Indentation.CSharp;
using ZeldaEditor.Control;
using ZeldaEditor.Scripting;
using ZeldaEditor.Util;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Common.Util;
using RoslynPad.Editor;
using System.Windows.Shapes;
using Microsoft.CodeAnalysis;
using System.IO;

namespace ZeldaEditor.Windows {
	/// <summary>
	/// Interaction logic for ScriptEditor.xaml
	/// </summary>
	public partial class ScriptEditor : Window {

		private EditorControl               editorControl;
		private Script                      script;
		private bool                        newScript;
		private bool                        internalScript;
		private Task<ScriptCompileResult>   compileTask;        // The async task that compiles the code as it changes.
		private bool                        needsRecompiling;   // Has the code changed and needs to be recompiled?
		private ScriptCompileError          displayedError;
		private string                      previousName;       // The name of the script when the editor was opened.
		private string                      previousCode;       // The code of the script when the editor was opened.
		private StoppableTimer				timer;
		private bool                        loaded;
		private int scriptStart;
		private int lineStart;
		

		private FoldingManager foldingManager;
		private ScriptFoldingStrategy foldingStrategy;

		private DocumentId documentID;

		private static ScriptRoslynHost host;
		static IHighlightingDefinition highlightingDefinition;

		public static void Initialize() {
			// Don't waste anytime waiting on this to finish
			Task.Run(() => new ScriptRoslynHost())
				.ContinueWith((result) => host = result.Result);

			// Load the CSharp extra highlighting
			string fullName = "ZeldaEditor.Themes.CSharpMultilineHighlighting.xshd";
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fullName))
			using (XmlTextReader reader = new XmlTextReader(stream))
				highlightingDefinition = HighlightingLoader.Load(reader, HighlightingManager.Instance);
		}
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public ScriptEditor(Script script, EditorControl editorControl,
			bool newScript, bool internalScript)
		{
			InitializeComponent();
			this.loaded = false;
			this.script = script;
			this.editorControl = editorControl;
			this.newScript = newScript;
			this.internalScript = internalScript;
			this.previousName = script.ID;
			this.previousCode = script.Code;
			//autoCompile = true;
			//compileOnClose = true;
			needsRecompiling = true;
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

			if (internalScript) {
				Title = "Script Editor: __internal_script__";
				textBoxName.Text = "__internal_script__";
				textBoxName.IsEnabled = false;
			}
			else {
				Title = "Script Editor: " + script.ID;
				textBoxName.Text = script.ID;
			}
			this.editor.Loaded += OnEditorLoaded;
			editor.TextArea.TextEntering += OnTextEntering;
			editor.TextArea.Margin = new Thickness(4, 4, 0, 4);
			editor.TextArea.TextView.Options.AllowScrollBelowDocument = true;

			// Selection Style
			editor.TextArea.SelectionCornerRadius = 0;
			editor.TextArea.SelectionBorder = null;
			editor.FontFamily = new FontFamily("Consolas");
			editor.FontSize = 12.667;
			editor.TextChanged += OnTextChanged;
			editor.TextArea.Caret.PositionChanged += OnCaretPositionChanged;

			timer = StoppableTimer.StartNew(
				TimeSpan.FromMilliseconds(500),
				DispatcherPriority.ApplicationIdle,
				TimerUpdate);
			TextOptions.SetTextFormattingMode(editor, TextFormattingMode.Display);
			editor.IsModified = false;
			editor.Focus();
			editor.TextArea.IndentationStrategy = new CSharpIndentationStrategy();
			editor.Options.ConvertTabsToSpaces = false;
			editor.TextArea.TextView.LineSpacing = 17.0 / 15.0;
			UpdateStatusBar();
			loaded = true;
		}


		//-----------------------------------------------------------------------------
		// UI Callbacks
		//-----------------------------------------------------------------------------

		private void OnTextEntering(object sender, TextCompositionEventArgs e) {
			// Prevent typing at the first index because we want it to be readonly.
			// Readonly segments don't allow preventing insertion at the first index.
			if (editor.CaretOffset == 0)
				e.Handled = true;
		}

		private void OnEditorLoaded(object sender, RoutedEventArgs e2) {

			editor.Loaded -= OnEditorLoaded;
			string workDir = System.IO.Path.GetDirectoryName(
				Assembly.GetEntryAssembly().Location);

			//ScriptCodeGenerator codeGenerator =
			//	new ScriptCodeGenerator(editorControl.World);

			//GeneratedScriptCode generatedCode =
			//	codeGenerator.GenerateTestCode(script, script.Code);
			//scriptStart = 
			//lineStart = 1;
			//string code = script.Code;

			string code = ScriptCodeGenerator.CreateRoslynScriptCode(script, null,
				out scriptStart, out lineStart);

			while (host == null)
				Thread.Sleep(10);
			
			documentID = editor.Initialize(host, new ClassificationHighlightColors(),
				workDir, code);

			var fieldInfo = typeof(RoslynCodeEditor).GetField(
				"_braceCompletionProvider",
				BindingFlags.NonPublic | BindingFlags.Instance);
			fieldInfo.SetValue(editor, new NoBraceCompletionProvider());

			var p = new TextSegmentReadOnlySectionProvider<TextSegment>(
				editor.TextArea.Document);
			p.Segments.Add(new TextSegment() {
				StartOffset = 0, Length = scriptStart
			});

			editor.TextArea.ReadOnlySectionProvider = p;
			editor.CaretOffset = scriptStart;
			editor.TextArea.TextView.LineTransformers.Add(
				new HighlightingColorizer(highlightingDefinition));
			editor.IsModified = false;
			
			Line line = null;
			foreach (var margin in editor.TextArea.LeftMargins) {
				if (margin is LineNumberMargin) {
					var lineNumbers = (LineNumberMargin) margin;
					lineNumbers.StartingLine = lineStart;
				}
			}
			if (line != null)
				editor.TextArea.LeftMargins.Remove(line);

			foldingManager = FoldingManager.Install(editor.TextArea);
			foldingStrategy = new ScriptFoldingStrategy();
			foldingStrategy.UpdateFoldings(foldingManager, editor.Document,
				scriptStart);
			
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

		private void OnRedoCommand(object sender, ExecutedRoutedEventArgs e) {
			editor.Redo();
		}

		private void CanExecuteRedo(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = editor.CanRedo;
		}

		private void OnRecompileCheck(object sender, ElapsedEventArgs e) {
			TimerUpdate();
		}

		private void OnFinished(object sender, RoutedEventArgs e) {
			if (UpdateScript()) {
				DialogResult = true;
				Close();
			}
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
			if (string.IsNullOrWhiteSpace(previousCode) &&
				string.IsNullOrWhiteSpace(editor.Text))
				result = TriggerMessageBox.Show(this, MessageIcon.Warning,
					"Are you sure you want to delete this script?", "Delete Script",
					MessageBoxButton.YesNo);

			if (result == MessageBoxResult.Yes) {
				if (!newScript && !internalScript) {
					editorControl.World.ScriptManager.RemoveScript(script);
					script.ID = "";
				}
				script.Code = "";
				if (!newScript)
					DialogResult = true;
				Close();
			}
		}

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
				host.CloseDocument(documentID);
				editorControl.EditorWindow.WorldTreeView.RefreshScripts(
					!script.IsHidden, script.IsHidden);
				editorControl.NeedsRecompiling = true;
			}
		}

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
				previousCode = editor.Text.Substring(scriptStart);
				script.Code = editor.Text.Substring(scriptStart);
				editor.IsModified = false;
				return true;
			}
		}

		private void UpdateStatusBar() {
			var position = CaretPosition;
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

		private int CalculateVisualColumn() {
			var position = editor.TextArea.Caret.Position;
			int column = position.Column;
			string line = editor.Text.Substring(editor.CaretOffset -
				(position.Column - 1), position.Column - 1);
			int tabSize = editor.Options.IndentationSize;
			if (tabSize != 1) {
				foreach (char c in line) {
					if (c == '\t')
						column += tabSize - 1;
				}
			}
			return column;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if the compile task is running.</summary>
		public bool IsCompiling {
			get { return (compileTask != null); }
		}

		public TextViewPosition CaretPosition {
			get {
				if (editor.TextArea.Caret.Position.Line <= lineStart)
					return new TextViewPosition(-1, -1, -1);
				return new TextViewPosition(
					editor.TextArea.Caret.Line - lineStart,
					editor.TextArea.Caret.Column,
					CalculateVisualColumn());
			}
		}

		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Begin compiling the script asyncronously.</summary>
		private void BeginCompilingScript() {
			script.Code = editor.Text.Substring(scriptStart);

			CompileTask task = editorControl.ScriptCompileService.CompileSingleScript(
				script, editorControl.World);
			task.Completed += OnCompileComplete;
			needsRecompiling = false;
		}

		// Check when the compiling has finished.
		private void TimerUpdate() {
			// Update folding
			foldingStrategy.UpdateFoldings(foldingManager, editor.Document,
				scriptStart);

			// Check if we have finished compiling.
			if (IsCompiling) {
				if (compileTask.IsCompleted) {
					ScriptCompileResult results = compileTask.Result;
					compileTask = null;
					OnCompileComplete(results, null);
				}
			}
			// Begin recompiling
			else if (needsRecompiling) {
				BeginCompilingScript();
			}
		}

		// Called once an asyncronous compiling task has completed.
		private void OnCompileComplete(ScriptCompileResult results,
			GeneratedScriptCode generatedCode)
		{
			Dispatcher.Invoke(() => {
				// Update the script object.
				script.Code             = editor.Text.Substring(scriptStart);
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
			});
		}
	}
}
