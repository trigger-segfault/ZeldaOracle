using System;
using System.Collections.Generic;
using System.Text;

using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Particles;
using ZeldaOracle.Common.Input;

using GameFramework.MyGame.Editor;

namespace GameFramework.MyGame.Editor.Properties {
/** <summary>
 * The base property class to extend properties from.
 * </summary> */
public class PropertyCircleArea :  PropertyStruct {

	//========== CONSTANTS ===========
	#region Constants

	#endregion
	//=========== MEMBERS ============
	#region Members

	/** <summary> The x property of the point. </summary> */
	protected PropertyVector2D propCenter;
	/** <summary> The y property of the point. </summary> */
	protected PropertyDouble propRadius;
	/** <summary> True if the point is a size. </summary> */
	protected PropertyBool propEdge;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default property. </summary> */
	public PropertyCircleArea()
		: base(false) {

		this.propCenter	= (PropertyVector2D)AddProperty(new PropertyVector2D("Center", false, Vector2F.Zero, Vector2F.Zero, action));
		this.propRadius	= (PropertyDouble)AddProperty(new PropertyDouble("Radius", 0.0, 0.0, new RangeF(0.0f, Double.PositiveInfinity), action));
		this.propEdge	= (PropertyBool)AddProperty(new PropertyBool("Edge Only", false, false, action));
	}

	/** <summary> Constructs the default property. </summary> */
	public PropertyCircleArea(string name, CircleArea value, CircleArea defaultValue, PropertyAction action = null)
		: base(name, false, value, defaultValue, action) {

		this.propCenter	= (PropertyVector2D)AddProperty(new PropertyVector2D("Center", false, value.Center, defaultValue.Center, action));
		this.propRadius	= (PropertyDouble)AddProperty(new PropertyDouble("Radius", value.Radius, defaultValue.Radius, new RangeF(0.0, Double.PositiveInfinity), action));
		this.propEdge	= (PropertyBool)AddProperty(new PropertyBool("Edge Only", value.EdgeOnly, defaultValue.EdgeOnly, action));
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties
	//--------------------------------
	#region Values

	/** <summary> Gets or sets the real value of the property. </summary> */
	public CircleArea Value {
		get { return (CircleArea)this.value; }
		set {
			this.value = value;
			text = value.ToString();
		}
	}
	/** <summary> Gets or sets the real default value of the property. </summary> */
	public CircleArea DefaultValue {
		get { return (CircleArea)this.value; }
		set {
			this.value = value;
			text = value.ToString();
		}
	}
	/** <summary> Gets or sets the text of the property. </summary> */
	public override string Text {
		set {}
	}

	#endregion
	//--------------------------------
	#endregion
	//=========== UPDATING ===========
	#region Updating

	/** <summary> Called every step to update the property. </summary> */
	public override void Update(double time, Point2I position) {
		base.Update(time, position);

		if (propCenter.IsChanged) {
			CircleArea newValue = (CircleArea)value;

			newValue.Center = propCenter.Value;
			value = newValue;
			if (action != null)
				action();
 		}
		if (propRadius.IsChanged) {
			CircleArea newValue = (CircleArea)value;

			newValue.Radius = propRadius.Value;
			value = newValue;
			if (action != null)
				action();
		}
		if (propEdge.IsChanged) {
			CircleArea newValue = (CircleArea)value;

			newValue.EdgeOnly = propEdge.Value;
			value = newValue;
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
		Cancel();
	}

	#endregion
}
} // end namespace
