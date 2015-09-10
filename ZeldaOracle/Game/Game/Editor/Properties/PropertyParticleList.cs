using System;
using System.Collections.Generic;
using System.Text;

using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Particles;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Common.Scripts;

using GameFramework.MyGame.Editor;

namespace GameFramework.MyGame.Editor.Properties {
/** <summary>
 * The base property class to extend properties from.
 * </summary> */
public class PropertyParticleList : Property {

	//========== CONSTANTS ===========
	#region Constants

	private int MaxListRows;

	#endregion
	//========== DELEGATES ===========
	#region Delegates

	#endregion
	//=========== MEMBERS ============
	#region Members
	
	// Shared
	/** <summary> The list of particle types. </summary> */
	protected static List<ParticleType> particles = null;

	// Containment
	/** <summary> The current selection in the combo box. </summary> */
	protected int selection;

	// Editing
	/** <summary> True if the combo box drop down is open. </summary> */
	protected bool dropDown;
	/** <summary> The scroll position of the combo list. </summary> */
	protected int listPosition;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default property. </summary> */
	public PropertyParticleList()
		: base() {

		// Containment
		this.selection		= -1;

		// Editing
		this.dropDown		= false;
		this.listPosition	= 0;

		if (particles == null) {
			UpdateList();
		}

		this.MaxListRows	= 5;
	}
	/** <summary> Constructs the default property. </summary> */
	public PropertyParticleList(string name, ParticleType value, ParticleType defaultValue, PropertyAction action = null)
		: base(name, value, defaultValue, action) {

		// Containment
		if (value != null)
			this.text = value.Name;
		this.selection		= -1;

		// Editing
		this.dropDown		= false;
		this.listPosition	= 0;

		if (particles == null) {
			UpdateList();
		}

		this.MaxListRows	= 5;
	}
	/** <summary> Constructs the default property. </summary> */
	public PropertyParticleList(string name, int maxRows, ParticleType value, ParticleType defaultValue, PropertyAction action = null)
		: base(name, value, defaultValue, action) {

		// Containment
		if (value != null)
			this.text = value.Name;
		this.selection		= -1;

		// Editing
		this.dropDown		= false;
		this.listPosition	= 0;

		if (particles == null) {
			UpdateList();
		}

		this.MaxListRows	= maxRows;
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties
	//--------------------------------
	#region Containment

	/** <summary> Gets the list of particles. </summary> */
	public List<ParticleType> Particles {
		get { return particles; }
	}

	#endregion
	//--------------------------------
	#region Values

	/** <summary> Gets or sets the current selection. </summary> */
	public int Selection {
		get { return selection; }
		set {
			selection	= value;
			if (selection >= 0 && selection < particles.Count) {
				text		= particles[value].Name;
				this.value	= particles[value];
			}
		}
	}
	/** <summary> Gets the name of the current selection. </summary> */
	public string SelectionName {
		get { return ((selection >= 0 && selection < particles.Count) ? particles[selection].Name : ""); }
	}
	/** <summary> Gets the value of the current selection. </summary> */
	public ParticleType SelectionValue {
		get { return ((selection >= 0 && selection < particles.Count) ? particles[selection] : null); }
	}
	/** <summary> Gets the number of items in the list. </summary> */
	public int ItemCount {
		get { return particles.Count; }
	}
	/** <summary> Returns true if the value is invalid. </summary> */
	public override bool IsInvalid {
		get {
			for (int i = 0; i < particles.Count; i++) {
				if (particles[i].Name == text)
					return false;
			}
			return true;
		}
	}

	#endregion
	//--------------------------------
	#endregion
	//========== MANAGEMENT ==========
	#region Management

	/** <summary> Updates the particle list. </summary> */
	public static void UpdateList() {
		if (particles == null)
			particles = new List<ParticleType>();
		else
			particles.Clear();

		particles.AddRange(Resources.ParticleTypes);
	}

	#endregion
	//=========== UPDATING ===========
	#region Updating

	/** <summary> Called every step to update the property. </summary> */
	public override void Update(double time, Point2I position) {

		base.Update(time, position);

		if (selection >= particles.Count || selection == -1 || particles[selection] != value) {
			selection = -1;

			for (int i = 0; i < particles.Count; i++) {
				if (particles[i] == value) {
					text = particles[i].Name;
					selection = i;
					break;
				}
			}
			if (selection == -1) {
				selection = 0;
				text = particles[0].Name;
			}
		}
		if (particles[selection].Name != text) {
			text = particles[selection].Name;
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
			listPosition = 0;
			clickedInside = true;
		}
		else if (valueRect.Contains(mousePos) && Mouse.IsButtonDoubleClicked(MouseButtons.Right)) {
			dropDown = false;
			listPosition = 0;

			ParticlePropertyGridBase g = (ParticlePropertyGridBase)grid;

			g.particleChanged = true;
			g.propParticleSelection.Selection = selection;
		}

		int listWidth = 200;

		for (int i = 0; i < particles.Count; i++) {
			listWidth = (int)GMath.Max(listWidth, grid.Font.MeasureString(particles[i].Name).X + TextPadding * 3);
		}

		Rectangle2I itemRect = new Rectangle2I(position + new Point2I(grid.GridWidth - listWidth - 1, RowHeight), new Point2I(listWidth, RowHeight));

		if (dropDown) {

			if (Mouse.IsWheelUp() && listPosition > 0) {
				listPosition--;
			}
			else if (Mouse.IsWheelDown() && listPosition < ItemCount - MaxListRows) {
				listPosition++;
			}

			for (int i = 0; i < particles.Count && i < MaxListRows; i++) {
				if (itemRect.Contains(mousePos) && Mouse.IsButtonPressed(MouseButtons.Left)) {
					selection = i + listPosition;
					text = particles[i + listPosition].Name;
					value = particles[i + listPosition];
					dropDown = false;
					clickedInside = true;
					changed = true;
					if (action != null)
						action();
				}
				itemRect.Y += RowHeight;
			}

			if (!clickedInside && Mouse.IsButtonPressed(MouseButtons.Left)) {
				dropDown = false;
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
		//Point2I pos = position + new Point2I(grid.NamePosition, RowHeight);
		Point2I pos = position + new Point2I(grid.NamePosition + grid.NameWidth - 9 - 10, RowHeight);
		Rectangle2I addRect	= new Rectangle2I(pos + new Point2I(4, 5), new Point2I(9, 9));


		int listWidth = 200;

		for (int i = 0; i < particles.Count; i++) {
			listWidth = (int)GMath.Max(listWidth, grid.Font.MeasureString(particles[i].Name).X + TextPadding * 3);
		}
		Rectangle2I itemRect = new Rectangle2I(position + new Point2I(grid.GridWidth - listWidth - 1, RowHeight), new Point2I(listWidth, RowHeight));

		Rectangle2I buttonRect	= new Rectangle2I(position + new Point2I(grid.GridWidth - 19, 0), new Point2I(18, 18));

		g.FillRectangle(buttonRect, colorBackground);

		buttonRect.Inflate(-2, -2);
		g.FillRectangle(buttonRect, colorCursor);

		g.DrawSprite(grid.SpriteSheet["dropdown"], position + new Point2I(grid.GridWidth - 18, 1), Color.White);

		if (dropDown) {



			for (int i = 0; i < particles.Count && i < MaxListRows; i++) {
				g.FillRectangle(itemRect, colorBackground);

				if (selection == i + listPosition) {
					g.FillRectangle(itemRect.Inflated(-2, -2), colorTextBackgroundHighlight);
					g.DrawString(grid.Font, particles[i + listPosition].Name, new Point2I(itemRect.X + TextPadding, (int)itemRect.Center.Y), Align.Left | Align.Int, colorTextHighlight);
				}
				else if (itemRect.Contains(mousePos)) {
					g.FillRectangle(itemRect.Inflated(-2, -2), colorBackgroundHighlight);
					g.DrawString(grid.Font, particles[i + listPosition].Name, new Point2I(itemRect.X + TextPadding, (int)itemRect.Center.Y), Align.Left | Align.Int, colorText);
				}
				else {
					g.DrawString(grid.Font, particles[i + listPosition].Name, new Point2I(itemRect.X + TextPadding, (int)itemRect.Center.Y), Align.Left | Align.Int, colorText);
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

	}

	#endregion
	//========== MANAGEMENT ==========
	#region Management


	#endregion
}
} // end namespace
