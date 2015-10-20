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
	
	public class TextMessagePropertyEditor : FormPropertyEditor {
		public override Form CreateForm(object value) {
			TextMessageEditorForm form = new TextMessageEditorForm();
			form.MessageText = (string) value;
			return form;
		}

		public override object OnResultOkay(Form form, object value) {
			return (form as TextMessageEditorForm).MessageText;
		}
	}
}
