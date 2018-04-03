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
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Common;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace ZeldaEditor.PropertiesEditor {
	public class PropertiesContainer : ICustomTypeDescriptor {

		private static readonly Type EventEditor = typeof(EventPropertyEditor);

		private static Dictionary<string, Type> typeEditors;


		private ZeldaPropertyGrid	propertyGrid;
		private IPropertyObject		propertyObject;
		private IEventObject		eventObject;

		private PropertyDescriptorCollection propertyDescriptors;
		private PropertyDescriptorCollection eventDescriptors;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		static PropertiesContainer() {
			typeEditors = new Dictionary<string, Type>();

			AddEditor<ResourcePropertyEditor<Animation>>("animation");
			AddEditor<ResourcePropertyEditor<CollisionModel>>("collision_model");
			AddEditor<ResourcePropertyEditor<Music>>("music");
			AddEditor<ResourcePropertyEditor<Sound>>("sound");
			AddEditor<ResourcePropertyEditor<ISprite>>("sprite");
			AddEditor<ResourcePropertyEditor<Zone>>("zone");
			AddEditor<RewardPropertyEditor>("reward");
			AddEditor<AreaPropertyEditor>("area");
			AddEditor<LevelPropertyEditor>("level");
			AddEditor<EnumPropertyEditor>("enum");
			AddEditor<EnumFlagPropertyEditor>("enum_flags");
			AddEditor<TextMessagePropertyEditor>("text_message");
			AddEditor<WarpPropertyEditor>("warp");
			AddEditor<DirectionPropertyEditor>("direction");
			AddEditor<AnglePropertyEditor>("angle");
			AddEditor<Point2ISingleAxisPropertyEditor>("single_axis");
			AddEditor<PathPropertyEditor>("path");
		}

		public PropertiesContainer(ZeldaPropertyGrid propertyGrid, IPropertyObject obj) {
			this.propertyGrid   = propertyGrid;

			propertyObject	= obj;
			if (obj is IEventObject)
				eventObject = (IEventObject) obj;
			
			propertyDescriptors = CollectProperties();
			eventDescriptors = CollectEvents();
		}

		public static void AddEditor<Editor>(string typeName) where Editor : ITypeEditor {
			typeEditors.Add(typeName, typeof(Editor));
		}
		

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		[Browsable(false)]
		public Properties Properties {
			get { return propertyObject?.Properties; }
		}

		[Browsable(false)]
		public EventCollection Events {
			get { return eventObject?.Events; }
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
			if (propertyGrid.IsEvents)
				return eventDescriptors;
			else
				return propertyDescriptors;
		}

		public PropertyDescriptorCollection CollectProperties() {
			List<PropertyDescriptor> props  = new List<PropertyDescriptor>();

			if (propertyObject == null)
				return new PropertyDescriptorCollection(props.ToArray());

			// Create the list of property descriptors
			foreach (Property property in Properties.GetAllProperties()) {
				PropertyDocumentation documentation = property.Documentation;
				
				// Find the editor
				List<Attribute> propertyAttributes = new List<Attribute>();
				EditorAttribute editorAttr = null;
				if (documentation != null && documentation.EditorType != "") {
					if (typeEditors.ContainsKey(documentation.EditorType)) {
						Type editorType = typeEditors[documentation.EditorType];
						editorAttr = new EditorAttribute(editorType, editorType);
					}
				}
				if (editorAttr == null && property.Type == VarType.Point) {
					Type editorType = typeof(Point2IPropertyEditor);
					editorAttr = new EditorAttribute(editorType, editorType);
				}
				if (editorAttr != null)
					propertyAttributes.Add(editorAttr);

				if (Properties.BaseProperties != null) {
					Property baseProperty = Properties.BaseProperties.GetProperty(property.Name, true);
					if (baseProperty != null) {
						var defaultAttr = new DefaultValueAttribute(baseProperty.ObjectValue);
						propertyAttributes.Add(defaultAttr);
					}
				}

				// Create the property descriptor
				props.Add(new CustomPropertyDescriptor(propertyGrid.EditorControl, propertyObject,
					property.Name, Properties, propertyAttributes.ToArray()));
			}
			return new PropertyDescriptorCollection(props.ToArray());
		}


		public PropertyDescriptorCollection CollectEvents() {
			List<PropertyDescriptor> props  = new List<PropertyDescriptor>();

			if (eventObject == null)
				return new PropertyDescriptorCollection(props.ToArray());

			// Create the list of property descriptors
			foreach (Event evnt in Events.GetEvents()) {
				// Find the editor
				List<Attribute> eventAttributes = new List<Attribute>();
				eventAttributes.Add(new EditorAttribute(EventEditor, EventEditor));
				eventAttributes.Add(new DefaultValueAttribute(""));
				
				// Create the event descriptor
				props.Add(new CustomEventDescriptor(propertyGrid.EditorControl, eventObject,
					evnt.Name, Events, eventAttributes.ToArray()));
			}
			return new PropertyDescriptorCollection(props.ToArray());
		}


		/// <summary>gets the Properties.</summary>
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() {
			return ((ICustomTypeDescriptor) this).GetProperties(null);
		}

		/// <summary>Gets the property owner.</summary>
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) {
			return this;
		}
	}

}
