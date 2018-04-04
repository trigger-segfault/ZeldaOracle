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
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Content;
using ZeldaEditor.Control;
using Xceed.Wpf.Toolkit.PropertyGrid;
using ZeldaOracle.Common;

namespace ZeldaEditor.PropertiesEditor {
	
	public class CustomPropertyDescriptor : PropertyDescriptor {
		private EditorControl           editorControl;
		private Properties				modifiedProperties;
		private string					propertyName;
		private IPropertyObject         propertyObject;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public CustomPropertyDescriptor(EditorControl editorControl, IPropertyObject propertyObject, string propertyName, Properties modifiedProperties, Attribute[] attributes) :
			base(propertyName, attributes)
		{
			this.editorControl      = editorControl;
			this.propertyObject     = propertyObject;
			this.propertyName		= propertyName;
			this.modifiedProperties	= modifiedProperties;
		}

		public CustomPropertyDescriptor(EditorControl editorControl, IPropertyObject propertyObject, string propertyName, Properties modifiedProperties) :
			this(editorControl, propertyObject, propertyName, modifiedProperties, null) {
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Get the editor to use to edit this property.
		public override object GetEditor(Type editorBaseType) {
			return base.GetEditor(editorBaseType);
		}

		// Returns true if the property is modified.
		public override bool ShouldSerializeValue(object component) {
			return modifiedProperties.IsPropertyModified(propertyName);
		}

		// Is the value allowed to be reset?
		public override bool CanResetValue(object component) {
			return true;
		}
			
		// Get the displayed value of the property.
		public override object GetValue(object component) {
			return Property.ObjectValue;
		}

		// Reset the value to the default.
		public override void ResetValue(object component) {
			Property baseProperty = modifiedProperties.BaseProperties.GetProperty(propertyName, true);
			if (baseProperty != null)
				modifiedProperties.Set(propertyName, baseProperty);
		}

		// Set the displayed value of the property.
		// This method will only be called if the new value is different from the old one.
		public override void SetValue(object component, object value) {
			// Set the appropriate value.
			Property property = Property;
			modifiedProperties.SetObject(propertyName, value);
		}


		//-----------------------------------------------------------------------------
		// Overridden properties
		//-----------------------------------------------------------------------------
			
		// Is the property read-only?
		public override bool IsReadOnly {
			get { return Property.IsReadOnly; }
		}
			
		// Should the property be listed in the property grid?
		public override bool IsBrowsable {
			get { return Property.IsBrowsable; }
		}

		public override Type ComponentType {
			get { return propertyObject.GetType(); }
		}

		// Get the category this property should be listed under.
		public override string Category {
			get { return Property.Category; }
		}

		// Get the description for the property.
		public override string Description {
			get { return Property.Description; }
		}

		// Get the display name for the property.
		public override string DisplayName {
			get { return Property.FinalReadableName; }
		}

		public override Type PropertyType {
			get { return Property.FullType; }
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Property Property {
			get { return modifiedProperties.GetProperty(propertyName, true); }
		}

		public PropertyDocumentation Documentation {
			get { return Property.Documentation; }
		}

		public IPropertyObject PropertyObject {
			get { return propertyObject; }
		}

		public EditorControl EditorControl {
			get { return editorControl; }
		}
	}
}
