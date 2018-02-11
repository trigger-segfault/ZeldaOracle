using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Common.Translation;

namespace ZeldaOracle.Game.Entities {

	public class RoomGraphics {

		// Possible Improvement:
		//  - Use an object pool of Drawing Instructions
		
		//-----------------------------------------------------------------------------
		// Internal Drawing Instruction Class
		//-----------------------------------------------------------------------------

		private class DrawingInstruction {
			// Sprite
			public ISprite sprite;
			public SpriteDrawSettings settings;

			// Text
			public GameFont font;
			public DrawableString text;
			public ColorOrPalette color;
			public Align alignment;
			public Vector2F area;

			// General
			public Vector2F position;
			public DrawingInstruction next;
			public Vector2F depthOrigin;

			public DrawingInstruction(ISprite sprite, SpriteDrawSettings settings,
				Vector2F position, Vector2F depthOrigin)
			{
				this.sprite			= sprite;
				this.position		= position;
				this.settings		= settings;
				this.depthOrigin	= depthOrigin;
				this.next			= null;
			}

			public DrawingInstruction(GameFont font, DrawableString text, Vector2F position,
				ColorOrPalette color, Align alignment, Vector2F area, Vector2F depthOrigin)
			{
				this.font			= font;
				this.text			= text;
				this.color			= color;
				this.alignment		= alignment;
				this.area			= area;
				this.position		= position;
				this.depthOrigin	= depthOrigin;
				this.next			= null;
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
					if (instruction.sprite != null) {
						g.DrawSprite(instruction.sprite, instruction.settings,
							instruction.position);
					}
					else if (instruction.font != null) {
						g.DrawWrappedString(instruction.font, instruction.text,
							(Point2I) instruction.position, instruction.color,
							instruction.alignment, instruction.area);
					}
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
		// Animation Player Drawing Functions (no depth origin)
		//-----------------------------------------------------------------------------

		// Draw an animation player.
		public void DrawAnimationPlayer(AnimationPlayer animationPlayer, Vector2F position, DepthLayer depth) {
			DrawAnimationPlayer(animationPlayer, animationPlayer.PlaybackTime, position, depth, Vector2F.Zero);
		}
		
		// Draw an animation player at the given playback time.
		public void DrawAnimationPlayer(AnimationPlayer animationPlayer, float time, Vector2F position, DepthLayer depth) {
			DrawAnimationPlayer(animationPlayer, time, position, depth, Vector2F.Zero);
		}

		// Draw an animation player.
		public void DrawAnimationPlayer(AnimationPlayer animationPlayer, SpriteDrawSettings settings, Vector2F position, DepthLayer depth) {
			DrawAnimationPlayer(animationPlayer, settings, position, depth, Vector2F.Zero);
		}

		// Draw an animation player at the given playback time.
		public void DrawAnimationPlayer(AnimationPlayer animationPlayer, float time, SpriteDrawSettings settings, Vector2F position, DepthLayer depth) {
			DrawAnimationPlayer(animationPlayer, time, settings, position, depth, Vector2F.Zero);
		}


		//-----------------------------------------------------------------------------
		// Animation Player Drawing Functions (with depth origin)
		//-----------------------------------------------------------------------------

		// Draw an animation player.
		public void DrawAnimationPlayer(AnimationPlayer animationPlayer, Vector2F position, DepthLayer depth, Vector2F depthOrigin) {

			DrawSprite(animationPlayer.SpriteOrSubStrip, new SpriteDrawSettings(animationPlayer.PlaybackTime), position, depth, depthOrigin);
		}

		// Draw an animation player at the given time.
		public void DrawAnimationPlayer(AnimationPlayer animationPlayer, float time, Vector2F position, DepthLayer depth, Vector2F depthOrigin) {

			DrawSprite(animationPlayer.SpriteOrSubStrip, new SpriteDrawSettings(time), position, depth, depthOrigin);
		}

		// Draw an animation player.
		public void DrawAnimationPlayer(AnimationPlayer animationPlayer, SpriteDrawSettings settings, Vector2F position, DepthLayer depth, Vector2F depthOrigin) {
			settings.PlaybackTime = animationPlayer.PlaybackTime;
			DrawSprite(animationPlayer.SpriteOrSubStrip, settings, position, depth, depthOrigin);
		}

		// Draw an animation player at the given time.
		public void DrawAnimationPlayer(AnimationPlayer animationPlayer, float time, SpriteDrawSettings settings, Vector2F position, DepthLayer depth, Vector2F depthOrigin) {
			settings.PlaybackTime = time;
			DrawSprite(animationPlayer.SpriteOrSubStrip, settings, position, depth, depthOrigin);
		}


		//-----------------------------------------------------------------------------
		// Sprite Drawing Functions (no depth origin)
		//-----------------------------------------------------------------------------

		public void DrawSprite(ISprite sprite, Vector2F position, DepthLayer depth) {
			DrawSprite(sprite, SpriteDrawSettings.Default, position, depth, Vector2F.Zero);
		}

		public void DrawSprite(ISprite sprite, SpriteDrawSettings settings, Vector2F position, DepthLayer depth) {
			DrawSprite(sprite, settings, position, depth, Vector2F.Zero);
		}


		//-----------------------------------------------------------------------------
		// Sprite Drawing Functions (with depth origin)
		//-----------------------------------------------------------------------------

		public void DrawSprite(ISprite sprite, Vector2F position, DepthLayer depth, Vector2F depthOrigin) {
			DrawSprite(sprite, SpriteDrawSettings.Default, position, depth, depthOrigin);
		}

		public void DrawSprite(ISprite sprite, SpriteDrawSettings settings, Vector2F position, DepthLayer depth, Vector2F depthOrigin) {
			if (sprite == null)
				return;

			settings.Styles = StyleDefinitions;

			position = GameUtil.Bias(position);

			DrawingInstruction instruction = new DrawingInstruction(
					sprite, settings, position, depthOrigin);

			// Add the instruction to the end of the linked list for its layer.
			AddInstruction(instruction, depth);
		}


		//-----------------------------------------------------------------------------
		// String Drawing Functions (without depth origin)
		//-----------------------------------------------------------------------------

		/// <summary>Draws a formatted game string at the specified position.</summary>
		public void DrawString(GameFont font, DrawableString text, Vector2F position, ColorOrPalette color, DepthLayer depth) {
			DrawString(font, text, position, color, Align.TopLeft, Vector2F.Zero, depth, Vector2F.Zero);
		}

		/// <summary>Draws a formatted game string at the specified position.</summary>
		public void DrawString(GameFont font, DrawableString text, Vector2F position, ColorOrPalette color, Align alignment, DepthLayer depth) {
			DrawString(font, text, position, color, alignment, Vector2F.Zero, depth, Vector2F.Zero);
		}

		/// <summary>Draws a formatted game string at the specified position.</summary>
		public void DrawString(GameFont font, DrawableString text, Vector2F position, ColorOrPalette color, Align alignment, Vector2F area, DepthLayer depth) {
			DrawString(font, text, position, color, alignment, area, depth, Vector2F.Zero);
		}


		//-----------------------------------------------------------------------------
		// String Drawing Functions (with depth origin)
		//-----------------------------------------------------------------------------

		/// <summary>Draws a formatted game string at the specified position.</summary>
		public void DrawString(GameFont font, DrawableString text, Vector2F position, ColorOrPalette color, DepthLayer depth, Vector2F depthOrigin) {
			DrawString(font, text, position, color, Align.TopLeft, Vector2F.Zero, depth, depthOrigin);
		}

		/// <summary>Draws a formatted game string at the specified position.</summary>
		public void DrawString(GameFont font, DrawableString text, Vector2F position, ColorOrPalette color, Align alignment, DepthLayer depth, Vector2F depthOrigin) {
			DrawString(font, text, position, color, alignment, Vector2F.Zero, depth, depthOrigin);
		}

		/// <summary>Draws a formatted game string at the specified position.</summary>
		public void DrawString(GameFont font, DrawableString text, Vector2F position, ColorOrPalette color, Align alignment, Vector2F area, DepthLayer depth, Vector2F depthOrigin) {
			if (font == null || text.IsNull)
				return;
			
			position = GameUtil.Bias(position);

			DrawingInstruction instruction = new DrawingInstruction(font, text,
				position, color, alignment, area, depthOrigin);

			// Add the instruction to the end of the linked list for its layer.
			AddInstruction(instruction, depth);
		}


		//-----------------------------------------------------------------------------
		// Internal Drawing
		//-----------------------------------------------------------------------------

		/// <summary>Add the instruction to the end of the linked list for its layer.</summary>
		private void AddInstruction(DrawingInstruction instruction, DepthLayer depth) {
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


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public StyleDefinitions StyleDefinitions {
			get { return roomControl.Zone.StyleDefinitions; }
		}
	}
}
