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
public class PropertyComboStruct : Property {

	//========== CONSTANTS ===========
	#region Constants

	#endregion
	//========== DELEGATES ===========
	#region Delegates

	#endregion
	//=========== MEMBERS ============
	#region Members

	// Containment
	/** <summary> The list of strings in the combo box. </summary> */
	//protected List<string> nameList;
	/** <summary> The list of structs in the combo box. </summary> */
	protected List<PropertyStruct> structList;
	/** <summary> The current selection in the combo box. </summary> */
	protected int selection;

	// Editing
	/** <summary> True if the combo box drop down is open. </summary> */
	protected bool dropDown;


	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default property. </summary> */
	public PropertyComboStruct()
		: base() {

		// Containment
		//this.nameList		= new List<string>();
		this.structList		= new List<PropertyStruct>();
		this.selection		= -1;

		// Editing
		this.dropDown		= false;
	}

	/** <summary> Constructs the default property. </summary> */
	public PropertyComboStruct(string name, string value, string defaultValue, PropertyAction action = null)
		: base(name, value, defaultValue, action) {

		// Containment
		//this.nameList		= new List<string>();
		this.structList		= new List<PropertyStruct>();
		this.selection		= -1;

		// Editing
		this.dropDown		= false;
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
			for (int i = 0; i < structList.Count; i++) {
				structList[i].PropertyGrid = grid;
			}
		}
	}
	/** <summary> Gets the list of names. </summary> */
	/*public List<string> NameList {
		get { return nameList; }
	}*/
	/** <summary> Gets the list of values. </summary> */
	public List<PropertyStruct> StructList {
		get { return structList; }
	}

	#endregion
	//--------------------------------
	#region Values

	/** <summary> Gets or sets the current selection. </summary> */
	public int Selection {
		get { return selection; }
		set {
			selection	= value;
			if (selection >= 0 && selection < structList.Count) {
				text		= structList[value].Name;
				this.value	= structList[value].ObjValue;
			}
		}
	}
	/** <summary> Gets the name of the current selection. </summary> */
	public string SelectionName {
		get { return ((selection >= 0 && selection < structList.Count) ? structList[selection].Name : ""); }
	}
	/** <summary> Gets the value of the current selection. </summary> */
	public object SelectionValue {
		get { return ((selection >= 0 && selection < structList.Count) ? structList[selection].ObjValue : null); }
	}
	/** <summary> Gets the number of items in the list. </summary> */
	public int ItemCount {
		get { return structList.Count; }
	}
	/** <summary> Returns true if the value is invalid. </summary> */
	public override bool IsInvalid {
		get {
			for (int i = 0; i < structList.Count; i++) {
				if (structList[i].Name == text)
					return false;
			}
			return true;
		}
	}

	#endregion
	//--------------------------------
	#region Dimensions

	/** <summary> Gets the number of rows in the property. </summary> */
	public override int Rows {
		get {
			return (selection != -1 ? structList[selection].Rows : 1);
		}
	}

	#endregion
	//--------------------------------
	#endregion
	//========== MANAGEMENT ==========
	#region Management

	/** <summary> Adds an item to the combo box. </summary> */
	public void AddItem(PropertyStruct property) {
		property.PropertyGrid = grid;
		property.AllowEditing = false;
		structList.Add(property);
	}
	/** <summary> Removes all items from the combo box. </summary> */
	public void ClearItems() {
		structList.Clear();

		selection = -1;
	}
	/** <summary> Sets the list of items in the combo box. </summary> */
	public void SetItemList(PropertyStruct[] structList) {
		this.structList.Clear();
		this.structList.AddRange(structList);

		selection = -1;
	}

	#endregion
	//=========== UPDATING ===========
	#region Updating

	/** <summary> Called every step to update the property. </summary> */
	public override void Update(double time, Point2I position) {

		if (selection != -1) {
			structList[selection].Update(time, position);
		}

		base.Update(time, position);

		if (selection != -1) {
			changed = structList[selection].IsChanged;

			for (int i = 0; i < structList.Count; i++) {
				structList[i].IsExpanded = structList[selection].IsExpanded;
			}
		}
		if (changed) {
			value = structList[selection].ObjValue;
			if (action != null)
				action();
		}

		if (editing)
			StopEditing();

		if (grid.DropDownOpen && grid.DropDownProperty != this)
			return;

		if (dropDown && !grid.DropDownOpen)
			grid.OpenDropDown(this);
		else if (!dropDown && grid.DropDownOpen)
			grid.CloseDropDown();

		Point2I mousePos		= (Point2I)Mouse.GetPosition();
		Rectangle2I valueRect	= new Rectangle2I(position + new Point2I(grid.ValuePosition + SeparatorPadding, 0),
													new Point2I(grid.ValueWidth - SeparatorPadding * 2, RowHeight));

		bool clickedInside = false;

		if (valueRect.Contains(mousePos) && Mouse.IsButtonPressed(MouseButtons.Left)) {
			dropDown = !dropDown;
			clickedInside = true;
		}

		int listWidth = 200;

		Rectangle2I itemRect = new Rectangle2I(position + new Point2I(grid.GridWidth - listWidth - 1, RowHeight), new Point2I(listWidth, RowHeight));

		for (int i = 0; i < structList.Count && dropDown; i++) {
			if (itemRect.Contains(mousePos) && Mouse.IsButtonPressed(MouseButtons.Left)) {
				selection = i;
				text = structList[i].Name;
				value = structList[i].ObjValue;
				dropDown = false;
				clickedInside = true;
				action();
			}
			itemRect.Y += RowHeight;
		}

		if (!clickedInside && Mouse.IsButtonPressed(MouseButtons.Left)) {
			dropDown = false;
		}

	}

	#endregion
	//=========== DRAWING ============
	#region Drawing

	/** <summary> Called every step to draw the property. </summary> */
	public override void Draw(Graphics2D g, Point2I position, int indentLevel = 0) {

		if (selection != -1) {
			structList[selection].Draw(g, position, indentLevel);
		}

		base.Draw(g, position, indentLevel);


		Point2I mousePos		= (Point2I)Mouse.GetPosition();
		//Point2I pos = position + new Point2I(grid.NamePosition, RowHeight);
		Point2I pos = position + new Point2I(grid.NamePosition + grid.NameWidth - 9 - 10, RowHeight);
		Rectangle2I addRect	= new Rectangle2I(pos + new Point2I(4, 5), new Point2I(9, 9));


		int listWidth = 200;
		Rectangle2I itemRect = new Rectangle2I(position + new Point2I(grid.GridWidth - listWidth - 1, RowHeight), new Point2I(listWidth, RowHeight));

		Rectangle2I buttonRect	= new Rectangle2I(position + new Point2I(grid.GridWidth - 19, 0), new Point2I(18, 18));

		g.FillRectangle(buttonRect, colorBackground);

		buttonRect.Inflate(-2, -2);
		g.FillRectangle(buttonRect, colorCursor);

		g.DrawSprite(grid.SpriteSheet["dropdown"], position + new Point2I(grid.GridWidth - 18, 1), Color.White);

		if (dropDown) {
			for (int i = 0; i < structList.Count; i++) {
				g.FillRectangle(itemRect, colorBackground);

				if (selection == i) {
					g.FillRectangle(itemRect.Inflated(-2, -2), colorTextBackgroundHighlight);
					g.DrawString(grid.Font, structList[i].Name, new Point2I(itemRect.X + TextPadding, (int)itemRect.Center.Y), Align.Left | Align.Int, colorTextHighlight);
				}
				else if (itemRect.Contains(mousePos)) {
					g.FillRectangle(itemRect.Inflated(-2, -2), colorBackgroundHighlight);
					g.DrawString(grid.Font, structList[i].Name, new Point2I(itemRect.X + TextPadding, (int)itemRect.Center.Y), Align.Left | Align.Int, colorText);
				}
				else {
					g.DrawString(grid.Font, structList[i].Name, new Point2I(itemRect.X + TextPadding, (int)itemRect.Center.Y), Align.Left | Align.Int, colorText);
				}


				itemRect.Y += RowHeight;
			}

			itemRect.Height = itemRect.Y - RowHeight - position.Y;
			itemRect.Y = position.Y + RowHeight;

			itemRect.Inflate(1, 1);

			g.DrawRectangle(itemRect, 1, colorArrow);

			Rectangle2I valueRect	= new Rectangle2I(position + new Point2I(grid.ValuePosition - 1, -1),
														new Point2I(grid.ValueWidth + 1, RowHeight + 1));
			g.DrawRectangle(valueRect, 1, colorArrow);
		}


		Rectangle2I expandRect	= new Rectangle2I(position + new Point2I(3, 5), new Point2I(9, 9));

		if (selection != -1 && structList[selection].IsExpanded) {
			g.FillRectangle(expandRect, colorArrow);
			g.DrawSprite(grid.SpriteSheet["minusbutton"], expandRect.Point, Color.White);

			/*Point2I pos2				= position + new Point2I(0, RowHeight);

			for (int i = 0; i < structList.Count; i++) {
				if (grid.DropDownOpen && grid.DropDownProperty == structList[i]) {
					grid.DropDownPosition = pos2;
				}
				else {
					structList[i].Draw(g, pos, indentLevel + 1);
				}
				pos2.Y += structList[i].Height;
			}*/
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
