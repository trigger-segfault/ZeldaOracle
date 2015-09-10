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
public class PropertyColor :  PropertyStruct {

	//========== CONSTANTS ===========
	#region Constants

	#endregion
	//=========== MEMBERS ============
	#region Members

	/** <summary> The red property of the point. </summary> */
	protected PropertyInt propR;
	/** <summary> The green property of the point. </summary> */
	protected PropertyInt propG;
	/** <summary> The blue property of the point. </summary> */
	protected PropertyInt propB;
	/** <summary> The alpha property of the point. </summary> */
	protected PropertyInt propA;
	/** <summary> True if alpha is used. </summary> */
	protected bool alpha;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default property. </summary> */
	public PropertyColor()
		: base(true) {
		this.alpha		= false;

		this.propR		= (PropertyInt)AddProperty(new PropertyInt("Red", 255, 255, new RangeI(0, 255), action));
		this.propG		= (PropertyInt)AddProperty(new PropertyInt("Green", 255, 255, new RangeI(0, 255), action));
		this.propB		= (PropertyInt)AddProperty(new PropertyInt("Blue", 255, 255, new RangeI(0, 255), action));
		if (this.alpha)
			this.propA	= (PropertyInt)AddProperty(new PropertyInt("Alpha", 255, 255, new RangeI(0, 255), action));
		else
			this.propA	= null;
	}

	/** <summary> Constructs the default property. </summary> */
	public PropertyColor(string name, bool alpha, Color value, Color defaultValue, PropertyAction action = null)
		: base(name, true, value, defaultValue, action) {
		this.alpha		= alpha;

		this.propR		= (PropertyInt)AddProperty(new PropertyInt("Red", value.R, defaultValue.R, new RangeI(0, 255), action));
		this.propG		= (PropertyInt)AddProperty(new PropertyInt("Green", value.G, defaultValue.G, new RangeI(0, 255), action));
		this.propB		= (PropertyInt)AddProperty(new PropertyInt("Blue", value.B, defaultValue.B, new RangeI(0, 255), action));
		if (this.alpha)
			this.propA	= (PropertyInt)AddProperty(new PropertyInt("Alpha", value.A, defaultValue.A, new RangeI(0, 255), action));
		else
			this.propA	= null;
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties
	//--------------------------------
	#region Values

	/** <summary> Gets or sets the real value of the property. </summary> */
	public Color Value {
		get { return (Color)this.value; }
		set {
			this.value = value;
			text = value.ToString();
		}
	}
	/** <summary> Gets or sets the real default value of the property. </summary> */
	public Color DefaultValue {
		get { return (Color)this.value; }
		set {
			this.value = value;
			text = value.ToString();
		}
	}
	/** <summary> Gets or sets the text of the property. </summary> */
	public override string Text {
		set {
			bool error = false;

			Color newValue = Color.White;
			try {
				newValue = Color.Parse(editingText);
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
				propR.Value = newValue.R;
				propG.Value = newValue.G;
				propB.Value = newValue.B;
				if (alpha)
					propA.Value = newValue.A;
			}
		}
	}

	#endregion
	//--------------------------------
	#region Dimensions

	/** <summary> The extra text padding to use. </summary> */
	public override int TextOffset {
		get { return 22; }
	}

	#endregion
	//--------------------------------
	#endregion
	//=========== UPDATING ===========
	#region Updating

	/** <summary> Called every step to update the property. </summary> */
	public override void Update(double time, Point2I position) {
		base.Update(time, position);

		if (propR.IsChanged) {
			Color newValue = (Color)value;
			newValue.R = (byte)propR.Value;
			value = newValue;
			text = newValue.ToString();
			if (action != null)
				action();
		}
		if (propG.IsChanged) {
			Color newValue = (Color)value;
			newValue.G = (byte)propG.Value;
			value = newValue;
			text = newValue.ToString();
			if (action != null)
				action();
		}
		if (propB.IsChanged) {
			Color newValue = (Color)value;
			newValue.B = (byte)propB.Value;
			value = newValue;
			text = newValue.ToString();
			if (action != null)
				action();
		}
		if (alpha) {
			if (propA.IsChanged) {
				Color newValue = (Color)value;
				newValue.A = (byte)propA.Value;
				value = newValue;
				text = newValue.ToString();
				if (action != null)
					action();
			}
		}
	}

	#endregion
	//=========== DRAWING ============
	#region Drawing

	/** <summary> Called every step to draw the property. </summary> */
	public override void Draw(Graphics2D g, Point2I position, int indentLevel = 0) {

		base.Draw(g, position, indentLevel);

		Point2I colorSize = new Point2I(20, 14);
		Rectangle2I colorRect = new Rectangle2I(position + new Point2I(grid.ValuePosition + 2, 2), colorSize);

		g.DrawRectangle(colorRect, 1, colorOutline);

		colorRect.Inflate(-1, -1);

		Color color = (Color)value;
		color.A = 255;

		g.FillRectangle(colorRect, Color.White);
		g.FillRectangle(colorRect, color);
	}

	#endregion
	//========== MANAGEMENT ==========
	#region Management

	/** <summary> Sets the value of the property after finished editing. </summary> */
	public override void Finish() {
		bool error = false;

		Color newValue = Color.White;
		try {
			newValue = Color.Parse(editingText);
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
			propR.Value = newValue.R;
			propG.Value = newValue.G;
			propB.Value = newValue.B;
			if (alpha)
				propA.Value = newValue.A;
		}
		else {
			Cancel();
		}
	}

	#endregion
}
} // end namespace
