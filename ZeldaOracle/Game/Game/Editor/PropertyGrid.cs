using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Particles;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Common.Scripts;

using GameFramework.MyGame.Debug;
using GameFramework.MyGame.Main;
//using GameFramework.MyGame.Editor.Properties;

namespace GameFramework.MyGame.Editor {
/** <summary>
 * A grid used for a side bar of properties
 * </summary> */
public class PropertyGrid {

	//========== CONSTANTS ===========
	#region Constants

	protected static Color colorSeparator				= new Color(100, 100, 100);
	protected static Color colorBackground				= new Color(40, 40, 40);
	protected static Color colorOutline					= new Color(70, 70, 70);
	protected static Color colorStretch					= new Color(128, 128, 128);

	protected const int scrollBarWidth					= 0;

	#endregion
	//=========== MEMBERS ============
	#region Members

	// Containment
	/** <summary> The game manager. </summary> */
	protected GameManager game;
	/** <summary> The list of properties in the property grid. </summary> */
	protected List<Property> properties;

	// Dimensions
	/** <summary> The index of the column being resized. </summary> */
	protected int dragging;
	/** <summary> The index of the column bar being hovered over. </summary> */
	protected int hovering;
	/** <summary> The width of the name column. </summary> */
	public int nameWidth;
	/** <summary> The width of the value column. </summary> */
	public int valueWidth;
	/** <summary> The ration of the name and value column widths. </summary> */
	protected double nameValueRatio;
	/** <summary> The scroll position of the property grid. </summary> */
	protected int scrollPosition;

	// Visual
	/** <summary> The regular font used for the properties. </summary> */
	protected Font fontRegular;
	/** <summary> The bold font used for the properties. </summary> */
	protected Font fontBold;
	/** <summary> The sprite sheet used for the properties. </summary> */
	protected SpriteSheet spriteSheet;

