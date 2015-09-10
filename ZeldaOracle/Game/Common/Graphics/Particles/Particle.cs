using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaColor		= Microsoft.Xna.Framework.Color;

using ZeldaOracle.Common;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using Color			= ZeldaOracle.Common.Graphics.Color;

namespace ZeldaOracle.Common.Graphics.Particles {
/** <summary>
 * The main class for all particles.
 * <para>Author: David Jordan</para>
 * </summary> */
public class Particle {

	//========== CONSTANTS ===========
	#region Constants

	#endregion
	//=========== MEMBERS ============
	#region Members

	// General
	/** <summary> The system containing this particle. </summary> */
	private ParticleSystem system;
	/** <summary> The type of this particle. </summary> */
	private ParticleType type;
	/** <summary> The age of the particle in seconds. </summary> */
	private double age;
	/** <summary> True if the particle is alive. </summary> */
	private bool alive;
	/** <summary> The life span of this particle in seconds. </summary> */
	private double lifeSpan;

	// Visual
	/** <summary> The drawable object for this particle. </summary> */
	private Drawable drawable;
	/** <summary> The frame index of the particle's animation. </summary> */
	private double frameIndex;
	/** <summary> The rotation speed of the particle. </summary> */
	private double rotationSpeed;
	/** <summary> The scale increase of the particle over time. </summary> */
	private double scaleIncrease;

	// Motion
	/** <summary> The position of the particle. </summary> */
	private Vector2F position;
	/** <summary> The velocity of the particle. </summary> */
	private Vector2F velocity;
	/** <summary> The speed multiplier of the particle. </summary> */
	private double speedVelocity;
	/** <summary> The speed friction of the particle. </summary> */
	private double speedFriction;
	/** <summary> The directional speed of the particle. </summary> */
	private double directionVelocity;
	/** <summary> The directional friction of the particle. </summary> */
	private double directionFriction;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default particle and adds the particle system reference. </summary> */
	public Particle(ParticleSystem system) {
		this.system = system;
	}
	/** <summary> Initializes the particle type, position, and velocity. </summary> */
	public void Initialize(ParticleType type, Vector2F initialPosition, Vector2F initialVelocity) {
		this.type = type;

		// General
		alive    = true;
		age      = 0;
		lifeSpan = GRandom.NextDouble(type.LifeSpan);

		// Visual
		drawable           = new Drawable(type.Drawable);
		drawable.Rotation += GRandom.NextDouble(type.InitialRotation);
		drawable.Scale    *= GRandom.NextDouble(type.InitialScale);
		drawable.Depth     = GRandom.NextDouble(type.InitialDepth);
		rotationSpeed      = GRandom.NextDouble(type.RotationSpeed);
		scaleIncrease      = GRandom.NextDouble(type.ScaleIncrease);
		if (type.Colors.Length > 0)
			drawable.Color = type.Colors[0];

		// Sprite / Animation
		frameIndex      = GRandom.NextInt(type.Sprites.Length);
		drawable.Sprite = null;
		if (type.Sprites.Length > 0)
			drawable.Sprite = type.Sprites[(int)frameIndex];

		// Motion
		speedVelocity     = GRandom.NextDouble(type.SpeedIncrease);
		speedFriction     = GRandom.NextDouble(type.SpeedFriction);
		directionVelocity = GRandom.NextDouble(type.DirectionIncrease);
		directionFriction = GRandom.NextDouble(type.DirectionFriction);
		position          = initialPosition;
		velocity          = initialVelocity;
		velocity.Direction += GRandom.NextDouble(type.InitialDirectionOffset);
		if (type.RotateFromDirection)
			drawable.Rotation += velocity.Direction;
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties
	//--------------------------------
	#region Containment

	/** <summary> Gets or sets the particle system of this particle. </summary> */
	public ParticleSystem ParticleSystem {
		get { return system; }
		set { system = value; }
	}
	/** <summary> Gets or sets the type of this particle. </summary> */
	public ParticleType ParticleType {
		get { return type; }
		set { type = value; }
	}
	/** <summary> Gets or sets if the particle is alive. </summary> */
	public bool IsAlive {
		get { return alive; }
		set { alive = value; }
	}
	/** <summary> Gets or sets the drawable for the particle. </summary> */
	public Drawable Drawable {
		get { return drawable; }
		set { drawable = value; }
	}

	#endregion
	//--------------------------------
	#region Visual

	/** <summary> Gets or sets the scale increase of the particle. </summary> */
	public double ScaleIncrease {
		get { return scaleIncrease; }
		set { scaleIncrease = value; }
	}

	public Vector2F Position {
		get { return position; }
		set { position = value; }
	}
	
	#endregion
	//--------------------------------
	#endregion
	//=========== UPDATING ===========
	#region Updating

	/** <summary> Called every step to update the particle. </summary> */
	public void Update(double time) {
		// Life
		age += time;
		if (age >= lifeSpan) {
			Destroy();
			return;
		}

		// Animation
		if (type.Sprites.Length > 1) {
			frameIndex = (frameIndex + (time * type.AnimationSpeed)) % type.Sprites.Length;
			drawable.Sprite = type.Sprites[(int) frameIndex];
		}

		// Sprite rotation and scale
		drawable.Rotation += rotationSpeed * time;
		drawable.Rotation *= 1.0 - type.RotationFriction;
		drawable.Rotation += GRandom.NextDouble(-type.RotationJitter, type.RotationJitter);
		drawable.Scale    += scaleIncrease * time;
		drawable.Scale    += GRandom.NextDouble(-type.ScaleJitter, type.ScaleJitter);

		position.X += GRandom.NextDouble(-type.PositionJitter, type.PositionJitter);
		position.Y += GRandom.NextDouble(-type.PositionJitter, type.PositionJitter);

		drawable.Scale = GMath.Clamp(drawable.Scale, type.ScaleClamp.Min, type.ScaleClamp.Max);// MathHelper.Clamp(drawable.Scale, type.ScaleClamp.Min, type.ScaleClamp.Max);

		// Color
		if (type.Colors.Length > 1) {
			double colorIndex = Math.Min(type.Colors.Length - 1,
				(age / lifeSpan) * (type.Colors.Length - 1));
			double colorLerp = colorIndex - (int)colorIndex;
			drawable.Color = (Color)XnaColor.Lerp(
				((XnaColor)type.Colors[(int)colorIndex]),
				((XnaColor)type.Colors[(int) colorIndex + 1]),
				(float)colorLerp);
		}

		// Fading
		if (type.FadeDelay > 0) {
			double alpha = (lifeSpan - age) / type.FadeDelay;
			alpha = Math.Max(0, Math.Min(1, alpha));
			XnaColor color = (XnaColor)drawable.Color;
			color *= (float)alpha;
			drawable.Color = (Color)color;
		}

		// Translational motion
		position += velocity * time;
		velocity += type.Gravity * time;
		velocity *= 1.0f - speedFriction;
		if (speedVelocity != 0) {
			double length = velocity.Length;
			velocity = velocity.Normalized;
			velocity *= length + (speedVelocity * time);
		}

		// Directional motion
		if (directionVelocity != 0) {
			velocity.Direction += directionVelocity * time;
			if (type.RotateFromDirection)
				drawable.Rotation += directionVelocity * time;
		}
	}

	#endregion
	//=========== DRAWING ============
	#region Drawing

	/** <summary> Called every step to draw the particle. </summary> */
	public void Draw(Graphics2D g) {
		drawable.Draw(g, position);
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
