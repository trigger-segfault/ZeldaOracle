using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using ZeldaEditor.Util;

namespace ZeldaEditor.WinForms {
	public class WpfMouseWheelMessageFilter : IMessageFilter {
		private FrameworkElement element;

		public WpfMouseWheelMessageFilter(FrameworkElement element) {
			this.element = element;
		}

		public bool PreFilterMessage(ref Message m) {
			if (m.Msg == NativeMethods.WM_MOUSEWHEEL) {
				var focusElement = FocusManager.GetFocusedElement(element);
				if (element.IsMouseOver && !element.IsFocused &&
					focusElement != element && focusElement == null)
				{
					element.Focus();
					HwndSource hwndSource = (HwndSource)HwndSource.FromVisual(element);
					NativeMethods.SendMessage(hwndSource.Handle, m.Msg, m.WParam, m.LParam);
					return true;
				}
			}

			return false;
		}

		public void AddFilter() {
			System.Windows.Forms.Application.AddMessageFilter(this);
		}
		public void RemoveFilter() {
			System.Windows.Forms.Application.RemoveMessageFilter(this);
		}
	}

	public class WinFormsMouseWheelMessageFilter : IMessageFilter {
		private GraphicsDeviceControl control;

		public WinFormsMouseWheelMessageFilter(GraphicsDeviceControl control) {
			this.control = control;
		}

		public bool PreFilterMessage(ref Message m) {
			if (m.Msg == NativeMethods.WM_MOUSEWHEEL) {
				if (control.IsMouseOver && !control.Focused) {
					control.Focus();
					NativeMethods.SendMessage(control.Handle, m.Msg, m.WParam, m.LParam);
					return true;
				}
			}

			return false;
		}

		public void AddFilter() {
			System.Windows.Forms.Application.AddMessageFilter(this);
		}
		public void RemoveFilter() {
			System.Windows.Forms.Application.RemoveMessageFilter(this);
		}
	}
}
