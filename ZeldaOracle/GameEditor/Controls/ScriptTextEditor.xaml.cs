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
using ZeldaEditor.Themes;
using Trigger = ZeldaOracle.Common.Scripting.Trigger;

namespace ZeldaEditor.Controls {
	/// <summary>
	/// Interaction logic for ScriptTextEditor.xaml
	/// </summary>
	public partial class ScriptTextEditor : UserControl {

		private static ScriptRoslynHost host;
		private Trigger trigger;
		private Script script;
		private ScriptRoslynInfo scriptInfo;
		private FoldingManager foldingManager;
		private ScriptFoldingStrategy foldingStrategy;
		private DocumentId documentID;
		private StoppableTimer timer;
		private bool needsFoldingUpdate;
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
			needsFoldingUpdate = false;

			// Editor options
			editor.TextArea.IndentationStrategy = new CSharpIndentationStrategy();
			editor.Options.ConvertTabsToSpaces = false;
			
			// Setup events
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
			editor.TextArea.TextEntering += OnTextEntering;
			editor.TextChanged += OnTextChanged;
			editor.TextArea.Caret.PositionChanged += OnCaretPositionChanged;
			DataObject.AddPastingHandler(editor, OnPasting);
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

		/// <summary>Call this to update the name of the script method.</summary>
		public void UpdateMethodName(string newName) {
			int oldLength = scriptInfo.MethodNameLength;
			scriptInfo.ResizeMethodName(newName.Length);
			editor.Document.Replace(scriptInfo.MethodNameStart, oldLength, newName);
			needsFoldingUpdate = true;
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
				string workingDirectory = Path.GetDirectoryName(
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
					Highlighting.Script.ToColorizer());
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

		private void OnPasting(object sender, DataObjectPastingEventArgs e) {
			// Prevent pasting at the first index because we want it to be readonly.
			// Readonly segments don't allow preventing insertion at the first index.
			if (documentID != null &&
				editor.CaretOffset == 0 || editor.SelectionStart == 0)
				e.CancelCommand();
		}

		private void OnTextEntering(object sender, TextCompositionEventArgs e) {
			// Prevent typing at the first index because we want it to be readonly.
			// Readonly segments don't allow preventing insertion at the first index.
			if (documentID != null &&
				editor.CaretOffset == 0 || editor.SelectionStart == 0)
				e.Handled = true;
		}

		/// <summary>Forwards the TextChanged event to our ScriptCodeChanged event.
		/// </summary>
		private void OnTextChanged(object sender, EventArgs e) {
			if (!suppressEvents) {
				needsFoldingUpdate = true;
				OnScriptCodeChanged(e);
				CommandManager.InvalidateRequerySuggested();
			}
		}

		private void OnTimerUpdate() {
			if (documentID != null && script != null && needsFoldingUpdate) {
				needsFoldingUpdate = false;
				foldingStrategy.UpdateFoldings(
					foldingManager, editor.Document, scriptInfo);
			}
		}

		private void OnCaretPositionChanged(object sender, EventArgs e) {
			int length = editor.Text.Length;
			if (editor.CaretOffset == 0 && scriptInfo.MethodStart != 0)
				editor.CaretOffset = scriptInfo.MethodStart;
			else if (editor.CaretOffset == length && scriptInfo.EndLength != 0)
				editor.CaretOffset -= scriptInfo.EndLength;
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Initialize the Roslyn Script Host.</summary>
		public static void Initialize() {
			// Don't waste anytime waiting on this to finish
			Task.Run(() => new ScriptRoslynHost())
				.ContinueWith((result) => { host = result.Result; });
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Create the code that's shown in the editor. This code will have
		/// to include members and parameters for intellisense to work properly.
		/// </summary>
		private string CreateScriptCode(out ScriptRoslynInfo scriptInfo) {
			// Create the editable code from the script's code
			string code;
			if (script == null) {
				code = "";
				scriptInfo = new ScriptRoslynInfo();
			}
			else {
				string name = "script";
				if (trigger != null)
					name = trigger.Name;
				else if (script != null)
					name = script.ID;
				code = ScriptCodeGenerator.CreateRoslynScriptCode(script, name,
					out scriptInfo);
			}
			return code;
		}

		/// <summary>Loads a new script into the Roslyn Pad Editor.</summary>
		private void ChangeScript() {
			suppressEvents = true;

			// Reset so foldings stay closed
			foldingManager.Reset();

			// Update the code
			string code = CreateScriptCode(out scriptInfo);
			editor.Text = code;

			// Add a readonly section for members and parameters
			var sectionProvider = new TextSegmentReadOnlySectionProvider<TextSegment>(
				editor.TextArea.Document);
			if (script != null) {
				if (scriptInfo.ScriptStart != 0) {
					sectionProvider.Segments.Add(new TextSegment() {
						StartOffset = 0,
						Length = scriptInfo.ScriptStart,
					});
				}
				if (scriptInfo.EndLength != 0) {
					sectionProvider.Segments.Add(new TextSegment() {
						StartOffset = editor.Text.Length - scriptInfo.EndLength,
						Length = scriptInfo.EndLength,
					});
				}
			}
			editor.TextArea.ReadOnlySectionProvider = sectionProvider;
			
			foreach (UIElement margin in editor.TextArea.LeftMargins) {
				if (margin is LineNumberMargin) {
					LineNumberMargin lineNumbers = (LineNumberMargin) margin;
					lineNumbers.StartingLine = scriptInfo.ScriptStartLine;
				}
			}

			if (script != null)
				foldingStrategy.UpdateFoldings(foldingManager, editor.Document, scriptInfo);

			editor.CaretOffset = scriptInfo.ScriptStart;
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

		public Trigger Trigger {
			get { return trigger; }
			set {
				if (trigger != value) {
					trigger = value;
					script = trigger?.Script;
					if (documentID != null)
						ChangeScript();
				}
			}
		}

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
				if (editor.TextArea.Caret.Position.Line <= scriptInfo.ScriptStartLine)
					return new TextViewPosition(-1, -1, -1);
				return new TextViewPosition(
					editor.TextArea.Caret.Line - scriptInfo.ScriptStartLine,
					editor.TextArea.Caret.Column,
					CalculateVisualColumn());
			}
		}
		
		/// <summary>Gets actual script code of the current document.</summary>
		public string ScriptCode {
			get {
				return editor.Text.Substring(scriptInfo.ScriptStart,
					editor.Text.Length - scriptInfo.ScriptStart -
					scriptInfo.EndLength);
			}
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
