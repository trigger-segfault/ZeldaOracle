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
		private EventCollection     events;
		private List<Property>		propertyList;
		private List<Event>			eventList;

		private Dictionary<string, Type> typeEditors;

		private static readonly Type EventEditor = typeof(EventPropertyEditor);


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PropertiesContainer(ZeldaPropertyGrid propertyGrid) {
			this.properties		= null;
			this.events         = null;
			this.propertyList	= new List<Property>();
			this.eventList      = new List<Event>();
			this.propertyGrid	= propertyGrid;
			this.typeEditors    = new Dictionary<string, Type>();


			AddEditor<ResourcePropertyEditor<Animation>>("animation");
			AddEditor<ResourcePropertyEditor<CollisionModel>>("collision_model");
			AddEditor<ResourcePropertyEditor<Song>>("song");
			AddEditor<ResourcePropertyEditor<Sound>>("sound");
			AddEditor<ResourcePropertyEditor<Sprite>>("sprite");
			AddEditor<ResourcePropertyEditor<Zone>>("zone");
			AddEditor<RewardPropertyEditor>("reward");
			AddEditor<DungeonPropertyEditor>("dungeon");
			AddEditor<LevelPropertyEditor>("level");
			AddEditor<EnumPropertyEditor>("enum");
			AddEditor<EnumFlagPropertyEditor>("enum_flags");
			AddEditor<TextMessagePropertyEditor>("text_message");
			AddEditor<WarpPropertyEditor>("warp");
			AddEditor<DirectionPropertyEditor>("direction");
			AddEditor<AnglePropertyEditor>("angle");
		}

		public void AddEditor<Editor>(string typeName) where Editor : ITypeEditor {
			typeEditors.Add(typeName, typeof(Editor));
		}
		
		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		public void Clear() {
			properties = null;
			events = null;
			propertyList.Clear();
			eventList.Clear();
		}

		public void AddProperties(Properties properties) {
			foreach (Property property in  properties.GetAllProperties()) {
				if (property.IsBrowsable)
					propertyList.Add(property);
			}
		}

		public void AddEvents(EventCollection events) {
			if (events != null) {
				foreach (Event evnt in events.GetEvents()) {
					eventList.Add(evnt);
				}
			}
		}

		public void Set(Properties properties, EventCollection events) {
			Clear();
			this.properties = properties;
			this.events = events;
			AddProperties(properties);
			AddEvents(events);
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
		// ICustomTypeDescriptor
		//-----------------------------------------------------------------------------

		/// <summary>Gets the Attributes for the object.</summary>
		AttributeCollection ICustomTypeDescriptor.GetAttributes() {
			return new AttributeCollection(null);
		}

		/// <summary>Gets the Class name.</summary>
		string ICustomTypeDescriptor.GetClassName() {
			return null;
		}

		/// <summary>Gets the component Nam.e</summary>
		string ICustomTypeDescriptor.GetComponentName() {
			return null;
		}

		/// <summary>Gets the Type Converter.</summary>
		TypeConverter ICustomTypeDescriptor.GetConverter() {
			return null;
		}

		/// <summary>Gets the Default Event.</summary>
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() {
			return null;
		}

		/// <summary>Gets the Default Property.</summary>
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() {
			return null;
		}

		/// <summary>Gets the Editor.</summary>
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) {
			return null;
		}

		/// <summary>Gets the Events.</summary>
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) {
			return new EventDescriptorCollection(null);
		}

		/// <summary>Gets the events.</summary>
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() {
			return new EventDescriptorCollection(null);
		}

		/// <summary>Gets the properties.</summary>
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) {
			if (propertyGrid.IsEvents) {
				return CollectEvents();
			}
			else {
				return CollectProperties();
			}
		}

		public PropertyDescriptorCollection CollectProperties() {
			List<PropertyDescriptor> props  = new List<PropertyDescriptor>();
			
			// Create the list of property descriptors.
			for (int i = 0; i < propertyList.Count; i++) {
				Property property   = propertyList[i];
				PropertyDocumentation documentation = property.Documentation;

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
				props.Add(new CustomPropertyDescriptor(propertyGrid.EditorControl, propertyGrid.PropertyObject,
					property.Name, properties, propertyAttributes.ToArray()));
			}
			return new PropertyDescriptorCollection(props.ToArray());
		}


		public PropertyDescriptorCollection CollectEvents() {
			List<PropertyDescriptor> props  = new List<PropertyDescriptor>();

			if (events == null)
				new PropertyDescriptorCollection(props.ToArray());

			// Create the list of property descriptors.
			foreach (Event evnt in events.GetEvents()) {
				// Find the editor.
				List<Attribute> eventAttributes = new List<Attribute>();
				eventAttributes.Add(new EditorAttribute(EventEditor, EventEditor));
				eventAttributes.Add(new DefaultValueAttribute(""));
				
				// Create the event descriptor.
				props.Add(new CustomEventDescriptor(propertyGrid.EditorControl, propertyGrid.EventObject,
					evnt.Name, events, eventAttributes.ToArray()));
			}
			return new PropertyDescriptorCollection(props.ToArray());
		}


		/// <summary>gets the Properties.</summary>
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() {
			return ((ICustomTypeDescriptor)this).GetProperties(null);
		}

		/// <summary>Gets the property owner.</summary>
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) {
			return this;
		}
	}

}
