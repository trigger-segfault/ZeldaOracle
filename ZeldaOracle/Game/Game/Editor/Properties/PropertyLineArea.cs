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
public class PropertyLineArea :  PropertyStruct {

	//========== CONSTANTS ===========
	#region Constants

	#endregion
	//=========== MEMBERS ============
	#region Members

	/** <summary> The x property of the point. </summary> */
	protected PropertyVector2D propEnd1;
	/** <summary> The x property of the point. </summary> */
	protected PropertyVector2D propEnd2;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default property. </summary> */
	public PropertyLineArea()
		: base(false) {

		this.propEnd1	= (PropertyVector2D)AddProperty(new PropertyVector2D("End 1", false, Vector2F.Zero, Vector2F.Zero, action));
		this.propEnd2	= (PropertyVector2D)AddProperty(new PropertyVector2D("End 2", false, Vector2F.Zero, Vector2F.Zero, action));
	}

	/** <summary> Constructs the default property. </summary> */
	public PropertyLineArea(string name, LineArea value, LineArea defaultValue, PropertyAction action = null)
		: base(name, false, value, defaultValue, action) {

		this.propEnd1	= (PropertyVector2D)AddProperty(new PropertyVector2D("End 1", false, value.End1, defaultValue.End1, action));
		this.propEnd2	= (PropertyVector2D)AddProperty(new PropertyVector2D("End 2", false, value.End2, defaultValue.End2, action));
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties
	//--------------------------------
	#region Values

	/** <summary> Gets or sets the real value of the property. </summary> */
	public LineArea Value {
		get { return (LineArea)this.value; }
		set {
			this.value = value;
			text = value.ToString();
		}
	}
	/** <summary> Gets or sets the real default value of the property. </summary> */
	public LineArea DefaultValue {
		get { return (LineArea)this.value; }
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

		if (propEnd1.IsChanged) {
			LineArea newValue = (LineArea)value;

			newValue.End1 = propEnd1.Value;
			value = newValue;
			if (action != null)
				action();
		}
		if (propEnd2.IsChanged) {
			LineArea newValue = (LineArea)value;

			newValue.End2 = propEnd2.Value;
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
