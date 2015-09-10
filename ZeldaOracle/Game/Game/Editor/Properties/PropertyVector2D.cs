using System;
using System.Text;

using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;

using GameFramework.MyGame.Editor;
using System.Collections.Generic;

namespace GameFramework.MyGame.Editor.Properties {
/** <summary>
 * The base property class to extend properties from.
 * </summary> */
public class PropertyVector2D :  PropertyStruct {

	//========== CONSTANTS ===========
	#region Constants

	#endregion
	//=========== MEMBERS ============
	#region Members

	/** <summary> The x property of the point. </summary> */
	protected PropertyDouble propX;
	/** <summary> The y property of the point. </summary> */
	protected PropertyDouble propY;
	/** <summary> True if the point is a size. </summary> */
	protected bool size;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default property. </summary> */
	public PropertyVector2D()
		: base(true) {
		this.size		= false;

		this.propX		= (PropertyDouble)AddProperty(new PropertyDouble(size ? "Width" : "X", 0.0, 0.0, RangeF.Full));
		this.propY		= (PropertyDouble)AddProperty(new PropertyDouble(size ? "Height" : "Y", 0.0, 0.0, RangeF.Full));
	}

	/** <summary> Constructs the default property. </summary> */
	public PropertyVector2D(string name, bool size, Vector2F value, Vector2F defaultValue, PropertyAction action = null)
		: base(name, true, value, defaultValue, action) {
		this.size		= size;

		this.propX		= (PropertyDouble)AddProperty(new PropertyDouble(size ? "Width" : "X", value.X, defaultValue.X, RangeF.Full, action));
		this.propY		= (PropertyDouble)AddProperty(new PropertyDouble(size ? "Height" : "Y", value.Y, defaultValue.Y, RangeF.Full, action));
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties
	//--------------------------------
	#region Values

	/** <summary> Gets or sets the real value of the property. </summary> */
	public Vector2F Value {
		get { return (Vector2F)this.value; }
		set {
			this.value = value;
			text = value.ToString();
		}
	}
	/** <summary> Gets or sets the real default value of the property. </summary> */
	public Vector2F DefaultValue {
		get { return (Vector2F)this.value; }
		set {
			this.value = value;
			text = value.ToString();
		}
	}
	/** <summary> Gets or sets the text of the property. </summary> */
	public override string Text {
		set {
			bool error = false;

			Vector2F newValue = Vector2F.Zero;
			try {
				newValue = Vector2F.Parse(editingText);
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
				this.value = newValue;
				base.Finish();
				text = value.ToString();
				propX.Value = newValue.X;
				propY.Value = newValue.Y;
			}
		}
	}

	#endregion
	//--------------------------------
	#endregion
	//=========== UPDATING ===========
	#region Updating

	/** <summary> Called every step to update the property. </summary> */
	public override void Update(double time, Point2I position) {
		base.Update(time, position);

		if (propX.IsChanged) {

			Vector2F newValue = (Vector2F)value;

			newValue.X = propX.Value;
			value = newValue;
			text = newValue.ToString();
			if (action != null)
				action();
		}
		if (propY.IsChanged) {

			Vector2F newValue = (Vector2F)value;

			newValue.Y = propY.Value;
			value = newValue;
			text = newValue.ToString();
			if (action != null)
				action();
		}
	}

	#endregion
	//=========== DRAWING ============
	#region Drawing


	#endregion
	//========== MANAGEMENT ==========
	#region Management

	/** <summary> Sets the value of the property after finished editing. </summary> */
	public override void Finish() {
		bool error = false;

		Vector2F newValue = Vector2F.Zero;
		try {
			newValue = Vector2F.Parse(editingText);
		} catch (FormatException e) {
			error = true;
		} catch (ArgumentNullException e) {
			error = true;
		} catch (OverflowException e) {
			error = true;
		}

		if (!error) {
			value = newValue;
			base.Finish();
			text = value.ToString();
			propX.Value = newValue.X;
			propY.Value = newValue.Y;
		}
		else {
			Cancel();
		}
	}

	#endregion
}
} // end namespace
