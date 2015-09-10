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
public class PropertyRangeD :  PropertyStruct {

	//========== CONSTANTS ===========
	#region Constants

	#endregion
	//=========== MEMBERS ============
	#region Members

	/** <summary> The min property of the range. </summary> */
	protected PropertyDouble propMin;
	/** <summary> The max property of the range. </summary> */
	protected PropertyDouble propMax;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default property. </summary> */
	public PropertyRangeD()
		: base(true) {

		this.propMin	= (PropertyDouble)AddProperty(new PropertyDouble("Min", 0.0, 0.0, RangeF.Full));
		this.propMax	= (PropertyDouble)AddProperty(new PropertyDouble("Max", 0.0, 0.0, RangeF.Full));
	}

	/** <summary> Constructs the default property. </summary> */
	public PropertyRangeD(string name, RangeF value, RangeF defaultValue, PropertyAction action = null)
		: base(name, true, value, defaultValue, action) {

		this.propMin	= (PropertyDouble)AddProperty(new PropertyDouble("Min", value.Min, defaultValue.Min, RangeF.Full));
		this.propMax	= (PropertyDouble)AddProperty(new PropertyDouble("Max", value.Max, defaultValue.Max, RangeF.Full));
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties
	//--------------------------------
	#region Values

	/** <summary> Gets or sets the real value of the property. </summary> */
	public RangeF Value {
		get { return (RangeF)this.value; }
		set {
			this.value = value;
			text = value.ToString();
		}
	}
	/** <summary> Gets or sets the real default value of the property. </summary> */
	public RangeF DefaultValue {
		get { return (RangeF)this.value; }
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
				propMin.Value = newValue.X;
				propMax.Value = newValue.Y;
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

		if (propMin.IsChanged) {
			RangeF newValue = (RangeF)value;
			newValue.Min = propMin.Value;
			value = newValue;
			text = newValue.ToString();
			if (action != null)
				action();
		}
		if (propMax.IsChanged) {
			RangeF newValue = (RangeF)value;
			newValue.Max = propMax.Value;
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
			RangeF newValue2 = new RangeF(newValue.X, newValue.Y);
			value = newValue2;
			base.Finish();
			text = value.ToString();
			propMin.Value = newValue.X;
			propMax.Value = newValue.Y;
		}
		else {
			Cancel();
		}
	}

	#endregion
}
} // end namespace
