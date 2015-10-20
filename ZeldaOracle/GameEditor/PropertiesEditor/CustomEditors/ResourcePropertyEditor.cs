using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaOracle.Common.Content;

namespace ZeldaEditor.PropertiesEditor.CustomEditors {
	public class ResourcePropertyEditor<T> : DropDownPropertyEditor {
        
		public override void CreateList(ListBox listBox, object value) {
			Dictionary<string, T> resourceMap = Resources.GetResourceDictionary<T>();
			listBox.Items.Add("(none)");
			foreach (KeyValuePair<string, T> entry in resourceMap) {
				listBox.Items.Add(entry.Key);
			}
		}

		public override object OnItemSelected(ListBox listBox, int index, object value) {
			if (listBox.SelectedIndex == 0)
				return "";
			else 
				return (string) listBox.Items[index];
		}
	}
}
