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
public class EffectPropertyGrid : ParticlePropertyGridBase {

	// General
	public Property propName;
	public Property propSection;
	public PropertyList propEmitters;


	public EffectPropertyGrid(GameManager game, ParticleType particle, ParticleEmitter emitter, ParticleEffectType e)
		: base(game, "Effect", e.Name, particle, emitter, e) {


		PropertyGroup group;

		group = new PropertyGroup("General");
		propName			= (Property)group.AddProperty(new Property("Name", e.Name, "", delegate() {

			if (propName.Text.Length > 0 && !Resources.ParticleEffectExists(propName.Text)) {
				Resources.RenameParticleEffect(e.Name, propName.Text);
				name = propName.Text;
			}
			else
				propName.Text = e.Name;

			PropertyEffectList.UpdateList();
		}));
		propSection			= (Property)group.AddProperty(new Property("Section Name", e.Comments, "", delegate() {

			e.Comments = propSection.Text;
		}));
		this.AddProperty(group);

		group = new PropertyGroup("Effect");
		propEmitters = (PropertyList)group.AddProperty(new PropertyList("Actions", "Emitter", "Emitters", delegate() {
			PropertyComboStruct combo = new PropertyComboStruct("Emitter Area", null, null);
			combo.AddItem(new PropertyEffectAction("Burst", false));
			combo.AddItem(new PropertyEffectAction("Stream", true));
			combo.Selection = 0;
			return combo;
		}, delegate() {
			List<ParticleEffectAction> actions = new List<ParticleEffectAction>();

			for (int i = 0; i < propEmitters.PropertyCount; i++) {
				actions.Add((ParticleEffectAction)((PropertyComboStruct)propEmitters.Properties[i]).SelectionValue);
			}

			e.Actions = actions;
		}));

		for (int i = 0; i < e.Actions.Count; i++) {
			ParticleEffectAction action = e.Actions[i];

			PropertyComboStruct combo = new PropertyComboStruct("Emitter Area", null, null);
			if (action.IsStream) {
				combo.AddItem(new PropertyEffectAction("Burst", false));
				combo.AddItem(new PropertyEffectAction("Stream", action));
				combo.Selection = 1;
			}
			else {
				combo.AddItem(new PropertyEffectAction("Burst", action));
				combo.AddItem(new PropertyEffectAction("Stream", true));
				combo.Selection = 0;
			}
			propEmitters.AddProperty(combo);
		}
		propEmitters.IsExpanded = true;

		this.AddProperty(group);
	}
}
}
