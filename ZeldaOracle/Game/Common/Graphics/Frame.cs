using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using XnaColor		= Microsoft.Xna.Framework.Color;

using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Content;

namespace ZeldaOracle.Common.Graphics {
/** <summary>
 * The frame used in animations.
 * </summary> */
public class Frame {

	//========== CONSTANTS ===========
	#region Constants

	#endregion
	//=========== MEMBERS ============
	#region Members
	
	// Containment
	/** <summary> The texture of the frame. </summary> */
	[ContentSerializerIgnore]
	private Texture2D texture = null;
	/** <summary> The name of the frame's texture. </summary> */
	[ContentSerializer(ElementName = "Texture")]
	private string textureName;

	// Animation
	/** <summary> The duration of the frame. </summary> */
	[ContentSerializer(ElementName = "Duration")]
	private int duration;

	// Transformation
	/** <summary> The source rectangle of the frame's image. </summary> */
	[ContentSerializer(ElementName = "SourceRect", Optional = true)]
	private Rectangle2D sourceRect = new Rectangle2D();
	/** <summary> The draw offset of the frame. </summary> */
	[ContentSerializer(ElementName = "Offset", Optional = true)]
	private Vector2D offset = new Vector2D();
	/** <summary> The draw origin of the frame. </summary> */
	[ContentSerializer(ElementName = "Origin", Optional = true)]
	private Vector2D origin = new Vector2D();
	/** <summary> The draw rotation of the frame. </summary> */
	[ContentSerializer(ElementName = "Rotation", Optional = true)]
	private double rotation = 0.0;
	/** <summary> The draw scale of the frame. </summary> */
	[ContentSerializer(ElementName = "Scale", Optional = true)]
	private Vector2D scale = new Vector2D(1, 1);

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs a frame for serialization. </summary> */
	public Frame() {
		// Containment
		this.texture		= null;
		this.textureName	= "";

		// Animation
		this.duration		= 0;

		// Transformation
		this.sourceRect		= Rectangle2D.Zero;
		this.offset			= Vector2D.Zero;
		this.origin			= Vector2D.Zero;
		this.rotation		= 0.0;
		this.scale			= new Vector2D(1, 1);
	}
	/** <summary> Constructs a frame with the specified image and duration. </summary> */
	public Frame(Texture2D image, int duration) {
		// Containment
		this.texture		= image;
		this.textureName	= "";

		// Animation
		this.duration		= duration;

		// Transformation
		this.sourceRect		= Rectangle2D.Zero;
		this.offset			= Vector2D.Zero;
		this.origin			= Vector2D.Zero;
		this.rotation		= 0.0;
		this.scale			= new Vector2D(1, 1);
	}
	/** <summary> Constructs a frame with the specified details. </summary> */
	public Frame(Texture2D image, int duration, Rectangle2D sourceRect) {
		// Containment
		this.texture		= image;
		this.textureName	= "";

		// Animation
		this.duration		= duration;

		// Transformation
		this.sourceRect		= sourceRect;
		this.offset			= Vector2D.Zero;
		this.origin			= Vector2D.Zero;
		this.rotation		= 0.0;
		this.scale			= new Vector2D(1, 1);
	}
	/** <summary> Constructs a frame with the specified details. </summary> */
	public Frame(Texture2D image, int duration, Rectangle2D sourceRect, Vector2D offset, Vector2D origin, double rotation, Vector2D scale) {
		// Containment
		this.texture		= image;
		this.textureName	= "";

		// Animation
		this.duration		= duration;

		// Transformation
		this.sourceRect		= sourceRect;
		this.offset			= offset;
		this.origin			= origin;
		this.rotation		= GMath.Plusdir(rotation);
		this.scale			= scale;
	}
	/** <summary> Constructs a frame with the specified details. </summary> */
	public Frame(Texture2D image, int duration, Vector2D offset, Vector2D origin, double rotation, Vector2D scale) {
		// Containment
		this.texture		= image;
		this.textureName	= "";

		// Animation
		this.duration		= duration;

		// Transformation
		this.sourceRect		= Rectangle2D.Zero;
		this.offset			= offset;
		this.origin			= origin;
		this.rotation		= GMath.Plusdir(rotation);
		this.scale			= scale;
	}

	/** <summary> Constructs a frame with the specified image and duration. </summary> */
	public Frame(string imageName, int duration) {
		// Containment
		this.texture		= null;
		this.textureName	= imageName;

		// Animation
		this.duration		= duration;

		// Transformation
		this.sourceRect		= Rectangle2D.Zero;
		this.offset			= Vector2D.Zero;
		this.origin			= Vector2D.Zero;
		this.rotation		= 0.0;
		this.scale			= new Vector2D(1, 1);
	}
	/** <summary> Constructs a frame with the specified details. </summary> */
	public Frame(string imageName, int duration, Rectangle2D sourceRect) {
		// Containment
		this.texture		= null;
		this.textureName	= imageName;

		// Animation
		this.duration		= duration;

		// Transformation
		this.sourceRect		= sourceRect;
		this.offset			= Vector2D.Zero;
		this.origin			= Vector2D.Zero;
		this.rotation		= 0.0;
		this.scale			= new Vector2D(1, 1);
	}
	/** <summary> Constructs a frame with the specified details. </summary> */
	public Frame(string imageName, int duration, Rectangle2D sourceRect, Vector2D offset, Vector2D origin, double rotation, Vector2D scale) {
		// Containment
		this.texture		= null;
		this.textureName	= imageName;

		// Animation
		this.duration		= duration;

		// Transformation
		this.sourceRect		= sourceRect;
		this.offset			= offset;
		this.origin			= origin;
		this.rotation		= GMath.Plusdir(rotation);
		this.scale			= scale;
	}
	/** <summary> Constructs a frame with the specified details. </summary> */
	public Frame(string imageName, int duration, Vector2D offset, Vector2D origin, double rotation, Vector2D scale) {
		// Containment
		this.texture		= null;
		this.textureName	= imageName;

		// Animation
		this.duration		= duration;

		// Transformation
		this.sourceRect		= Rectangle2D.Zero;
		this.offset			= offset;
		this.origin			= origin;
		this.rotation		= GMath.Plusdir(rotation);
		this.scale			= scale;
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties
	//--------------------------------
	#region Containment

	/** <summary> Gets the texture of the frame. </summary> */
	[ContentSerializerIgnore]
	public Texture2D Texture {
		get { return texture ?? ImageLoader.Get(textureName); }
	}

	#endregion
	//--------------------------------
	#region Animation

	/** <summary> Gets the duration of the frame. </summary> */
	[ContentSerializerIgnore]
	public int Duration {
		get { return duration; }
	}

	#endregion
	//--------------------------------
	#region Transformation

	/** <summary> Gets the source rectangle of the frame. </summary> */
	[ContentSerializerIgnore]
	public Rectangle2D SourceRect {
		get { return sourceRect; }
	}
	/** <summary> Gets the offset of the frame. </summary> */
	[ContentSerializerIgnore]
	public Vector2D Offset {
		get { return offset; }
	}
	/** <summary> Gets the origin of the frame. </summary> */
	[ContentSerializerIgnore]
	public Vector2D Origin {
		get { return origin; }
	}
	/** <summary> Gets the rotation of the frame. </summary> */
	[ContentSerializerIgnore]
	public double Rotation {
		get { return rotation; }
	}
	/** <summary> Gets the scale of the frame. </summary> */
	[ContentSerializerIgnore]
	public Vector2D Scale {
		get { return scale; }
	}

	#endregion
	//--------------------------------
	#endregion
}
} // End namespace
