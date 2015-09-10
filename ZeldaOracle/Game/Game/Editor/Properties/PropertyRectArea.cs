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
public class PropertyRectArea :  PropertyStruct {

	//========== CONSTANTS ===========
	#region Constants

	#endregion
	//=========== MEMBERS ============
	#region Members

	/** <summary> The x property of the point. </summary> */
	protected PropertyVector2D propPoint;
	/** <summary> The x property of the point. </summary> */
	protected PropertyVector2D propSize;
	/** <summary> True if the point is a size. </summary> */
	protected PropertyBool propEdge;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default property. </summary> */
	public PropertyRectArea()
		: base(false) {

		this.propPoint	= (PropertyVector2D)AddProperty(new PropertyVector2D("Point", false, Vector2F.Zero, Vector2F.Zero, action));
		this.propSize	= (PropertyVector2D)AddProperty(new PropertyVector2D("Size", true, Vector2F.Zero, Vector2F.Zero, action));
		this.propEdge	= (PropertyBool)AddProperty(new PropertyBool("Edge Only", false, false, action));
	}

	/** <summary> Constructs the default property. </summary> */
	public PropertyRectArea(string name, RectArea value, RectArea defaultValue, PropertyAction action = null)
		: base(name, false, value, defaultValue, action) {

		this.propPoint	= (PropertyVector2D)AddProperty(new PropertyVector2D("Point", false, value.Point, defaultValue.Point, action));
		this.propSize	= (PropertyVector2D)AddProperty(new PropertyVector2D("Size", true, value.Size, defaultValue.Size, action));
		this.propEdge	= (PropertyBool)AddProperty(new PropertyBool("Edge Only", value.EdgeOnly, defaultValue.EdgeOnly, action));
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties
	//--------------------------------
	#region Values

	/** <summary> Gets or sets the real value of the property. </summary> */
	public RectArea Value {
		get { return (RectArea)this.value; }
		set {
			this.value = value;
			text = value.ToString();
		}
	}
	/** <summary> Gets or sets the real default value of the property. </summary> */
	public RectArea DefaultValue {
		get { return (RectArea)this.value; }
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
			RectArea newValue = (RectArea)value;

			newValue.Point = propPoint.Value;
			value = newValue;
			if (action != null)
				action();
		}
		if (propSize.IsChanged) {
			RectArea newValue = (RectArea)value;

			newValue.Size = propSize.Value;
			value = newValue;
			if (action != null)
				action();
		}
		if (propEdge.IsChanged) {
			RectArea newValue = (RectArea)value;

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
