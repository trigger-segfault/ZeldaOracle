using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ZeldaOracle.Common.Properties;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Content;
using ZeldaEditor.Control;
using ZeldaOracle.Game.Items.Rewards;


namespace ZeldaEditor.PropertiesEditor {
	
	class ResourcePropertyEditor<T> : UITypeEditor {
        private IWindowsFormsEditorService svc = null;

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}

		public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value) {
			svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			
			//Foo foo = value as Foo;
			
			if (svc != null)// && foo != null)
			{
				ListBox comboBox = new ListBox();
				comboBox.BorderStyle = BorderStyle.None;

				Dictionary<string, T> resourceMap = Resources.GetResourceDictionary<T>();
				comboBox.Items.Add("(none)");
				foreach (KeyValuePair<string, T> entry in resourceMap) {
					comboBox.Items.Add(entry.Key);
				}

				comboBox.SelectedValueChanged += new EventHandler(this.ValueChanged);
				//SetEditorProps((ComboBox) context.Instance, comboBox);
				
				svc.DropDownControl(comboBox);

				if (comboBox.SelectedIndex >= 0) {
					if (comboBox.SelectedIndex == 0)
						value = "";
					else 
						value = (string) comboBox.Items[comboBox.SelectedIndex];
				}
			}
			return value; // can also replace the wrapper object here
		}

        private void ValueChanged(object sender, EventArgs e) {
            if (svc != null) {
                svc.CloseDropDown();
            }
        }
	}


	[TypeConverter(typeof(PropertiesContainer.CustomObjectConverter))]
	public class PropertiesContainer {
		
		private PropertyGridControl	propertyGridControl;
		private Properties			properties;
		private List<Property>		propertyList;

		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PropertiesContainer(PropertyGridControl propertyGridControl) {
			this.properties				= null;
			this.propertyList			= new List<Property>();
			this.propertyGridControl	= propertyGridControl;
		}
		
		
		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		public void Clear() {
			properties = null;
			propertyList.Clear();
		}

		public void AddProperties(Properties properties) {
			// Add the base properties.
			if (properties.BaseProperties != null)
				AddProperties(properties.BaseProperties);
			
			int basePropertyCount = propertyList.Count;

			// Add the properties.
			foreach (KeyValuePair<string, Property> propertyEntry in properties.PropertyMap) {
				bool hasBaseProperty = false;

				// Check if there is a matching base property.
				for (int i = 0; i < basePropertyCount; i++) {
					if (propertyList[i].Name == propertyEntry.Value.Name) {
						propertyList[i] = propertyEntry.Value;
						hasBaseProperty = true;
						break;
					}
				}
				if (!hasBaseProperty)
					propertyList.Add(propertyEntry.Value);
			}

			for (int i = 0; i < propertyList.Count; i++) {
				if (propertyList[i].HasDocumentation && propertyList[i].Documentation.Hidden) {
					propertyList.RemoveAt(i);
					i--;
				}
			}
		}

		public void AddBaseProperties(Properties properties, Properties modifiedProperties) {
			// Add the base properties.
			if (properties.BaseProperties != null)
				AddBaseProperties(properties.BaseProperties, modifiedProperties);

			int basePropertyCount = propertyList.Count;

			// Add the properties.
			foreach (KeyValuePair<string, Property> propertyEntry in properties.PropertyMap) {
				bool hasBaseProperty = false;

				// Check if there is a matching base property.
				for (int i = 0; i < basePropertyCount; i++) {
					if (propertyList[i].Name == propertyEntry.Value.Name) {
						propertyList[i] = propertyEntry.Value;
						hasBaseProperty = true;
						break;
					}
				}
				if (!hasBaseProperty)
					propertyList.Add(propertyEntry.Value);
			}
		}

		public void Set(Properties properties) {
			Clear();
			this.properties = properties;
			AddProperties(properties);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		[Browsable(false)]
		public Properties Properties {
			get { return properties; }
		}

		[Browsable(false)]
		public List<Property> PropertyList {
			get { return propertyList; }
		}

		[Browsable(false)]
		public PropertyGridControl PropertyGridControl {
			get { return propertyGridControl; }
		}


		//-----------------------------------------------------------------------------
		// Internal Classes
		//-----------------------------------------------------------------------------

		private class CustomObjectConverter : ExpandableObjectConverter {
			public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
				PropertiesContainer obj = (value as PropertiesContainer);
				if (obj == null)
					return new PropertyDescriptorCollection(new PropertyDescriptor[] {});

				List<Property> propertyList	= obj.PropertyList;
				PropertyDescriptor[] props	= new PropertyDescriptor[propertyList.Count];

				// Create the list of property descriptors.
				for (int i = 0; i < propertyList.Count; i++) {
					Property property	= propertyList[i];
					string name			= property.Name;
					UITypeEditor editor = null;

					// Find the documentation for this property.
					Properties objProperties = obj.Properties;
					PropertyDocumentation documentation = null;
					while (objProperties != null) {
						if (objProperties.PropertyMap.ContainsKey(property.Name)) {
							Property p = objProperties.PropertyMap[property.Name];
							if (p.Documentation != null) {
								documentation = p.Documentation;
								break;
							}
						}
						objProperties = objProperties.BaseProperties;
					}

					// Find the editor.
					if (documentation != null)
						editor = obj.PropertyGridControl.GetUITypeEditor(documentation.EditorType);

					// Create the property descriptor.
					props[i] = new CustomPropertyDescriptor(
						documentation, editor, property, obj.Properties);
				}

				return new PropertyDescriptorCollection(props);
			}
		}

	}

}
