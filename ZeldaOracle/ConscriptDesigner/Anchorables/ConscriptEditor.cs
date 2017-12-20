using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using ConscriptDesigner.Content;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Indentation;

namespace ConscriptDesigner.Anchorables {
	public class ConscriptEditor : ContentFileDocument {

		//-----------------------------------------------------------------------------
		// Static Members
		//-----------------------------------------------------------------------------

		/// <summary>The highlighting definition for all conscripts. May be split up later.</summary>
		private static IHighlightingDefinition highlightingDefinition;


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The text editor for the conscript.</summary>
		private TextEditor editor;

		/// <summary>The timer to update if the file is modified.
		/// Needed because it doesn't always change right after an action.</summary>
		private DispatcherTimer modifiedTimer;
		/// <summary>True if the title is currently displayed as modified.</summary>
		private bool isTitleModified;

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the conscript editor and loads the highlighting.</summary>
		static ConscriptEditor() {
			string fullName = "ConscriptDesigner.Themes.ConscriptHighlighting.xshd";
			using (var stream = typeof(ConscriptEditor).Assembly.GetManifestResourceStream(fullName))
			using (var reader = new XmlTextReader(stream))
				highlightingDefinition = HighlightingLoader.Load(reader, HighlightingManager.Instance);
		}
		
		/// <summary>Constructs the conscript editor.</summary>
		public ConscriptEditor(ContentScript file) :
			base(file)
		{
			Border border = CreateBorder();
			this.editor = new TextEditor();
			this.editor.TextArea.TextView.LineSpacing = 1.24;
			this.editor.Load(file.FilePath);
			this.editor.TextChanged += OnTextChanged;
			this.isTitleModified = false;

			this.editor.TextArea.Margin = new Thickness(4, 4, 0, 4);
			this.editor.TextArea.TextView.Options.AllowScrollBelowDocument = true;
			this.editor.TextArea.SelectionCornerRadius = 0;
			this.editor.TextArea.SelectionBorder = null;
			this.editor.FontFamily = new FontFamily("Consolas");
			this.editor.FontSize = 12;
			this.editor.TextChanged += OnTextChanged;
			this.editor.TextArea.IndentationStrategy = new DefaultIndentationStrategy();
			this.editor.ShowLineNumbers = true;
			this.editor.SyntaxHighlighting = highlightingDefinition;
			TextOptions.SetTextFormattingMode(this.editor, TextFormattingMode.Display);
			border.Child = this.editor;

			this.modifiedTimer = new DispatcherTimer(
				TimeSpan.FromSeconds(0.05),
				DispatcherPriority.ApplicationIdle,
				delegate { CheckModified(); }, Dispatcher);
			this.modifiedTimer.Stop();

			Title = file.Name;
			Content = border;
		}

		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		/// <summary>Called every so often to check if the file is modified.</summary>
		private void CheckModified() {
			modifiedTimer.Stop();
			if ((editor.IsModified || File.IsModifiedOverride) != isTitleModified)
				UpdateTitle();
			CommandManager.InvalidateRequerySuggested();
		}

		/// <summary>Called when the text is changed in order to update the title.</summary>
		private void OnTextChanged(object sender, EventArgs e) {
			modifiedTimer.Start();
		}


		//-----------------------------------------------------------------------------
		// Title
		//-----------------------------------------------------------------------------

		/// <summary>Updates the title and appends an asterisk if it's modified.</summary>
		public void UpdateTitle() {
			isTitleModified = (editor.IsModified || File.IsModifiedOverride);
			Title = File.Name + (isTitleModified ? "*" : "");
		}


		//-----------------------------------------------------------------------------
		// Actions
		//-----------------------------------------------------------------------------

		/// <summary>Saves the current text in the editor to the file.</summary>
		public void Save() {
			editor.Save(File.FilePath);
			UpdateTitle();
			CommandManager.InvalidateRequerySuggested();
		}

		/// <summary>Reloads the text in the editor from the file.</summary>
		public void Reload() {
			editor.Load(File.FilePath);
			UpdateTitle();
		}

		/// <summary>Undoes the last action in the editor.</summary>
		public void Undo() {
			editor.Undo();
			modifiedTimer.Start();
		}

		/// <summary>Redoes the last action in the editor.</summary>
		public void Redo() {
			editor.Redo();
			modifiedTimer.Start();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		/// <summary>Returns true if the editor considers the text to be modified.</summary>
		public bool IsModified {
			get { return editor.IsModified; }
		}

		/// <summary>Returns true if the editor can undo.</summary>
		public bool CanUndo {
			get { return editor.CanUndo; }
		}

		/// <summary>Returns true if the editor can redo.</summary>
		public bool CanRedo {
			get { return editor.CanRedo; }
		}
	}
}
