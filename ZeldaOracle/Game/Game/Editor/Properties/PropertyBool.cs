using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;

namespace GameFramework.MyGame.Editor.Properties {
/** <summary>
 * The base property class to extend properties from.
 * </summary> */
public class PropertyBool : Property {

	//=========== MEMBERS ============
	#region Members

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default property. </summary> */
	public PropertyBool()
		: base() {
	}
	/** <summary> Constructs the default property. </summary> */
	public PropertyBool(string name, bool value, bool defaultValue, PropertyAction action = null)
		: base(name, value, defaultValue, action) {
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties
	//--------------------------------
	#region Values

	/** <summary> Gets or sets the real value of the property. </summary> */
	public bool Value {
		get { return (bool)this.value; }
		set {
			this.value = value;
			text = value.ToString();
		}
	}
	/** <summary> Gets or sets the real default value of the property. </summary> */
	public bool DefaultValue {
		get { return (bool)this.value; }
		set {
			this.value = value;
			text = value.ToString();
		}
	}
	/** <summary> Gets or sets the value of the property. </summary> */
	public override object ObjValue {
		set {
			this.value = value;
			text = value.ToString();
		}
	}
	/** <summary> Gets or sets the text of the property. </summary> */
	public override string Text {
		set {
			bool error = false;
			bool newValue = false;
			try {
				newValue = Boolean.Parse(value);
			}
			catch (FormatException e) {
				error = true;
			}
			catch (ArgumentNullException e) {
				error = true;
			}
			catch (OverflowException e) {
				error = true;
			}

			if (!error) {
				text = value;
				this.value = newValue;
			}
		}
	}

	#endregion
	//--------------------------------
	#region Dimensions

	/** <summary> Gets the number of rows in the property. </summary> */
	public override int Rows {
		get { return 1; }
	}

	#endregion
	//--------------------------------
	#endregion
	//=========== UPDATING ===========
	#region Updating

	/** <summary> Called every step to update the property. </summary> */
	public override void Update(double time, Point2I position) {
		base.Update(time, position);
	}

	#endregion
	//=========== DRAWING ============
	#region Drawing

	/** <summary> Called every step to draw the property. </summary> */
	public override void Draw(Graphics2D g, Point2I position, int indentLevel = 0) {
		base.Draw(g, position, indentLevel);
	}

	#endregion
	//========== MANAGEMENT ==========
	#region Management

	/** <summary> Sets the value of the property after finished editing. </summary> */
	public override void Finish() {
		bool error = false;
		bool newValue = false;
		try {
			newValue = Boolean.Parse(editingText);
		}
		catch (FormatException e) {
			error = true;
		}
		catch (ArgumentNullException e) {
			error = true;
		}
		catch (OverflowException e) {
			error = true;
		}

		if (!error) {
			value = newValue;
			base.Finish();
			text = value.ToString();
		}
		else {
			Cancel();
		}
	}
	/** <summary> Cancels the editing of the property and does not set the value. </summary> */
	public override void Cancel() {

		base.Cancel();
	}

	#endregion
}
} // end namespace
