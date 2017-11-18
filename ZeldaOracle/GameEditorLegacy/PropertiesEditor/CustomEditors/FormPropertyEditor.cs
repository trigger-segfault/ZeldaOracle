using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ZeldaOracle.Game.Items.Rewards;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaEditor.Control;

namespace ZeldaEditor.PropertiesEditor.CustomEditors {
	

	public abstract class FormPropertyEditor : CustomPropertyEditor {
		private Form form;

		public FormPropertyEditor() {
			editStyle = UITypeEditorEditStyle.Modal;
			form = null;
		}
		
		public override object EditProperty(object value) {
			form = CreateForm(value);
			DialogResult result = editorService.ShowDialog(form);
			if (result == DialogResult.OK)
				return OnResultOkay(form, value);
			return OnResultCancel(form, value);
		}

		public abstract Form CreateForm(object value);

		public abstract object OnResultOkay(Form form, object value);

		public virtual object OnResultCancel(Form form, object value) {
			return value;
		}
	}

}
