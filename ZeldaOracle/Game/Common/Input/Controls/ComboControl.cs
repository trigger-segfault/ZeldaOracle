using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Input.Controls {
	/**<summary>A control class for holding down multiple inputs.</summary>*/
	public class ComboControl : ControlHandler {

		//=========== MEMBERS ============
		#region Members

		/**<summary>The list of controls to be held down.</summary>*/
		public ControlHandler[] Controls;

		#endregion
		//========= CONSTRUCTORS =========
		#region Constructors

		/**<summary>Constructs the default control combo.</summary>*/
		public ComboControl() {
			this.Controls		= new ControlHandler[0];
		}
		/**<summary>Constructs a control combo with the specified controls.</summary>*/
		public ComboControl(ControlHandler[] controls) {
			this.Controls		= controls;
		}

		#endregion
		//========== PROPERTIES ==========
		#region Properties

		/**<summary>Gets the name of the control.</summary>*/
		public override string Name {
			get {
				string name = "";
				for (int i = 0; i < Controls.Length; i++) {
					name += (i > 0 ? "+" : "") + Controls[i].Name;
				}
				return name;
			}
		}

		#endregion
		//============ EVENTS ============
		#region Events

		/**<summary>Returns true if the control was pressed.</summary>*/
		public override bool Pressed() {
			bool pressed = false;
			for (int i = 0; i < Controls.Length; i++) {
				if (Controls[i].Pressed())
					pressed = true;
				else if (Controls[i].Up())
					return false;
			}
			return pressed;
		}
		/**<summary>Returns true if the control was released.</summary>*/
		public override bool Released() {
			bool released = false;
			for (int i = 0; i < Controls.Length; i++) {
				if (Controls[i].Released())
					released = true;
				else if (Controls[i].Up())
					return false;
			}
			return released;
		}
		/**<summary>Returns true if the control is down.</summary>*/
		public override bool Down() {
			for (int i = 0; i < Controls.Length; i++) {
				if (Controls[i].Up())
					return false;
			}
			return true;
		}
		/**<summary>Returns true if the control is up.</summary>*/
		public override bool Up() {
			for (int i = 0; i < Controls.Length; i++) {
				if (Controls[i].Up())
					return true;
			}
			return false;
		}

		#endregion
	}
} // end namespace
