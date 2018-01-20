using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaColor = Microsoft.Xna.Framework.Color;

using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Translation;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Common.Graphics {

	// The 2D graphics object class for simplifying drawing.
	public class Graphics2D {

		// Containment:
		/// <summary>The sprite batch of the graphics object.</summary>
		private SpriteBatch spriteBatch;
		/// <summary>The previous render targets of the graphics object.</summary>
		private Stack<RenderTargetBinding[]> preivousRenderTargets;

		// Drawing Settings:
		/// <summary>The current translated position of the graphics object.</summary>
		private Vector2F translation;
		/// <summary>The stack of translations deviating from (0, 0).</summary>
		private Stack<Vector2F> translationStack;
		/// <summary>True if the graphics translation will be taken into account.</summary>
		private bool useTranslation;
		/// <summary>True if all draws will use integer precision.</summary>
		private bool useIntPrecision;

		// Vector Graphics:
		/// <summary>A white 1x1 texture used for drawing vector graphics.</summary>
		private Texture2D white1x1;
		/// <summary>A white 2x2 texture used for drawing vector graphics.</summary>
		private Texture2D white2x2;

	
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		// Constructs a 2D graphics object containing the sprite batch.
		public Graphics2D(SpriteBatch spriteBatch) {
			// Containment
			this.spriteBatch			= spriteBatch;
			this.preivousRenderTargets	= new Stack<RenderTargetBinding[]>();

			// Drawing Settings
			this.translation			= Vector2F.Zero;
			this.translationStack		= new Stack<Vector2F>();
			this.useTranslation			= true;
			this.useIntPrecision		= false;

			// Vector Graphics
			this.white1x1				= new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
			this.white2x2				= new Texture2D(spriteBatch.GraphicsDevice, 2, 2);
			this.white1x1.SetData(new XnaColor[] { XnaColor.White });
			this.white2x2.SetData(new XnaColor[] { XnaColor.White, XnaColor.White, XnaColor.White, XnaColor.White });
		}
	

		//-----------------------------------------------------------------------------
		// Operators
		//-----------------------------------------------------------------------------

		// Cast to a SpriteBatch.
		public static explicit operator SpriteBatch(Graphics2D graphics) {
			return graphics.spriteBatch;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Containment.

		/// <summary>Gets the sprite batch of the graphics object.</summary>
		public SpriteBatch SpriteBatch {
			get { return spriteBatch; }
		}
		/// <summary>Gets the graphics device of the graphics object.</summary>
		public GraphicsDevice GraphicsDevice {
			get { return spriteBatch.GraphicsDevice; }
		}
		/// <summary>Returns true if the sprite batch has been disposed.</summary>
		public bool IsDisposed {
			get { return spriteBatch.IsDisposed; }
		}

		//Drawing Settings.

		/// <summary>Gets the current translation of the graphics object.</summary>
		public Vector2F Translation {
			get { return translation; }
		}
		/// <summary>Gets or sets if the translation is used.</summary>
		public bool UseTranslation {
			get { return useTranslation; }
			set { useTranslation = value; }
		}
		/// <summary>Gets or sets if integer precision is used with translations.</summary>
		public bool UseIntegerPrecision {
			get { return useIntPrecision; }
			set { useIntPrecision = value; }
		}


		//-----------------------------------------------------------------------------
		// Drawing Internal
		//-----------------------------------------------------------------------------

		/// <summary>Translates the position based on the translation and precision settings.</summary>
		private Vector2 NewPos(Vector2F position) {
			if (useTranslation)
				return (UseIntegerPrecision ? (Vector2) GMath.Round(position + translation) : (Vector2)(position + translation));
			else
				return (Vector2) position;
		}

		/// <summary>Translates the position of the string based on the translation and precision settings.</summary>
		private Vector2 NewStringPos(Vector2F position, SpriteFont font, string text, Align alignment) {
			Vector2F stringSize = font.MeasureString(text);
			bool intAlign = (alignment & Align.Int) != 0;
			if (((alignment & Align.Left) != 0) == ((alignment & Align.Right) != 0))
				position.X -= (intAlign ? (int)(stringSize.X / 2.0f) : (stringSize.X / 2.0f));
			else if ((alignment & Align.Right) != 0)
				position.X -= stringSize.X;
			if (((alignment & Align.Top) != 0) == ((alignment & Align.Bottom) != 0))
				position.Y -=(intAlign ? (int)(stringSize.Y / 2.0f) : (stringSize.Y / 2.0f));
			else if ((alignment & Align.Bottom) != 0)
				position.Y -= stringSize.Y;

			if (useTranslation)
				return (UseIntegerPrecision ? (Vector2) GMath.Floor(position + translation) : (Vector2) (position + translation));
			else
				return (Vector2) position;
		}

		/// <summary>Translates the rectangle based on the translation and precision settings.</summary>
		private Rectangle NewRect(Rectangle2F destinationRect) {
			if (useTranslation)
				return (UseIntegerPrecision ? (Rectangle) new Rectangle2F(GMath.Round(destinationRect.Point + translation), GMath.Round(destinationRect.Size)) : (Rectangle) (destinationRect + translation));
			else
				return (Rectangle) destinationRect;
		}


		//-----------------------------------------------------------------------------
		// Image Drawing
		//-----------------------------------------------------------------------------

		/// <summary>Draws the image at the specified position.</summary>
		public void DrawImage(Texture2D texture, Vector2F position) {
			spriteBatch.Draw(texture, NewPos(position), XnaColor.White);
		}
		/// <summary>Draws the image at the specified position.</summary>
		public void DrawImage(Texture2D texture, Vector2F position, Color color, double depth = 0.0) {
			spriteBatch.Draw(texture, NewPos(position), null, color, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, (float)depth);
		}
		/// <summary>Draws the image at the specified position.</summary>
		public void DrawImage(Texture2D texture, Vector2F position, Vector2F origin, Vector2F scale, double rotation, Color? color = null, Flip flip = Flip.None, double depth = 0.0) {
			spriteBatch.Draw(texture, NewPos(position), null, color ?? XnaColor.White, (float)GMath.ConvertToRadians(rotation), (Vector2)origin, (Vector2)scale, (SpriteEffects) flip, (float)depth);
		}
		/// <summary>Draws the image at the specified region.</summary>
		public void DrawImage(Texture2D texture, Rectangle2F destinationRect, Vector2F origin, double rotation, Color? color = null, Flip flip = Flip.None, double depth = 0.0) {
			spriteBatch.Draw(texture, NewRect(destinationRect), null, color ?? XnaColor.White, (float)GMath.ConvertToRadians(rotation), (Vector2)origin, (SpriteEffects) flip, (float)depth);
		}

		/// <summary>Draws part of the image at the specified position.</summary>
		public void DrawImage(Texture2D texture, Vector2F position, Rectangle2F sourceRect) {
			spriteBatch.Draw(texture, NewPos(position), (Rectangle)sourceRect, XnaColor.White);
		}
		/// <summary>Draws part of the image at the specified position.</summary>
		public void DrawImage(Texture2D texture, Vector2F position, Rectangle2F sourceRect, Color color, double depth = 0.0) {
			spriteBatch.Draw(texture, NewPos(position), (Rectangle)sourceRect, color, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, (float)depth);
		}
		/// <summary>Draws part of the image at the specified position.</summary>
		public void DrawImage(Texture2D texture, Vector2F position, Rectangle2F sourceRect, Vector2F origin, Vector2F scale, double rotation, Color? color = null, Flip flip = Flip.None, double depth = 0.0) {
			spriteBatch.Draw(texture, NewPos(position), (Rectangle)sourceRect, color ?? XnaColor.White, (float)GMath.ConvertToRadians(rotation), (Vector2)origin, (Vector2)scale, (SpriteEffects) flip, (float)depth);
		}
		// Draws part of the image at the specified region.</summary>
		public void DrawImage(Texture2D texture, Rectangle2F destinationRect, Rectangle2F sourceRect, Vector2F origin, double rotation, Color? color = null, Flip flip = Flip.None, double depth = 0.0) {
			spriteBatch.Draw(texture, NewRect(destinationRect), (Rectangle)sourceRect, color ?? XnaColor.White, (float)GMath.ConvertToRadians(rotation), (Vector2)origin, (SpriteEffects) flip, (float)depth);
		}


		//-----------------------------------------------------------------------------
		// Sprite drawing
		//-----------------------------------------------------------------------------
	

		public void DrawSprite(ISprite sprite, int variantID, Vector2F position, float depth = 0.0f) {
			if (sprite == null) return;
			foreach (SpritePart part in sprite.GetParts(new SpriteDrawSettings(variantID))) {
				Image image = part.Image.GetVariant(variantID);
				if (image.Texture.IsDisposed)
					Console.Write("");
				spriteBatch.Draw(image, NewPos(position) + (Vector2) part.CenterDrawOffset, (Rectangle) part.SourceRect,
					XnaColor.White, part.RotationRadians, (Vector2) part.Center, 1.0f, (SpriteEffects) part.FlipEffects, depth);
			}
		}

		public void DrawSprite(ISprite sprite, float x, float y, float depth = 0.0f) {
			DrawSprite(sprite, new Vector2F(x, y), depth);
		}

		public void DrawSprite(ISprite sprite, int variantID, float x, float y, float depth = 0.0f) {
			DrawSprite(sprite, variantID, new Vector2F(x, y), depth);
		}

		public void DrawSprite(ISprite sprite, Vector2F position, float depth = 0.0f) {
			if (sprite == null) return;
			foreach (SpritePart part in sprite.GetParts(SpriteDrawSettings.Default)) {
				if (part.Image.Texture.IsDisposed)
					Console.Write("");
				spriteBatch.Draw(part.Image, NewPos(position) + (Vector2) part.CenterDrawOffset, (Rectangle) part.SourceRect,
					XnaColor.White, part.RotationRadians, (Vector2) part.Center, 1.0f, (SpriteEffects) part.FlipEffects, depth);
			}
		}

		public void DrawSprite(ISprite sprite, Rectangle2F destination, float depth = 0.0f) {
			DrawSprite(sprite, 0, destination, depth);
		}

		public void DrawSprite(ISprite sprite, int variantID, Rectangle2F destination, float depth = 0.0f) {
			if (sprite == null) return;
			foreach (SpritePart part in sprite.GetParts(new SpriteDrawSettings(variantID))) {
				Image image = part.Image.GetVariant(variantID);
				if (image.Texture.IsDisposed)
					Console.Write("");
				destination.Point = NewPos(destination.Point) + (Vector2F) part.DrawOffset;
				spriteBatch.Draw(image, (Rectangle) destination, (Rectangle) part.SourceRect,
					XnaColor.White, 0.0f, Vector2.Zero, SpriteEffects.None, depth);
			}
		}

		public void DrawSprite(ISprite sprite, int variantID, Vector2F position, Color color, float depth = 0.0f) {
			if (sprite == null) return;
			foreach (SpritePart part in sprite.GetParts(new SpriteDrawSettings(variantID))) {
				Image image = part.Image.GetVariant(variantID);
				if (image.Texture.IsDisposed)
					Console.Write("");
				spriteBatch.Draw(image, NewPos(position) + (Vector2) part.CenterDrawOffset, (Rectangle) part.SourceRect,
					(XnaColor) color, part.RotationRadians, (Vector2) part.Center, 1.0f, (SpriteEffects) part.FlipEffects, depth);
			}
		}

		public void DrawSprite(ISprite sprite, float x, float y, Color color, float depth = 0.0f) {
			DrawSprite(sprite, new Vector2F(x, y), color, depth);
		}

		public void DrawSprite(ISprite sprite, int variantID, float x, float y, Color color, float depth = 0.0f) {
			DrawSprite(sprite, variantID, new Vector2F(x, y), color, depth);
		}

		public void DrawSprite(ISprite sprite, Vector2F position, Color color, float depth = 0.0f) {
			if (sprite == null) return;
			foreach (SpritePart part in sprite.GetParts(SpriteDrawSettings.Default)) {
				if (part.Image.Texture.IsDisposed)
					Console.Write("");
				spriteBatch.Draw(part.Image, NewPos(position) + (Vector2) part.CenterDrawOffset, (Rectangle)part.SourceRect,
					(XnaColor)color, part.RotationRadians, (Vector2) part.Center, 1.0f, (SpriteEffects) part.FlipEffects, depth);
			}
		}

		public void DrawSprite(ISprite sprite, Rectangle2F destination, Color color, float depth = 0.0f) {
			DrawSprite(sprite, 0, destination, color, depth);
		}

		public void DrawSprite(ISprite sprite, int variantID, Rectangle2F destination, Color color, float depth = 0.0f) {
			if (sprite == null) return;
			foreach (SpritePart part in sprite.GetParts(new SpriteDrawSettings(variantID))) {
				Image image = part.Image.GetVariant(variantID);
				destination.Point = NewPos(destination.Point) + (Vector2F)part.DrawOffset;
				if (image.Texture.IsDisposed)
					Console.Write("");
				spriteBatch.Draw(image, (Rectangle)destination, (Rectangle)part.SourceRect,
					(XnaColor) color, 0.0f, Vector2.Zero, SpriteEffects.None, depth);
			}
		}

		//-----------------------------------------------------------------------------
		// Animation drawing
		//-----------------------------------------------------------------------------
	
		// Draw an animation during at the given time stamp and position.
		public void DrawAnimation(Animation animation, float time, Vector2F position, float depth = 0.0f) {
			DrawAnimation(animation, time, position.X, position.Y, depth);
		}

		// Draw an animation during at the given time stamp and position.
		public void DrawAnimation(Animation animation, float time, float x, float y, float depth = 0.0f) {
			DrawAnimation(animation, 0, time, x, y, depth);
		}
	
		// Draw an animation during at the given time stamp and position.
		public void DrawAnimation(Animation animation, int variantID, float time, Vector2F position, float depth = 0.0f) {
			DrawAnimation(animation, variantID, time, position.X, position.Y, depth);
		}

		// Draw an animation during at the given time stamp and position.
		public void DrawAnimation(Animation animation, int variantID, float time, float x, float y, float depth = 0.0f) {
			DrawSprite(animation, new SpriteDrawSettings(variantID, time), new Vector2F(x, y), depth);

			/*if (animation.LoopMode == LoopMode.Repeat) {
				if (animation.Duration == 0)
					time = 0;
				else
					time %= animation.Duration;
			}
			x = GMath.Round(x); 
			y = GMath.Round(y);

			for (int i = 0; i < animation.FrameCount; ++i) {
				AnimationFrameOld frame = animation.Frames[i];
				if (time < frame.StartTime)
					return;
				if (time < frame.StartTime + frame.Duration || (time >= animation.Duration && frame.StartTime + frame.Duration == animation.Duration))
					DrawSprite(frame.Sprite, variantID, x, y, depth);
			}*/
		}

		public void DrawAnimation(AnimationPlayer animationPlayer, Vector2F position, float depth = 0.0f) {
			DrawSprite(animationPlayer.SpriteOrSubStrip, new SpriteDrawSettings(animationPlayer.PlaybackTime), position, depth);
		}

		public void DrawAnimation(AnimationPlayer animationPlayer, int variantID, Vector2F position, float depth = 0.0f) {
			DrawSprite(animationPlayer.SpriteOrSubStrip, new SpriteDrawSettings(variantID, animationPlayer.PlaybackTime), position, depth);
		}


		// Draw an animation during at the given time stamp and position.
		public void DrawAnimation(Animation animation, float time, Vector2F position, Color color, float depth = 0.0f) {
			DrawAnimation(animation, time, position.X, position.Y, color, depth);
		}

		// Draw an animation during at the given time stamp and position.
		public void DrawAnimation(Animation animation, float time, float x, float y, Color color, float depth = 0.0f) {
			DrawAnimation(animation, 0, time, x, y, color, depth);
		}

		// Draw an animation during at the given time stamp and position.
		public void DrawAnimation(Animation animation, int variantID, float time, Vector2F position, Color color, float depth = 0.0f) {
			DrawAnimation(animation, variantID, time, position.X, position.Y, color, depth);
		}

		// Draw an animation during at the given time stamp and position.
		public void DrawAnimation(Animation animation, int variantID, float time, float x, float y, Color color, float depth = 0.0f) {
			DrawSprite(animation, new SpriteDrawSettings(variantID, time), new Vector2F(x, y), color, depth);

			/*if (animation.LoopMode == LoopMode.Repeat) {
				if (animation.Duration == 0)
					time = 0;
				else
					time %= animation.Duration;
			}
			x = GMath.Round(x);
			y = GMath.Round(y);

			for (int i = 0; i < animation.FrameCount; ++i) {
				AnimationFrameOld frame = animation.Frames[i];
				if (time < frame.StartTime)
					return;
				if (time < frame.StartTime + frame.Duration || (time >= animation.Duration && frame.StartTime + frame.Duration == animation.Duration))
					DrawSprite(frame.Sprite, variantID, x, y, color, depth);
			}*/
		}

		public void DrawAnimation(AnimationPlayer animationPlayer, Vector2F position, Color color, float depth = 0.0f) {
			DrawAnimation(animationPlayer.SubStrip, animationPlayer.PlaybackTime, position, color, depth);
		}


		//-----------------------------------------------------------------------------
		// ISprite drawing
		//-----------------------------------------------------------------------------
		
		public void DrawSprite(ISprite sprite, SpriteDrawSettings settings, float x, float y, float depth = 0.0f) {
			DrawSprite(sprite, settings, new Vector2F(x, y), Color.White, depth);
		}

		public void DrawSprite(ISprite sprite, SpriteDrawSettings settings, Vector2F position, float depth = 0.0f) {
			DrawSprite(sprite, settings, position, Color.White, depth);
		}

		public void DrawSprite(ISprite sprite, SpriteDrawSettings settings, float x, float y, Color color, float depth = 0.0f) {
			DrawSprite(sprite, settings, new Vector2F(x, y), color, depth);
		}

		public void DrawSprite(ISprite sprite, SpriteDrawSettings settings, Vector2F position, Color color, float depth = 0.0f) {
			if (sprite == null) return;
			foreach (SpritePart part in sprite.GetParts(settings)) {
				Image image = part.Image.GetVariant(settings.VariantID);
				if (image.Texture.IsDisposed)
					Console.Write("");
				spriteBatch.Draw(image, NewPos(position) + (Vector2) part.CenterDrawOffset, (Rectangle) part.SourceRect,
					(XnaColor) color, part.RotationRadians, (Vector2) part.Center, 1.0f, (SpriteEffects) part.FlipEffects, depth);
			}
		}
		
		public void DrawSprite(ISprite sprite, SpriteDrawSettings settings, Rectangle2F destination, Color color, float depth = 0.0f) {
			if (sprite == null) return;
			foreach (SpritePart part in sprite.GetParts(settings)) {
				Image image = part.Image.GetVariant(settings.VariantID);
				if (image.Texture.IsDisposed)
					Console.Write("");
				destination.Point = NewPos(destination.Point) + part.CenterDrawOffset;
				spriteBatch.Draw(image, (Rectangle) destination, (Rectangle) part.SourceRect,
					(XnaColor) color, part.RotationRadians, (Vector2) part.Center, (SpriteEffects) part.FlipEffects, depth);
			}
		}


		//-----------------------------------------------------------------------------
		// Text Drawing
		//-----------------------------------------------------------------------------

		// Draws the string at the specified position.
		public void DrawRealString(SpriteFont font, string text, Vector2F position, Align alignment, Color color) {
			spriteBatch.DrawString(font, text, NewStringPos(position, font, text, alignment), (XnaColor)color);
		}

		// Draws the string at the specified position.
		public void DrawRealString(SpriteFont font, string text, Vector2F position, Align alignment,
			Color color, Vector2F origin, double rotation, double scale, SpriteEffects flipEffect = SpriteEffects.None, float depth = 0.0f)
		{
			spriteBatch.DrawString(font, text, NewStringPos(position, font, text, alignment),
				(XnaColor)color, (float)rotation, (Vector2)origin, (float)scale, flipEffect, depth);
		}

		// Draws the string at the specified position.
		public void DrawRealString(SpriteFont font, string text, Vector2F position, Align alignment,
				Color color, Vector2F origin, double rotation, Vector2F scale,
				SpriteEffects flipEffect = SpriteEffects.None, float depth = 0.0f)
		{
			spriteBatch.DrawString(font, text, NewStringPos(position, font, text, alignment), (XnaColor)color, (float)rotation, (Vector2)origin, (Vector2)scale, flipEffect, depth);
		}

		// Draws a game string at the specified position
		public void DrawString(GameFont font, string text, Point2I position, Color color, float depth = 0.0f) {
			DrawLetterString(font, FormatCodes.FormatString(text), position, color, depth);
		}
		// Draws a formatted game string at the specified position
		public void DrawLetterString(GameFont font, LetterString letterString, Point2I position, Color color, float depth = 0.0f) {
			for (int i = 0; i < letterString.Length; i++) {
				spriteBatch.Draw(
					font.SpriteSheet.Image,
					NewRect(new Rectangle2I(
						position.X + i * (font.SpriteSheet.CellSize.X + font.CharacterSpacing),
						position.Y,
						font.SpriteSheet.CellSize.X,
						font.SpriteSheet.CellSize.Y
					)),
					(Rectangle)new Rectangle2I(
						font.SpriteSheet.Offset.X + ((int)letterString[i].Char % font.CharactersPerRow) * (font.SpriteSheet.CellSize.X + font.SpriteSheet.Spacing.X),
						font.SpriteSheet.Offset.Y + ((int)letterString[i].Char / font.CharactersPerRow) * (font.SpriteSheet.CellSize.Y + font.SpriteSheet.Spacing.Y),
						font.SpriteSheet.CellSize.X,
						font.SpriteSheet.CellSize.Y
					),
					(letterString[i].Color == Letter.DefaultColor ? color : letterString[i].Color), 0.0f, Vector2.Zero, SpriteEffects.None, depth
				);
			}
		}
		// Draws a wrapped game string at the specified position
		public void DrawWrappedString(GameFont font, string text, int width, Point2I position, Color color, float depth = 0.0f) {
			DrawWrappedLetterString(font, font.WrapString(text, width), width, position, color, depth);
		}
		// Draws a formatted wrapped game string at the specified position
		public void DrawWrappedLetterString(GameFont font, WrappedLetterString wrappedString, int width, Point2I position, Color color, float depth = 0.0f) {
			for (int i = 0; i < wrappedString.Lines.Length; i++) {
				DrawLetterString(font, wrappedString.Lines[i], position + new Point2I(0, font.LineSpacing * i), color, depth);
			}
		}


		//-----------------------------------------------------------------------------
		// Vector graphics
		//-----------------------------------------------------------------------------

		// Draws the specified line.
		public void DrawLine(Line2F line, float thickness, Color color, float depth = 0.0f) {
			DrawImage(white1x1, line.Center + new Vector2F(0.5f, 0.5f), new Vector2F(0.5f, 0.5f),
				new Vector2F((line.Length + thickness), thickness),
				line.Direction, color, Flip.None, depth);
		}

		// Draws the specified rectangle.
		public void DrawRectangle(Rectangle2F rect, float thickness, Color color, float depth = 0.0f) {
			FillRectangle(new Rectangle2F(rect.Left, rect.Top, rect.Width, 1.0f), color, depth);
			FillRectangle(new Rectangle2F(rect.Left, rect.Bottom - 1, rect.Width, 1.0f), color, depth);
			FillRectangle(new Rectangle2F(rect.Left, rect.Top + 1, 1.0f, rect.Height - 2), color, depth);
			FillRectangle(new Rectangle2F(rect.Right - 1, rect.Top + 1, 1.0f, rect.Height - 2), color, depth);
			//DrawLine(new Line2F(rect.Point, rect.Point + new Vector2F(rect.Width - 1, 0.0f)), thickness, color, depth);
			//DrawLine(new Line2F(rect.Point, rect.Point + new Vector2F(0.0f, rect.Height - 1)), thickness, color, depth);
			//DrawLine(new Line2F(rect.Point + rect.Size - Vector2F.One, rect.Point + new Vector2F(rect.Width - 1, 0.0f)), thickness, color, depth);
			//DrawLine(new Line2F(rect.Point + rect.Size - Vector2F.One, rect.Point + new Vector2F(0.0f, rect.Height - 1)), thickness, color, depth);
		}

		// Draws the specified filled rectangle.
		public void FillRectangle(Rectangle2F rect, Color color, double depth = 0.0) {
			rect.X = GMath.Round(rect.X);
			rect.Y = GMath.Round(rect.Y);
			DrawImage(white1x1, rect, Vector2F.Zero, 0.0, color, Flip.None, depth);
			//DrawImage(white1x1, rect.Center + 0.5, new Vector2D(0.5, 0.5), rect.Size, 0, color, SpriteEffects.None, depth);
		}
	

		//-----------------------------------------------------------------------------
		// Debug drawing
		//-----------------------------------------------------------------------------
	
		// Draw a collision model as a solid color.
		public void DrawCollisionModel(CollisionModel model, Vector2F position, Color color) {
			for (int i = 0; i < model.Boxes.Count; i++)
				FillRectangle(Rectangle2F.Translate(model.Boxes[i], position), color);
		}


		//-----------------------------------------------------------------------------
		// Misc. Drawing
		//-----------------------------------------------------------------------------

		// Clears the render target to black.
		public void Clear() {
			spriteBatch.GraphicsDevice.Clear(XnaColor.Black);
		}

		// Clears the render target.
		public void Clear(Color color) {
			spriteBatch.GraphicsDevice.Clear(color);
		}


		//-----------------------------------------------------------------------------
		// Settings
		//-----------------------------------------------------------------------------

		// Translates the origin of the graphics object.
		/*public void Translate(float x, float y) {
			translation += new Vector2F(x, y);
		}*/

		// Translates the origin of the graphics object.
		/*public void Translate(Vector2F distance) {
			translation += distance;
		}*/

		/// <summary>Pushes the new translation onto the translation stack.</summary>
		public void PushTranslation(float x, float y) {
			translation += new Vector2F(x, y);
			translationStack.Push(translation);
		}

		/// <summary>Pushes the new translation onto the translation stack.</summary>
		public void PushTranslation(Vector2F distance) {
			translation += distance;
			translationStack.Push(translation);
		}

		/// <summary>Pops the current translation from the translation stack.</summary>
		public void PopTranslation() {
			translationStack.Pop();
			translation = (translationStack.Any() ? translationStack.Peek() : Vector2F.Zero);
		}

		/// <summary>Pops the current translation from the translation stack.</summary>
		public void PopTranslation(int popCount) {
			for (int i = 0; i < popCount; i++) {
				translationStack.Pop();
			}
			translation = (translationStack.Any() ? translationStack.Peek() : Vector2F.Zero);
		}

		/// <summary>Resets the translation of the graphics object and clears the translation stack.</summary>
		public void ResetTranslation() {
			translation = Vector2F.Zero;
			translationStack.Clear();
		}


		//-----------------------------------------------------------------------------
		// Begin/End
		//-----------------------------------------------------------------------------

		// Begins the drawing to the sprite batch.
		public void Begin() {
			spriteBatch.Begin();
		}

		// Begins the drawing to the sprite batch.
		public void Begin(BlendState blendState) {
			spriteBatch.Begin(SpriteSortMode.Deferred, blendState);
		}

		// Begins the drawing to the sprite batch.
		public void Begin(SpriteSortMode sortMode, BlendState blendState) {
			spriteBatch.Begin(sortMode, blendState);
		}

		// Begins the drawing to the sprite batch.
		public void Begin(DrawMode mode) {
			spriteBatch.Begin(mode.SortMode, mode.BlendState,
				mode.SamplerState, mode.DepthStencilState,
				mode.RasterizerState, mode.Effect, mode.Transform);
		}

		// Ends the drawing to the sprite batch.
		public void End() {
			spriteBatch.End();
		}
	

		//-----------------------------------------------------------------------------
		// Render Targets
		//-----------------------------------------------------------------------------

		// Sets the render target to draw to.
		public void SetRenderTarget(RenderTarget2D renderTarget) {
			if (renderTarget != null)
				preivousRenderTargets.Push(spriteBatch.GraphicsDevice.GetRenderTargets());
			else
				preivousRenderTargets.Clear();
			spriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);
		}

		// Resets the render target back to the previous one.
		public void ResetRenderTarget() {
			spriteBatch.GraphicsDevice.SetRenderTargets(preivousRenderTargets.Pop());
		}
	

		//-----------------------------------------------------------------------------
		// Disposal
		//-----------------------------------------------------------------------------

		// Immediately releases the unmanaged resources used by the sprite batch.
		public void Dispose() {
			spriteBatch.Dispose();
		}

	}
} // End namespace
