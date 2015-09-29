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


namespace ZeldaEditor {

	class RewardPropertyEditor : UITypeEditor {
        private IWindowsFormsEditorService svc;
		private RewardManager rewardManager;
		private ListBox listBox;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public RewardPropertyEditor(RewardManager rewardManager) {
			this.rewardManager	= rewardManager;
			this.svc			= null;
			this.listBox		= new ListBox();
			
			// Add resources to the list.
			listBox.Items.Add("(none)");
			foreach (KeyValuePair<string, Reward> entry in rewardManager.RewardDictionary) {
				listBox.Items.Add(entry.Key);
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}

		public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value) {
			svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			
			if (svc != null)
			{
				listBox.SelectedValueChanged += new EventHandler(this.ValueChanged);
				svc.DropDownControl(listBox);

				if (listBox.SelectedIndex >= 0) {
					if (listBox.SelectedIndex == 0)
						value = "";
					else 
						value = (string) listBox.Items[listBox.SelectedIndex];
				}
			}
			return value;
		}

        private void ValueChanged(object sender, EventArgs e) {
            if (svc != null)
                svc.CloseDropDown();
        }
	}

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

	
	class TextMessagePropertyEditor : UITypeEditor {
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}

		public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value) {
			IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			if (svc != null) {
				using (TextMessageEditForm form = new TextMessageEditForm()) {
					form.MessageText = (string) value;
					if (svc.ShowDialog(form) == DialogResult.OK)
						value = form.MessageText;
				}
			}
			return value;
		}
	}
	
	public class PropertyInstance {
		private Property modifiedProperty;
		private Property baseProperty;

		public PropertyInstance(Property baseProperty, Property modifiedProperty) {
			this.baseProperty		= baseProperty;
			this.modifiedProperty	= modifiedProperty;
		}
		
		public Property ModifiedProperty {
			get { return modifiedProperty; }
			set { modifiedProperty = value; }
		}
		
		public Property BaseProperty {
			get { return baseProperty; }
			set { baseProperty = value; }
		}
	}

	[TypeConverter(typeof(PropertiesContainer.CustomObjectConverter))]
	public class PropertiesContainer {
		
		private List<PropertyInstance> propertyInstances;
		private Properties baseProperties;
		private Properties properties;
		private PropertyGridControl propertyGridControl;

		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PropertiesContainer(PropertyGridControl propertyGridControl) {
			this.properties				= null;
			this.baseProperties			= null;
			this.propertyInstances		= new List<PropertyInstance>();
			this.propertyGridControl	= propertyGridControl;
		}
		
		
		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		public void Clear() {
			properties = null;
			baseProperties = null;
			propertyInstances.Clear();
		}

		public void Set(Properties properties, Properties baseProperties) {
			Clear();

			this.properties = properties;
			this.baseProperties = baseProperties;

			// Populate Property instancess.
			
			// Add base properties.
			if (baseProperties != null) {
				foreach (KeyValuePair<string, Property> propertyEntry in baseProperties.PropertyMap) {
					propertyInstances.Add(new PropertyInstance(propertyEntry.Value, null));
				}
			}

			int basePropertyCount = propertyInstances.Count;

			// Add modified properties.
			if (properties != null) {
				foreach (KeyValuePair<string, Property> propertyEntry in properties.PropertyMap) {
					bool hasBaseProperty = false;

					// Check if there is a matching base property.
					for (int i = 0; i < basePropertyCount; i++) {
						if (propertyInstances[i].BaseProperty.Name == propertyEntry.Value.Name) {
							hasBaseProperty = true;
							propertyInstances[i].ModifiedProperty = propertyEntry.Value;
							break;
						}
					}

					if (!hasBaseProperty)
						propertyInstances.Add(new PropertyInstance(null, propertyEntry.Value));
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		[Browsable(false)]
		public Properties Properties {
			get { return properties; }
		}

		[Browsable(false)]
		public Properties BaseProperties {
			get { return baseProperties; }
		}

		[Browsable(false)]
		public List<PropertyInstance> PropertyInstances {
			get { return propertyInstances; }
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
				var stdProps = base.GetProperties(context, value, attributes);
				
				PropertiesContainer obj	= value as PropertiesContainer;
				List<PropertyInstance> instances = (obj == null ? null : obj.PropertyInstances);

				PropertyDescriptor[] props = new PropertyDescriptor[stdProps.Count + (instances == null ? 0 : instances.Count)];
				stdProps.CopyTo(props, 0);

				if (instances != null) {
					for (int i = 0; i < instances.Count; i++) {
						PropertyInstance instance = instances[i];

						string name = CustomPropertyDescriptor.GetDisplayName(
							instance.ModifiedProperty,
							instance.BaseProperty);

						UITypeEditor editor = null;
						if (instance.BaseProperty != null && instance.BaseProperty.HasDocumentation)
							editor = obj.PropertyGridControl.GetUITypeEditor(instance.BaseProperty.Documentation.EditorType);


						props[i] = new CustomPropertyDescriptor(
							name,
							editor,
							instance.ModifiedProperty,
							instance.BaseProperty,
							obj.Properties);
					}
				}

				return new PropertyDescriptorCollection(props);
			}
		}

		private class CustomPropertyDescriptor : PropertyDescriptor {
			private Property property;
			private Property baseProperty;
			private Properties modifiedProperties;
			private UITypeEditor editor;

			public static string GetDisplayName(Property property, Property baseProperty) {
				if (baseProperty != null && baseProperty.HasDocumentation)
					return baseProperty.Documentation.ReadableName;
				return (baseProperty != null ? baseProperty.Name : property.Name);
			}

			public Property Property {
				get { return (property == null ? baseProperty : property); }
			}


			//-----------------------------------------------------------------------------
			// Constructor
			//-----------------------------------------------------------------------------

			public CustomPropertyDescriptor(string name, UITypeEditor editor, Property property, Property baseProperty, Properties modifiedProperties) :
				base(name, null)
			{
				this.editor				= editor;
				this.property			= property;
				this.baseProperty		= baseProperty;
				this.modifiedProperties	= modifiedProperties;
			}


			//-----------------------------------------------------------------------------
			// Overridden methods
			//-----------------------------------------------------------------------------

			// Returns true if the property is modified.
			public override bool ShouldSerializeValue(object component) {
				if (baseProperty == null)
					return true;
				if (property == null)
					return false;
				return !property.EqualsValue(baseProperty);
			}

			// Is the value allowed to be reset?
			public override bool CanResetValue(object component) {
				return true;
			}

			// Reset the value to the default.
			public override void ResetValue(object component) {
				// TODO: override ResetValue
			}

			// Get the editor to use to edit this property.
			public override object GetEditor(Type editorBaseType) {
				if (editorBaseType == typeof(UITypeEditor)) {
					if (editor != null)
						return editor;
					return base.GetEditor(editorBaseType);
				}
				else {
					return base.GetEditor(editorBaseType);
				}
			}

			// Set the value of the property.
			public override void SetValue(object component, object value) {
				if (property == null) {
					property = new ZeldaOracle.Common.Properties.Property(baseProperty);
					modifiedProperties.Add(property);
				}

				if (Property.Type == ZeldaOracle.Common.Properties.PropertyType.String)
					Property.StringValue = (string) value;
				if (Property.Type == ZeldaOracle.Common.Properties.PropertyType.Integer)
					Property.IntValue = (int) value;
				if (Property.Type == ZeldaOracle.Common.Properties.PropertyType.Float)
					Property.FloatValue = (float) value;
				if (Property.Type == ZeldaOracle.Common.Properties.PropertyType.Boolean)
					Property.BoolValue = (bool) value;

				// Check if base and modified are the same. If so, remove the modified one.
				if (baseProperty != null && property != null && baseProperty.EqualsValue(property)) {
					Console.WriteLine("Removing modified property");
					modifiedProperties.Remove(property.Name);
					property = null;
				}
			}
			
			// Get the value of the property.
			public override object GetValue(object component) {
				if (Property.Type == ZeldaOracle.Common.Properties.PropertyType.String)
					return Property.StringValue;
				if (Property.Type == ZeldaOracle.Common.Properties.PropertyType.Integer)
					return Property.IntValue;
				if (Property.Type == ZeldaOracle.Common.Properties.PropertyType.Float)
					return Property.FloatValue;
				if (Property.Type == ZeldaOracle.Common.Properties.PropertyType.Boolean)
					return Property.BoolValue;
				return null;
			}


			//-----------------------------------------------------------------------------
			// Overridden properties
			//-----------------------------------------------------------------------------
			
			// Is the property read-only?
			public override bool IsReadOnly {
				get { return false; }
			}
			
			// Should the property be listed in the property grid?
			public override bool IsBrowsable {
				get { return false; }
			}

			public override Type ComponentType {
				get { return typeof(PropertiesContainer); }
			}

			// Get the category this property should be listed under.
			public override string Category {
				get {
					if (baseProperty != null && baseProperty.HasDocumentation)
						return baseProperty.Documentation.Category;
					return "";
				}
			}

			// Get the description for the property.
			public override string Description {
				get {
					if (baseProperty != null && baseProperty.HasDocumentation)
						return baseProperty.Documentation.Description;
					return "";
				}
			}

			// Get the display name for the property.
			public override string Name {
				get {
					if (baseProperty != null && baseProperty.HasDocumentation)
						return baseProperty.Documentation.ReadableName;
					return (baseProperty != null ? baseProperty.Name : property.Name);
				}
			}
			
			public override Type PropertyType {
				get {
					if (Property.Type == ZeldaOracle.Common.Properties.PropertyType.String)
						return typeof(string);
					if (Property.Type == ZeldaOracle.Common.Properties.PropertyType.Integer)
						return typeof(int);
					if (Property.Type == ZeldaOracle.Common.Properties.PropertyType.Float)
						return typeof(float);
					if (Property.Type == ZeldaOracle.Common.Properties.PropertyType.Boolean)
						return typeof(bool);
					return null;
				}
			}

		}
	}

}
