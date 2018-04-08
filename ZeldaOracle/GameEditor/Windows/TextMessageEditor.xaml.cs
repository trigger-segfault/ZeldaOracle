using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ZeldaEditor.Control;
using ZeldaEditor.Controls;
using ZeldaEditor.Themes;
using ZeldaEditor.WinForms;
using ZeldaOracle.Common.Translation;
using ZeldaOracle.Common.Util;

namespace ZeldaEditor.Windows {
	/// <summary>
	/// Interaction logic for TextMessageEditorWindow.xaml
	/// </summary>
	public partial class TextMessageEditor : Window {
		
		/// <summary>The XNA display for the text message reader.</summary>
		private TextMessageDisplay textDisplay;
		/// <summary>True if events should be supressed.</summary>
		private bool supressEvents;
		/// <summary>True if the text message has been saved at least once.</summary>
		private bool saved;
		/// <summary>The state of the text message when it was last saved.</summary>
		private string savedTextMessage;
		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the text message editor.</summary>
		public TextMessageEditor(string message, EditorControl editorControl) {
			supressEvents = true;
			InitializeComponent();

			message = FormatCodes.UnescapeMessage(message);
			savedTextMessage = message;
			saved = false;

			// Create the text display
			string displayMessage = FormatCodes.EscapeMessage(message);
			textDisplay                = new TextMessageDisplay(this, displayMessage);
			textDisplay.EditorControl  = editorControl;
			textDisplay.Name           = "textDisplay";
			textDisplay.Dock           = System.Windows.Forms.DockStyle.Fill;
			textDisplay.MessageFinished += OnTextMessageFinished;
			host.Child = textDisplay;

			editor.TextChanged += OnTextChanged;
			editor.Text = message;
			editor.SyntaxHighlighting = Highlighting.TextMessage;
			editor.TextArea.Margin = new Thickness(4, 4, 0, 4);
			editor.TextArea.TextView.Options.AllowScrollBelowDocument = true;
			editor.TextArea.SelectionCornerRadius = 0;
			editor.TextArea.SelectionBorder = null;
			editor.FontFamily = new FontFamily("Lucida Console");
			editor.FontSize = 12.667;
			//TextOptions.SetTextFormattingMode(editor, TextFormattingMode.Display);
			editor.WordWrap = true;
			editor.CaretOffset = message.Length;
			editor.TextArea.Caret.PositionChanged += OnCaretPositionChanged;

			UpdateStatusBar();

			supressEvents = false;
		}


		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------
		
		// General --------------------------------------------------------------------------

		private void OnCaretPositionChanged(object sender, EventArgs e) {
			if (supressEvents) return;
			UpdateTextDisplay();
			UpdateStatusBar();
		}

		private void OnTextChanged(object sender, EventArgs e) {
			if (supressEvents) return;
			UpdateTextDisplay();
			UpdateStatusBar();
			CommandManager.InvalidateRequerySuggested();
		}

		private void OnFinished(object sender, RoutedEventArgs e) {
			savedTextMessage = FormatCodes.EscapeMessage(editor.Text);
			editor.IsModified = false;
			saved = true;
			DialogResult = true;
			Close();
		}

		// Format Codes ---------------------------------------------------------------

		private void OnColorCodeSelected(object sender, FormatCodeEventArgs e) {
			InsertFormatCode(e.FormatCode);
			int caretOffset = editor.CaretOffset;
			InsertFormatCode(e.FormatCode);
			editor.CaretOffset = caretOffset;
		}

		private void OnStringCodeSelected(object sender, FormatCodeEventArgs e) {
			InsertFormatCode(e.FormatCode);
		}

		private void OnParagraphSelected(object sender, RoutedEventArgs e) {
			InsertFormatCode("<p>\n");
		}

		// Text Display ---------------------------------------------------------------

		private void OnTextMessageFinished(object sender, EventArgs e) {
			Dispatcher.Invoke(() => {
				buttonNextLine.Content = "Restart";
			});
		}

		private void OnHostPreviewKeyDown(object sender, KeyEventArgs e) {
			// Treat enter while the display is focused like pressing a key in game.
			if (e.Key == Key.Enter) {
				OnNextLine();
			}
		}

		private void OnNextLine(object sender = null, RoutedEventArgs e = null) {
			buttonNextLine.Content = "Next Line";
			textDisplay.EnterPressed();
		}


		//-----------------------------------------------------------------------------
		// Commands
		//-----------------------------------------------------------------------------
		
		private void SaveTextMessage(object sender, ExecutedRoutedEventArgs e) {
			savedTextMessage = FormatCodes.EscapeMessage(editor.Text);
			editor.IsModified = false;
			saved = true;
		}

		private void OnRedoCommand(object sender, ExecutedRoutedEventArgs e) {
			editor.Redo();
		}

		private void CanSaveTextMessage(object sender, CanExecuteRoutedEventArgs e) {
			if (supressEvents) return;
			e.CanExecute = editor.IsModified;
		}

		private void CanExecuteRedo(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = editor.CanRedo;
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Updates the status bar's Line/Col/Char displays.</summary>
		private void UpdateStatusBar() {
			var position = CaretPosition;
			statusLine.Content = "Line " + position.Line;
			statusColumn.Content = "Col " + position.VisualColumn;
			statusChar.Content = "Char " + position.Column;
		}

		/// <summary>Updates the text reader display and preserves the caret position.</summary>
		private void UpdateTextDisplay() {
			buttonNextLine.Content = "Next Line";
			int caretPosition = editor.CaretOffset;
			string text = FormatCodes.EscapeMessage(editor.Text, ref caretPosition);
			textDisplay.UpdateMessage(text, caretPosition);
		}

		/// <summary>Inserts the specified format code text.</summary>
		private void InsertFormatCode(string code) {
			if (!editor.IsFocused)
				editor.Focus();
			editor.Document.Insert(editor.CaretOffset, code);
		}

		/// <summary>Calculates the visual caret position for the Col status bar item.</summary>
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
		// Showing
		//-----------------------------------------------------------------------------

		/// <summary>Shows the text message editor with the specified message.</summary>
		public static string Show(Window owner, string message, EditorControl editorControl) {
			TextMessageEditor editor = new TextMessageEditor(message, editorControl);
			editor.Owner = owner;
			var result = editor.ShowDialog();
			if ((result.HasValue && result.Value) || editor.saved) {
				return editor.savedTextMessage;
			}
			return null;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the correct caret position of the text message editor.</summary>
		public TextViewPosition CaretPosition {
			get {
				return new TextViewPosition(
					editor.TextArea.Caret.Location,
					CalculateVisualColumn());
			}
		}
	}
}
