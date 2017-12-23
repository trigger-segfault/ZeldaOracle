using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace ConscriptDesigner.Windows {
	/// <summary>Shows an error that occured in the program.</summary>
	public partial class ErrorMessageBox : Window {

		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The exception that was raised.</summary>
		private Exception exception = null;
		/// <summary>The non-exception object that was raised.</summary>
		private object exceptionObject = null;
		/// <summary>True if viewing the full exception.</summary>
		private bool viewingFull = false;
		/// <summary>The timer for changing the copy button back to its original text.</summary>
		private DispatcherTimer copyTimer;
		/// <summary>The text of the copy to clipboard button.</summary>
		private readonly string copyText;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		/// <summary>Constructs the error message box with an exception object.</summary>
		public ErrorMessageBox(object exceptionObject, bool alwaysContinue) {
			InitializeComponent();

			this.textBlockMessage.Text = "Exception:\n";
			this.exception = (exceptionObject is Exception ? exceptionObject as Exception : null);
			this.exceptionObject = (exceptionObject is Exception ? null : exceptionObject);
			if (this.exception != null)
				this.textBlockMessage.Text += this.exception.Message;
			else if (this.exceptionObject != null)
				this.textBlockMessage.Text += this.exceptionObject.ToString();
			this.copyText = buttonCopy.Content as string;
			if (!(exceptionObject is Exception)) {
				this.buttonException.IsEnabled = false;
			}
			if (alwaysContinue) {
				this.buttonExit.Visibility = Visibility.Collapsed;
				this.buttonContinue.IsDefault = true;
			}

			this.copyTimer = new DispatcherTimer(
				TimeSpan.FromSeconds(1),
				DispatcherPriority.ApplicationIdle,
				OnCopyTimer,
				Dispatcher);
			this.copyTimer.Stop();
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		private void OnWindowClosing(object sender, CancelEventArgs e) {
			copyTimer.Stop();
		}

		private void OnExit(object sender, RoutedEventArgs e) {
			DialogResult = true;
			Close();
		}

		private void OnCopyTimer(object sender, EventArgs e) {
			buttonCopy.Content = copyText;
			copyTimer.Stop();
		}

		private void OnCopyToClipboard(object sender, RoutedEventArgs e) {
			Clipboard.SetText(exception != null ? exception.ToString() : exceptionObject.ToString());
			buttonCopy.Content = "Exception Copied!";
			copyTimer.Stop();
			copyTimer.Start();
		}

		private void OnSeeFullException(object sender, RoutedEventArgs e) {
			viewingFull = !viewingFull;
			if (!viewingFull) {
				buttonException.Content = "See Full Exception";
				textBlockMessage.Text = "Exception:\n" + exception.Message;
				clientArea.Height = 230;
				scrollViewer.ScrollToTop();
			}
			else {
				buttonException.Content = "Hide Full Exception";
				// Size may not be changed yet so just incase we also have OnMessageSizeChanged
				textBlockMessage.Text = "Exception:\n" + exception.ToString();
				clientArea.Height = Math.Min(480, Math.Max(230, textBlockMessage.ActualHeight + 102));
				scrollViewer.ScrollToTop();
			}
		}

		private void OnMessageSizeChanged(object sender, SizeChangedEventArgs e) {
			if (viewingFull) {
				clientArea.Height = Math.Min(480, Math.Max(230, textBlockMessage.ActualHeight + 102));
				scrollViewer.ScrollToTop();
			}
		}

		private void OnPreviewKeyDown(object sender, KeyEventArgs e) {
			var focused = FocusManager.GetFocusedElement(this);
			switch (e.Key) {
			case Key.Right:
				if (focused == buttonContinue && buttonExit.Visibility == Visibility.Visible)
					buttonExit.Focus();
				else if (focused == buttonCopy)
					buttonContinue.Focus();
				else if (focused == buttonException)
					buttonCopy.Focus();
				e.Handled = true;
				break;
			case Key.Left:
				if (focused == null) {
					if (buttonExit.Visibility == Visibility.Visible)
						buttonContinue.Focus();
					else
						buttonCopy.Focus();
				}
				else if (focused == buttonExit)
					buttonContinue.Focus();
				else if (focused == buttonContinue)
					buttonCopy.Focus();
				else if (focused == buttonCopy && buttonException.IsEnabled)
					buttonException.Focus();
				e.Handled = true;
				break;
			}
		}

		private void OnRequestNavigate(object sender, RequestNavigateEventArgs e) {
			Process.Start((sender as Hyperlink).NavigateUri.ToString());
		}


		//-----------------------------------------------------------------------------
		// Showing
		//-----------------------------------------------------------------------------
		
		/// <summary>Shows an error message box with an exception object.</summary>
		public static bool Show(object exceptionObject, bool alwaysContinue = false) {
			ErrorMessageBox messageBox = new ErrorMessageBox(exceptionObject, alwaysContinue);
			var result = messageBox.ShowDialog();
			return result.HasValue && result.Value;
		}
	}
}
