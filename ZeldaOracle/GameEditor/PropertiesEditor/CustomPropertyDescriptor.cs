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
		private Property				property;
		private PropertyDocumentation	documentation;
		private UITypeEditor			editor;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public CustomPropertyDescriptor(PropertyDocumentation documentation, UITypeEditor editor, Property property, Properties modifiedProperties) :
			base(documentation == null ? property.Name : documentation.ReadableName, null)
		{
			this.documentation		= documentation;
			this.editor				= editor;
			this.property			= property;
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
			return modifiedProperties.IsPropertyModified(property.Name);
		}

		// Is the value allowed to be reset?
		public override bool CanResetValue(object component) {
			return true;
		}
			
		// Get the displayed value of the property.
		public override object GetValue(object component) {
			if (property.Type == ZeldaOracle.Common.Scripting.PropertyType.List)
				return null;
			return property.ObjectValue;
		}

		// Reset the value to the default.
		public override void ResetValue(object component) {
			// TODO: override ResetValue
			Property rootProperty = property.GetRootProperty();
			if (property != rootProperty)
				property.SetValue(rootProperty);
		}

		// Set the displayed value of the property.
		public override void SetValue(object component, object value) {
			bool isModified = modifiedProperties.IsPropertyModified(property.Name);

			// If the property hasn't been modified, then create a new property for it.
			if (!isModified) {
				property = new ZeldaOracle.Common.Scripting.Property(property);
				modifiedProperties.Add(property);
			}

			// Set the appropriate value.
			if (property.Type == ZeldaOracle.Common.Scripting.PropertyType.String)
				property.StringValue = (string) value;
			if (property.Type == ZeldaOracle.Common.Scripting.PropertyType.Integer)
				property.IntValue = (int) value;
			if (property.Type == ZeldaOracle.Common.Scripting.PropertyType.Float)
				property.FloatValue = (float) value;
			if (property.Type == ZeldaOracle.Common.Scripting.PropertyType.Boolean)
				property.BoolValue = (bool) value;
				
			// Check if base and modified are the same. If so, remove the modified one.
			if (isModified && !modifiedProperties.IsPropertyModified(property.Name)) {
				modifiedProperties.Remove(property.Name);
				property = modifiedProperties.GetProperty(property.Name, true);
			}
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
				return property.Name;
			}
		}
			
		public override Type PropertyType {
			get {
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
			get { return property; }
		}

		public PropertyDocumentation Documentation {
			get { return documentation; }
		}
	}
}
