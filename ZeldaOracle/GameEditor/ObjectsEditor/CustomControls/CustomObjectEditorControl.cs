using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using ZeldaOracle.Common.Scripting;
using ZeldaEditor.Control;

namespace ZeldaEditor.ObjectsEditor.CustomControls {

	public class CustomObjectEditorControl : UserControl {

		private int column1X;
		private int column2X;
		private int rowY;
		private List<CustomControl> customControls;
		private ObjectEditor objectEditor;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public CustomObjectEditorControl() {
			this.SuspendLayout();
			this.Name			= "CustomObjectEditorControl";
			this.AutoSizeMode	= AutoSizeMode.GrowAndShrink;
			this.AutoSize		= true;

			rowY		= 0;//20;
			column1X	= 0;//20;
			column2X	= 100;

			customControls = new List<CustomControl>();
		}

		public void Initialize(ObjectEditor objectEditor) {
			this.objectEditor = objectEditor;
			this.ResumeLayout(false);
			this.PerformLayout();
			Initialize();
		}


		//-----------------------------------------------------------------------------
		// Virtual Methods
		//-----------------------------------------------------------------------------
		
		public virtual void Initialize() {}

		public virtual void SetupObject(IPropertyObject obj) {}

		public virtual void ApplyChanges(IPropertyObject obj) {}


		//-----------------------------------------------------------------------------
		// Control Customization
		//-----------------------------------------------------------------------------
		
		protected CheckBox AddCheckBox(string text) {
			CheckBox checkBox	= new CheckBox();
			checkBox.AutoSize	= true;
			checkBox.Location	= Column1Point;
			checkBox.Name		= "checkBox1";
			checkBox.Size		= new System.Drawing.Size(80, 17);
			checkBox.TabIndex	= 0;
			checkBox.Text		= text;
			checkBox.UseVisualStyleBackColor = true;
			
			AddCustomControl(new CustomControl() {
				Name	= "",
				Control	= checkBox,
				Label	= null,
			});
			return checkBox;
		}
		
		protected ComboBox AddComboBox(string text, params object[] items) {
			Label label = new Label();
			label.Location	= Column1Point;
			label.AutoSize	= true;
			label.Text		= text;

			ComboBox comboBox	= new ComboBox();
			comboBox.AutoSize	= true;
			comboBox.Location	= Column2Point;
			comboBox.Name		= "checkBox1";
			comboBox.Size		= new System.Drawing.Size(80, 17);
			comboBox.TabIndex	= 0;
			comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			for (int i = 0; i < items.Length; i++)
				comboBox.Items.Add(items[i]);
			if (items.Length > 0)
				comboBox.SelectedIndex = 0;

			AddCustomControl(new CustomControl() {
				Name	= "",
				Control	= comboBox,
				Label	= label,
			});
			return comboBox;
		}

		private void AddCustomControl(CustomControl customControl) {
			customControls.Add(customControl);
			if (customControl.Label != null)
				Controls.Add(customControl.Label);
			if (customControl.Control != null)
				Controls.Add(customControl.Control);
			rowY += 24;
		}
		

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		private Point Column1Point {
			get { return new Point(column1X, rowY); }
		}
		
		private Point Column2Point {
			get { return new Point(column2X, rowY); }
		}
		
		public ObjectEditor ObjectEditor {
			get { return objectEditor; }
		}
		
		public EditorControl EditorControl {
			get { return objectEditor.EditorControl; }
		}
	}

	internal class CustomControl {
		public System.Windows.Forms.Control Control { get; set; }
		public Label Label { get; set; }
		public string Name { get; set; }
	}
}
