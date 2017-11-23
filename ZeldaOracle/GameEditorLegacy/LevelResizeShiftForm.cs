using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Common.Content;

namespace ZeldaEditor {
	public partial class LevelResizeShiftForm : Form {

		private static bool shiftMode;

		private static Point2I levelSize;
		private static Point2I levelShift;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public LevelResizeShiftForm() {
			InitializeComponent();
			
			buttonOK.DialogResult		= DialogResult.OK;
			buttonCancel.DialogResult	= DialogResult.Cancel;
		}


		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------

		private void numericLevelWidth_ValueChanged(object sender, EventArgs e) {
			if (shiftMode)
				levelShift.X = (int)numericLevelWidth.Value;
			else
				levelSize.X = (int)numericLevelWidth.Value;
		}

		private void numericLevelHeight_ValueChanged(object sender, EventArgs e) {
			if (shiftMode)
				levelShift.Y = (int)numericLevelHeight.Value;
			else
				levelSize.Y = (int)numericLevelHeight.Value;
		}


		//-----------------------------------------------------------------------------
		// Show
		//-----------------------------------------------------------------------------

		public static DialogResult ShowResize(IWin32Window owner, Point2I levelSize) {
			using (LevelResizeShiftForm form = new LevelResizeShiftForm()) {
				LevelResizeShiftForm.levelSize	= levelSize;
				LevelResizeShiftForm.levelShift	= Point2I.Zero;
				LevelResizeShiftForm.shiftMode	= false;

				form.Text						= "Resize Level";
				form.labelWidth.Text			= "Width:";
				form.labelHeight.Text			= "Height:";
				form.numericLevelWidth.Minimum	= 1;
				form.numericLevelHeight.Minimum	= 1;
				form.numericLevelWidth.Maximum	= 100;
				form.numericLevelHeight.Maximum	= 100;
				form.numericLevelWidth.Value	= levelSize.X;
				form.numericLevelHeight.Value	= levelSize.Y;
				return form.ShowDialog(owner);
			}
		}

		public static DialogResult ShowShift(IWin32Window owner, Point2I levelSize) {
			using (LevelResizeShiftForm form = new LevelResizeShiftForm()) {
				LevelResizeShiftForm.levelSize	= levelSize;
				LevelResizeShiftForm.levelShift	= Point2I.Zero;
				LevelResizeShiftForm.shiftMode	= true;

				form.Text						= "Shift Level";
				form.labelWidth.Text			= "X:";
				form.labelHeight.Text			= "Y:";
				form.numericLevelWidth.Minimum	= -levelSize.X;
				form.numericLevelHeight.Minimum	= -levelSize.Y;
				form.numericLevelWidth.Maximum	= levelSize.X;
				form.numericLevelWidth.Maximum	= levelSize.Y;
				form.numericLevelWidth.Value	= 0;
				form.numericLevelHeight.Value	= 0;
				return form.ShowDialog(owner);
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public static int LevelWidth {
			get { return levelSize.X; }
		}
		
		public static int LevelHeight {
			get { return levelSize.Y; }
		}
		
		public static Point2I LevelSize {
			get { return levelSize; }
		}

		public static int LevelShiftX {
			get { return levelShift.X; }
		}

		public static int LevelShiftY {
			get { return levelShift.Y; }
		}

		public static Point2I LevelShift {
			get { return levelShift; }
		}
	}
}
