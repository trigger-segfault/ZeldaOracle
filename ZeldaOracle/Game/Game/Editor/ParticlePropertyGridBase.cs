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
using GameFramework.MyGame.Editor.Properties;

namespace GameFramework.MyGame.Editor {
public class ParticlePropertyGridBase : PropertyGrid {

	public ParticleType particle;
	public ParticleEmitter emitter;
	public ParticleEffectType effect;

	public string name;
	public string typeName;

	public bool particleChanged;
	public bool emitterChanged;
	public bool effectChanged;
	public bool displayChanged;

	public int particleState;
	public int emitterState;
	public int effectState;

	public PropertyGroup propEditorGroup;
	public PropertyParticleList propParticleSelection;
	public PropertyEmitterList propEmitterSelection;
	public PropertyEffectList propEffectSelection;
	public PropertyEffectList propDisplaySelection;

	public ParticlePropertyGridBase(GameManager game, string typeName, string name, ParticleType particle, ParticleEmitter emitter, ParticleEffectType effect)
		: base(game) {

		this.name				= name;
		this.typeName			= typeName;

		this.particleChanged	= false;
		this.emitterChanged		= false;
		this.effectChanged		= false;

		this.particleState		= 0;
		this.emitterState		= 0;
		this.effectState		= 0;

		this.particle			= particle;
		this.emitter			= emitter;
		this.effect				= effect;

		int listRows = 20;

		propEditorGroup = new PropertyGroup(typeName + " Editor");
		propParticleSelection = (PropertyParticleList)propEditorGroup.AddProperty(new PropertyParticleList("Particle Types", listRows, Resources.ParticleTypes[0], null, delegate() {
			particleChanged = true;
		}));
		{
			int particleIndex = -1;
			for (int i = 0; i < propParticleSelection.ItemCount; i++) {
				if (particle.Name == propParticleSelection.Particles[i].Name) {
					particleIndex = i;
					break;
				}
			}
			propParticleSelection.Selection = particleIndex;
		}

		{
			propEmitterSelection = (PropertyEmitterList)propEditorGroup.AddProperty(new PropertyEmitterList("Emitter Types", listRows, Resources.ParticleEmitters[0], null, delegate() {
				emitterChanged = true;
			}));
			int emitterIndex = -1;
			for (int i = 0; i < propEmitterSelection.ItemCount; i++) {
				if (emitter.Name == propEmitterSelection.Emitters[i].Name) {
					emitterIndex = i;
					break;
				}
			}
			propEmitterSelection.Selection = emitterIndex;
		}

		{
			propEffectSelection = (PropertyEffectList)propEditorGroup.AddProperty(new PropertyEffectList("Effect Types", listRows, Resources.ParticleEffects[0], null, delegate() {
				effectChanged = true;
			}));
			int effectIndex = -1;
			for (int i = 0; i < propEffectSelection.ItemCount; i++) {
				if (effect.Name == propEffectSelection.Effects[i].Name) {
					effectIndex = i;
					break;
				}
			}
			propEffectSelection.Selection = effectIndex;
		}

		{
			propDisplaySelection = (PropertyEffectList)propEditorGroup.AddProperty(new PropertyEffectList("Display Effect", listRows, game.effectType, null, delegate() {
				displayChanged = true;
			}));
			int effectIndex = -1;
			for (int i = 0; i < propDisplaySelection.ItemCount; i++) {
				if (effect.Name == propDisplaySelection.Effects[i].Name) {
					effectIndex = i;
					break;
				}
			}
			propDisplaySelection.Selection = effectIndex;
		}

		this.AddProperty(propEditorGroup);
	}


	//=========== UPDATING ===========
	#region Updating

