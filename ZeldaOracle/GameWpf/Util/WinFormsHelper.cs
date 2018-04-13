using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.Integration;
using FormsControl = System.Windows.Forms.Control;

namespace ZeldaWpf.Util {
	public static class WinFormsHelper {

		private static readonly Type AdapterType;
		private static readonly FieldInfo AdapterHostField;

		static WinFormsHelper() {
			const string adapterName = "System.Windows.Forms.Integration.WinFormsAdapter";
			Assembly adapterAssembly = typeof(WindowsFormsHost).Assembly;
			AdapterType = adapterAssembly.GetType(adapterName);
			AdapterHostField = AdapterType.GetField("_host", BindingFlags.NonPublic | BindingFlags.Instance);
		}

		public static Window GetWindow(FormsControl control) {
			WindowsFormsHost host = GetHost(control);
			return Window.GetWindow(host);
		}

		public static WindowsFormsHost GetHost(FormsControl control) {
			if (control.Parent != null) {
				return GetHost(control.Parent);
			}
			else if (control.GetType() == AdapterType) {
				return (WindowsFormsHost) AdapterHostField.GetValue(control);
			}
			else {
				throw new Exception("The top parent of a control within a host " +
					"should be a System.Windows.Forms.Integration.WinFormsAdapter, " +
					"but that is not what was found!");
			}
		}
	}
}
