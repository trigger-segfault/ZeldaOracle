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
			public Vector2F position;
			public int imageVariant;
			public DrawingInstruction next;
			public Vector2F depthOrigin;

			public DrawingInstruction(Sprite sprite, int imageVariant, Vector2F position, Vector2F depthOrigin) {
				this.sprite			= sprite;
				this.position		= position;
				this.imageVariant	= imageVariant;
				this.next			= null;
				this.depthOrigin	= depthOrigin;
			}
		}

		
		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------
		
		public RoomControl roomControl;
		private DrawingInstruction[] layerHeads; // The heads of the drawing instruction queues for each layer.
		private DrawingInstruction[] layerTails; // The tails of the drawing instruction queues for each layer.
		private int[] layerCounts;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public RoomGraphics(RoomControl roomControl) {
			this.roomControl	= roomControl;
			this.layerHeads		= new DrawingInstruction[(int) DepthLayer.Count];
			this.layerTails		= new DrawingInstruction[(int) DepthLayer.Count];
			this.layerCounts	= new int[(int) DepthLayer.Count];
		}

		
		//-----------------------------------------------------------------------------
		// Rendering Flow
		//-----------------------------------------------------------------------------
		
		// Clear all drawing instructions.
		public void Clear() {
			// Clear the instruction queues for each depth layer.
			for (int i = 0; i < layerHeads.Length; i++) {
				layerHeads[i]	= null;
				layerTails[i]	= null;
				layerCounts[i]	= 0;
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
								 instruction.position.X,
								 instruction.position.Y);
					instruction = instruction.next;
				}
			}
		}

		// Sort a depth layer. Sprites with smaller y coordinates
		// will be drawn before ones with larger y coordinates.
		public void SortDepthLayer(DepthLayer layer) {
			// Check if the layer is empty.
			int layerIndex = (int) layer;
			if (layerHeads[layerIndex] == null)
				return;

			// Create a list of the drawing instructions.
			List<DrawingInstruction> instructions = new List<DrawingInstruction>(layerCounts[layerIndex]);
			DrawingInstruction instruction = layerHeads[layerIndex];
			while (instruction != null) {
				instructions.Add(instruction);
				instruction = instruction.next;
			}

			// Sort the list by depth origin Y.
			instructions.Sort(delegate(DrawingInstruction a, DrawingInstruction b) {
				if (Math.Round(a.depthOrigin.Y) < Math.Round(b.depthOrigin.Y))
					return -1;
				return 1;
			});

			// Update the drawing instruction linked list.
			layerHeads[layerIndex] = instructions[0];
			layerTails[layerIndex] = instructions[instructions.Count - 1];
			for (int i = 0; i < instructions.Count; i++) {
				if (i + 1 < instructions.Count)
					instructions[i].next = instructions[i + 1];
				else
					instructions[i].next = null;
			}
		}
		
		
		//-----------------------------------------------------------------------------
		// Entity Drawing Functions (no variants, no depth origin)
		//-----------------------------------------------------------------------------

		// Draw an animation player.
		public void DrawAnimation(AnimationPlayer animationPlayer, Vector2F position, DepthLayer depth) {
			DrawAnimation(animationPlayer, 0, position, depth, Vector2F.Zero);
		}
		
		// Draw an sprite or animation at the given playback time.
		public void DrawAnimation(SpriteAnimation spriteAnimation, float time, Vector2F position, DepthLayer depth) {
			DrawAnimation(spriteAnimation, 0, time, position, depth, Vector2F.Zero);
		}
		
		// Draw an animation at the given playback time.
		public void DrawAnimation(Animation animation, float time, Vector2F position, DepthLayer depth) {
			DrawAnimation(animation, 0, time, position, depth, Vector2F.Zero);
		}
		
		// Draw a sprite.
		public void DrawSprite(Sprite sprite, Vector2F position, DepthLayer depth) {
			DrawSprite(sprite, 0, position, depth, Vector2F.Zero);
		}

		
		//-----------------------------------------------------------------------------
		// Entity Drawing Functions (with variants, no depth origin)
		//-----------------------------------------------------------------------------
		
		// Draw an animation player.
		public void DrawAnimation(AnimationPlayer animationPlayer, int imageVariant, Vector2F position, DepthLayer depth) {
			DrawAnimation(animationPlayer, imageVariant, position, depth, Vector2F.Zero);
		}
		
		// Draw an sprite or animation at the given playback time.
		public void DrawAnimation(SpriteAnimation spriteAnimation, int imageVariant, float time, Vector2F position, DepthLayer depth) {
			DrawAnimation(spriteAnimation, imageVariant, time, position, depth, Vector2F.Zero);
		}
		
		// Draw an animation at the given playback time.
		public void DrawAnimation(Animation animation, int imageVariant, float time, Vector2F position, DepthLayer depth) {
			DrawAnimation(animation, imageVariant, time, position, depth, Vector2F.Zero);
		}
		
		// Draw a sprite.
		public void DrawSprite(Sprite sprite, int imageVariant, Vector2F position, DepthLayer depth) {
			DrawSprite(sprite, imageVariant, position, depth, Vector2F.Zero);
		}

		
		//-----------------------------------------------------------------------------
		// Entity Drawing Functions (no variants, with depth origin)
		//-----------------------------------------------------------------------------
		
		// Draw an animation player.
		public void DrawAnimation(AnimationPlayer animationPlayer, Vector2F position, DepthLayer depth, Vector2F depthOrigin) {
			DrawAnimation(animationPlayer, 0, position, depth, depthOrigin);
		}
		
		// Draw an sprite or animation at the given playback time.
		public void DrawAnimation(SpriteAnimation spriteAnimation, float time, Vector2F position, DepthLayer depth, Vector2F depthOrigin) {
			DrawAnimation(spriteAnimation, 0, time, position, depth, depthOrigin);
		}
		
		// Draw an animation at the given playback time.
		public void DrawAnimation(Animation animation, float time, Vector2F position, DepthLayer depth, Vector2F depthOrigin) {
			DrawAnimation(animation, 0, time, position, depth, depthOrigin);
		}
		
		// Draw a sprite.
		public void DrawSprite(Sprite sprite, Vector2F position, DepthLayer depth, Vector2F depthOrigin) {
			DrawSprite(sprite, 0, position, depth, depthOrigin);
		}

		
		//-----------------------------------------------------------------------------
		// Entity Drawing Functions (with variants, with depth origin)
		//-----------------------------------------------------------------------------
		
		// Draw an animation player.
		public void DrawAnimation(AnimationPlayer animationPlayer, int imageVariant, Vector2F position, DepthLayer depth, Vector2F depthOrigin) {
			if (animationPlayer.SubStrip != null)
				DrawAnimation(animationPlayer.SubStrip, imageVariant, animationPlayer.PlaybackTime, position, depth, depthOrigin);
		}
		
		// Draw an sprite or animation at the given playback time.
		public void DrawAnimation(SpriteAnimation spriteAnimation, int imageVariant, float time, Vector2F position, DepthLayer depth, Vector2F depthOrigin) {
			if (spriteAnimation.IsAnimation)
				DrawAnimation(spriteAnimation.Animation, imageVariant, time, position, depth, depthOrigin);
			else
				DrawSprite(spriteAnimation.Sprite, imageVariant, position, depth, depthOrigin);
		}
		
		// Draw an animation at the given playback time.
		public void DrawAnimation(Animation animation, int imageVariant, float time, Vector2F position, DepthLayer depth, Vector2F depthOrigin) {
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
					DrawSprite(frame.Sprite, imageVariant, position, depth, depthOrigin);
				}
			}
		}
		
		// Draw a sprite.
		public void DrawSprite(Sprite sprite, int imageVariant, Vector2F position, DepthLayer depth, Vector2F depthOrigin) {
			DrawingInstruction instruction = new DrawingInstruction(
					sprite, imageVariant, position, depthOrigin);

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
			layerCounts[layerIndex]++;
		}
	}
}
