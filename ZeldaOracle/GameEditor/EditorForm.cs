using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsGraphicsDevice;
//using ZeldaOracle.Common.Content;

namespace ZeldaEditor {
	public partial class EditorForm : Form {

		public EditorForm() {
			InitializeComponent();
			
		}

		private void myXnaControl1_MouseDown(object sender, MouseEventArgs e) {
			Console.WriteLine("DOWN");
		}
	}
}
