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
public class ParticlePropertyGrid : ParticlePropertyGridBase {

	public SpriteSheet sheet;

	// General
	public Property propName;
	public Property propSection;
	public PropertyRangeD propLifeSpan;
	public PropertyDouble propFadeDelay;
	
	// Scale
	public PropertyRangeD propScaleInit;
	public PropertyRangeD propScaleIncrease;
	public PropertyDouble propScaleJitter;
	public PropertyRangeD propScaleClamp;

	// Speed
	public PropertyDouble propPosJitter;
	public PropertyRangeD propSpeedInit;
	public PropertyRangeD propSpeedIncrease;
	public PropertyRangeD propSpeedFriction;

	// Direction
	public PropertyRangeD propDirInit;
	public PropertyRangeD propDirIncrease;
	public PropertyRangeD propDirFriction;
	public PropertyVector2D propGravity;

	// Rotation
	public PropertyRangeD propRotInit;
	public PropertyRangeD propRotIncrease;
	public PropertyDouble propRotFriction;
	public PropertyDouble propRotJitter;

	// Sprites
	public PropertyDouble propAnimSpeed;
	public PropertyCombo propSpriteSheet;
	public PropertyList propSprites;
	public PropertyList propColors;


	public ParticlePropertyGrid(GameManager game, ParticleType p, ParticleEmitter emitter, ParticleEffectType effect)
		: base(game, "Particle", p.Name, p, emitter, effect) {

		this.sheet = Resources.GetSpriteSheet("sheet_particles");

		PropertyGroup group;

		group = new PropertyGroup("General");
		propName			= (Property)group.AddProperty(new Property("Name", p.Name, "", delegate() {

			if (propName.Text.Length > 0 && !Resources.ParticleTypeExists(propName.Text)) {
				Resources.RenameParticleType(particle.Name, propName.Text);
				name = propName.Text;
			}
			else
				propName.Text = particle.Name;

			PropertyParticleList.UpdateList();
		}));
		propSection			= (Property)group.AddProperty(new Property("Section Name", p.Comments, "", delegate() {

			p.Comments = propSection.Text;
		}));
		this.AddProperty(group);


		group = new PropertyGroup("Time");
		propLifeSpan		= (PropertyRangeD)group.AddProperty(new PropertyRangeD("Life Span", p.LifeSpan, new RangeF(1), delegate() {
			particle.LifeSpan = propLifeSpan.Value;
		}));
		propFadeDelay		= (PropertyDouble)group.AddProperty(new PropertyDouble("Fade Delay", p.FadeDelay, -1.0, new RangeF(-1, Double.PositiveInfinity), delegate() {
			particle.FadeDelay = propFadeDelay.Value;
		}));
		this.AddProperty(group);


		group = new PropertyGroup("Scale");
		propScaleInit		= (PropertyRangeD)group.AddProperty(new PropertyRangeD("Initial Scale", p.InitialScale, new RangeF(1), delegate() {
			particle.InitialScale = propScaleInit.Value;
		}));
		propScaleIncrease	= (PropertyRangeD)group.AddProperty(new PropertyRangeD("Scale Increase", p.ScaleIncrease, RangeF.Zero, delegate() {
			particle.ScaleIncrease = propScaleIncrease.Value;
		}));
		propScaleJitter		= (PropertyDouble)group.AddProperty(new PropertyDouble("Scale Jitter", p.ScaleJitter, 0.0, RangeF.Full, delegate() {
			particle.ScaleJitter = propScaleJitter.Value;
		}));
		propScaleClamp		= (PropertyRangeD)group.AddProperty(new PropertyRangeD("Scale Clamp", p.ScaleClamp, new RangeF(-10000, 10000), delegate() {
			particle.ScaleClamp = propScaleClamp.Value;
		}));
		this.AddProperty(group);


		group = new PropertyGroup("Speed");
		propPosJitter		= (PropertyDouble)group.AddProperty(new PropertyDouble("Position Jitter", p.PositionJitter, 0.0, RangeF.Full, delegate() {
			particle.PositionJitter = propPosJitter.Value;
		}));
		propSpeedInit		= (PropertyRangeD)group.AddProperty(new PropertyRangeD("Initial Speed", p.Speed, RangeF.Zero, delegate() {
			particle.Speed = propSpeedInit.Value;
		}));
		propSpeedIncrease	= (PropertyRangeD)group.AddProperty(new PropertyRangeD("Speed Increase", p.SpeedIncrease, RangeF.Zero, delegate() {
			particle.SpeedIncrease = propSpeedIncrease.Value;
		}));
		propSpeedFriction	= (PropertyRangeD)group.AddProperty(new PropertyRangeD("Speed Friction", p.SpeedFriction, RangeF.Zero, delegate() {
			particle.SpeedFriction = propSpeedFriction.Value;
		}));
		this.AddProperty(group);
		

		group = new PropertyGroup("Direction");
		propDirInit			= (PropertyRangeD)group.AddProperty(new PropertyRangeD("Initial Direction", p.InitialDirectionOffset, RangeF.Zero, delegate() {
			particle.InitialDirectionOffset = propDirInit.Value;
		}));
		propDirIncrease		= (PropertyRangeD)group.AddProperty(new PropertyRangeD("Direction Increase", p.DirectionIncrease, RangeF.Zero, delegate() {
			particle.DirectionIncrease = propDirIncrease.Value;
		}));
		propDirFriction		= (PropertyRangeD)group.AddProperty(new PropertyRangeD("Direction Friction", p.DirectionFriction, RangeF.Zero, delegate() {
			particle.DirectionFriction = propDirFriction.Value;
		}));
		propGravity			= (PropertyVector2D)group.AddProperty(new PropertyVector2D("Gravity", false, p.Gravity, Vector2F.Zero, delegate() {
			particle.Gravity = propGravity.Value;
		}));
		this.AddProperty(group);


		group = new PropertyGroup("Rotation");
		propRotInit			= (PropertyRangeD)group.AddProperty(new PropertyRangeD("Initial Rotation", p.InitialRotation, RangeF.Zero, delegate() {
			particle.InitialRotation = propRotInit.Value;
		}));
		propRotIncrease		= (PropertyRangeD)group.AddProperty(new PropertyRangeD("Rotation Increase", p.RotationSpeed, RangeF.Zero, delegate() {
			particle.RotationSpeed = propRotIncrease.Value;
		}));
		propRotFriction		= (PropertyDouble)group.AddProperty(new PropertyDouble("Rotation Friction", p.RotationFriction, 0.0, RangeF.Full, delegate() {
			particle.RotationFriction = propRotFriction.Value;
		}));
		propRotJitter		= (PropertyDouble)group.AddProperty(new PropertyDouble("Rotation Jitter", p.RotationJitter, 0.0, RangeF.Full, delegate() {
			particle.RotationJitter = propRotJitter.Value;
		}));
		this.AddProperty(group);


		group = new PropertyGroup("Sprites");
		propAnimSpeed		= (PropertyDouble)group.AddProperty(new PropertyDouble("Animation Speed", p.AnimationSpeed, 0.0, new RangeF(0.0, Double.PositiveInfinity), delegate() {
			particle.AnimationSpeed = propAnimSpeed.Value;
		}));

		propSpriteSheet		= (PropertyCombo)group.AddProperty(new PropertyCombo("Sprite Sheet", "sheet_particles", "sheet_particles", delegate() {
			sheet = (SpriteSheet)propSpriteSheet.SelectionValue;

			for (int i = 0; i < propSprites.PropertyCount; i++) {
				((PropertySpriteList)propSprites.Properties[i]).SpriteSheet = sheet;
			}

		}));
		int sheetIndex			= -1;
		SpriteSheet[] sheets	= Resources.SpriteSheets;
		string[] sheetNames		= new string[Resources.SpriteSheetCount];

		for (int i = 0; i < Resources.SpriteSheetCount; i++) {
			sheetNames[i] = sheets[i].Name;
			if (sheets[i].Name == "sheet_particles") {
				sheetIndex = i;
			}
		}
		propSpriteSheet.SetItemList(sheetNames, sheets);
		propSpriteSheet.Selection = sheetIndex;

		propSprites			= (PropertyList)group.AddProperty(new PropertyList("Sprites", "Sprite", "Sprites", delegate() {
			return new PropertySpriteList("", sheet, "", "");

		}, delegate() {
			Sprite[] sprites = new Sprite[propSprites.PropertyCount];

			for (int i = 0; i < propSprites.PropertyCount; i++) {
				sprites[i] = ((PropertySpriteList)propSprites.Properties[i]).SelectionValue;
			}
			
			particle.Sprites = sprites;
		}));
		for (int i = 0; i < particle.Sprites.Length; i++) {
			PropertySpriteList list = (PropertySpriteList)propSprites.AddProperty();
			int spriteIndex = -1;
			for (int j = 0; j < list.ItemCount; j++) {
				if (list.Sprites[j] == particle.Sprites[i]) {
					spriteIndex = j;
					break;
				}
			}
			list.Selection = spriteIndex;
		}

		propColors			= (PropertyList)group.AddProperty(new PropertyList("Colors", "Color", "Colors", delegate() {
			return new PropertyColor("", true, Color.White, Color.White);
		}, delegate() {
			Color[] colors = new Color[propColors.PropertyCount];

			for (int i = 0; i < propColors.PropertyCount; i++) {
				colors[i] = ((PropertyColor)propColors.Properties[i]).Value;
			}
			particle.Colors = colors;
		}));
		for (int i = 0; i < particle.Colors.Length; i++) {
			propColors.AddProperty(new PropertyColor("", true, particle.Colors[i], Color.White));
		}

		this.AddProperty(group);

	}

