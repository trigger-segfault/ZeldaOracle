using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ZeldaEditor.Control;
using ZeldaOracle.Game;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;

namespace ZeldaEditor.PropertiesEditor.CustomEditors {
	public class EnumComboBox : DropDownPropertyEditor {

		private FieldInfo[] members;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public EnumComboBox() {

		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void CreateList(ListBox listBox, object value) {
			string enumName = "";
			if (propertyDescriptor.Documentation != null)
				enumName = propertyDescriptor.Documentation.EditorSubType;

			// Find the type of the enum from a string.
			Type enumType = null;
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				Type[] types = assembly.GetTypes();
				for (int i = 0; i < types.Length; i++) {
					if (types[i].Name.Equals(enumName, StringComparison.OrdinalIgnoreCase)) {
						enumType = types[i];
						break;
					}
				}
				if (enumType != null)
					break;
			}
				
			// Create and show the combo box.
			if (enumType != null && enumType.IsEnum) {
				// Add enum items to the combo box.
				members = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
				for (int i = 0; i < members.Length; i++) {
					bool valid = true;
					// Don't list values with the 'Hidden' attribute.
					foreach (CustomAttributeData attribute in members[i].CustomAttributes) {
						if (attribute.AttributeType == typeof(ScriptingAttributes.Hidden)) {
							valid = false;
							break;
						}
					}
					if (valid)
						listBox.Items.Add(members[i].Name);
				}
			}
			else {
				MessageBox.Show("Error: Unknown enum type '" + enumName + "'");
			}
		}

		public override object OnItemSelected(ListBox listBox, int index, object value) {
			return members[index].Name.ToLower();
		}
	}
}
