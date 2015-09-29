using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Properties;


namespace ZeldaEditor {
	
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
		

		private Properties baseProperties;
		private Properties properties;
		private List<PropertyInstance> propertyInstances;
		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PropertiesContainer() {
			properties = null;
			baseProperties = null;
			propertyInstances = new List<PropertyInstance>();
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
						props[i] = new CustomPropertyDescriptor(
							instances[i].ModifiedProperty,
							instances[i].BaseProperty,
							obj.Properties);
					}

					/*
					int index = stdProps.Count;

					// Add base properties.

					foreach (KeyValuePair<string, Property> prop in properties.PropertyMap) {
						Property baseProperty = null;
						if (baseProperties != null && baseProperties.Exists(prop.Key)) {
							baseProperty = baseProperties[prop.Key];
						}
						props[index++] = new CustomPropertyDescriptor(prop.Value, baseProperty);
					}*/
				}

				return new PropertyDescriptorCollection(props);
			}
		}

		private class CustomPropertyDescriptor : PropertyDescriptor {
			private Property property;
			private Property baseProperty;
			private Properties modifiedProperties;


			//-----------------------------------------------------------------------------
			// Constructor
			//-----------------------------------------------------------------------------

			public CustomPropertyDescriptor(Property property, Property baseProperty, Properties modifiedProperties)
				: base(property != null ? property.Name : baseProperty.Name, null)
			{
				this.property = property;
				this.baseProperty = baseProperty;
				this.modifiedProperties = modifiedProperties;
			}

			public Property Property {
				get { return (property == null ? baseProperty : property); }
			}


			//-----------------------------------------------------------------------------
			// Overridden methods
			//-----------------------------------------------------------------------------

			public override string Category {
				get { return ""; }
			}

			public override string Description {
				get { return ""; }
			}

			public override string Name {
				get { return (property != null ? property.Name : baseProperty.Name); }
			}

			public override bool ShouldSerializeValue(object component) {
				if (baseProperty == null)
					return true;
				if (property == null)
					return false;
				return !property.EqualsValue(baseProperty);
			}

			public override void ResetValue(object component) {
			}

			public override bool IsReadOnly {
				get { return false; }
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

			public override bool CanResetValue(object component) {
				return true;
			}

			public override Type ComponentType {
				get { return typeof(CustomObjectType); }
			}

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
		
			/*
			public override string	Category									{ get { return property.Category; } }
			public override string	Description									{ get { return property.Description; } }
			public override string	Name										{ get { return property.Name; } }
			public override bool	ShouldSerializeValue(object component)		{ return !property.Value.Equals(property.DefaultValue); }
			public override void	ResetValue(object component)				{ property.Value = null; }
			public override bool	IsReadOnly									{ get { return property.IsReadOnly; } }
			public override Type	PropertyType								{ get { return property.Type; } }
			public override bool	CanResetValue(object component)				{ return true; }
			public override Type	ComponentType								{ get { return typeof(CustomObjectType); } }
			public override void	SetValue(object component, object value)	{ property.Value = value; }
			public override object	GetValue(object component)					{ return property.Value; }
			*/
		}
	}

	#region OLD

	public class CustomProperty {
		private string	name;
		private string	description;
		private string	category;
		private Type	type;
		private object	value;
		private object	defaultValue;
		private bool	isReadOnly;
		private bool	isVisible;


		public static CustomProperty Create<T>(string name, object value, object defaultValue, string description, string category) {
			CustomProperty property = new CustomProperty();
			property.name			= name;
			property.value			= value;
			property.defaultValue	= defaultValue;
			property.description	= description;
			property.category		= category;
			property.type			= typeof(T);
			property.isReadOnly		= false;
			property.isVisible		= true;
			return property;
		}
		
		public CustomProperty() {
			this.name			= "";
			this.description	= "";
			this.category		= "";
			this.type			= null;
			this.value			= null;
			this.defaultValue	= null;
			this.isReadOnly		= false;
			this.isVisible		= true;
		}


		public CustomProperty(string name, object value, bool isReadOnly, bool isVisible) {
			this.name		= name;
			this.value		= value;
			this.isReadOnly	= isReadOnly;
			this.isVisible	= isVisible;
		}

		public string Name {
			get { return name; }
			set { name = value; }
		}

		public string Description {
			get { return description; }
			set { description = value; }
		}

		public string Category {
			get { return category; }
			set { category = value; }
		}

		public object Value {
			get { return value; }
			set { this.value = value; }
		}

		public object DefaultValue {
			get { return defaultValue; }
			set { defaultValue = value; }
		}

		public bool IsReadOnly {
			get { return isReadOnly; }
			set { isReadOnly = value; }
		}

		public bool IsVisible {
			get { return isVisible; }
			set { isVisible = value; }
		}

		public Type Type {
			get { return type; }
			set { type = value; }
		}
	}


	[TypeConverter(typeof(CustomObjectType.CustomObjectConverter))]
	public class CustomObjectType {

		private readonly List<CustomProperty> properties;


		public CustomObjectType() {
			properties = new List<CustomProperty>();
		}

		public void AddProperty(CustomProperty property) {
			properties.Add(property);
		}

		[Browsable(false)]
		public List<CustomProperty> Properties {
			get { return properties; }
		}

		private class CustomObjectConverter : ExpandableObjectConverter {
			public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
				var stdProps = base.GetProperties(context, value, attributes);
				
				CustomObjectType obj = value as CustomObjectType;
				List<CustomProperty> customProps = obj == null ? null : obj.Properties;
				PropertyDescriptor[] props = new PropertyDescriptor[stdProps.Count + (customProps == null ? 0 : customProps.Count)];
				stdProps.CopyTo(props, 0);

				if (customProps != null) {
					int index = stdProps.Count;
					foreach (CustomProperty prop in customProps) {
						props[index++] = new CustomPropertyDescriptor(prop);
					}
				}

				return new PropertyDescriptorCollection(props);
			}
		}

		private class CustomPropertyDescriptor : PropertyDescriptor {
			private readonly CustomProperty prop;

			public CustomPropertyDescriptor(CustomProperty prop)
				: base(prop.Name, null)
			{
				this.prop = prop;
			}

			public override string	Category									{ get { return prop.Category; } }
			public override string	Description									{ get { return prop.Description; } }
			public override string	Name										{ get { return prop.Name; } }
			public override bool	ShouldSerializeValue(object component)		{ return !prop.Value.Equals(prop.DefaultValue); }
			public override void	ResetValue(object component)				{ prop.Value = null; }
			public override bool	IsReadOnly									{ get { return prop.IsReadOnly; } }
			public override Type	PropertyType								{ get { return prop.Type; } }
			public override bool	CanResetValue(object component)				{ return true; }
			public override Type	ComponentType								{ get { return typeof(CustomObjectType); } }
			public override void	SetValue(object component, object value)	{ prop.Value = value; }
			public override object	GetValue(object component)					{ return prop.Value; }
		}
	}

	#endregion
}
