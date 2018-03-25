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
using System.Xml;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Indentation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Options;
using RoslynPad.Editor;
//using ICSharpCode.CodeCompletion;
using RoslynPad.Roslyn;
using ZeldaEditor.Control;
using ZeldaEditor.Scripting;
using ZeldaEditor.Util;
using ZeldaOracle.Game;
using ZeldaOracle.Game.Control.Scripting;

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
		//private bool                        autoCompile;
		//private bool                        compileOnClose;
		private StoppableTimer				timer;
		private bool                        loaded;
		private int scriptStart;
		
		private FieldInfo completionFieldInfo;

		private DocumentId documentID;

		//private static CSharpCompletion     completion;

		private static CustomRoslynHost host;
		static IHighlightingDefinition highlightingDefinition;

		public static void Initialize() {
			/*List<Assembly> assemblies = new List<Assembly>();
			assemblies.Add(Assembly.Load("RoslynPad.Roslyn.Windows"));
			assemblies.Add(Assembly.Load("RoslynPad.Editor.Windows"));
			//assemblies.AddRange(Assemblies.Scripting);
			RoslynHostReferences refs = RoslynHostReferences.Default;
			refs = refs.With(assemblyReferences: Assemblies.PureScripting);
			host = new RoslynHost(additionalAssemblies: assemblies, references: refs);*/
			host = new CustomRoslynHost();
			var fullName = "ZeldaEditor.Themes.CSharpMultilineHighlighting.xshd";
			using (var stream = typeof(TextMessageEditor).Assembly.GetManifestResourceStream(fullName))
			using (var reader = new XmlTextReader(stream))
				highlightingDefinition = HighlightingLoader.Load(reader, HighlightingManager.Instance);
		}

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
			//editor.Completion = completion;
			//editor.Document.FileName = "dummyFileName.cs";
			//editor.Script = script;
			//editor.EditorControl = editorControl;
			//editor.Text = script.Code;
			editor.TextArea.TextEntering += OnTextEntering;
			editor.TextArea.Margin = new Thickness(4, 4, 0, 4);
			editor.TextArea.TextView.Options.AllowScrollBelowDocument = true;
			//editor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");

			// Selection Style
			editor.TextArea.SelectionCornerRadius = 0;
			editor.TextArea.SelectionBorder = null;
			editor.FontFamily = new FontFamily("Lucida Console");
			editor.FontSize = 12.667;
			editor.TextChanged += OnTextChanged;
			editor.TextArea.Caret.PositionChanged += OnCaretPositionChanged;
			timer = StoppableTimer.StartNew(
				TimeSpan.FromMilliseconds(500),
				DispatcherPriority.ApplicationIdle,
				RecompileUpdate);
			//timer = new DispatcherTimer(TimeSpan.FromMilliseconds(500), DispatcherPriority.ApplicationIdle, delegate { RecompileUpdate(); }, Dispatcher);
			TextOptions.SetTextFormattingMode(editor, TextFormattingMode.Display);
			editor.IsModified = false;
			editor.Focus();
			//editor.TextArea.IndentationStrategy = new DefaultIndentationStrategy();
			UpdateStatusBar();
			loaded = true;

			completionFieldInfo = typeof(CodeTextEditor).GetField("_completionWindow", BindingFlags.Instance | BindingFlags.NonPublic);
		}

		private void OnTextEntering(object sender, TextCompositionEventArgs e) {
			// Prevent typeing at the first index because we want it to be readonly
			if (editor.CaretOffset == 0)
				e.Handled = true;
		}

		private void OnEditorLoaded(object sender, RoutedEventArgs e2) {

			editor.Loaded -= OnEditorLoaded;
			string workDir = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

			/*DocumentCreationArgs args = new DocumentCreationArgs(
					new ScriptTextContainer(editorControl, editor, script),
					workDir);
			editor.CreatingDocument += (o, e) => {
				e.DocumentId = host.AddDocument(args);
				//documentID = e.DocumentId;
			};*/

			
			string code = editorControl.World.ScriptManager.CreateRoslynScriptCode(script, out scriptStart);
			
			documentID = editor.Initialize(host, new ClassificationHighlightColors(),
				workDir, code);

			var p = new TextSegmentReadOnlySectionProvider<TextSegment>(editor.TextArea.Document);
			p.Segments.Add(new TextSegment() { StartOffset = 0, Length = scriptStart });
			editor.TextArea.ReadOnlySectionProvider = p;
			editor.CaretOffset = scriptStart;
			editor.TextArea.TextView.LineTransformers.Add(new HighlightingColorizer(highlightingDefinition));
			editor.IsModified = false;
			//var space = host.CreateWorkspace();
			/*var space = MSBuildWorkspace.Create();
			var p = ProjectInfo.Create(null, VersionStamp.Default,
				"derp", "derp.dll", LanguageNames.CSharp, );
			var d = DocumentInfo.Create(null, null);
			d.SourceCodeKind = SourceCodeKind.
			p.ParseOptions.
			space.SetCurrentSolution(Solution);
			space.Options = DocumentOptionSet
			Console.WriteLine(editor.CompletionProvider.GetType());
			Console.WriteLine(editor.CompletionProvider.GetType());*/
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

		public static bool ShowRegularEditor(Window owner, Script script, EditorControl editorControl, bool newScript) {
			ScriptEditor editor = new ScriptEditor(script, editorControl, newScript, false);
			editor.Owner = owner;
			var result = editor.ShowDialog();
			return result.HasValue && result.Value;
		}

		public static bool ShowCustomEditor(Window owner, Script script, EditorControl editorControl, bool newScript) {
			ScriptEditor editor = new ScriptEditor(script, editorControl, newScript, true);
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

			if (DialogResult.HasValue && DialogResult.Value) {
				host.CloseDocument(documentID);
				editorControl.EditorWindow.WorldTreeView.RefreshScripts(!script.IsHidden, script.IsHidden);
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
			script.Code = editor.Text.Substring(scriptStart);

			editorControl.CompileScriptAsync(script, OnCompileComplete, true);

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

				//if (!script.IsHidden)
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
			if (string.IsNullOrWhiteSpace(previousCode) && string.IsNullOrWhiteSpace(editor.Text.Substring(scriptStart)))
				result = TriggerMessageBox.Show(this, MessageIcon.Warning, "Are you sure you want to delete this script?", "Delete Script", MessageBoxButton.YesNo);

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

		private void OnRedoCommand(object sender, ExecutedRoutedEventArgs e) {
			editor.Redo();
		}

		private void CanExecuteRedo(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = editor.CanRedo;
		}
	}
}
