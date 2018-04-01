using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using Microsoft.CodeAnalysis;
using ICSharpCode.AvalonEdit.Indentation.CSharp;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using RoslynPad.Editor;
using ZeldaEditor.Scripting;
using ZeldaEditor.Util;
using ZeldaOracle.Game.Control.Scripting;

namespace ZeldaEditor.Controls {
	/// <summary>
	/// Interaction logic for ScriptTextEditor.xaml
	/// </summary>
	public partial class ScriptTextEditor : UserControl {

		private static ScriptRoslynHost host;
		private Script script;
		private int scriptStart;
		private int lineStart;
		private FoldingManager foldingManager;
		private ScriptFoldingStrategy foldingStrategy;
		static IHighlightingDefinition highlightingDefinition;
		private DocumentId documentID;
		private StoppableTimer timer;
		private bool suppressEvents;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ScriptTextEditor() {
			InitializeComponent();
			suppressEvents = false;
					
			// Display settings
			editor.TextArea.Margin = new Thickness(4, 4, 0, 4);
			editor.TextArea.TextView.Options.AllowScrollBelowDocument = true;
			editor.TextArea.TextView.LineSpacing = 17.0 / 15.0;
			TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);

			// Selection Style
			editor.TextArea.SelectionCornerRadius = 0;
			editor.TextArea.SelectionBorder = null;
			editor.FontFamily = new FontFamily("Consolas");
			editor.FontSize = 12.667;
			
			// Setup the text folding manager
			foldingManager = FoldingManager.Install(editor.TextArea);
			foldingStrategy = new ScriptFoldingStrategy();

			// Editor options
			editor.TextArea.IndentationStrategy = new CSharpIndentationStrategy();
			editor.Options.ConvertTabsToSpaces = false;
			
			// Setup events
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
			editor.TextArea.TextEntering += OnTextEntering;
			editor.TextChanged += OnTextChanged;
			documentID = null;

			timer = StoppableTimer.StartNew(
				TimeSpan.FromMilliseconds(500),
				DispatcherPriority.ApplicationIdle,
				OnTimerUpdate);
			
			editor.IsModified = false;
			
			Focus();
		}

		
		//-----------------------------------------------------------------------------
		// Text Control
		//-----------------------------------------------------------------------------
		
		/// <summary>Redoes the most recent undone command.</summary>
		/// <returns>True is the redo operation was successful, false is the redo stack is empty.</returns>
		public bool Redo() {
			return editor.Redo();
		}
		
		/// <summary>Occurs when the ScriptCode property changes.</summary>
		public event EventHandler ScriptCodeChanged;

		/// <summary>Raises the <see cref="ScriptCodeChanged"/> event.</summary>
		protected virtual void OnScriptCodeChanged(EventArgs e) {
			if (!suppressEvents)
				ScriptCodeChanged?.Invoke(this, e);
		}
		

		//-----------------------------------------------------------------------------
		// UI Event Callbacks
		//-----------------------------------------------------------------------------
		
		private void OnLoaded(object sender, RoutedEventArgs e2) {
			while (host == null)
				Thread.Sleep(10);
			
			suppressEvents = true;
			{
				// Create the document for the Roslyn host
				string workingDirectory = System.IO.Path.GetDirectoryName(
					Assembly.GetEntryAssembly().Location);
				documentID = editor.Initialize(host, new ClassificationHighlightColors(),
					workingDirectory, "");
			
				// Disable automatic brace completion
				FieldInfo fieldInfo = editor.GetType().GetField(
					"_braceCompletionProvider",
					BindingFlags.NonPublic | BindingFlags.Instance);
				fieldInfo.SetValue(editor, new NoBraceCompletionProvider());

				// Setup the syntax highlighter
				editor.TextArea.TextView.LineTransformers.Add(
					new HighlightingColorizer(highlightingDefinition));
			}
			suppressEvents = false;
			
			// Update the text for the initial script
			ChangeScript();
		}

		private void OnUnloaded(object sender, RoutedEventArgs e) {
			// Close the document from the Roslyn Host
			if (documentID != null) {
				host.CloseDocument(documentID);
				documentID = null;
			}
			timer.Stop();
		}

