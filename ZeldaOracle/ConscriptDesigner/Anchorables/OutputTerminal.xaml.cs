using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ConscriptDesigner.Anchorables {
	/// <summary>
	/// Interaction logic for OutputTerminal.xaml
	/// </summary>
	public partial class OutputTerminal : UserControl {

		private const int MaxLines = 300;

		private RequestCloseAnchorable anchorable;

		public OutputTerminal(RequestCloseAnchorable anchorable) {
			InitializeComponent();
			this.anchorable = anchorable;
			anchorable.Title = "Output Console";
			AppendLine("");

			Console.SetOut(new OutputTextWriter(this));
		}

		public RequestCloseAnchorable Anchorable {
			get { return anchorable; }
		}

		public void Close() {
			var standardOutput = new StreamWriter(Console.OpenStandardOutput());
			standardOutput.AutoFlush = true;
			Console.SetOut(standardOutput);
		}

		public void Clear() {
			Dispatcher.Invoke(() => {
				stackPanel.Children.Clear();
				AppendLine("");
			});
		}

		public void WriteLine(string line) {
			Write(line + "\n");
		}
		public void Write(string text) {
			Dispatcher.Invoke(() => {
				string[] lines = text.Replace("\r", "").Split('\n');
				AppendText(lines[0]);
				for (int i = 1; i < lines.Length; i++) {
					AppendLine(lines[i]);
				}
			});
		}

		private void AppendText(string text) {
			TextBlock textBlock = (TextBlock) stackPanel.Children[stackPanel.Children.Count - 1];
			textBlock.Text += text;
		}

		private void AppendLine(string text) {
			TextBlock textBlock = new TextBlock();
			textBlock.Text = text;
			stackPanel.Children.Add(textBlock);
			while (stackPanel.Children.Count > MaxLines) {
				stackPanel.Children.RemoveAt(0);
			}
			scrollViewer.ScrollToBottom();
		}
	}

	public class OutputTextWriter : TextWriter {
		private OutputTerminal terminal;

		public OutputTextWriter(OutputTerminal terminal) {
			this.terminal = terminal;
		}
		public override Encoding Encoding {
			get { return Encoding.UTF8; }
		}
		public override void Write(string value) {
			terminal.Write(value);
		}
		public override void Write(char value) {
			terminal.Write(new string(value, 1));
		}
	}
}
