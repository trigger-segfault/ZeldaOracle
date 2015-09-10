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
 * The class that manages the creation of particles in an effect.
 * <para>Author: David Jordan</para>
 * </summary> */
public class ParticleEmitter {

	//========== CONSTANTS ===========
	#region Constants

	#endregion
	//=========== MEMBERS ============
	#region Members

	/** <summary> The name of the particle emitter. </summary> */
	private string name;
	/** <summary> The comments of the particle emitter in the script file. </summary> */
	private string comments;
	/** <summary> The area type of the emitter. </summary> */
	private EmitterArea area;
	/** <summary> True if the emitter uses direction based on the origin. </summary> */
	private bool originBasedDirection;
	/** <summary> The direction of the emitter as a vector. </summary> */
	private Vector2F direction;
	/** <summary> The origin of the emitter. </summary> */
	private Vector2F origin;
	/** <summary> The speed range of the emitted particles. </summary> */
	private RangeF speed;
	/** <summary> True if the speed of emitted particles is scaled by the distance from the origin. </summary> */
	private bool speedScaledByDistance;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default particle emitter. </summary> */
	public ParticleEmitter(EmitterArea area, double speed, bool speedScaledByDistance) :
		this(area, Vector2F.Zero, new RangeF(speed), speedScaledByDistance) {
	}
	/** <summary> Constructs the default particle emitter. </summary> */
	public ParticleEmitter(EmitterArea area, double speedMin, double speedMax, bool speedScaledByDistance) :
		this(area, Vector2F.Zero, new RangeF(speedMin, speedMax), speedScaledByDistance) {
	}
	/** <summary> Constructs the default particle emitter. </summary> */
	public ParticleEmitter(EmitterArea area, Vector2F origin, double speedMin, double speedMax, bool speedScaledByDistance) :
		this(area, origin, new RangeF(speedMin, speedMax), speedScaledByDistance) {
	}
	/** <summary> Constructs the default particle emitter. </summary> */
	public ParticleEmitter(EmitterArea area, Vector2F origin, RangeF speed, bool speedScaledByDistance) {
		this.area                  = area;
		this.originBasedDirection  = true;
		this.origin                = origin;
		this.speed                 = speed;
		this.speedScaledByDistance = speedScaledByDistance;
		this.direction             = Vector2F.Zero;
		this.comments = "";
	}
	/** <summary> Constructs the default particle emitter. </summary> */
	public ParticleEmitter(EmitterArea area) :
		this(area, Vector2F.Zero, new RangeF(0)) {
	}
	/** <summary> Constructs the default particle emitter. </summary> */
	public ParticleEmitter(EmitterArea area, Vector2F direction, RangeF speed) {
		this.area                  = area;
		this.originBasedDirection  = false;
		this.direction             = direction;
		this.speed                 = speed;
		this.speedScaledByDistance = false;
		this.origin                = Vector2F.Zero;
		this.comments = "";
	}
	/** <summary> Constructs the default particle emitter. </summary> */
	public ParticleEmitter() : this("") {

	}
	/** <summary> Constructs the default particle emitter. </summary> */
	public ParticleEmitter(string name) {
		this.name = name;
		area                  = new PointArea(0, 0);
		origin                = Vector2F.Zero;
		direction             = Vector2F.Zero;
		originBasedDirection  = false;
		speed                 = new RangeF(0);
		speedScaledByDistance = false;
		this.comments = "";
	}
	/** <summary> Constructs the default particle emitter. </summary> */
	public ParticleEmitter(ParticleEmitter copy) {
		name                  = copy.name;
		area                  = copy.Area.Copy();
		origin                = copy.origin;
		direction             = copy.direction;
		originBasedDirection  = copy.originBasedDirection;
		speed                 = copy.speed;
		speedScaledByDistance = copy.speedScaledByDistance;
		this.comments = copy.comments;
	}

	#endregion
	//========== PARTICLES ===========
	#region Particles

	/** <summary> Bursts all particles out of the emitter. </summary> */
	public void Burst(ParticleSystem system, ParticleType type, int count, Vector2F position) {
		for (int i = 0; i < count; ++i)
			EmitSingle(system, type, position);
	}
	/** <summary> Emits a single particle. </summary> */
	public void EmitSingle(ParticleSystem system, ParticleType type, Vector2F position) {
		Vector2F pos = area.GetRandomLocation();
		Vector2F vel = Vector2F.Zero;
		double spd = GRandom.NextDouble(speed);

		if (speedScaledByDistance)
			spd *= pos.DistanceTo(origin);

		if (originBasedDirection) {
			Vector2F v = pos - origin;
			if (v.LengthSquared < 0.00001f)
				vel = new Vector2F(spd, GRandom.NextDouble(360), true);
			else
				vel = v.Normalized * spd;
		}
		else
			vel = direction * spd;

		system.Create(type, position + pos, vel);
	}
	/** <summary> Emits a single particle. </summary> */
	public void Emit(ParticleSystem system, ParticleType type, Vector2F position, Vector2F velocity) {
		Vector2F v = area.GetRandomLocation();
		system.Create(type, position + v, velocity);
	}
	/** <summary> Emits multiple particles. </summary> */
	public void Emit(ParticleSystem system, ParticleType type, int count, Vector2F position, Vector2F velocity) {
		for (int i = 0; i < count; ++i)
			Emit(system, type, position, velocity);
	}


	#endregion
	//=========== SETTINGS ===========
	#region Settings

	/** <summary> Sets the motion of the emitter. </summary> */
	public void SetMotion(Vector2F velocityMin, Vector2F velocityMax) {
		direction = velocityMin.Normalized;
	}
	/** <summary> Sets the area of the emitter. </summary> */
	public void SetArea(EmitterArea area) {
		this.area = area;
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties

	/** <summary> Gets or sets the name of the particle emitter. </summary> */
	public string Name {
		get { return name; }
		set { name = value; }
	}
	/** <summary> Gets or sets the comments of the particle emitter in the script file. </summary> */
	public string Comments {
		get { return comments; }
		set { comments = value; }
	}
	/** <summary> Gets or sets the area type of the emitter. </summary> */
	public EmitterArea Area {
		get { return area; }
		set { area = value; }
	}
	/** <summary> Gets or sets the speed range of the emitted particles. </summary> */
	public RangeF Speed {
		get { return speed; }
		set { speed = value; }
	}
	/** <summary> Gets or sets the origin of the emitter. </summary> */
	public Vector2F Origin {
		get { return origin; }
		set { origin = value; }
	}
	/** <summary> Gets or sets the direction of the emitter as a vector. </summary> */
	public Vector2F Direction {
		get { return direction; }
		set { direction = value; }
	}
	/** <summary> Gets or sets if the speed of emitted particles is scaled by the distance from the origin. </summary> */
	public bool IsSpeedScaledByDistance {
		get { return speedScaledByDistance; }
		set { speedScaledByDistance = value; }
	}
	/** <summary> Gets or sets if the emitter uses direction based on the origin. </summary> */
	public bool IsOriginBasedDirection {
		get { return originBasedDirection; }
		set { originBasedDirection = value; }
	}
	
	#endregion
}
} // end namespace
