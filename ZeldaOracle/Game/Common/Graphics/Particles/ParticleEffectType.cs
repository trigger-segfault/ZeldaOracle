using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZeldaOracle.Common;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Common.Graphics.Particles {
/** <summary>
 * The class that encompasses a collection of emitters to create an effect.
 * <para>Author: David Jordan</para>
 * </summary> */
public class ParticleEffectType {

	//========== CONSTANTS ===========
	#region Constants

	#endregion
	//=========== MEMBERS ============
	#region Members

	/** <summary> The name of the particle effect type. </summary> */
	private string name;
	/** <summary> The comments of the particle effect in the script file. </summary> */
	private string comments;
	/** <summary> The collection of emitter actions in the particle effect. </summary> */
	private List<ParticleEffectAction> actions;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default particle effect type. </summary> */
	public ParticleEffectType() :
		this("") {
	}
	/** <summary> Constructs the default particle effect type. </summary> */
	public ParticleEffectType(string name) {
		this.name = name;
		this.comments = "";
		this.actions = new List<ParticleEffectAction>();
	}
	/** <summary> Constructs the default particle effect type. </summary> */
	public ParticleEffectType(ParticleEffectType clone) {
		this.name = clone.name;
		this.comments = clone.comments;
		this.actions = new List<ParticleEffectAction>();
		for (int i = 0; i < clone.actions.Count; i++) {
			this.actions.Add(new ParticleEffectAction(clone.actions[i]));
		}
	}

	#endregion
	//========== PARTICLES ===========
	#region Particles

	/** <summary> Adds a new burst action to the effect. </summary> */
	public void AddBurst(ParticleEmitter emitter, ParticleType type, int count, double delay = 0.0f) {
		AddAction(new ParticleEffectAction(emitter, type, count, -1, delay, false));
	}
	/** <summary> Adds a new stream action to the effect. </summary> */
	public void AddStream(ParticleEmitter emitter, ParticleType type, double frequency, double delay = 0.0f) {
		AddStream(emitter, type, frequency, -1, delay);
	}
	/** <summary> Adds a new stream action to the effect. </summary> */
	public void AddStream(ParticleEmitter emitter, ParticleType type, double frequency, double duration, double delay = 0.0f) {
		AddAction(new ParticleEffectAction(emitter, type, frequency, duration, delay, true));
	}
	/** <summary> Adds a new action to the effect. </summary> */
	public ParticleEffectAction AddAction(ParticleEffectAction action) {
		actions.Add(action);
		return action;
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties

	/** <summary> Gets or sets the name of the particle effect. </summary> */
	public string Name {
		get { return name; }
		set { name = value; }
	}
	/** <summary> Gets or sets the comments of the particle effect in the script file. </summary> */
	public string Comments {
		get { return comments; }
		set { comments = value; }
	}
	/** <summary> Gets or sets the list of actions in the particle effect. </summary> */
	public List<ParticleEffectAction> Actions {
		get { return actions; }
		set { actions = value; }
	}

	#endregion
}
} // end namespace
