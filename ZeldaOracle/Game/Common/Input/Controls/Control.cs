using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Input.Controls {
/** <summary>
 * An abstract control class.
 * </summary> */
public abstract class Control {

	//========== PROPERTIES ==========
	#region Properties

	/** <summary> Gets the name of the control. </summary> */
	public abstract string Name {
		get;
	}

	#endregion
	//============ EVENTS ============
	#region Events

	/** <summary> Returns true if the control was pressed. </summary> */
	public abstract bool Pressed();
	/** <summary> Returns true if the control was released. </summary> */
	public abstract bool Released();
	/** <summary> Returns true if the control is down. </summary> */
	public abstract bool Down();
	/** <summary> Returns true if the control is up. </summary> */
	public abstract bool Up();

	#endregion
}
} // end namespace
