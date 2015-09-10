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
public class PropertyStruct :  Property {

	//========== CONSTANTS ===========
	#region Constants


	#endregion
	//=========== MEMBERS ============
	#region Members

	// Containment
	/** <summary> The list of properties in the group. </summary> */
	protected List<Property> properties;
	/** <summary> True if the properties in the group are being shown. </summary> */
	protected bool expanded;
	/** <summary> True if the main property can be edited. </summary> */
	protected bool allowEditing;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default property. </summary> */
	public PropertyStruct(bool allowEditing)
		: base() {
		// Containment
		this.properties			= new List<Property>();
		this.expanded			= false;
		this.allowEditing		= allowEditing;
	}

	/** <summary> Constructs the default property. </summary> */
	public PropertyStruct(string name, bool allowEditing, object value, object defaultValue, PropertyAction action = null)
		: base(name, value, defaultValue, action) {
		// Containment
		this.properties			= new List<Property>();
		this.expanded			= false;
		this.allowEditing		= allowEditing;
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties
	//--------------------------------
	#region Containment

	/** <summary> Gets or sets the property grid containing this property. </summary> */
	public override PropertyGrid PropertyGrid {
		get { return grid; }
		set {
			grid = value;
			for (int i = 0; i < properties.Count; i++) {
				properties[i].PropertyGrid = grid;
			}
		}
	}
	/** <summary> Gets the list of properties in the property group. </summary> */
	public List<Property> Properties {
		get { return properties; }
	}
	/** <summary> Gets the property at the specified index in the property group. </summary> */
	public Property this[int index] {
		get { return properties[index]; }
	}
	/** <summary> Gets the number of properties in the property group. </summary> */
	public int PropertyCount {
		get { return properties.Count; }
	}

	#endregion
	//--------------------------------
	#region Values

	/** <summary> Gets or sets if the struct is expanded. </summary> */
	public bool IsExpanded {
		get { return expanded; }
		set { expanded = value; }
	}
	/** <summary> Gets or sets if the main property can be edited. </summary> */
	public bool AllowEditing {
		get { return allowEditing; }
		set { allowEditing = value; }
	}

	#endregion
	//--------------------------------
	#region Dimensions

	/** <summary> Gets the number of rows in the property. </summary> */
	public override int Rows {
		get {
			int numRows = 1;
			for (int i = 0; i < properties.Count && expanded; i++)
				numRows += properties[i].Rows;
			return numRows;
		}
	}

	#endregion
	//--------------------------------
	#endregion
	//========== MANAGEMENT ==========
	#region Management

	/** <summary> Adds the property to the property struct. </summary> */
	public virtual Property AddProperty(Property property) {
		property.PropertyGrid = grid;
		property.Action = action;
		properties.Add(property);
		return property;
	}
	/** <summary> Removes the property from the property struct. </summary> */
	public virtual void RemoveProperty(int index) {
		properties.RemoveAt(index);
	}
	/** <summary> Removes all properties from the property struct. </summary> */
	public virtual void ClearProperty() {
		properties.Clear();
	}

	#endregion
	//=========== UPDATING ===========
	#region Updating

	/** <summary> Called every step to update the property. </summary> */
	public override void Update(double time, Point2I position) {

		base.Update(time, position);

		Point2I mousePos		= (Point2I)Mouse.GetPosition();
		Rectangle2I expandRect	= new Rectangle2I(position + new Point2I(3, 5), new Point2I(9, 9));
		Point2I pos				= position + new Point2I(0, RowHeight);

		if (editing && !allowEditing)
			StopEditing();

		for (int i = 0; i < properties.Count && expanded; i++) {
			properties[i].Update(time, pos);
			pos.Y += properties[i].Height;
			if (properties[i].IsChanged)
				changed = true;
		}


		if (grid.DropDownOpen && grid.DropDownProperty != this)
			return;

		if (expandRect.Contains(mousePos)) {
			if (Mouse.IsButtonPressed(MouseButtons.Left)) {
				expanded = !expanded;
				if (expanded) {
					for (int i = 0; i < properties.Count && expanded; i++) {
						properties[i].Update(time, pos);
						pos.Y += properties[i].Height;
						if (properties[i].IsChanged)
							changed = true;
					}
				}
			}
		}
	}

	#endregion
	//=========== DRAWING ============
	#region Drawing

	/** <summary> Called every step to draw the property. </summary> */
	public override void Draw(Graphics2D g, Point2I position, int indentLevel = 0) {

		base.Draw(g, position, indentLevel);

		Point2I mousePos		= (Point2I)Mouse.GetPosition();
		Rectangle2I expandRect	= new Rectangle2I(position + new Point2I(3, 5), new Point2I(9, 9));

		if (expanded) {
			g.FillRectangle(expandRect, colorArrow);
			g.DrawSprite(grid.SpriteSheet["minusbutton"], expandRect.Point, Color.White);

			Point2I pos				= position + new Point2I(0, RowHeight);

			for (int i = 0; i < properties.Count; i++) {
				if (grid.DropDownOpen && grid.DropDownProperty == properties[i]) {
					grid.DropDownPosition = pos;
					grid.DropDownIndent = indentLevel + 1;
				}
				else {
					properties[i].Draw(g, pos, indentLevel + 1);
				}
				pos.Y += properties[i].Height;
			}
		}
		else {
			g.FillRectangle(expandRect, colorArrow);
			g.DrawSprite(grid.SpriteSheet["plusbutton"], expandRect.Point, Color.White);
		}
	}

	#endregion
	//========== MANAGEMENT ==========
	#region Management


	#endregion
}
} // end namespace
