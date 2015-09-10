using System;
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
public class Property {

	//========== CONSTANTS ===========
	#region Constants

	/** <summary> The height of a single property row. </summary> */
	public const int RowHeight					= 19;
	/** <summary> The padding of text. </summary> */
	public const int TextPadding				= 6;
	/** <summary> The padding of indented text. </summary> */
	public const int TextIndentPadding			= 8;
	/** <summary> The padding of the resize separator. </summary> */
	public const int SeparatorPadding			= 3;
	/** <summary> The width of the sidebar. </summary> */
	public const int SideBarWidth				= 15;
	/** <summary> The minimum width allowed for columns. </summary> */
	public const int MinColumnWidth				= 40;

	protected static Color colorSideBar					= new Color(70, 70, 70);
	protected static Color colorBackground				= new Color(40, 40, 40);
	protected static Color colorBackgroundHighlight		= new Color(60, 60, 60);
	protected static Color colorOutline					= new Color(70, 70, 70);

	protected static Color colorText					= Color.White;
	protected static Color colorInvalid					= new Color(255, 0, 0);
	protected static Color colorTextHighlight			= new Color(40, 40, 40);
	protected static Color colorTextBackgroundHighlight	= Color.White;

	protected static Color colorHotkey					= new Color(128, 128, 128);
	protected static Color colorArrow					= new Color(160, 160, 160);
	protected static Color colorCursor					= new Color(128, 128, 128);

	#endregion
	//========== DELEGATES ===========
	#region Delegates

	/** <summary> The delegate called when the property is changed. </summary> */
	public delegate void PropertyAction();

	#endregion
	//=========== MEMBERS ============
	#region Members

	// Containment
	/** <summary> The property grid containing this property. </summary> */
	protected PropertyGrid grid;

	// Values
	/** <summary> The name of the property. </summary> */
	protected string name;
	/** <summary> The value of the property. </summary> */
	protected object value;
	/** <summary> The default value of the property. </summary> */
	protected object defaultValue;
	/** <summary> The text of the property. </summary> */
	protected string text;
	/** <summary> The action called when the property is changed. </summary> */
	protected PropertyAction action;

	// Editing
	/** <summary> True if the property is being edited. </summary> */
	protected bool editing;
	/** <summary> The current text of the edited property. </summary> */
	protected string editingText;
	/** <summary> The timer used for the cursor. </summary> */
	protected double cursorTimer;
	/** <summary> The position of the cursor. </summary> */
	protected int cursorPos;
	/** <summary> The highlight range of the cursor. </summary> */
	protected RangeI cursorHighlight;
	/** <summary> True if the cursor is dragging to highlight text. </summary> */
	protected bool highlighting;
	/** <summary> The starting index of the highlighting action. </summary> */
	protected int highlightStart;
	/** <summary> True if the property was changed. </summary> */
	protected bool changed;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default property. </summary> */
	public Property() {
		// Values
		this.name				= "";
		this.value				= null;
		this.defaultValue		= null;
		this.text				= "";
		this.action				= null;

		// Editing
		this.editing			= false;
		this.editingText		= "";
		this.cursorTimer		= 0.0;
		this.cursorPos			= 0;
		this.cursorHighlight	= RangeI.Zero;
		this.highlighting		= false;
		this.highlightStart		= 0;
		this.changed			= false;
	}

