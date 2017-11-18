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
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Content;
using ZeldaEditor.Control;
using ZeldaOracle.Game.Items.Rewards;
using ZeldaOracle.Common.Scripting;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using ZeldaOracle.Game.Worlds;
using ZeldaEditor.PropertiesEditor.CustomEditors;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Game.Tiles;
using System.Diagnostics;

namespace ZeldaEditor.PropertiesEditor {
	

	//[TypeConverter(typeof(PropertiesContainer.CustomObjectConverter))]
	public class PropertiesContainer : ICustomTypeDescriptor {
		
		private ZeldaPropertyGrid	propertyGrid;
		private Properties			properties;
		private List<Property>		propertyList;

		private Dictionary<string, Type> typeEditors;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PropertiesContainer(ZeldaPropertyGrid propertyGrid) {
			this.properties		= null;
			this.propertyList	= new List<Property>();
			this.propertyGrid	= propertyGrid;
			this.typeEditors    = new Dictionary<string, Type>();


			AddEditor<ResourcePropertyEditor<Animation>>("animation");
			AddEditor<ResourcePropertyEditor<CollisionModel>>("collision_model");
			AddEditor<ResourcePropertyEditor<Song>>("song");
			AddEditor<ResourcePropertyEditor<Sound>>("sound");
			AddEditor<ResourcePropertyEditor<Sprite>>("sprite");
			AddEditor<ResourcePropertyEditor<Zone>>("zone");
			AddEditor<RewardPropertyEditor>("reward");
			//AddEditor<ScriptPropertyEditor>("script");
			AddEditor<DungeonPropertyEditor>("dungeon");
			AddEditor<LevelPropertyEditor>("level");
			AddEditor<EnumPropertyEditor>("enum");
			AddEditor<EnumFlagPropertyEditor>("enum_flags");
			AddEditor<ScriptPropertyEditor>("script");
			AddEditor<TextMessagePropertyEditor>("text_message");

			// Create custom property editor types.
			/*typeEditors["sprite"]           = new ResourcePropertyEditor<Sprite>();
			typeEditors["animation"]        = new ResourcePropertyEditor<Animation>();
			typeEditors["collision_model"]  = new ResourcePropertyEditor<CollisionModel>();
			typeEditors["song"]             = new ResourcePropertyEditor<Song>();
			typeEditors["sound"]            = new ResourcePropertyEditor<Sound>();
			typeEditors["zone"]             = new ResourcePropertyEditor<Zone>();
			typeEditors["reward"]           = new RewardPropertyEditor(editorControl.RewardManager);
			typeEditors["text_message"]     = new TextMessagePropertyEditor();
			typeEditors["script"]           = new ScriptPropertyEditor();
			typeEditors["sprite_index"]     = new SpriteIndexComboBox();
			typeEditors["direction"]        = new DirectionPropertyEditor();
			typeEditors["angle"]            = null;
			typeEditors["enum"]             = new EnumComboBox();
			typeEditors["enum_flags"]       = null;
			typeEditors["dungeon"]          = new DungeonPropertyEditor();
			typeEditors["level"]            = new LevelPropertyEditor();*/
		}

		public void AddEditor<Editor>(string typeName) where Editor : ITypeEditor {
			typeEditors.Add(typeName, typeof(Editor));
		}
		
		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		public void Clear() {
			properties = null;
			propertyList.Clear();
		}

		public void AddProperties(Properties properties) {
			foreach (Property property in  properties.GetAllProperties()) {
				PropertyDocumentation doc = property.GetDocumentation();
				if (doc == null || !doc.IsHidden)
					propertyList.Add(property);
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
		public ZeldaPropertyGrid PropertyGrid {
			get { return propertyGrid; }
		}


		//-----------------------------------------------------------------------------
		// Internal Classes
		//-----------------------------------------------------------------------------

		/*private class CustomObjectConverter : ExpandableObjectConverter {
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
					PropertyDocumentation documentation = property.GetDocumentation();
					
					// Find the editor.
					if (documentation != null)
						editor = obj.PropertyGrid.GetUITypeEditor(documentation.EditorType);

					// Create the property descriptor.
					props[i] = new CustomPropertyDescriptor(
						documentation, editor, property.Name, obj.Properties);
				}

				return new PropertyDescriptorCollection(props);
			}
		}*/


		//-----------------------------------------------------------------------------
		// ICustomTypeDescriptor
		//-----------------------------------------------------------------------------

		/// <summary>
		/// Gets the Attributes for the object
		/// </summary>
		AttributeCollection ICustomTypeDescriptor.GetAttributes() {
			return new AttributeCollection(null);
		}

		/// <summary>
		/// Gets the Class name
		/// </summary>
		string ICustomTypeDescriptor.GetClassName() {
			return null;
		}

		/// <summary>
		/// Gets the component Name
		/// </summary>
		string ICustomTypeDescriptor.GetComponentName() {
			return null;
		}

		/// <summary>
		/// Gets the Type Converter
		/// </summary>
		TypeConverter ICustomTypeDescriptor.GetConverter() {
			return null;
		}

		/// <summary>
		/// Gets the Default Event
		/// </summary>
		/// <returns></returns>
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() {
			return null;
		}

		/// <summary>
		/// Gets the Default Property
		/// </summary>
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() {
			return null;
		}

		/// <summary>
		/// Gets the Editor
		/// </summary>
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) {
			return null;
		}

		/// <summary>
		/// Gets the Events
		/// </summary>
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) {
			return new EventDescriptorCollection(null);
		}

		/// <summary>
		/// Gets the events
		/// </summary>
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() {
			return new EventDescriptorCollection(null);
		}
		
		/// <summary>
		/// Gets the properties
		/// </summary>
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) {
			List<PropertyDescriptor> props  = new List<PropertyDescriptor>();

			// Create the list of property descriptors.
			for (int i = 0; i < propertyList.Count; i++) {
				Property property   = propertyList[i];
				PropertyDocumentation documentation = property.GetDocumentation();

				bool isEvent = (documentation != null && documentation.EditorType == "script");

				if (isEvent != propertyGrid.IsEvents)
					continue;

				// Find the editor.
				List<Attribute> propertyAttributes = new List<Attribute>();
				EditorAttribute editorAttr = null;
				if (documentation != null && documentation.EditorType != "") {
					if (typeEditors.ContainsKey(documentation.EditorType)) {
						Type editorType = typeEditors[documentation.EditorType];
						editorAttr = new EditorAttribute(editorType, editorType);
					}
				}
				if (editorAttr == null && property.Type == PropertyType.Point) {
					Type editorType = typeof(Point2IPropertyEditor);
					editorAttr = new EditorAttribute(editorType, editorType);
				}
				if (editorAttr != null)
					propertyAttributes.Add(editorAttr);

				if (properties.BaseProperties != null) {
					Property baseProperty = properties.BaseProperties.GetProperty(property.Name, true);
					if (baseProperty != null) {
						var defaultAttr = new DefaultValueAttribute(baseProperty.ObjectValue);
						propertyAttributes.Add(defaultAttr);
					}
				}

				// Create the property descriptor.
				props.Add(new CustomPropertyDescriptor(propertyGrid.PropertyObject,
					documentation, property.Name, properties, propertyAttributes.ToArray()));
			}
			return new PropertyDescriptorCollection(props.ToArray());
		}


		/// <summary>
		/// gets the Properties
		/// </summary>
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() {
			return ((ICustomTypeDescriptor)this).GetProperties(null);
		}

		/// <summary>
		/// Gets the property owner
		/// </summary>
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) {
			return this;
		}
	}

}
