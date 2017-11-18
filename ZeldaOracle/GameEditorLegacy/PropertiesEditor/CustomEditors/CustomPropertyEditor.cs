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
	public abstract class CustomPropertyEditor : UITypeEditor {

		private ZeldaPropertyGrid				propertyGrid;
        protected IWindowsFormsEditorService	editorService;
		protected CustomPropertyDescriptor		propertyDescriptor;
		protected Property						property;
		protected UITypeEditorEditStyle			editStyle;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public CustomPropertyEditor() {
			propertyGrid		= null;
			editorService		= null;
			property			= null;
			propertyDescriptor	= null;
			editStyle			= UITypeEditorEditStyle.None;
		}
		
		public void Initialize(ZeldaPropertyGrid propertyGrid) {
			this.propertyGrid = propertyGrid;
			Initialize();
		}


		//-----------------------------------------------------------------------------
		// Virtual Methods
		//-----------------------------------------------------------------------------

		public virtual void Initialize() {}

		public abstract object EditProperty(object value);


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return editStyle;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;

			if (editorService != null) {
				propertyDescriptor = (CustomPropertyDescriptor) context.PropertyDescriptor;
				property = propertyDescriptor.Property;
				value = EditProperty(value);
			}

			return value;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public ZeldaPropertyGrid PropertyGrid {
			get { return propertyGrid; }
		}

		public EditorControl EditorControl {
			get { return propertyGrid.EditorControl; }
		}
	}
}
