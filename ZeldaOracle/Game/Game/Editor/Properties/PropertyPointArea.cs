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
public class PropertyPointArea :  PropertyStruct {

	//========== CONSTANTS ===========
	#region Constants

	#endregion
	//=========== MEMBERS ============
	#region Members

	/** <summary> The x property of the point. </summary> */
	protected PropertyVector2D propPoint;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default property. </summary> */
	public PropertyPointArea()
		: base(false) {

		this.propPoint	= (PropertyVector2D)AddProperty(new PropertyVector2D("Point", false, Vector2F.Zero, Vector2F.Zero, action));
	}

	/** <summary> Constructs the default property. </summary> */
	public PropertyPointArea(string name, PointArea value, PointArea defaultValue, PropertyAction action = null)
		: base(name, false, value, defaultValue, action) {

		this.propPoint	= (PropertyVector2D)AddProperty(new PropertyVector2D("Point", false, value.Point, defaultValue.Point, action));
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties
	//--------------------------------
	#region Values

	/** <summary> Gets or sets the real value of the property. </summary> */
	public PointArea Value {
		get { return (PointArea)this.value; }
		set {
			this.value = value;
			text = value.ToString();
		}
	}
	/** <summary> Gets or sets the real default value of the property. </summary> */
	public PointArea DefaultValue {
		get { return (PointArea)this.value; }
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

		if (propPoint.IsChanged) {
			PointArea newValue = (PointArea)value;

			newValue.Point = propPoint.Value;
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
