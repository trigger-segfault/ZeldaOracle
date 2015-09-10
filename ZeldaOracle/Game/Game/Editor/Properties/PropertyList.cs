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
public class PropertyList :  PropertyStruct {

	//========== CONSTANTS ===========
	#region Constants

	#endregion
	//========== DELEGATES ===========
	#region Delegates

	/** <summary> The delegate called when the property is changed. </summary> */
	public delegate Property AddPropertyAction();

	#endregion
	//=========== MEMBERS ============
	#region Members

	/** <summary> The name of the types in the list. </summary> */
	protected string typeName;
	/** <summary> The plural name of the types in the list. </summary> */
	protected string typeNamePlural;
	/** <summary> The action used to add a new property to the list. </summary> */
	protected AddPropertyAction addAction;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default property. </summary> */
	public PropertyList()
		: base(false) {

		this.typeName		= "";
		this.typeNamePlural	= "";
		this.addAction		= null;
	}

	/** <summary> Constructs the default property. </summary> */
	public PropertyList(string name, string typeName, string typeNamePlural, AddPropertyAction addAction, PropertyAction action = null)
		: base(name, false, "0 " + typeNamePlural, "0 " + typeNamePlural, action) {

		this.typeName		= typeName;
		this.typeNamePlural	= typeNamePlural;
		this.addAction		= addAction;
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties
	//--------------------------------
	#region Values

	/** <summary> Gets or sets the real value of the property. </summary> */
	public object[] Values {
		get {
			object[] list = new object[properties.Count];
			for (int i = 0; i < properties.Count; i++) {
				list[i] = properties[i].ObjValue;
			}
			return list;
		}
		set {}
	}
	/** <summary> Gets or sets the text of the property. </summary> */
	public override string Text {
		set {}
	}
	/** <summary> Gets or sets the type name of the property list. </summary> */
	public string TypeName {
		get { return typeName; }
		set { typeName = value; }
	}
	/** <summary> Gets or sets the plural type name of the property list. </summary> */
	public string TypeNamePlural {
		get { return typeNamePlural; }
		set { typeNamePlural = value; }
	}

	#endregion
	//--------------------------------
	#region Dimensions

	/** <summary> Gets the number of rows in the property. </summary> */
	public override int Rows {
		get {
			int numRows = (expanded ? 2 : 1);
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
	public Property AddProperty() {
		return AddProperty(addAction());
	}
	/** <summary> Adds the property to the property group. </summary> */
	public override Property AddProperty(Property property) {
		property.PropertyGrid = grid;
		property.Name = typeName + " " + (properties.Count + 1).ToString();
		property.Action = action;
		properties.Add(property);

		text = properties.Count.ToString() + " " + (properties.Count == 1 ? typeName : typeNamePlural);
		return property;
	}
	/** <summary> Removes the property from the property group. </summary> */
	public override void RemoveProperty(int index) {
		properties.RemoveAt(index);
		for (int i = index; i < properties.Count; i++) {
			properties[i].Name = typeName + " " + (i + 1).ToString();
		}

		text = properties.Count.ToString() + " " + (properties.Count == 1 ? typeName : typeNamePlural);
	}
	/** <summary> Removes all properties from the property group. </summary> */
	public override void ClearProperty() {
		properties.Clear();
	}

	#endregion
	//=========== UPDATING ===========
	#region Updating

	/** <summary> Called every step to update the property. </summary> */
	public override void Update(double time, Point2I position) {
		base.Update(time, position);


		if (grid.DropDownOpen && grid.DropDownProperty != this)
			return;

		if (expanded) {
			//Point2I pos = position + new Point2I(grid.NamePosition, RowHeight);
			Point2I pos = position + new Point2I(grid.NamePosition + grid.NameWidth - 9 - 10, RowHeight);
			Rectangle2I addRect	= new Rectangle2I(pos + new Point2I(4, 5), new Point2I(9, 9));
			Point2I mousePos = (Point2I)Mouse.GetPosition();

			for (int i = 0; i < properties.Count; i++) {

				if (addRect.Contains(mousePos) && Mouse.IsButtonPressed(MouseButtons.Left)) {
					pos.Y += properties[i].Height;
					addRect.Y += properties[i].Height;
					RemoveProperty(i);
					changed = true;
					if (action != null)
						action();
					continue;
				}
				pos.Y += properties[i].Height;
				addRect.Y += properties[i].Height;
			}

			if (addRect.Contains(mousePos) &&
				Mouse.IsButtonPressed(MouseButtons.Left)) {
				AddProperty(addAction());
				if (action != null)
					action();
				changed = true;
			}
		}
	}

	#endregion
	//=========== DRAWING ============
	#region Drawing

	/** <summary> Called every step to draw the property. </summary> */
	public override void Draw(Graphics2D g, Point2I position, int indentLevel = 0) {

		base.Draw(g, position, indentLevel);

		if (!expanded)
			return;

		Point2I mousePos		= (Point2I)Mouse.GetPosition();
		//Point2I pos = position + new Point2I(grid.NamePosition, RowHeight);
		Point2I pos = position + new Point2I(grid.NamePosition + grid.NameWidth - 9 - 10, RowHeight);
		Rectangle2I addRect	= new Rectangle2I(pos + new Point2I(4, 5), new Point2I(9, 9));


		for (int i = 0; i < properties.Count; i++) {
			g.FillRectangle(addRect, colorArrow);
			g.DrawSprite(grid.SpriteSheet["minusbutton"], addRect.Point, Color.White);
			pos.Y += properties[i].Height;
			addRect.Y += properties[i].Height;
		}

		pos.X -= grid.NamePosition;

		Rectangle2I sideBarRect	= new Rectangle2I(new Point2I(position.X + grid.SideBarPosition, pos.Y), new Point2I(SideBarWidth, RowHeight));
		Rectangle2I nameRect	= new Rectangle2I(new Point2I(position.X + grid.NamePosition, pos.Y), new Point2I(grid.NameWidth, RowHeight));
		Rectangle2I valueRect	= new Rectangle2I(new Point2I(position.X + grid.ValuePosition, pos.Y), new Point2I(grid.ValueWidth, RowHeight));

		//======== Draw the sidebar ========

		g.FillRectangle(sideBarRect, colorSideBar);

		//======== Draw the name column ========

		// Draw the name column background
		g.FillRectangle(nameRect, colorBackground);

		// Highlight the name column
		if (nameRect.Inflated(-SeparatorPadding, 0).Contains(mousePos)) {
			Rectangle2I highlightRect = nameRect;
			highlightRect.Point	+= 2;
			highlightRect.Size	-= 5;
			g.FillRectangle(highlightRect, colorBackgroundHighlight);
		}

		// Draw the name grid outline
		nameRect.Point	-= 1;
		nameRect.Size	+= 1;
		g.DrawRectangle(nameRect, 1, colorOutline);

		//======== Draw the value column ========

		// Draw the value column background
		g.FillRectangle(valueRect, colorBackground);

		// Highlight the value column
		if (valueRect.Inflated(-SeparatorPadding, 0).Contains(mousePos)) {
			Rectangle2I highlightRect = valueRect;
			highlightRect.Point	+= 2;
			highlightRect.Size	-= 5;
			g.FillRectangle(highlightRect, colorBackgroundHighlight);
		}

		// Draw the value grid outline
		valueRect.Point	-= 1;
		valueRect.Size	+= 1;
		g.DrawRectangle(valueRect, 1, colorOutline);


		g.FillRectangle(addRect, colorArrow);
		g.DrawSprite(grid.SpriteSheet["plusbutton"], addRect.Point, Color.White);
	}

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
