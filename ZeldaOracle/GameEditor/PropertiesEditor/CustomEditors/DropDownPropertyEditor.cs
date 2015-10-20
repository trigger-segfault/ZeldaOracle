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
	public abstract class DropDownPropertyEditor : CustomPropertyEditor {

		private ListBox listBox;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public DropDownPropertyEditor() {
			editStyle = UITypeEditorEditStyle.DropDown;
		}

		//-----------------------------------------------------------------------------
		// Virtual Methods
		//-----------------------------------------------------------------------------

		public virtual void CreateList(ListBox listBox, object value) {

		}

		public virtual object OnItemSelected(ListBox listBox, int index, object value) {
			return value;
		}
		
		public virtual object OnCancel(ListBox listBox, object value) {
			return value;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------
		
		public override object EditProperty(object value) {
			listBox = new ListBox();
			listBox.BorderStyle = BorderStyle.None;
			listBox.SelectedValueChanged += new EventHandler(this.ValueChanged);

			// Create the list box.
			CreateList(listBox, value);
			if (listBox.Items.Count == 0)
				return OnCancel(listBox, value);

			// Show the combo box.
			editorService.DropDownControl(listBox);
			if (listBox.SelectedIndex >= 0)
				return OnItemSelected(listBox, listBox.SelectedIndex, value);
			return OnCancel(listBox, value);
		}

        private void ValueChanged(object sender, EventArgs e) {
			if (editorService != null) {
                editorService.CloseDropDown();
            }
        }
	}
}
