using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ZeldaOracle.Common;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Common.Graphics.Particles {
/** <summary>
 * The main class for containing multiple particle emitters creating a single effect.
 * <para>Author: David Jordan</para>
 * </summary> */
public class ParticleEffect {

	//========== CONSTANTS ===========
	#region Constants

	#endregion
	//=========== MEMBERS ============
	#region Members

	/** <summary> The system containing this particle. </summary> */
	private ParticleSystem system;
	/** <summary> The type of this effect. </summary> */
	private ParticleEffectType effectType;
	/** <summary> The timer in seconds used by the effect. </summary> */
	private double timer;
	/** <summary> The duration used by the effect in seconds. </summary> */
	private double duration;
	/** <summary> The position of the effect. </summary> */
	private Vector2F position;
	/** <summary> True if the effect is alive. </summary> */
	private bool alive;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default particle effect and adds the particle system reference. </summary> */
	public ParticleEffect(ParticleSystem system) {
		this.system = system;
	}
	/** <summary> Initializes the particle effect type and position. </summary> */
	public void Initialize(ParticleEffectType effectType, Vector2F position) {
		this.effectType = effectType;
		this.position   = position;
		this.timer      = 0;
		this.alive      = true;

		duration = 0;
		for (int i = 0; i < effectType.Actions.Count; ++i) {
			ParticleEffectAction action = effectType.Actions[i];
			if (action.IsStream && action.Duration < 0) {
				duration = -1;
				break;
			}
			duration = GMath.Max(duration, action.Delay + action.Duration);
		}
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties

	/** <summary> Gets or sets the particle system of this particle effect. </summary> */
	public ParticleSystem ParticleSystem {
		get { return system; }
		set { system = value; }
	}
	/** <summary> Gets or sets the type of this particle effect. </summary> */
	public ParticleEffectType EffectType {
		get { return effectType; }
		set { effectType = value; }
	}
	/** <summary> Gets or sets if the particle effect is alive. </summary> */
	public bool IsAlive {
		get { return alive; }
		set { alive = false; }
	}
	/** <summary> Gets or sets the position of the particle effect. </summary> */
	public Vector2F Position {
		get { return position; }
		set { position = value; }
	}

	#endregion
	//=========== UPDATING ===========
	#region Updating

	/** <summary> Called every step to update the particle effect. </summary> */
	public void Update(double deltaTime) {
		for (int i = 0; i < effectType.Actions.Count; ++i) {
			ParticleEffectAction action = effectType.Actions[i];
			int count = action.Count;

			if (action.IsStream && (action.Duration < 0 || timer < action.Delay + action.Duration)) {
				int c1 = (int)(action.Frequency * timer);
				int c2 = (int)(action.Frequency * (timer + deltaTime));
				count = c2 - c1;
			}

			if (count > 0)
				action.Emitter.Burst(system, action.Type, count, position);
		}

		timer += deltaTime;

		if (duration >= 0 && timer >= duration)
			alive = false;
	}

	#endregion
	//========== MANAGEMENT ==========
	#region Management

	/** <summary> Destroys the particle and marks it for removal. </summary> */
	public void Destroy() {
		alive = false;
	}

	#endregion
}
} // end namespace
