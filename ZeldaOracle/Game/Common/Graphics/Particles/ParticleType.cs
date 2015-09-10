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
 * The a particle type storing information about particles to be created.
 * <para>Author: David Jordan</para>
 * </summary> */
public class ParticleType {

	//========== CONSTANTS ===========
	#region Constants

	#endregion
	//=========== MEMBERS ============
	#region Members

	// General
	/** <summary> The name of the particle type. </summary> */
	private string name;
	/** <summary> The comments of the particle type in the script file. </summary> */
	private string comments;
	/** <summary> The life span of the particle type. </summary> */
	private RangeF lifeSpan;
	/** <summary> The fade delay of the particle type in seconds. </summary> */
	private double fadeDelay;

	// Scale
	/** <summary> The initial scale of the particle type. </summary> */
	private RangeF initialScale;
	/** <summary> The scale increase of the particle type. </summary> */
	private RangeF scaleIncrease;
	/** <summary> The scale jitter of the particle type. </summary> */
	private double scaleJitter;
	/** <summary> The scale clamp of the particle type. </summary> */
	private RangeF scaleClamp;

	// Speed
	/** <summary> The position jitter of the particle type. </summary> */
	private double positionJitter;
	/** <summary> The initial speed of the particle type. </summary> */
	private RangeF speed;
	/** <summary> The speed increase of the particle type. </summary> */
	private RangeF speedIncrease;
	/** <summary> The speed friction of the particle type. </summary> */
	private RangeF speedFriction;

	// Direction
	/** <summary> The initial direction of the particle type. </summary> */
	private RangeF initialDirectionOffset;
	/** <summary> The direction speed of the particle type. </summary> */
	private RangeF directionIncrease;
	/** <summary> The direction friction of the particle type. </summary> */
	private RangeF directionFriction;
	/** <summary> The gravity of the particle type. </summary> */
	private Vector2F gravity;

	// Rotation
	/** <summary> The initial rotation of the particle type. </summary> */
	private RangeF initialRotation;
	/** <summary> The rotation speed of the particle type. </summary> */
	private RangeF rotationSpeed;
	/** <summary> The rotation friction of the particle type. </summary> */
	private double rotationFriction;
	/** <summary> The rotation jitter of the particle type. </summary> */
	private double rotationJitter;
	/** <summary> True if the rotation of the particle type is based on the direction. </summary> */
	private bool rotateFromDirection;

	// Visual
	/** <summary> The drawable object of the particle type. </summary> */
	private Drawable drawable;
	/** <summary> The animation speed of the particle type. </summary> */
	private double animationSpeed;
	/** <summary> The initial depth of the particle type. </summary> */
	private RangeF initialDepth;
	/** <summary> The sprite list of the particle type. </summary> */
	private Sprite[] sprites;
	/** <summary> The color list of the particle type. </summary> */
	private Color[] colors;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default particle type. </summary> */
	public ParticleType() :
		this("") {
	}
	/** <summary> Constructs the default particle type. </summary> */
	public ParticleType(string name) {
		// General.
		this.name            = name;
		lifeSpan             = new RangeF(1);

		// Visual
		drawable             = new Drawable();
		sprites              = new Sprite[] { };
		animationSpeed       = 0;
		initialRotation      = RangeF.Zero;
		rotationSpeed        = RangeF.Zero;
		rotationJitter       = 0;
		rotateFromDirection  = false;
		rotationFriction     = 0;
		initialScale         = new RangeF(1);
		scaleIncrease        = RangeF.Zero;
		scaleJitter          = 0;
		scaleClamp           = new RangeF(-10000, 10000);
		initialDepth         = RangeF.Zero;
		colors               = new Color[] { Color.White };
		fadeDelay            = -1;

		// Motion.
		speed                  = RangeF.Zero;
		speedIncrease          = RangeF.Zero;
		speedFriction          = RangeF.Zero;
		initialDirectionOffset = RangeF.Zero;
		directionIncrease      = RangeF.Zero;
		directionFriction      = RangeF.Zero;
		gravity                = Vector2F.Zero;
		positionJitter         = 0;
		this.comments = "";
	}
	/** <summary> Constructs the default particle type. </summary> */
	public ParticleType(string name, ParticleType copy) :
		this(copy) {
		this.name = name;
	}
	/** <summary> Constructs the default particle type. </summary> */
	public ParticleType(ParticleType copy) {
		// General.
		name                 = copy.name;
		lifeSpan             = copy.lifeSpan;

		// Visual
		drawable             = new Drawable(copy.drawable);
		sprites              = new List<Sprite>(copy.sprites).ToArray();
		animationSpeed       = copy.animationSpeed;
		initialRotation      = copy.initialRotation;
		rotationSpeed        = copy.rotationSpeed;
		rotateFromDirection  = copy.rotateFromDirection;
		rotationJitter       = copy.rotationJitter;
		rotationFriction     = copy.rotationFriction;
		initialScale         = copy.initialScale;
		scaleIncrease        = copy.scaleIncrease;
		scaleJitter          = copy.scaleJitter;
		scaleClamp           = copy.scaleClamp;
		initialDepth         = copy.initialDepth;
		colors               = new List<Color>(copy.colors).ToArray();
		fadeDelay            = copy.fadeDelay;

		// Motion.
		speed                  = copy.speed;
		speedIncrease          = copy.speedIncrease;
		speedFriction          = copy.speedFriction;
		initialDirectionOffset = copy.initialDirectionOffset;
		directionIncrease      = copy.directionIncrease;
		directionFriction      = copy.directionFriction;
		gravity                = copy.gravity;
		positionJitter         = copy.positionJitter;
		this.comments = copy.comments;
	}

