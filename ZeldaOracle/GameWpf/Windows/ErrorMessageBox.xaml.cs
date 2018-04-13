using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Media;
using System.Timers;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Util;
using ZeldaWpf.Controls;
using ZeldaWpf.Util;

namespace ZeldaWpf.Windows {
	/// <summary>Shows an error that occured in the program.</summary>
	public partial class ErrorMessageBox : TimersWindow {

		//-----------------------------------------------------------------------------
		// Settings
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the name of the program to show in the window.</summary>
		public static string ProgramName { get; set; } = "Zelda Oracle";

		/// <summary>Gets or sets the error icon to show for this window and the
		/// taskbar.</summary>
		public static ImageSource ErrorIcon { get; set; }


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The exception object that was raised.</summary>
		private object exception;
		/// <summary>True if viewing the full exception.</summary>
		private bool viewingFull;
		/// <summary>The timer for changing the copy button back to its original text.</summary>
		private ScheduledEvent copyTimer;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the error message box with an exception object.</summary>
		private ErrorMessageBox(object ex, bool alwaysContinue) {
			InitializeComponent();

			viewingFull = false;
			textBlockMessage.Text = "Exception:\n";
			exception = ex;
			if (IsException)
				textBlockMessage.Text += Exception.MessageWithInner();
			else
				textBlockMessage.Text += exception.ToString();
			if (!IsException) {
				buttonException.IsEnabled = false;
			}
			if (alwaysContinue) {
				buttonExit.Visibility = Visibility.Collapsed;
				buttonContinue.IsDefault = true;
			}

			// Apply static settings
			textBlockName.Text = "Unhandled Exception in " + ProgramName + "!";
			Icon = ErrorIcon;
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		private void OnWindowLoaded(object sender, RoutedEventArgs e) {
			// Only play a noisy sound if this window was not prompted by the user
			if (buttonExit.Visibility == Visibility.Visible) {
				SystemSounds.Hand.Play();
			}
		}

		private void OnExit(object sender, RoutedEventArgs e) {
			DialogResult = true;
			Close();
		}

		private void OnCopyToClipboard(object sender, RoutedEventArgs e) {
			Clipboard.SetText(exception.ToString());
			buttonCopy.Content = "Exception Copied!";
			copyTimer?.Cancel();
			copyTimer = ScheduledEvents.Start(1, TimerPriority.Low, () => {
				buttonCopy.Content = "Copy to Clipboard";
			});
		}

		private void OnSeeFullException(object sender, RoutedEventArgs e) {
			viewingFull = !viewingFull;
			if (!viewingFull) {
				buttonException.Content = "See Full Exception";
				textBlockMessage.Text = "Exception:\n" + Exception.MessageWithInner();
				clientArea.Height = 230;
				scrollViewer.ScrollToTop();
			}
			else {
				buttonException.Content = "Hide Full Exception";
				// Size may not be changed yet so just
				// incase we also have OnMessageSizeChanged.
				textBlockMessage.Text = "Exception:\n" + Exception.ToStringWithInner();
				UpdateFullMessageSize();
			}
		}

		private void OnMessageSizeChanged(object sender, SizeChangedEventArgs e) {
			if (viewingFull)
				UpdateFullMessageSize();
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
		// Internal Methods
		//-----------------------------------------------------------------------------

		private void UpdateFullMessageSize() {
			clientArea.Height = GMath.Clamp(
				textBlockMessage.ActualHeight + 102,
				230, 480);
			scrollViewer.ScrollToTop();
		}


		//-----------------------------------------------------------------------------
		// Showing
		//-----------------------------------------------------------------------------

		/// <summary>Shows an error message box with an exception object.</summary>
		public static bool Show(object exception, bool alwaysContinue = false) {
			ErrorMessageBox messageBox = new ErrorMessageBox(exception, alwaysContinue);
			var result = messageBox.ShowDialog();
			return result.HasValue && result.Value;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets if the exception object is an exception.</summary>
		public bool IsException {
			get { return exception is Exception; }
		}

		/// <summary>Gets the exception object as an exception.</summary>
		public Exception Exception {
			get { return exception as Exception; }
		}
	}
}
