using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using ZeldaOracle.Common.Content;

namespace ZeldaEditor.PropertiesEditor.CustomEditors {
	public class ResourcePropertyEditor<T> : ComboBoxEditor {

		protected override IEnumerable CreateItemsSource(PropertyItem item) {
			Dictionary<string, T> resourceMap = Resources.GetResourceDictionary<T>();
			string[] resourceIds = new string[resourceMap.Count];
			int index = 0;
			foreach (var resource in resourceMap) {
				resourceIds[index] = resource.Key;
				index++;
			}
			return resourceIds;
		}
	}
}
