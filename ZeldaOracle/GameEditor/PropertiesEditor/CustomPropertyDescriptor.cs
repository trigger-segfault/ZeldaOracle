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

namespace ZeldaEditor.PropertiesEditor {
	
	public class CustomPropertyDescriptor : PropertyDescriptor {
		private Properties				modifiedProperties;
		private string					propertyName;
		private PropertyDocumentation	documentation;
		private UITypeEditor			editor;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public CustomPropertyDescriptor(PropertyDocumentation documentation, UITypeEditor editor, string propertyName, Properties modifiedProperties) :
			base(documentation == null ? propertyName : documentation.ReadableName, null)
		{
			this.documentation		= documentation;
			this.editor				= editor;
			this.propertyName		= propertyName;
			this.modifiedProperties	= modifiedProperties;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Get the editor to use to edit this property.
		public override object GetEditor(Type editorBaseType) {
			if (editor != null && editorBaseType == typeof(UITypeEditor))
				return editor;
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
			Property property = Property;
			if (property.Type == ZeldaOracle.Common.Scripting.PropertyType.List)
				return null;
			return property.ObjectValue;
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
			if (property.Type == ZeldaOracle.Common.Scripting.PropertyType.String)
				modifiedProperties.Set(propertyName, (string) value);
			if (property.Type == ZeldaOracle.Common.Scripting.PropertyType.Integer)
				modifiedProperties.Set(propertyName, (int) value);
			if (property.Type == ZeldaOracle.Common.Scripting.PropertyType.Float)
				modifiedProperties.Set(propertyName, (float) value);
			if (property.Type == ZeldaOracle.Common.Scripting.PropertyType.Boolean)
				modifiedProperties.Set(propertyName, (bool) value);
		}


		//-----------------------------------------------------------------------------
		// Overridden properties
		//-----------------------------------------------------------------------------
			
		// Is the property read-only?
		public override bool IsReadOnly {
			get {
				if (documentation != null)
					return !documentation.IsEditable;
				return false;
			}
		}
			
		// Should the property be listed in the property grid?
		public override bool IsBrowsable {
			get {
				if (documentation != null)
					return !documentation.IsHidden;
				return true;
			}
		}

		public override Type ComponentType {
			get { return typeof(PropertiesContainer); }
		}

		// Get the category this property should be listed under.
		public override string Category {
			get {
				if (documentation != null)
					return documentation.Category;
				return "";
			}
		}

		// Get the description for the property.
		public override string Description {
			get {
				if (documentation != null)
					return documentation.Description;
				return "";
			}
		}

		// Get the display name for the property.
		public override string Name {
			get {
				if (documentation != null)
					return documentation.ReadableName;
				return propertyName;
			}
		}
			
		public override Type PropertyType {
			get {
				Property property = Property;
				if (property.Type == ZeldaOracle.Common.Scripting.PropertyType.String)
					return typeof(string);
				if (property.Type == ZeldaOracle.Common.Scripting.PropertyType.Integer)
					return typeof(int);
				if (property.Type == ZeldaOracle.Common.Scripting.PropertyType.Float)
					return typeof(float);
				if (property.Type == ZeldaOracle.Common.Scripting.PropertyType.Boolean)
					return typeof(bool);
				return null;
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Property Property {
			get {
				return modifiedProperties.GetProperty(propertyName, true);
				//return property;
			}
		}

		public PropertyDocumentation Documentation {
			get { return documentation; }
		}
	}
}
