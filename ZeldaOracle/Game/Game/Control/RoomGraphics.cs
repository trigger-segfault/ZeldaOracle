using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Control;

namespace ZeldaOracle.Game.Entities {

	public class RoomGraphics {

		// Possible Improvement:
		//  - Use an object pool of Drawing Instructions
		
		//-----------------------------------------------------------------------------
		// Internal Drawing Instruction Class
		//-----------------------------------------------------------------------------

		private class DrawingInstruction {
			public Sprite sprite;
			public float x;
			public float y;
			public int imageVariant;
			public DrawingInstruction next;

			public DrawingInstruction(Sprite sprite, int imageVariant, float x, float y) {
				this.sprite = sprite;
				this.x = x;
				this.y = y;
				this.imageVariant = imageVariant;
				this.next = null;
			}
		}

		
		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------
		
		public RoomControl roomControl;
		private DrawingInstruction[] layerHeads; // The heads of the drawing instruction queues for each layer.
		private DrawingInstruction[] layerTails; // The tails of the drawing instruction queues for each layer.


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public RoomGraphics(RoomControl roomControl) {
			this.roomControl	= roomControl;
			this.layerHeads		= new DrawingInstruction[(int) DepthLayer.Count];
			this.layerTails		= new DrawingInstruction[(int) DepthLayer.Count];
		}

		
		//-----------------------------------------------------------------------------
		// Rendering Flow
		//-----------------------------------------------------------------------------
		
		// Clear all drawing instructions.
		public void Clear() {
			// Clear the instruction queues for each depth layer.
			for (int i = 0; i < layerHeads.Length; i++) {
				layerHeads[i] = null;
				layerTails[i] = null;
			}
		}

		// Draw all the queued drawing instructions to a graphics object.
		public void DrawAll(Graphics2D g) {
			// Draw all instructions from the lowest layer to the highest layer.
			for (int i = 0; i < layerHeads.Length; i++) {
				DrawingInstruction instruction = layerHeads[i];

				while (instruction != null) {
					g.DrawSprite(instruction.sprite,
								 instruction.imageVariant,
								 instruction.x,
								 instruction.y);
					instruction = instruction.next;
				}
			}
		}

		
		//-----------------------------------------------------------------------------
		// Entity Drawing Functions
		//-----------------------------------------------------------------------------
		
		// Draw an animation player.
		public void DrawAnimation(AnimationPlayer animationPlayer, Vector2F position, DepthLayer depth) {
			DrawAnimation(animationPlayer, 0, position, depth);
		}
		
		// Draw an sprite or animation at the given playback time.
		public void DrawAnimation(SpriteAnimation spriteAnimation, float time, Vector2F position, DepthLayer depth) {
			DrawAnimation(spriteAnimation, 0, time, position, depth);
		}
		
		// Draw an animation at the given playback time.
		public void DrawAnimation(Animation animation, float time, Vector2F position, DepthLayer depth) {
			DrawAnimation(animation, 0, time, position, depth);
		}
		
		// Draw a sprite.
		public void DrawSprite(Sprite sprite, Vector2F position, DepthLayer depth) {
			DrawSprite(sprite, 0, position, depth);
		}

		
		//-----------------------------------------------------------------------------
		// Entity Drawing Functions (With Variants)
		//-----------------------------------------------------------------------------
		
		// Draw an animation player.
		public void DrawAnimation(AnimationPlayer animationPlayer, int imageVariant, Vector2F position, DepthLayer depth) {
			if (animationPlayer.SubStrip != null)
				DrawAnimation(animationPlayer.SubStrip, imageVariant, animationPlayer.PlaybackTime, position, depth);
		}
		
		// Draw an sprite or animation at the given playback time.
		public void DrawAnimation(SpriteAnimation spriteAnimation, int imageVariant, float time, Vector2F position, DepthLayer depth) {
			if (spriteAnimation.IsAnimation)
				DrawAnimation(spriteAnimation.Animation, imageVariant, time, position, depth);
			else
				DrawSprite(spriteAnimation.Sprite, imageVariant, position, depth);
		}
		
		// Draw an animation at the given playback time.
		public void DrawAnimation(Animation animation, int imageVariant, float time, Vector2F position, DepthLayer depth) {
			if (animation.LoopMode == LoopMode.Repeat) {
				if (animation.Duration == 0)
					time = 0;
				else
					time %= animation.Duration;
			}

			position.X = GMath.Round(position.X);
			position.Y = GMath.Round(position.Y);

			for (int i = 0; i < animation.Frames.Count; ++i) {
				AnimationFrame frame = animation.Frames[i];
				if (time < frame.StartTime)
					return;
				if (time < frame.StartTime + frame.Duration ||
					(time >= animation.Duration &&
					frame.StartTime + frame.Duration == animation.Duration))
				{
					DrawSprite(frame.Sprite, imageVariant, position, depth);
				}
			}
		}
		
		// Draw a sprite.
		public void DrawSprite(Sprite sprite, int imageVariant, Vector2F position, DepthLayer depth) {
			DrawingInstruction instruction = new DrawingInstruction(
					sprite, imageVariant, position.X, position.Y);

			// Add the instruction to the end of the linked list for its layer.
			int layerIndex = (int) depth;
			if (layerHeads[layerIndex] == null) {
				layerHeads[layerIndex] = instruction;
				layerTails[layerIndex] = instruction;
			}
			else {
				layerTails[layerIndex].next = instruction;
				layerTails[layerIndex] = instruction;
			}
		}
	}
}