	/** <summary> Called every step to update the property grid. </summary> */
	public override void Update(double time) {

		Point2I pos					= new Point2I(game.ScreenSize.X - GridWidth - scrollBarWidth, 1 - scrollPosition);
		Rectangle2I sideSeparator	= new Rectangle2I(pos + new Point2I(SideBarPosition - Property.SeparatorPadding * 2, 0), new Point2I(Property.SeparatorPadding * 2, game.ScreenSize.Y - 1));
		Rectangle2I gridSeparator	= new Rectangle2I(pos + new Point2I(ValuePosition - Property.SeparatorPadding, 0), new Point2I(Property.SeparatorPadding * 2, game.ScreenSize.Y - 1));
		Point2I mousePos			= (Point2I)Mouse.GetPosition();
		bool down					= Mouse.IsButtonPressed(MouseButtons.Left);
		if (propEditorGroup.IsExpanded) {


			sideSeparator.Y += Property.RowHeight * 4 + 1;
			sideSeparator.Height -= Property.RowHeight * 4 + 1;

			if (!dropDown) {

				Rectangle2I addRect = new Rectangle2I(pos + new Point2I(-4 - 19 * 2, Property.RowHeight + 1), new Point2I(16, 16));
				Rectangle2I copyRect = new Rectangle2I(pos + new Point2I(-4 - 19, Property.RowHeight + 1), new Point2I(16, 16));
				Rectangle2I minusRect = new Rectangle2I(pos + new Point2I(-4, Property.RowHeight + 1), new Point2I(16, 16));

				if (addRect.Contains(mousePos) && down)
					particleState = 1;
				else if (copyRect.Contains(mousePos) && down)
					particleState = 2;
				else if (minusRect.Contains(mousePos) && down && Resources.ParticleTypeCount > 1)
					particleState = 3;

				addRect.Y += Property.RowHeight;
				copyRect.Y += Property.RowHeight;
				minusRect.Y += Property.RowHeight;

				if (addRect.Contains(mousePos) && down)
					emitterState = 1;
				else if (copyRect.Contains(mousePos) && down)
					emitterState = 2;
				else if (minusRect.Contains(mousePos) && down && Resources.ParticleEmitterCount > 1)
					emitterState = 3;

				addRect.Y += Property.RowHeight;
				copyRect.Y += Property.RowHeight;
				minusRect.Y += Property.RowHeight;

				if (addRect.Contains(mousePos) && down)
					effectState = 1;
				else if (copyRect.Contains(mousePos) && down)
					effectState = 2;
				else if (minusRect.Contains(mousePos) && down && Resources.ParticleEffectCount > 1)
					effectState = 3;
			}
		}


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
	public override void Draw(Graphics2D g) {

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

		if (propEditorGroup.IsExpanded) {
			pos					= new Point2I(game.ScreenSize.X - GridWidth - scrollBarWidth, 1);
			Rectangle2I addRect = new Rectangle2I(pos + new Point2I(-4 - 19 * 2, Property.RowHeight + 1 - scrollPosition), new Point2I(16, 16));
			Rectangle2I copyRect = new Rectangle2I(pos + new Point2I(-4 - 19, Property.RowHeight + 1 - scrollPosition), new Point2I(16, 16));
			Rectangle2I minusRect = new Rectangle2I(pos + new Point2I(-4, Property.RowHeight + 1 - scrollPosition), new Point2I(16, 16));
			Point2I mousePos = (Point2I)Mouse.GetPosition();

			g.FillRectangle(new Rectangle2I(pos + new Point2I(-4 - 19 * 2 - 2, Property.RowHeight - 1 - scrollPosition), new Point2I(3 + 19 * 2 + 2 + 2, Property.RowHeight * 3 + 1)).Inflated(1, 1), colorOutline);

			Color colorBack = new Color(128, 128, 128);
			Color colorHighlight = new Color(160, 160, 160);

			Color colorPlus = new Color(200, 190, 0);
			Color colorCopy = new Color(0, 200, 60);
			Color colorMinus = new Color(220, 0, 0);

			g.FillRectangle(addRect, colorBack);
			g.FillRectangle(copyRect, colorBack);
			g.FillRectangle(minusRect, colorBack);

			if (addRect.Contains(mousePos))
				g.FillRectangle(addRect, colorHighlight);
			else if (copyRect.Contains(mousePos))
				g.FillRectangle(copyRect, colorHighlight);
			else if (minusRect.Contains(mousePos))
				g.FillRectangle(minusRect, colorHighlight);

			g.DrawSprite(spriteSheet["checkbox_disabled"], addRect.Point, colorPlus);
			g.DrawSprite(spriteSheet["checkbox_disabled"], copyRect.Point, colorCopy);
			g.DrawSprite(spriteSheet["checkbox_disabled"], minusRect.Point, colorMinus);
			g.FillRectangle(new Rectangle2I((Point2I)addRect.Center + new Point2I(-5, -1), new Point2I(10, 2)), colorPlus);
			g.FillRectangle(new Rectangle2I((Point2I)addRect.Center + new Point2I(-1, -5), new Point2I(2, 10)), colorPlus);
			g.FillRectangle(new Rectangle2I((Point2I)copyRect.Center + new Point2I(-5, -1), new Point2I(10, 2)), colorCopy);
			g.FillRectangle(new Rectangle2I((Point2I)copyRect.Center + new Point2I(-1, -5), new Point2I(2, 10)), colorCopy);
			g.FillRectangle(new Rectangle2I((Point2I)minusRect.Center + new Point2I(-5, -1), new Point2I(10, 2)), colorMinus);

			addRect.Y += Property.RowHeight;
			copyRect.Y += Property.RowHeight;
			minusRect.Y += Property.RowHeight;
			g.FillRectangle(addRect, colorBack);
			g.FillRectangle(copyRect, colorBack);
			g.FillRectangle(minusRect, colorBack);

			if (addRect.Contains(mousePos))
				g.FillRectangle(addRect, colorHighlight);
			else if (copyRect.Contains(mousePos))
				g.FillRectangle(copyRect, colorHighlight);
			else if (minusRect.Contains(mousePos))
				g.FillRectangle(minusRect, colorHighlight);

			g.DrawSprite(spriteSheet["checkbox_disabled"], addRect.Point, colorPlus);
			g.DrawSprite(spriteSheet["checkbox_disabled"], copyRect.Point, colorCopy);
			g.DrawSprite(spriteSheet["checkbox_disabled"], minusRect.Point, colorMinus);
			g.FillRectangle(new Rectangle2I((Point2I)addRect.Center + new Point2I(-5, -1), new Point2I(10, 2)), colorPlus);
			g.FillRectangle(new Rectangle2I((Point2I)addRect.Center + new Point2I(-1, -5), new Point2I(2, 10)), colorPlus);
			g.FillRectangle(new Rectangle2I((Point2I)copyRect.Center + new Point2I(-5, -1), new Point2I(10, 2)), colorCopy);
			g.FillRectangle(new Rectangle2I((Point2I)copyRect.Center + new Point2I(-1, -5), new Point2I(2, 10)), colorCopy);
			g.FillRectangle(new Rectangle2I((Point2I)minusRect.Center + new Point2I(-5, -1), new Point2I(10, 2)), colorMinus);

			addRect.Y += Property.RowHeight;
			copyRect.Y += Property.RowHeight;
			minusRect.Y += Property.RowHeight;
			g.FillRectangle(addRect, colorBack);
			g.FillRectangle(copyRect, colorBack);
			g.FillRectangle(minusRect, colorBack);

			if (addRect.Contains(mousePos))
				g.FillRectangle(addRect, colorHighlight);
			else if (copyRect.Contains(mousePos))
				g.FillRectangle(copyRect, colorHighlight);
			else if (minusRect.Contains(mousePos))
				g.FillRectangle(minusRect, colorHighlight);

			g.DrawSprite(spriteSheet["checkbox_disabled"], addRect.Point, colorPlus);
			g.DrawSprite(spriteSheet["checkbox_disabled"], copyRect.Point, colorCopy);
			g.DrawSprite(spriteSheet["checkbox_disabled"], minusRect.Point, colorMinus);
			g.FillRectangle(new Rectangle2I((Point2I)addRect.Center + new Point2I(-5, -1), new Point2I(10, 2)), colorPlus);
			g.FillRectangle(new Rectangle2I((Point2I)addRect.Center + new Point2I(-1, -5), new Point2I(2, 10)), colorPlus);
			g.FillRectangle(new Rectangle2I((Point2I)copyRect.Center + new Point2I(-5, -1), new Point2I(10, 2)), colorCopy);
			g.FillRectangle(new Rectangle2I((Point2I)copyRect.Center + new Point2I(-1, -5), new Point2I(2, 10)), colorCopy);
			g.FillRectangle(new Rectangle2I((Point2I)minusRect.Center + new Point2I(-5, -1), new Point2I(10, 2)), colorMinus);

			addRect.Y += Property.RowHeight;
			copyRect.Y += Property.RowHeight;
			minusRect.Y += Property.RowHeight;
		}

		if (dropDownProp != null) {
			dropDownProp.Draw(g, dropDownPos, dropDownIndent);
		}
	}

	#endregion
}
}
