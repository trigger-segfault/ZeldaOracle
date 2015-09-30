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

namespace ZeldaEditor.PropertiesEditor.CustomEditors {
	public partial class TextMessageEditorForm : Form {


		public TextMessageEditorForm() {
			InitializeComponent();
			
			buttonOkay.DialogResult		= DialogResult.OK;
			buttonCancel.DialogResult	= DialogResult.Cancel;
		}

		public string MessageText {
			get {
				return textBox.Text.Replace("\r\n", "<n>").Replace("\n", "<n>");
			}
			set {
				textBox.Text = value.Replace("<n>", "\r\n");
			}
		}

	}
	
	
	class TextMessagePropertyEditor : UITypeEditor {
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}

		public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value) {
			IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			if (svc != null) {
				using (TextMessageEditorForm form = new TextMessageEditorForm()) {
					form.MessageText = (string) value;
					if (svc.ShowDialog(form) == DialogResult.OK)
						value = form.MessageText;
				}
			}
			return value;
		}
	}
}