	/*public ParticlePropertyGrid(GameManager game,)
		: base(game) {

		this.particle = null;
		PropertyGroup group;

		group = new PropertyGroup("General");
		group.AddProperty(new Property("Name", "particle_1", "particle_1"));
		propLifeSpan		= (PropertyRangeD)group.AddProperty(new PropertyRangeD("Life Span", new RangeD(1), new RangeD(1)));
		propFadeDelay		= (PropertyDouble)group.AddProperty(new PropertyDouble("Fade Delay", -1.0, -1.0, new RangeD(-1, Double.PositiveInfinity)));
		this.AddProperty(group);


		group = new PropertyGroup("Scale");
		propScaleInit		= (PropertyRangeD)group.AddProperty(new PropertyRangeD("Initial Scale", new RangeD(1), new RangeD(1)));
		propScaleIncrease	= (PropertyRangeD)group.AddProperty(new PropertyRangeD("Scale Increase", RangeD.Zero, RangeD.Zero));
		propScaleJitter		= (PropertyDouble)group.AddProperty(new PropertyDouble("Scale Jitter", 0.0, 0.0, RangeD.Full));
		propScaleClamp		= (PropertyRangeD)group.AddProperty(new PropertyRangeD("Scale Clamp", new RangeD(-10000, 10000), new RangeD(-10000, 10000)));
		this.AddProperty(group);


		group = new PropertyGroup("Speed");
		propPosJitter		= (PropertyDouble)group.AddProperty(new PropertyDouble("Position Jitter", 0.0, 0.0, RangeD.Full));
		propSpeedInit		= (PropertyRangeD)group.AddProperty(new PropertyRangeD("Initial Speed", RangeD.Zero, RangeD.Zero));
		propSpeedIncrease	= (PropertyRangeD)group.AddProperty(new PropertyRangeD("Speed Increase", RangeD.Zero, RangeD.Zero));
		propSpeedFriction	= (PropertyRangeD)group.AddProperty(new PropertyRangeD("Speed Friction", RangeD.Zero, RangeD.Zero));
		this.AddProperty(group);


		group = new PropertyGroup("Rotation");
		propRotInit			= (PropertyRangeD)group.AddProperty(new PropertyRangeD("Initial Rotation", RangeD.Zero, RangeD.Zero));
		propRotIncrease		= (PropertyRangeD)group.AddProperty(new PropertyRangeD("Rotation Increase", RangeD.Zero, RangeD.Zero));
		propRotFriction		= (PropertyDouble)group.AddProperty(new PropertyDouble("Rotation Friction", 0.0, 0.0, RangeD.Full));
		propRotJitter		= (PropertyDouble)group.AddProperty(new PropertyDouble("Rotation Jitter", 0.0, 0.0, RangeD.Full));
		this.AddProperty(group);


		group = new PropertyGroup("Sprites");
		propAnimSpeed		= (PropertyDouble)group.AddProperty(new PropertyDouble("Animation Speed", 1.0, 1.0, new RangeD(0.0, Double.PositiveInfinity)));

		propSpriteSheet		= (PropertyCombo)group.AddProperty(new PropertyCombo("Sprite Sheet", "sheet_particles", "sheet_particles"));
		propSpriteSheet.AddItem("sheet_particles");
		propSpriteSheet.AddItem("sheet_background");
		propSpriteSheet.AddItem("sheet_entities");
		propSpriteSheet.Selection = 0;

		propSprites			= (PropertyList)group.AddProperty(new PropertyList("Sprites", "Sprite", "Sprites", delegate() {
			return new Property("", "sprite_1", "");
		}));

		propColors			= (PropertyList)group.AddProperty(new PropertyList("Colors", "Color", "Colors", delegate() {
			return new PropertyColor("", true, Color.White, Color.White);
		}));
		propColors.AddProperty(new PropertyColor("", true, Color.White, Color.White));

		this.AddProperty(group);

	}*/



}
}