	// Editing
	/** <summary> True if a drop down menu is open and other properties should not be updatable. </summary> */
	protected bool dropDown;
	/** <summary> The property using the drop down menu. </summary> */
	protected Property dropDownProp;
	/** <summary> The position of the drop down menu. </summary> */
	protected Point2I dropDownPos;
	/** <summary> The indent level of the drop down menu. </summary> */
	protected int dropDownIndent;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default property grid. </summary> */
	public PropertyGrid(GameManager game) {
		// Containment
		this.game			= game;
		this.properties		= new List<Property>();

		// Dimensions
		this.dragging		= 0;
		this.hovering		= 0;
		this.nameWidth		= 150;
		this.valueWidth		= 200;
		this.nameValueRatio	= 0.5;
		this.scrollPosition	= 0;

		// Visual
		this.fontRegular	= Resources.GetFont("font_debug_menu");
		this.fontBold		= Resources.GetFont("font_debug_menu_bold");
		this.spriteSheet	= Resources.GetSpriteSheet("sheet_debug_menu");

		// Editing
		this.dropDown		= false;
		this.dropDownProp	= null;
		this.dropDownPos	= Point2I.Zero;
		this.dropDownIndent	= 0;
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties
	//--------------------------------
	#region Containment

	/** <summary> Gets the list of properties in the property grid. </summary> */
	public List<Property> Properties {
		get { return properties; }
	}
	/** <summary> Gets the property at the specified index in the property grid. </summary> */
	public Property this[int index] {
		get { return properties[index]; }
	}
	/** <summary> Gets the number of properties in the property grid. </summary> */
	public int PropertyCount {
		get { return properties.Count; }
	}

	#endregion
	//--------------------------------
	#region Dimensions

	/** <summary> Gets the width of the name column </summary> */
	public int NameWidth {
		get { return nameWidth; }
	}
	/** <summary> Gets the width of the value column </summary> */
	public int ValueWidth {
		get { return valueWidth; }
	}
	/** <summary> Gets the width of both columns and the sidebar. </summary> */
	public int GridWidth {
		get { return Property.SideBarWidth + nameWidth + valueWidth; }
	}
	/** <summary> Gets the position of the sidebar. </summary> */
	public int SideBarPosition {
		get { return 0; }
	}
	/** <summary> Gets the position of the name column </summary> */
	public int NamePosition {
		get { return Property.SideBarWidth; }
	}
	/** <summary> Gets the position of the value column </summary> */
	public int ValuePosition {
		get { return Property.SideBarWidth + nameWidth; }
	}

	#endregion
	//--------------------------------
	#region Visuals

	/** <summary> Gets the regular font used for the properties. </summary> */
	public Font Font {
		get { return fontRegular; }
	}
	/** <summary> Gets the bold font used for the properties. </summary> */
	public Font FontBold {
		get { return fontBold; }
	}
	/** <summary> Gets the sprite sheet used for the properties. </summary> */
	public SpriteSheet SpriteSheet {
		get { return spriteSheet; }
	}

	#endregion
	//--------------------------------
	#region Editing

	/** <summary> Returns true if a drop down menu is open. </summary> */
	public bool DropDownOpen {
		get { return dropDown; }
	}
	/** <summary> Gets the property owning the drop down menu. </summary> */
	public Property DropDownProperty {
		get { return dropDownProp; }
	}
	/** <summary> Gets or sets the position of the drop down menu. </summary> */
	public Point2I DropDownPosition {
		get { return dropDownPos; }
		set { dropDownPos = value; }
	}
	/** <summary> Gets or sets the indent level of the drop down menu. </summary> */
	public int DropDownIndent {
		get { return dropDownIndent; }
		set { dropDownIndent = value; }
	}

	#endregion
	//--------------------------------
	#endregion
	//========== MANAGEMENT ==========
	#region Management

	/** <summary> Adds the property to the property grid. </summary> */
	public Property AddProperty(Property property) {
		property.PropertyGrid = this;
		properties.Add(property);
		return property;
	}
	/** <summary> Opens a drop down menu and specifies the calling property. </summary> */
	public void OpenDropDown(Property property) {
		dropDown		= true;
		dropDownProp	= property;
	}
	/** <summary> Closes the current drop down menu. </summary> */
	public void CloseDropDown() {
		dropDown		= false;
		dropDownProp	= null;
	}

	#endregion
	//=========== UPDATING ===========
	#region Updating

	/** <summary> Called every step to update the property grid. </summary>*/
	public virtual void Update(double time) {

		Point2I pos					= new Point2I(game.ScreenSize.X - GridWidth - scrollBarWidth, 1 - scrollPosition);
		Rectangle2I sideSeparator	= new Rectangle2I(pos + new Point2I(SideBarPosition - Property.SeparatorPadding * 2, 0), new Point2I(Property.SeparatorPadding * 2, game.ScreenSize.Y - 1));
		Rectangle2I gridSeparator	= new Rectangle2I(pos + new Point2I(ValuePosition - Property.SeparatorPadding, 0), new Point2I(Property.SeparatorPadding * 2, game.ScreenSize.Y - 1));
		Point2I mousePos			= (Point2I)Mouse.GetPosition();

		for (int i = 0; i < properties.Count; i++) {
			properties[i].Update(time, pos);
			pos.Y += properties[i].Height;
		}

		if (pos.Y + scrollPosition < game.ScreenSize.Y) {
			scrollPosition = 0;
		}
		else if (pos.Y < game.ScreenSize.Y) {
			scrollPosition = pos.Y + scrollPosition - game.ScreenSize.Y;
		}

		int scrollSpeed = 16;

		if (!dropDown) {
			if (Mouse.IsWheelUp()) {
				if (scrollPosition > 0) {
					scrollPosition = GMath.Max(0, scrollPosition - scrollSpeed);
				}
			}
			else if (Mouse.IsWheelDown()) {
				if (scrollPosition < pos.Y + scrollPosition - game.ScreenSize.Y) {
					scrollPosition = GMath.Min(pos.Y + scrollPosition - game.ScreenSize.Y, scrollPosition + scrollSpeed);
				}
			}
		}

		if (dragging == 0 && !dropDown) {
			hovering = 0;
			if (sideSeparator.Contains(mousePos))
				hovering = 1;
			if (gridSeparator.Contains(mousePos))
				hovering = 2;

			if (hovering != 0 && Mouse.IsButtonPressed(MouseButtons.Left)) {
				dragging = hovering;
				nameValueRatio = (double)nameWidth / (double)valueWidth;
			}
		}
		else if (dragging == 1) {
			int newWidth = game.ScreenSize.X - mousePos.X - Property.SeparatorPadding - scrollBarWidth;
			/*int width = GMath.Clamp(newWidth,
									Property.SideBarWidth + (int)GMath.Ceiling((double)Property.MinColumnWidth * (1.0 + GMath.Max(nameValueRatio, 1.0 / nameValueRatio))) + 1,
									game.ScreenSize.X / 2);*/
			int width = GMath.Clamp(newWidth, Property.SideBarWidth + nameWidth + Property.MinColumnWidth, game.ScreenSize.X / 2);
			valueWidth = width - (Property.SideBarWidth + nameWidth);
		}
		else if (dragging == 2) {
			int newValueWidth = game.ScreenSize.X - mousePos.X - scrollBarWidth;
			int width = GMath.Clamp(newValueWidth, Property.MinColumnWidth, GridWidth - Property.SideBarWidth - Property.MinColumnWidth);
			nameWidth = GridWidth - Property.SideBarWidth - width;
			valueWidth = width;
		}

		if (Mouse.IsButtonReleased(MouseButtons.Left)) {
			dragging = 0;
		}
	}

	#endregion
	//=========== DRAWING ============
	#region Drawing

	/** <summary> Called every step to draw the property grid. </summary> */
	public virtual void Draw(Graphics2D g) {

		Point2I pos					= new Point2I(game.ScreenSize.X - GridWidth - scrollBarWidth, 0);
		Rectangle2I sideSeparator	= new Rectangle2I(pos + new Point2I(SideBarPosition - Property.SeparatorPadding * 2, 0), new Point2I(Property.SeparatorPadding * 2, game.ScreenSize.Y));
		Rectangle2I gridSeparator	= new Rectangle2I(pos + new Point2I(ValuePosition - 1, 0), new Point2I(2, game.ScreenSize.Y));
		Rectangle2I backgroundRect	= new Rectangle2I(pos, new Point2I(GridWidth, game.ScreenSize.Y));
		pos.Y						-= scrollPosition;

		g.FillRectangle(backgroundRect, colorBackground);

		pos.Y++;
		for (int i = 0; i < properties.Count; i++) {
			if (dropDown && dropDownProp == properties[i]) {
				dropDownPos = pos;
				dropDownIndent = 0;
			}
			else {
				properties[i].Draw(g, pos);
			}
			pos.Y += properties[i].Height;
		}

		if (scrollPosition != 0)
			g.DrawRectangle(new Rectangle2I(game.ScreenSize.X - GridWidth - scrollBarWidth, 0, GridWidth, game.ScreenSize.Y), 1, colorOutline);
		else
			g.DrawRectangle(new Rectangle2I(game.ScreenSize.X - GridWidth - scrollBarWidth, -scrollPosition, GridWidth, pos.Y), 1, colorOutline);

		if (hovering == 1) {
			g.FillRectangle(sideSeparator, colorStretch);
		}
		else {
			g.FillRectangle(sideSeparator, colorSeparator);
			if (hovering == 2)
				g.FillRectangle(gridSeparator, colorStretch);
		}

		if (dropDownProp != null) {
			dropDownProp.Draw(g, dropDownPos, dropDownIndent);
		}
	}

	#endregion
}
} // end namespace
