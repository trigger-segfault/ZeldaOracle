using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using Xceed.Wpf.Toolkit.Primitives;
using ZeldaEditor.Util;

namespace ZeldaEditor.WinForms {
	/// <summary>Used to steal focus when scrolling or clicking over the area of a WPF control.</summary>
	public class WpfFocusMessageFilter : IMessageFilter {
		/// <summary>The WPF element to focus on.</summary>
		private FrameworkElement element;
		/// <summary>True if this filter ignores mouse buttons.</summary>
		private bool ignoreMouseButtons;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the WPF message filter.</summary>
		public WpfFocusMessageFilter(FrameworkElement element) {
			this.element = element;
		}


		//-----------------------------------------------------------------------------
		// Overloads
		//-----------------------------------------------------------------------------

		/// <summary>Filters out a message before it is dispatched.</summary>
		public bool PreFilterMessage(ref Message m) {
			if (m.Msg == NativeMethods.WM_MOUSEWHEEL) {
				
				var focusElement = FocusManager.GetFocusedElement(element);
				var focusScope = FocusManager.GetFocusScope(element);
				bool isDescendant = WpfHelper.IsDescendant(focusScope, focusElement as DependencyObject);
				if (element.IsMouseOver && !element.IsFocused &&
					((!ignoreMouseButtons && focusElement != element) ||
					focusElement == null || !(focusElement is ComboBoxItem || focusElement is SelectorItem)))
				{
					element.Focus();
					HwndSource hwndSource = (HwndSource)HwndSource.FromDependencyObject(element);
					NativeMethods.SendMessage(hwndSource.Handle, m.Msg, m.WParam, m.LParam);
					return true;
				}
			}
			else if (!ignoreMouseButtons && (m.Msg == NativeMethods.WM_LBUTTONDOWN ||
				m.Msg == NativeMethods.WM_RBUTTONDOWN || m.Msg == NativeMethods.WM_MBUTTONDOWN)) {
				var focusElement = FocusManager.GetFocusedElement(element);
				if (element.IsMouseOver && !element.IsFocused &&
					focusElement != element)
				{
					element.Focus();
					HwndSource hwndSource = (HwndSource)HwndSource.FromDependencyObject(element);
					NativeMethods.SendMessage(hwndSource.Handle, m.Msg, m.WParam, m.LParam);
					return true;
				}
			}

			return false;
		}


		//-----------------------------------------------------------------------------
		// Setup
		//-----------------------------------------------------------------------------

		/// <summary>Adds the message filter to the application.
		/// Call remove when the control is removed.</summary>
		public void AddFilter(bool ignoreMouseButtons = false) {
			this.ignoreMouseButtons = ignoreMouseButtons;
			System.Windows.Forms.Application.AddMessageFilter(this);
		}

		/// <summary>Removes the message filter from the application.</summary>
		public void RemoveFilter() {
			System.Windows.Forms.Application.RemoveMessageFilter(this);
		}
	}

	/// <summary>Used to steal focus when scrolling or clicking over the area of a WinForms control.</summary>
	public class WinFormsFocusMessageFilter : IMessageFilter {
		/// <summary>The WinForms control to focus on.</summary>
		private GraphicsDeviceControl control;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the WinForms message filter.</summary>
		public WinFormsFocusMessageFilter(GraphicsDeviceControl control) {
			this.control = control;
		}


		//-----------------------------------------------------------------------------
		// Overloads
		//-----------------------------------------------------------------------------

		/// <summary>Filters out a message before it is dispatched.</summary>
		public bool PreFilterMessage(ref Message m) {
			if (m.Msg == NativeMethods.WM_MOUSEWHEEL || m.Msg == NativeMethods.WM_LBUTTONDOWN ||
				m.Msg == NativeMethods.WM_RBUTTONDOWN || m.Msg == NativeMethods.WM_MBUTTONDOWN)
			{
				if (control.IsMouseOver && !control.Focused) {
					control.Focus();
					NativeMethods.SendMessage(control.Handle, m.Msg, m.WParam, m.LParam);
					return true;
				}
			}

			return false;
		}


		//-----------------------------------------------------------------------------
		// Setup
		//-----------------------------------------------------------------------------

		/// <summary>Adds the message filter to the application.
		/// Call remove when the control is removed.</summary>
		public void AddFilter() {
			System.Windows.Forms.Application.AddMessageFilter(this);
		}

		/// <summary>Removes the message filter from the application.</summary>
		public void RemoveFilter() {
			System.Windows.Forms.Application.RemoveMessageFilter(this);
		}
	}
}
