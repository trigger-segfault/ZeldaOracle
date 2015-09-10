using System;
using System.Collections.Generic;
using System.Text;

using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;

using GameFramework.MyGame.Editor;

namespace GameFramework.MyGame.Editor.Properties {
/** <summary>
 * The base property class to extend properties from.
 * </summary> */
public class PropertyGroup : Property {

	//========== CONSTANTS ===========
	#region Constants


	#endregion
	//========== DELEGATES ===========
	#region Delegates

	#endregion
	//=========== MEMBERS ============
	#region Members

	// Containment
	/** <summary> The list of properties in the group. </summary> */
	protected List<Property> properties;
	/** <summary> True if the properties in the group are being shown. </summary> */
	protected bool expanded;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default property. </summary> */
	public PropertyGroup() : base() {
		// Containment
		this.properties			= new List<Property>();
		this.expanded			= true;
	}

	/** <summary> Constructs the default property. </summary> */
	public PropertyGroup(string name) : base(name, null, null, null) {
		// Containment
		this.properties			= new List<Property>();
		this.expanded			= true;
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
	/** <summary> Gets or sets if the property group is expanded. </summary> */
	public bool IsExpanded {
		get { return expanded; }
		set { expanded = value; }
	}

	#endregion
	//--------------------------------
	#endregion
	//========== MANAGEMENT ==========
	#region Management

	/** <summary> Adds the property to the property group. </summary> */
	public Property AddProperty(Property property) {
		property.PropertyGrid = grid;
		properties.Add(property);
		return property;
	}
	/** <summary> Removes the property from the property group. </summary> */
	public void RemoveProperty(int index) {
		properties.RemoveAt(index);
	}
	/** <summary> Removes all properties from the property group. </summary> */
	public void ClearProperty() {
		properties.Clear();
	}

	#endregion
	//=========== UPDATING ===========
	#region Updating

	/** <summary> Called every step to update the property. </summary> */
	public override void Update(double time, Point2I position) {

		Rectangle2I expandRect	= new Rectangle2I(position + new Point2I(3, 5), new Point2I(9, 9));
		Rectangle2I baseRect	= new Rectangle2I(position + new Point2I(grid.NamePosition + SeparatorPadding, 0), new Point2I(grid.NameWidth + grid.ValueWidth - SeparatorPadding * 2, RowHeight));
		Rectangle2I baseSepRect	= new Rectangle2I(position + new Point2I(grid.ValuePosition - SeparatorPadding, 0), new Point2I(SeparatorPadding * 2, RowHeight));
		Point2I mousePos		= (Point2I)Mouse.GetPosition();
		Point2I pos				= position + new Point2I(0, RowHeight);

		for (int i = 0; i < properties.Count && expanded; i++) {
			properties[i].Update(time, pos);
			pos.Y += properties[i].Height;
		}

		if (grid.DropDownOpen && grid.DropDownProperty != this)
			return;
		
		if (expandRect.Contains(mousePos)) {
			if (Mouse.IsButtonPressed(MouseButtons.Left))
				expanded = !expanded;
		}
		else if (baseRect.Contains(mousePos) && !baseSepRect.Contains(mousePos)) {
			if (Mouse.IsButtonDoubleClicked(MouseButtons.Left))
				expanded = !expanded;
		}
	}

	#endregion
	//=========== DRAWING ============
	#region Drawing

	/** <summary> Called every step to draw the property. </summary> */
	public override void Draw(Graphics2D g, Point2I position, int indentLevel = 0) {

		Rectangle2I sideBarRect	= new Rectangle2I(position + new Point2I(grid.SideBarPosition, 0), new Point2I(SideBarWidth, RowHeight));
		Rectangle2I nameRect	= new Rectangle2I(position + new Point2I(grid.NamePosition,  0), new Point2I(grid.NameWidth,  RowHeight));
		Rectangle2I valueRect	= new Rectangle2I(position + new Point2I(grid.ValuePosition, 0), new Point2I(grid.ValueWidth, RowHeight));
		Point2I mousePos		= (Point2I)Mouse.GetPosition();
		Point2I textPos			= new Point2I(valueRect.X + TextPadding, (int)valueRect.Center.Y);

		Rectangle2I rect		= new Rectangle2I(position, new Point2I(grid.GridWidth, RowHeight));
		Rectangle2I expandRect	= new Rectangle2I(position + new Point2I(3, 5), new Point2I(9, 9));
		Rectangle2I baseRect	= new Rectangle2I(position + new Point2I(grid.NamePosition, 0), new Point2I(grid.NameWidth + grid.ValueWidth, RowHeight));

		g.FillRectangle(rect, colorOutline);

		g.DrawString(grid.FontBold, name, position + new Point2I(grid.NamePosition + TextPadding + indentLevel * TextIndentPadding, RowHeight / 2), Align.Left | Align.Int, colorText);

		g.DrawRectangle(baseRect, 1, colorOutline);

		if (expanded) {
			g.FillRectangle(expandRect, colorCursor);
			g.DrawSprite(grid.SpriteSheet["minusbutton"], expandRect.Point, Color.White);

			Point2I pos				= position + new Point2I(0, RowHeight);

			for (int i = 0; i < properties.Count; i++) {
				if (grid.DropDownOpen && grid.DropDownProperty == properties[i]) {
					grid.DropDownPosition = pos;
					grid.DropDownIndent = indentLevel;
				}
				else {
					properties[i].Draw(g, pos, indentLevel);
				}
				pos.Y += properties[i].Height;
			}
		}
		else {
			//g.FillRectangle(expandRect, colorArrow);
			g.FillRectangle(expandRect, colorCursor);
			g.DrawSprite(grid.SpriteSheet["plusbutton"], expandRect.Point, Color.White);
		}
	}

	#endregion
	//========== MANAGEMENT ==========
	#region Management


	#endregion
}
} // end namespace
