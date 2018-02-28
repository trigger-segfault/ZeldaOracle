using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using ConscriptDesigner.Control;
using ConscriptDesigner.Util;

namespace ConscriptDesigner.Anchorables {
	public class OutputConsole : RequestCloseAnchorable {

		//-----------------------------------------------------------------------------
		// Classes
		//-----------------------------------------------------------------------------

		/// <summary>A text writer to hijack the console output.</summary>
		private class OutputTextWriter : TextWriter {
			/// <summary>The output console to write to.</summary>
			private OutputConsole console;
			
			/// <summary>Constructs the console text writer.</summary>
			public OutputTextWriter(OutputConsole console) {
				this.console = console;
			}

			/// <summary>Gets the encoding.</summary>
			public override Encoding Encoding {
				get { return Encoding.UTF8; }
			}

			/// <summary>Write text to the console.</summary>
			public override void Write(string value) {
				console.Write(value);
			}

			/// <summary>Write a character to the console.</summary>
			public override void Write(char value) {
				console.Write(value);
			}
		}


		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>The maximum number of lines allowed in the console.</summary>
		private const int MaxLines = 400;


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The scroll viewer for displaying the console log.</summary>
		private ScrollViewer scrollViewer;
		/// <summary>The stack panel for each line of text.</summary>
		private VirtualizingStackPanel stackPanel;
		/// <summary>The timer to flush the buffer.</summary>
		private StoppableTimer updateTimer;
		/// <summary>The buffer for the text to be added to the output console.</summary>
		private string buffer;
		/// <summary>True if the console should be cleared during flush.</summary>
		private bool requestClear;
		
		/// <summary>Used to prevent duplication or loss of console text.</summary>
		private static readonly object myLock = new object();


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the output console.</summary>
		public OutputConsole() {
			Border border = CreateBorder();
			this.scrollViewer = new ScrollViewer();
			this.scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
			this.scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
			this.scrollViewer.Background = new SolidColorBrush(Color.FromRgb(230, 231, 232));
			this.scrollViewer.FontFamily = new FontFamily("Lucida Console");
			this.scrollViewer.FontSize = 11;
			this.scrollViewer.CanContentScroll = true;
			TextOptions.SetTextFormattingMode(this.scrollViewer, TextFormattingMode.Display);
			border.Child = this.scrollViewer;

			this.stackPanel = new VirtualizingStackPanel();
			this.scrollViewer.Content = this.stackPanel;


			buffer = "";
			requestClear = false;
			updateTimer = StoppableTimer.StartNew(
				TimeSpan.FromMilliseconds(20),
				DispatcherPriority.Render,
				UpdateTimer);
			/*updateTimer = new DispatcherTimer(
				TimeSpan.FromMilliseconds(20),
				DispatcherPriority.Render,
				delegate { UpdateTimer(); },
				Application.Current.Dispatcher);*/

			Closed += OnAnchorableClosed;
			AppendLine("");
			Console.SetOut(new OutputTextWriter(this));
			
			Title = "Output Console";
			Content = border;

		}


		//-----------------------------------------------------------------------------
		// XML Serialization
		//-----------------------------------------------------------------------------

		public override void ReadXml(XmlReader reader) {
			base.ReadXml(reader);
			DesignerControl.MainWindow.OutputConsole = this;
		}


		//-----------------------------------------------------------------------------
		// Overrides
		//-----------------------------------------------------------------------------
		
		/// <summary>Focuses on the anchorable's content.</summary>
		public override void Focus() {
			scrollViewer.Focus();
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		/// <summary>Flushes the buffer into the console log.</summary>
		private void UpdateTimer() {
			FlushBuffer();
		}

		/// <summary>Called when closing to re-establish the proper console output channel.</summary>
		private void OnAnchorableClosed(object sender, EventArgs e) {
			// Return console output to its rightful owner
			var standardOutput = new StreamWriter(Console.OpenStandardOutput());
			standardOutput.AutoFlush = true;
			Console.SetOut(standardOutput);
		}


		//-----------------------------------------------------------------------------
		// Console Output
		//-----------------------------------------------------------------------------

		/// <summary>Clears the console.</summary>
		public void Clear() {
			lock (myLock) {
				requestClear = true;
				buffer = "";
			}
		}

		/// <summary>Creates a new line if the current line isn't empty.</summary>
		public void NewLine() {
			FlushBuffer();
			TextBlock textBlock = (TextBlock) stackPanel.Children[stackPanel.Children.Count - 1];
			if (textBlock.Text != "")
				AppendLine("");
		}

		/// <summary>Write a line to the console.</summary>
		public void WriteLine(string line) {
			Write(line + "\n");
		}

		/// <summary>Write text to the console.</summary>
		public void Write(string text) {
			lock (myLock) {
				buffer += text;
			}
		}

		/// <summary>Write a character to the console.</summary>
		public void Write(char c) {
			lock (myLock) {
				buffer += new string(c, 1);
			}
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Flushes the buffer into the console log.</summary>
		private void FlushBuffer() {
			lock (myLock) {
				// Don't flush again until the previous flush is finished
				if (!buffer.Any() && !requestClear)
					return;
				if (requestClear) {
					stackPanel.Children.Clear();
					AppendLine("");
					requestClear = false;
				}
				if (buffer.Any()) {
					string[] lines = buffer.Replace("\r", "").Split('\n');
					AppendText(lines[0]);
					for (int i = 1; i < lines.Length; i++) {
						AppendLine(lines[i]);
					}
					scrollViewer.ScrollToBottom();
					buffer = "";
				}
			}
		}

		/// <summary>Appends text to the last line in the stack panel.</summary>
		private void AppendText(string text) {
			TextBlock textBlock = (TextBlock) stackPanel.Children[stackPanel.Children.Count - 1];
			textBlock.Text += text;
		}

		/// <summary>Appends a line to the stack panel.</summary>
		private void AppendLine(string text) {
			TextBlock textBlock = new TextBlock();
			textBlock.Padding = new Thickness(10, 1, 0, 0);
			textBlock.Text = text;
			stackPanel.Children.Add(textBlock);
			while (stackPanel.Children.Count > MaxLines) {
				stackPanel.Children.RemoveAt(0);
			}
		}
	}
}
