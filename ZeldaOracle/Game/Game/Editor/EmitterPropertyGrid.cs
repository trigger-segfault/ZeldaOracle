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
public class EmitterPropertyGrid : ParticlePropertyGridBase {

	// General
	public Property propName;
	public Property propSection;
	public PropertyComboStruct propEmitterArea;
	public PropertyVector2D propOrigin;
	public PropertyBool propOriginBased;
	public PropertyBool propSpeedScaled;
	public PropertyRangeD propSpeed;
	public PropertyVector2D propDirection;


	public EmitterPropertyGrid(GameManager game, ParticleType particle, ParticleEmitter e, ParticleEffectType effect)
		: base(game, "Emitter", e.Name, particle, e, effect) {

		PropertyGroup group;

		group = new PropertyGroup("General");
		propName			= (Property)group.AddProperty(new Property("Name", e.Name, "", delegate() {

			if (propName.Text.Length > 0 && !Resources.ParticleEmitterExists(propName.Text)) {
				Resources.RenameParticleEmitter(e.Name, propName.Text);
				name = propName.Text;
			}
			else
				propName.Text = e.Name;

			PropertyEmitterList.UpdateList();
		}));
		propSection			= (Property)group.AddProperty(new Property("Section Name", e.Comments, "", delegate() {

			e.Comments = propSection.Text;
		}));
		this.AddProperty(group);

		group = new PropertyGroup("Emitter");
		propEmitterArea		= (PropertyComboStruct)group.AddProperty(new PropertyComboStruct("Emitter Area", "", "", delegate() {
			e.Area = (EmitterArea)propEmitterArea.SelectionValue;
		}));
		propEmitterArea.AddItem(new PropertyPointArea("Point",
			(e.Area is PointArea ? (PointArea)e.Area : new PointArea(0, 0)), new PointArea(0, 0)));
		propEmitterArea.AddItem(new PropertyLineArea("Line",
			(e.Area is LineArea ? (LineArea)e.Area : new LineArea(0, 0, 0, 0)), new LineArea(0, 0, 0, 0)));
		propEmitterArea.AddItem(new PropertyRectArea("Rect",
			(e.Area is RectArea ? (RectArea)e.Area : new RectArea(0, 0, 0, 0)), new RectArea(0, 0, 0, 0)));
		propEmitterArea.AddItem(new PropertyCircleArea("Circle",
			(e.Area is CircleArea ? (CircleArea)e.Area : new CircleArea(0, 0, 0)), new CircleArea(0, 0, 0)));
		if (e.Area is PointArea)
			propEmitterArea.Selection = 0;
		if (e.Area is LineArea)
			propEmitterArea.Selection = 1;
		if (e.Area is RectArea)
			propEmitterArea.Selection = 2;
		if (e.Area is CircleArea)
			propEmitterArea.Selection = 3;


		propOrigin			= (PropertyVector2D)group.AddProperty(new PropertyVector2D("Origin", false, e.Origin, Vector2F.Zero, delegate() {
			e.Origin = propOrigin.Value;
		}));
		propOriginBased		= (PropertyBool)group.AddProperty(new PropertyBool("Origin Based Direction", e.IsOriginBasedDirection, false, delegate() {
			e.IsOriginBasedDirection = propOriginBased.Value;
		}));
		propSpeedScaled		= (PropertyBool)group.AddProperty(new PropertyBool("Speed Scaled by Distance", e.IsSpeedScaledByDistance, false, delegate() {
			e.IsSpeedScaledByDistance = propSpeedScaled.Value;
		}));
		propSpeed			= (PropertyRangeD)group.AddProperty(new PropertyRangeD("Speed", e.Speed, RangeF.Zero, delegate() {
			e.Speed = propSpeed.Value;
		}));
		propDirection		= (PropertyVector2D)group.AddProperty(new PropertyVector2D("Direction", false, e.Direction, Vector2F.Zero, delegate() {
			e.Direction = propDirection.Value;
		}));

		this.AddProperty(group);
	}
}
}
