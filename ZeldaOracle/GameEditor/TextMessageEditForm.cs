using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZeldaEditor {
	public partial class TextMessageEditForm : Form {


		public TextMessageEditForm() {
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
}