	/** <summary> Constructs the default property. </summary> */
	public Property(string name, object value, object defaultValue, PropertyAction action = null) {
		// Values
		this.name				= name;
		this.value				= value;
		this.defaultValue		= defaultValue;
		this.text				= (value != null ? value.ToString() : "");
		this.action				= action;

		// Editing
		this.editing			= false;
		this.editingText		= "";
		this.cursorTimer		= 0.0;
		this.cursorPos			= 0;
		this.cursorHighlight	= RangeI.Zero;
		this.highlighting		= false;
		this.highlightStart		= 0;
		this.changed			= false;
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties
	//--------------------------------
	#region Containment

	/** <summary> Gets or sets the property grid containing this property. </summary> */
	public virtual PropertyGrid PropertyGrid {
		get { return grid; }
		set { grid = value; }
	}

	#endregion
	//--------------------------------
	#region Values

	/** <summary> Gets or sets the name of the property. </summary> */
	public virtual string Name {
		get { return name; }
		set { name = value; }
	}
	/** <summary> Gets or sets the value of the property. </summary> */
	public virtual object ObjValue {
		get { return this.value; }
		set {
			this.value = value;
			text = value.ToString();
		}
	}
	/** <summary> Gets the default value of the property. </summary> */
	public virtual object ObjDefaultValue {
		get { return defaultValue; }
		set { defaultValue = value; }
	}
	/** <summary> Returns true if the value is the default value. </summary> */
	public virtual bool IsDefault {
		get { return (value != null ? value.Equals(defaultValue) : true); }
	}
	/** <summary> Returns true if the value is invalid. </summary> */
	public virtual bool IsInvalid {
		get { return false; }
	}
	/** <summary> Gets or sets the text of the property. </summary> */
	public virtual string Text {
		get { return text; }
		set { text = value; }
	}
	/** <summary> Gets or sets the action of the property. </summary> */
	public virtual PropertyAction Action {
		get { return action; }
		set { action = value; }
	}
	/** <summary> Returns true if the property was just changed. </summary> */
	public virtual bool IsChanged {
		get { return changed; }
	}

	#endregion
	//--------------------------------
	#region Dimensions

	/** <summary> Gets the number of rows in the property. </summary> */
	public virtual int Rows {
		get { return 1; }
	}
	/** <summary> Gets the height of the property. </summary> */
	public virtual int Height {
		get { return Rows * RowHeight; }
	}
	/** <summary> The extra text padding to use. </summary> */
	public virtual int TextOffset {
		get { return 0; }
	}

	#endregion
	//--------------------------------
	#endregion
	//=========== UPDATING ===========
	#region Updating

	/** <summary> Called every step to update the property. </summary> */
	public virtual void Update(double time, Point2I position) {

		changed = false;

		if (grid.DropDownOpen && grid.DropDownProperty != this)
			return;

		Rectangle2I valueRect	= new Rectangle2I(position + new Point2I(grid.ValuePosition, 0), new Point2I(grid.ValueWidth, RowHeight));
		Point2I mousePos		= (Point2I)Mouse.GetPosition();
		bool mouseInside		= valueRect.Inflated(-SeparatorPadding, 0).Contains(mousePos);
		mousePos.X				-= valueRect.X + TextPadding + TextOffset;

		// Get the positions of the characters in the text
		string typingText		= (editing ? editingText : text);
		double[] charPositions	= new double[typingText.Length + 1];
		RangeF[] charRanges		= new RangeF[typingText.Length + 1];
		int cursorCharPos		= -1;

		charPositions[0] = 0.0;
		charRanges[0].Min = -20.0;
		for (int i = 0; i < typingText.Length; i++) {
			double width			= grid.Font.MeasureString(typingText.Substring(0, i + 1)).X;

			charPositions[i + 1]	= width;
			charRanges[i].Max		= charPositions[i] + ((width - charPositions[i]) / 2.0);
			charRanges[i + 1].Min	= charPositions[i] + ((width - charPositions[i]) / 2.0);
		}
		charRanges[typingText.Length].Max = charRanges[typingText.Length].Min + 25.0;

		// Get the character position of the mouse
		for (int i = 0; i < typingText.Length + 1; i++) {
			if (charRanges[i].Contains(mousePos.X)) {
				cursorCharPos = i;
				break;
			}
		}
		if (cursorCharPos == -1) {
			if (mousePos.X > charPositions[typingText.Length])
				cursorCharPos = typingText.Length;
			else
				cursorCharPos = 0;
		}

		if (editing) {

			// 0.560 seconds per cursor state
			cursorTimer += time;
			while (cursorTimer >= 1.120) {
				cursorTimer -= 1.120;
			}


			if (Keyboard.IsKeyPressed(Keys.Backspace) || Keyboard.IsKeyPressed(Keys.Delete)) {
				if (!cursorHighlight.IsSingle) {
					editingText = editingText.Remove(cursorHighlight.Min, cursorHighlight.Range);
					cursorPos = cursorHighlight.Min;
					cursorHighlight.Max = cursorHighlight.Min;
					highlightStart = cursorHighlight.Min;
				}
				else if (cursorPos > 0) {
					editingText = editingText.Remove(cursorPos - 1, 1);
					cursorPos--;
					cursorHighlight	= new RangeI(cursorPos);
				}
			}
			else if (Keyboard.IsKeyPressed(Keys.Enter)) {
				Finish();
			}
			else if (Keyboard.IsKeyPressed(Keys.Left)) {
				if (cursorPos > 0 && !highlighting) {
					cursorPos--;
					cursorHighlight	= new RangeI(cursorPos);
					cursorTimer = 0.0;
				}
			}
			else if (Keyboard.IsKeyPressed(Keys.Right)) {
				if (cursorPos < editingText.Length && !highlighting) {
					cursorPos++;
					cursorHighlight	= new RangeI(cursorPos);
					cursorTimer = 0.0;
				}
			}
			else if (Keyboard.IsCharTyped() && Keyboard.GetCharTyped() >= ' ' && Keyboard.GetCharTyped() <= '~') {
				if (!cursorHighlight.IsSingle) {
					editingText = editingText.Remove(cursorHighlight.Min, cursorHighlight.Range);
					cursorPos = cursorHighlight.Min;
					cursorHighlight.Max = cursorHighlight.Min;
				}

				editingText = editingText.Insert(cursorPos, new string(Keyboard.GetCharTyped(), 1));
				cursorPos++;
				cursorHighlight	= new RangeI(cursorPos);

				cursorTimer = 0.0;
			}
			if (mouseInside && Mouse.IsButtonDoubleClicked(MouseButtons.Left)) {

				cursorHighlight = new RangeI(0, editingText.Length);
				highlightStart = 0;
				cursorPos = editingText.Length;
				cursorTimer = 0;
			}
			else if (Mouse.IsButtonPressed(MouseButtons.Left)) {
				if (!mouseInside) {
					Finish();
				}
				else {
					highlighting = true;
					highlightStart = cursorCharPos;
					cursorHighlight = new RangeI(cursorCharPos);

					cursorPos = cursorCharPos;
					cursorTimer = 0.0;
				}
			}
			if (highlighting) {
				if (cursorCharPos >= highlightStart) {
					cursorHighlight.Max = cursorCharPos;
					cursorHighlight.Min = highlightStart;
				}
				else {
					cursorHighlight.Min = cursorCharPos;
					cursorHighlight.Max = highlightStart;
				}

				if (cursorPos != cursorCharPos) {
					cursorPos = cursorCharPos;
					cursorTimer = 0.0;
				}

				if (Mouse.IsButtonReleased(MouseButtons.Left)) {
					highlighting = false;
				}
			}

		}
		else if (Mouse.IsButtonPressed(MouseButtons.Left) && mouseInside) {
			StartEditing();
			highlighting = true;
			highlightStart = cursorCharPos;
			cursorHighlight = new RangeI(cursorCharPos);

			cursorPos = cursorCharPos;
			cursorTimer = 0.0;
		}
	}

	#endregion
	//=========== DRAWING ============
	#region Drawing

	/** <summary> Called every step to draw the property. </summary> */
	public virtual void Draw(Graphics2D g, Point2I position, int indentLevel = 0) {

		Rectangle2I sideBarRect	= new Rectangle2I(position + new Point2I(grid.SideBarPosition, 0), new Point2I(SideBarWidth, RowHeight));
		Rectangle2I nameRect	= new Rectangle2I(position + new Point2I(grid.NamePosition,  0), new Point2I(grid.NameWidth,  RowHeight));
		Rectangle2I valueRect	= new Rectangle2I(position + new Point2I(grid.ValuePosition, 0), new Point2I(grid.ValueWidth, RowHeight));
		Point2I mousePos		= (Point2I)Mouse.GetPosition();
		Point2I textPos			= new Point2I(valueRect.X + TextPadding + TextOffset, (int)valueRect.Center.Y);

		// Get text character positions
		string typingText		= (editing ? editingText : text);
		double[] charPositions	= new double[typingText.Length + 1];
		bool mouseInside		= valueRect.Contains(mousePos);

		charPositions[0] = 0.0;
		for (int i = 0; i < typingText.Length; i++) {
			charPositions[i + 1]			= grid.Font.MeasureString(typingText.Substring(0, i + 1)).X;
		}

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

		// Draw the name text
		g.DrawString(grid.Font, name, new Point2I(nameRect.X + TextPadding + indentLevel * TextIndentPadding, (int)nameRect.Center.Y), Align.Left | Align.Int, colorText);

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

		// Draw the value text
		if (editing) {
			if (cursorHighlight.IsSingle) {
				g.DrawString(grid.Font, editingText, textPos, Align.Left | Align.Int, colorText);
			}
			else {
				// Draw text text before the highlighting
				if (cursorHighlight.Min != 0)
					g.DrawString(grid.Font, editingText.Substring(0, cursorHighlight.Min), textPos, Align.Left | Align.Int, colorText);

				// Draw the text highlighting
				g.FillRectangle(new Rectangle2F(textPos.X + charPositions[cursorHighlight.Min], position.Y + 1, charPositions[cursorHighlight.Max] - charPositions[cursorHighlight.Min], RowHeight - 3), colorTextBackgroundHighlight);

				// Draw the highlighted text
				g.DrawString(grid.Font, editingText.Substring(cursorHighlight.Min, cursorHighlight.Range), textPos + new Vector2F(charPositions[cursorHighlight.Min], 0), Align.Left | Align.Int, colorTextHighlight);

				// Draw text text after the highlighting
				if (cursorHighlight.Max != editingText.Length)
					g.DrawString(grid.Font, editingText.Substring(cursorHighlight.Max), textPos + new Vector2F(charPositions[cursorHighlight.Max], 0), Align.Left | Align.Int, colorText);
			}

			// Draw the cursor
			if (cursorTimer < 0.560) {
				double xpos = textPos.X + charPositions[cursorPos];
				g.DrawLine(new Line2F(xpos, position.Y + 3, xpos, position.Y + RowHeight - 3), 2, colorCursor);
			}
		}
		else {
			g.DrawString((IsDefault ? grid.Font : grid.FontBold), text, textPos, Align.Left | Align.Int, (!IsInvalid ? colorText : colorInvalid));
		}

		// Draw the value grid outline
		valueRect.Point	-= 1;
		valueRect.Size	+= 1;
		g.DrawRectangle(valueRect, 1, colorOutline);
	}

	#endregion
	//========== MANAGEMENT ==========
	#region Management

	/** <summary> Starts the property editing. </summary> */
	public virtual void StartEditing() {
		editing			= true;
		editingText		= text;
		cursorTimer		= 0.0;
		cursorPos		= editingText.Length;
		cursorHighlight	= new RangeI(cursorPos);
		highlighting	= false;
		highlightStart	= 0;
	}
	/** <summary> Stops the property editing. </summary> */
	public virtual void StopEditing() {
		editing			= false;
		editingText		= "";
		cursorTimer		= 0.0;
		cursorPos		= 0;
		cursorHighlight	= RangeI.Zero;
		highlighting	= false;
		highlightStart	= 0;
	}
	/** <summary> Sets the value of the property after finished editing. </summary> */
	public virtual void Finish() {


		text = editingText;
		if (action != null) {
			action();
		}
		changed = true;
		StopEditing();
	}
	/** <summary> Cancels the editing of the property and does not set the value. </summary> */
	public virtual void Cancel() {

		StopEditing();
	}

	#endregion
}
} // end namespace
