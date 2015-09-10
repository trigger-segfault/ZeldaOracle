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
 * The main class for managing a collection of particle effects and particles
 * <para>Author: David Jordan</para>
 * </summary> */
public class ParticleSystem {

	//========== CONSTANTS ===========
	#region Constants

	#endregion
	//=========== MEMBERS ============
	#region Members

	/** <summary> The list of particle effects in the system. </summary> */
	private List<ParticleEffect> effects;
	/** <summary> The list of particles in the system. </summary> */
	private List<Particle> particles;
	/** <summary> The draw mode used by the particle system. </summary> */
	private DrawMode drawMode;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the particle system. </summary> */
	public ParticleSystem() {
		particles = new List<Particle>();
		effects   = new List<ParticleEffect>();
		drawMode  = new DrawMode();
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties

	/** <summary> Gets or sets the draw mode of particle system. </summary> */
	public DrawMode DrawMode {
		get { return drawMode; }
		set { drawMode = value; }
	}
	/** <summary> Gets the number of particles in the particle system. </summary> */
	public int ParticleCount {
		get { return particles.Count; }
	}
	/** <summary> Gets the number of effects in the particle system. </summary> */
	public int EffectCount {
		get { return effects.Count; }
	}
	/** <summary> Gets the list of particles in the particle system. </summary> */
	public List<Particle> Particles {
		get { return particles; }
	}
	/** <summary> Gets the list of effects in the particle system. </summary> */
	public List<ParticleEffect> Effects {
		get { return effects; }
	}

	#endregion
	//=========== EFFECTS ============
	#region Effects

	/** <summary> Creates a new instance of a particle effect of the specified type at the given position. </summary> */
	public ParticleEffect CreateEffect(ParticleEffectType type, Vector2F position) {
		ParticleEffect e = new ParticleEffect(this);
		e.Initialize(type, position);
		effects.Add(e);
		return e;
	}
	/** <summary> Creates a new particle of the specified type, initial position, and initial velocity. </summary> */
	public Particle Create(ParticleType type, Vector2F position, Vector2F velocity) {
		Particle p = new Particle(this);
		p.Initialize(type, position, velocity);
		particles.Add(p);
		return p;
	}
	/** <summary> Clears all particles in the system. </summary> */
	public void Clear() {
		particles.Clear();
		effects.Clear();
	}

	#endregion
	//=========== UPDATING ===========
	#region Updating

	/** <summary> Updates the particles and effects in the system. </summary> */
	public void Update(double time) {
		// Update effects
		for (int i = 0; i < effects.Count; ++i) {
			effects[i].Update(time);
			if (!effects[i].IsAlive)
				effects.RemoveAt(i--);
		}

		// Update particles
		for (int i = 0; i < particles.Count; ++i) {
			particles[i].Update(time);
			if (!particles[i].IsAlive)
				particles.RemoveAt(i--);
		}
	}

	#endregion
	//=========== DRAWING ============
	#region Drawing

	/** <summary> Draws all the particles in the system. </summary> */
	public void Draw(Graphics2D g) {
		g.Begin(drawMode);

		// Draw particles
		for (int i = 0; i < particles.Count; ++i)
			particles[i].Draw(g);

		g.End();
	}

	#endregion
}
} // end namespace
