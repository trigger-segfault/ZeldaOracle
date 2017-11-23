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
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ZeldaEditor.Control;
using ZeldaEditor.Controls;
using ZeldaEditor.WinForms;
using static ICSharpCode.AvalonEdit.Highlighting.HighlightingManager;

namespace ZeldaEditor.Windows {
	/// <summary>
	/// Interaction logic for TextMessageEditorWindow.xaml
	/// </summary>
	public partial class TextMessageEditor : Window {

		TextMessageDisplay textDisplay;
		static IHighlightingDefinition highlightingDefinition;

		bool loaded = false;

		string savedTextMessage;
		bool saved;

		static TextMessageEditor() {
			var fullName = "ZeldaEditor.Themes.TextMessageHighlighting.xshd";
			using (var stream = typeof(TextMessageEditor).Assembly.GetManifestResourceStream(fullName))
				using (var reader = new XmlTextReader(stream))
					highlightingDefinition = HighlightingLoader.Load(reader, HighlightingManager.Instance);
		}

		public TextMessageEditor(string message, EditorControl editorControl) {
			InitializeComponent();

			message = UnescapeMessage(message);
			savedTextMessage = message;
			saved = false;

			// Create the text display.
			textDisplay                = new TextMessageDisplay(EscapeMessage(message));
			textDisplay.EditorControl  = editorControl;
			textDisplay.Name           = "textDisplay";
			textDisplay.Dock           = System.Windows.Forms.DockStyle.Fill;
			textDisplay.MessageFinished += OnTextMessageFinished;
			host.Child = textDisplay;

			editor.TextChanged += OnTextChanged;
			editor.Text = message;
			editor.SyntaxHighlighting = highlightingDefinition;
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

			loaded = true;
		}

		private void OnTextMessageFinished(object sender, EventArgs e) {
			Dispatcher.Invoke(() => {
				buttonNextLine.Content = "Restart";
			});
		}

		private void OnCaretPositionChanged(object sender, EventArgs e) {
			if (!loaded) return;
			UpdateTextDisplay();
			UpdateStatusBar();
		}
		
		public static string EscapeMessage(string text) {
			int caretPosition = 0;
			return EscapeMessage(text, ref caretPosition);
		}
		public static string EscapeMessage(string text, ref int caretPosition) {
			int originalCaretPosition = caretPosition;
			string newText = "";
			bool lastWasNewLine = true;
			bool inCode = false;
			string currentCode = "";
			caretPosition = -1;
			for (int i = 0; i < text.Length; i++) {
				if (i == originalCaretPosition)
					caretPosition = newText.Length;
				char c = text[i];
				if (inCode) {
					if (c == '>') {
						if (currentCode == "n" || currentCode == "p") {
							if (!lastWasNewLine) {
								lastWasNewLine = true;
								newText += "<" + currentCode + ">";
							}
							else if (currentCode == "p" && newText.Any()) {
								newText = newText.Substring(0, newText.Length - 3);
								if (caretPosition > newText.Length)
									caretPosition = newText.Length;
								newText += "<" + currentCode + ">";
							}
						}
						else {
							newText += "<" + currentCode + ">";
							lastWasNewLine = false;
						}
						inCode = false;
					}
					else {
						currentCode += c;
					}
				}
				else if (c == '\r') {
					// Skip this character
				}
				else if (c == '\n') {
					if (!lastWasNewLine) {
						lastWasNewLine = true;
						newText += "<n>";
					}
				}
				else if (c == '<') {
					inCode = true;
					currentCode = "";
				}
				else {
					lastWasNewLine = false;
					newText += c;
				}
			}
			if (caretPosition == -1)
				caretPosition = newText.Length;

			return newText;
			//return text.Replace("\r", "").Replace("\n", "<n>").Replace("<n><n>", "<n>").Replace("<p><n>", "<p>");
		}
		public static string UnescapeMessage(string text) {
			return text.Replace("\r", "").Replace("<n>", "\n").Replace("<p>", "<p>\n").Replace("\n", "\r\n");
		}

		private void OnTextChanged(object sender, EventArgs e) {
			if (!loaded) return;
			UpdateTextDisplay();
			UpdateStatusBar();
			CommandManager.InvalidateRequerySuggested();
		}

		private void OnFinished(object sender, RoutedEventArgs e) {
			savedTextMessage = EscapeMessage(editor.Text);
			editor.IsModified = false;
			saved = true;
			DialogResult = true;
			Close();
		}


		public static string Show(Window owner, string message, EditorControl editorControl) {
			TextMessageEditor editor = new TextMessageEditor(message, editorControl);
			editor.Owner = owner;
			var result = editor.ShowDialog();
			if ((result.HasValue && result.Value) || editor.saved) {
				return editor.savedTextMessage;
			}
			return null;
		}

		private void OnHostPreviewKeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Enter) {
				OnNextLine();
			}
		}

		private void UpdateStatusBar() {
			statusLine.Content = "Line " + editor.TextArea.Caret.Position.Line;
			statusColumn.Content = "Col " + editor.TextArea.Caret.Position.Column;
			statusChar.Content = "Char " + editor.CaretOffset;
		}

		private void UpdateTextDisplay() {
			buttonNextLine.Content = "Next Line";
			int caretPosition = editor.CaretOffset;
			string text = EscapeMessage(editor.Text, ref caretPosition);
			textDisplay.UpdateMessage(text, caretPosition);
		}

		private void OnNextLine(object sender = null, RoutedEventArgs e = null) {
			buttonNextLine.Content = "Next Line";
			textDisplay.EnterPressed();
		}

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

		private void InsertFormatCode(string code) {
			if (!editor.IsFocused) {
				editor.Focus();
				//editor.CaretOffset = editor.Text.Length;
			}
			editor.Document.Insert(editor.CaretOffset, code);
		}


		private void CanSaveTextMessage(object sender, CanExecuteRoutedEventArgs e) {
			if (!loaded) return;
			e.CanExecute = editor.IsModified;
		}

		private void SaveTextMessage(object sender, ExecutedRoutedEventArgs e) {
			savedTextMessage = EscapeMessage(editor.Text);
			editor.IsModified = false;
			saved = true;
		}
	}
}