		private void OnTextEntering(object sender, TextCompositionEventArgs e) {
			// Prevent typing at the first index because we want it to be readonly.
			// Readonly segments don't allow preventing insertion at the first index.
			if (documentID != null && editor.CaretOffset == 0)
				e.Handled = true;
		}

		/// <summary>Forwards the TextChanged event to our ScriptCodeChanged event.
		/// </summary>
		private void OnTextChanged(object sender, EventArgs e) {
			if (!suppressEvents) {
				OnScriptCodeChanged(e);
				CommandManager.InvalidateRequerySuggested();
			}
		}

		private void OnTimerUpdate() {
			if (documentID != null && script != null) {
				foldingStrategy.UpdateFoldings(
					foldingManager, editor.Document, scriptStart);
			}
		}
		

		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Initialize the Roslyn Script Host.</summary>
		public static void Initialize() {
			// Don't waste anytime waiting on this to finish
			Task.Run(() => new ScriptRoslynHost())
				.ContinueWith((result) => { host = result.Result; });

			// Load the CSharp extra highlighting
			string fullName = "ZeldaEditor.Themes.CSharpMultilineHighlighting.xshd";
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fullName))
			using (XmlTextReader reader = new XmlTextReader(stream))
				highlightingDefinition = HighlightingLoader.Load(reader, HighlightingManager.Instance);
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Create the code that's shown in the editor. This code will have
		/// to include members and parameters for intellisense to work properly.
		/// </summary>
		private string CreateScriptCode(out int scriptStart, out int lineStart) {
			//ScriptCodeGenerator codeGenerator =
			//	new ScriptCodeGenerator(editorControl.World);

			//GeneratedScriptCode generatedCode =
			//	codeGenerator.GenerateTestCode(script, script.Code);
			//scriptStart = 
			//lineStart = 1;
			//string code = script.Code;

			//scriptStart = 0;
			//editor.Text = "";

			// Create the editable code from the script's code
			string code;
			if (script == null) {
				code = "";
				scriptStart = 0;
				lineStart = 0;
			}
			else {
				code = ScriptCodeGenerator.CreateRoslynScriptCode(script,
					out scriptStart, out lineStart);
			}
			return code;
		}

		/// <summary>Loads a new script into the Roslyn Pad Editor.</summary>
		private void ChangeScript() {
			suppressEvents = true;

			// Update the code
			string code = CreateScriptCode(out scriptStart, out lineStart);
			editor.Text = code;

			// Add a readonly section for members and parameters
			var sectionProvider = new TextSegmentReadOnlySectionProvider<TextSegment>(
				editor.TextArea.Document);
			if (script != null) {
				sectionProvider.Segments.Add(new TextSegment() {
					StartOffset = 0,
					Length = scriptStart,
				});
			}
			editor.TextArea.ReadOnlySectionProvider = sectionProvider;
			
			//Line line = null;
			foreach (UIElement margin in editor.TextArea.LeftMargins) {
				if (margin is LineNumberMargin) {
					LineNumberMargin lineNumbers = (LineNumberMargin) margin;
					lineNumbers.StartingLine = lineStart;
				}
			}
			// TODO: What does this do? --david
			//if (line != null)
				//editor.TextArea.LeftMargins.Remove(line);

			if (script != null)
				foldingStrategy.UpdateFoldings(foldingManager, editor.Document, scriptStart);

			editor.CaretOffset = scriptStart;
			editor.IsModified = false;
			suppressEvents = false;
		}
		
		/// <summary>Calculates the line, column, and character of the caret position.
		/// </summary>
		private int CalculateVisualColumn() {
			TextViewPosition position = editor.TextArea.Caret.Position;
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

		public Script Script {
			get { return script; }
			set {
				if (script != value) {
					script = value;
					if (documentID != null)
						ChangeScript();
				}
			}
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
		
		/// <summary>Gets actual script code of the current document.</summary>
		public string ScriptCode {
			get { return editor.Text.Substring(scriptStart); }
		}
		
		/// <summary>Gets/Sets the 'modified' flag.</summary>
		public bool IsModified {
			get { return editor.IsModified; }
			set { editor.IsModified = value; }
		}

		/// <summary>Gets if the most recent undone command can be redone.</summary>
		public bool CanRedo {
			get { return editor.CanRedo; }
		}
	}
}