	#endregion
	//=========== SETTINGS ===========
	#region Settings

	/** <summary> Sets the animation. </summary> */
	/*public void SetAnimation(float animationSpeed, AnimationStrip strip) {
		this.animationSpeed = animationSpeed;
		this.sprites = strip.Frames.ToArray();
	}*/

	/** <summary> Sets the animation. </summary> */
	public void SetAnimation(float animationSpeed, params Sprite[] sprites) {
		this.animationSpeed = animationSpeed;
		this.sprites = sprites;
	}

	/** <summary> Sets the list of sprites. </summary> */
	public void SetSprite(params Sprite[] sprites) {
		this.sprites = sprites;
	}
	/** <summary> Sets the list of colors. </summary> */
	public void SetColors(params Color[] colors) {
		this.colors = colors;
	}
	/** <summary> Sets the life span as a single range. </summary> */
	public void SetLifeSpan(double lifeSpan) {
		SetLifeSpan(lifeSpan, lifeSpan);
	}
	/** <summary> Sets the life span. </summary> */
	public void SetLifeSpan(double lifeMin, double lifeMax) {
		lifeSpan = new RangeF(lifeMin, lifeMax);
	}
	/** <summary> Sets the scale. </summary> */
	public void SetScale(double initialScale, double scaleIncrease) {
		SetScale(initialScale, initialScale, scaleIncrease, scaleIncrease);
	}
	/** <summary> Sets the scale. </summary> */
	public void SetScale(double scaleMin, double scaleMax, double scaleIncrease) {
		SetScale(scaleMin, scaleMax, scaleIncrease, scaleIncrease);
	}
	/** <summary> Sets the scale. </summary> */
	public void SetScale(double scaleMin, double scaleMax, double scaleIncreaseMin, double scaleIncreaseMax) {
		this.initialScale  = new RangeF(scaleMin, scaleMax);
		this.scaleIncrease = new RangeF(scaleIncreaseMin, scaleIncreaseMax);
	}
	/** <summary> Sets the rotation. </summary> */
	public void SetRotation(double initialRot, double rotSpeed, double rotFriction, bool rotateWithDirection) {
		SetRotation(initialRot, initialRot, rotSpeed,
			rotFriction, rotateWithDirection);
	}
	/** <summary> Sets the rotation. </summary> */
	public void SetRotation(double initialRotMin, double initialRotMax,
		double rotSpeed, double rotFriction, bool rotateWithDirection) {
		SetRotation(initialRotMin, initialRotMax, rotSpeed, rotSpeed, rotFriction, rotateWithDirection);
	}
	/** <summary> Sets the rotation. </summary> */
	public void SetRotation(double initialRotMin, double initialRotMax,
							double rotSpeedMin, double rotSpeedMax,
							double rotFriction,
							bool rotateWithDirection) {
		this.initialRotation     = new RangeF(initialRotMin, initialRotMax);
		this.rotationSpeed       = new RangeF(rotSpeedMin, rotSpeedMax);
		this.rotationFriction    = rotFriction;
		this.rotateFromDirection = rotateWithDirection;
	}
	/** <summary> Sets the speed. </summary> */
	public void SetSpeed(double initialSpeedMin, double initialSpeedMax, double speedIncrease, double speedFriction) {
		SetSpeed(initialSpeedMin, initialSpeedMax, speedIncrease,
			speedIncrease, speedFriction, speedFriction);
	}
	/** <summary> Sets the speed. </summary> */
	public void SetSpeed(double initialSpeedMin, double initialSpeedMax,
							double speedIncreaseMin, double speedIncreaseMax,
							double speedFrictionMin, double speedFrictionMax) {
		this.speed         = new RangeF(initialSpeedMin, initialSpeedMax);
		this.speedIncrease = new RangeF(speedIncreaseMin, speedIncreaseMax);
		this.speedFriction = new RangeF(speedFrictionMin, speedFrictionMax);
	}
	/** <summary> Sets the direction. </summary> */
	public void SetDirection(double initialDirOffsetMin, double initialDirOffsetMax, double dirIncrease, double dirFriction) {
		SetDirection(initialDirOffsetMin, initialDirOffsetMax,
			dirIncrease, dirIncrease, dirFriction, dirFriction);
	}
	/** <summary> Sets the direction. </summary> */
	public void SetDirection(double initialDirOffsetMin, double initialDirOffsetMax,
								double dirIncreaseMin, double dirIncreaseMax,
								double dirFrictionMin, double dirFrictionMax) {
		this.initialDirectionOffset = new RangeF(initialDirOffsetMin, initialDirOffsetMax);
		this.directionIncrease = new RangeF(dirIncreaseMin, dirIncreaseMax);
		this.directionFriction = new RangeF(dirFrictionMin, dirFrictionMax);
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties
	//--------------------------------
	#region General

	/** <summary> Gets or sets the name of the particle type. </summary> */
	public string Name {
		get { return name; }
		set { name = value; }
	}
	/** <summary> Gets or sets the comments of the particle type in the script file. </summary> */
	public string Comments {
		get { return comments; }
		set { comments = value; }
	}
	/** <summary> Gets or sets the life span in seconds. </summary> */
	public RangeF LifeSpan {
		get { return lifeSpan; }
		set { lifeSpan = value; }
	}
	/** <summary> Gets or sets the fade delay in seconds. </summary> */
	public double FadeDelay {
		get { return fadeDelay; }
		set { fadeDelay = value; }
	}

	#endregion
	//--------------------------------
	#region Scale

	/** <summary> Gets or sets the initial scale. </summary> */
	public RangeF InitialScale {
		get { return initialScale; }
		set { initialScale = value; }
	}
	/** <summary> Gets or sets the scale increase. </summary> */
	public RangeF ScaleIncrease {
		get { return scaleIncrease; }
		set { scaleIncrease = value; }
	}
	/** <summary> Gets or sets the scale jitter. </summary> */
	public double ScaleJitter {
		get { return scaleJitter; }
		set { scaleJitter = value; }
	}
	/** <summary> Gets or sets the scale clamp. </summary> */
	public RangeF ScaleClamp {
		get { return scaleClamp; }
		set { scaleClamp = value; }
	}

	#endregion
	//--------------------------------
	#region Speed

	/** <summary> Gets or sets the position jitter. </summary> */
	public double PositionJitter {
		get { return positionJitter; }
		set { positionJitter = value; }
	}
	/** <summary> Gets or sets the initial speed. </summary> */
	public RangeF Speed {
		get { return speed; }
		set { speed = value; }
	}
	/** <summary> Gets or sets the acceleration. </summary> */
	public RangeF SpeedIncrease {
		get { return speedIncrease; }
		set { speedIncrease = value; }
	}
	/** <summary> Gets or sets the speed friction. </summary> */
	public RangeF SpeedFriction {
		get { return speedFriction; }
		set { speedFriction = value; }
	}

	#endregion
	//--------------------------------
	#region Direction

	/** <summary> Gets or sets the initial direction. </summary> */
	public RangeF InitialDirectionOffset {
		get { return initialDirectionOffset; }
		set { initialDirectionOffset = value; }
	}
	/** <summary> Gets or sets the direction speed. </summary> */
	public RangeF DirectionIncrease {
		get { return directionIncrease; }
		set { directionIncrease = value; }
	}
	/** <summary> Gets or sets the direction friction. </summary> */
	public RangeF DirectionFriction {
		get { return directionFriction; }
		set { directionFriction = value; }
	}
	/** <summary> Gets or sets the gravity. </summary> */
	public Vector2F Gravity {
		get { return gravity; }
		set { gravity = value; }
	}

	#endregion
	//--------------------------------
	#region Rotation

	/** <summary> Gets or sets the initial rotation in degrees. </summary> */
	public RangeF InitialRotation {
		get { return initialRotation; }
		set { initialRotation = value; }
	}
	/** <summary> Gets or sets the rotation speed in degrees. </summary> */
	public RangeF RotationSpeed {
		get { return rotationSpeed; }
		set { rotationSpeed = value; }
	}
	/** <summary> Gets or sets the rotation friction. </summary> */
	public double RotationFriction {
		get { return rotationFriction; }
		set { rotationFriction = value; }
	}
	/** <summary> Gets or sets the rotation jitter in degrees. </summary> */
	public double RotationJitter {
		get { return rotationJitter; }
		set { rotationJitter = value; }
	}
	/** <summary> Gets or sets if the rotation is set from the direction. </summary> */
	public bool RotateFromDirection {
		get { return rotateFromDirection; }
		set { rotateFromDirection = value; }
	}

	#endregion
	//--------------------------------
	#region Sprites

	/** <summary> Gets or sets the drawable object. </summary> */
	public Drawable Drawable {
		get { return drawable; }
		set { drawable = value; }
	}
	/** <summary> Gets or sets the animation speed. </summary> */
	public double AnimationSpeed {
		get { return animationSpeed; }
		set { animationSpeed = value; }
	}
	/** <summary> Gets or sets the initial depth. </summary> */
	public RangeF InitialDepth {
		get { return initialDepth; }
		set { initialDepth = value; }
	}
	/** <summary> Gets or sets the list of sprites. </summary> */
	public Sprite[] Sprites {
		get { return sprites; }
		set { sprites = value; }
	}
	/** <summary> Gets or sets the list of colors. </summary> */
	public Color[] Colors {
		get { return colors; }
		set { colors = value; }
	}

	#endregion
	//--------------------------------
	#endregion
}
} // end namespace
