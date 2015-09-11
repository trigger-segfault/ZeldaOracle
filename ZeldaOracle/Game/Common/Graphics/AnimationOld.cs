using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using XnaColor		= Microsoft.Xna.Framework.Color;

using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics {
/** <summary>
 * An animated collection of images for use with sprites.
 * </summary> */
public class AnimationOld {

	//========== CONSTANTS ===========
	#region Constants

	#endregion
	//=========== MEMBERS ============
	#region Members

	/** <summary> The list of frames in the animation. </summary> */
	[ContentSerializer(ElementName = "Frames", Optional = true)]
	private Frame[] frames = null;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs an animation for serialization. </summary> */
	public AnimationOld() {
		this.frames			= null;
	}
	/** <summary> Constructs an animation with the specified number of frames. </summary> */
	public AnimationOld(int numFrames) {
		this.frames			= new Frame[GMath.Max(0, numFrames)];
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties
	//--------------------------------
	#region Information

	/** <summary> Gets the frame at the specified index in the animation. </summary> */
	[ContentSerializerIgnore]
	public Frame this[int index] {
		get { return frames[index]; }
	}
	/** <summary> Gets the number of frames in the animation. </summary> */
	[ContentSerializerIgnore]
	public int NumFrames {
		get { return frames.Length; }
	}
	/** <summary> Gets the total duration of the animation. </summary> */
	[ContentSerializerIgnore]
	public int Duration {
		get {
			int duration = 0;
			foreach (Frame frame in frames) {
				duration += frame.Duration;
			}
			return duration;
		}
	}

	#endregion
	//--------------------------------
	#endregion
	//========== MANAGEMENT ==========
	#region Management
	
	/** <summary> Adds the specified frame to the next open slot in the animation. </summary> */
	public void AddFrame(Frame frame) {
		for (int i = 0; i < frames.Length; i++) {
			if (frames[i] == null) {
				frames[i] = frame;
				break;
			}
		}
	}
	/** <summary> Adds the new frame to the next open slot in the animation. </summary> */
	public void AddFrame(Texture2D image, int duration) {
		AddFrame(new Frame(image, duration));
	}
	/** <summary> Adds the new frame to the next open slot in the animation. </summary> */
	public void AddFrame(Texture2D image, int duration, Rectangle2D sourceRect) {
		AddFrame(new Frame(image, duration, sourceRect));
	}
	/** <summary> Adds the new frame to the next open slot in the animation. </summary> */
	public void AddFrame(Texture2D image, int duration, Rectangle2D sourceRect, Vector2D offset, Vector2D origin, double rotation, Vector2D scale) {
		AddFrame(new Frame(image, duration, sourceRect, offset, origin, rotation, scale));
	}
	/** <summary> Adds the new frame to the next open slot in the animation. </summary> */
	public void AddFrame(Texture2D image, int duration, Vector2D offset, Vector2D origin, double rotation, Vector2D scale) {
		AddFrame(new Frame(image, duration, offset, origin, rotation, scale));
	}

	#endregion
}
} // End namespace
