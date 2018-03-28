using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaColor = Microsoft.Xna.Framework.Color;

using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Translation;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.API;
using ZeldaOracle.Common.Util;
using ZeldaOracle.Common.Content;

namespace ZeldaOracle.Common.Graphics {

	/// <summary>The 2D graphics object class for simplifying drawing.</summary>
	public class Graphics2D {

		// Containment:
		/// <summary>The sprite batch of the graphics object.</summary>
		private SpriteBatch spriteBatch;
		/// <summary>True if drawing is in progress.</summary>
		private bool drawing;

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
		private static Texture2D white1x1;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the 2D graphics object using Content's sprite batch.</summary>
		public Graphics2D() : this(Resources.SpriteBatch) { }

		/// <summary>Constructs a 2D graphics object containing the sprite batch.</summary>
		public Graphics2D(SpriteBatch spriteBatch) {
			// Containment
			this.spriteBatch		= spriteBatch;
			this.drawing			= false;

			// Drawing Settings
			this.translation		= Vector2F.Zero;
			this.translationStack	= new Stack<Vector2F>();
			this.useTranslation		= true;
			this.useIntPrecision	= false;

			// Vector Graphics
			if (white1x1 == null) {
				white1x1			= new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
				white1x1.SetData(new XnaColor[] { XnaColor.White });
			}
		}


		//-----------------------------------------------------------------------------
		// Disposing
		//-----------------------------------------------------------------------------

		/// <summary>Immediately releases the unmanaged resources used by the sprite batch.</summary>
		public void Dispose() {
			spriteBatch.Dispose();
		}
		

		//-----------------------------------------------------------------------------
		// Operators
		//-----------------------------------------------------------------------------

		/// <summary>Cast to a SpriteBatch.</summary>
		public static explicit operator SpriteBatch(Graphics2D graphics) {
			return graphics.spriteBatch;
		}


		//-----------------------------------------------------------------------------
		// Image Drawing
		//-----------------------------------------------------------------------------

		// Position -------------------------------------------------------------------

		/// <summary>Draws the image at the specified position.</summary>
		public void DrawImage(Texture2D texture, Vector2F position) {
			DrawImage(texture, position, Color.White);
		}

		/// <summary>Draws the image at the specified position.</summary>
		public void DrawImage(Texture2D texture, Vector2F position, Color color) {
			spriteBatch.Draw(texture, NewPos(position), color.ToXnaColor());
		}

		/// <summary>Draws the scaled image at the specified position.</summary>
		public void DrawImage(Texture2D texture, Vector2F position, float scale) {
			DrawImage(texture, position, scale, Color.White);
		}

		/// <summary>Draws the scaled image at the specified position.</summary>
		public void DrawImage(Texture2D texture, Vector2F position, float scale, Color color) {
			spriteBatch.Draw(texture, NewPos(position), null, color.ToXnaColor(),
				0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		}
		
		// Position & Source Rect -----------------------------------------------------

		/// <summary>Draws part of the image at the specified position.</summary>
		public void DrawImage(Texture2D texture, Vector2F position, Rectangle2F sourceRect) {
			DrawImage(texture, position, sourceRect, Color.White);
		}

		/// <summary>Draws part of the image at the specified position.</summary>
		public void DrawImage(Texture2D texture, Vector2F position, Rectangle2F sourceRect,
			Color color)
		{
			spriteBatch.Draw(texture, NewPos(position), sourceRect.ToXnaRectangle(),
				color.ToXnaColor());
		}

		/// <summary>Draws part of the scaled image at the specified position.</summary>
		public void DrawImage(Texture2D texture, Vector2F position, Rectangle2F sourceRect,
			float scale)
		{
			DrawImage(texture, position, sourceRect, scale, Color.White);
		}

		/// <summary>Draws part of the scaled image at the specified position.</summary>
		public void DrawImage(Texture2D texture, Vector2F position, Rectangle2F sourceRect,
			float scale, Color color)
		{
			spriteBatch.Draw(texture, NewPos(position), sourceRect.ToXnaRectangle(),
				color.ToXnaColor(), 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		}
		
		// Destination Rect -----------------------------------------------------------

		/// <summary>Draws the image at the specified region.</summary>
		public void DrawImage(Texture2D texture, Rectangle2F destinationRect) {
			DrawImage(texture, destinationRect, Color.White);
		}

		/// <summary>Draws the image at the specified region.</summary>
		public void DrawImage(Texture2D texture, Rectangle2F destinationRect, Color color) {
			spriteBatch.Draw(texture, NewRect(destinationRect), color.ToXnaColor());
		}
		
		/// <summary>Draws part of the image at the specified region.</summary>
		public void DrawImage(Texture2D texture, Rectangle2F destinationRect,
			Rectangle2F sourceRect)
		{
			DrawImage(texture, destinationRect, sourceRect, Color.White);
		}

		/// <summary>Draws part of the image at the specified region.</summary>
		public void DrawImage(Texture2D texture, Rectangle2F destinationRect,
			Rectangle2F sourceRect, Color color) {
			spriteBatch.Draw(texture, NewRect(destinationRect),
				sourceRect.ToXnaRectangle(), color.ToXnaColor(), 0f, Vector2.Zero,
				SpriteEffects.None, 0f);
		}
		
		// Transformation -------------------------------------------------------------

		/// <summary>Draws the transformed image at the specified position.</summary>
		public void DrawImage(Texture2D texture, Vector2F position, Vector2F origin,
			Vector2F scale, float rotation, Flip flip, Color color)
		{
			spriteBatch.Draw(texture, NewPos(position), null, color.ToXnaColor(),
				GMath.CorrectAngle(rotation), origin.ToXnaVector2(),
				scale.ToXnaVector2(), (SpriteEffects) flip, 0f);
		}

		/// <summary>Draws the transformed image at the specified region.</summary>
		public void DrawImage(Texture2D texture, Rectangle2F destinationRect,
			Vector2F origin, float rotation, Flip flip, Color color)
		{
			spriteBatch.Draw(texture, NewRect(destinationRect), null,
				color.ToXnaColor(), GMath.CorrectAngle(rotation),
				origin.ToXnaVector2(), (SpriteEffects) flip, 0f);
		}

		/// <summary>Draws part of the transformed image at the specified position.</summary>
		public void DrawImage(Texture2D texture, Vector2F position, Rectangle2F sourceRect,
			Vector2F origin, Vector2F scale, float rotation, Flip flip,
			Color color)
		{
			spriteBatch.Draw(texture, NewPos(position), sourceRect.ToXnaRectangle(),
				color.ToXnaColor(), GMath.CorrectAngle(rotation),
				origin.ToXnaVector2(), scale.ToXnaVector2(), (SpriteEffects) flip, 0f);
		}

		/// <summary>Draws part of the transformed image at the specified region.</summary>
		public void DrawImage(Texture2D texture, Rectangle2F destinationRect,
			Rectangle2F sourceRect, Vector2F origin, float rotation, Flip flip,
			Color color)
		{
			spriteBatch.Draw(texture, NewRect(destinationRect),
				sourceRect.ToXnaRectangle(), color.ToXnaColor(),
				GMath.CorrectAngle(rotation), origin.ToXnaVector2(),
				(SpriteEffects) flip, 0f);
		}


		//-----------------------------------------------------------------------------
		// Sprite Drawing
		//-----------------------------------------------------------------------------

		/// <summary>Draw the sprite at the specified position.</summary>
		public void DrawSprite(ISprite sprite, float x, float y) {
			DrawSprite(sprite, SpriteSettings.Default, new Vector2F(x, y));
		}

		/// <summary>Draw the sprite at the specified position.</summary>
		public void DrawSprite(ISprite sprite, Vector2F position) {
			DrawSprite(sprite, SpriteSettings.Default, position);
		}

		/// <summary>Draw the sprite at the specified region.</summary>
		public void DrawSprite(ISprite sprite, Rectangle2F destination) {
			DrawSprite(sprite, SpriteSettings.Default, destination);
		}

		/// <summary>Draw the sprite at the specified position.</summary>
		public void DrawSprite(ISprite sprite, float x, float y, Color color) {
			DrawSprite(sprite, SpriteSettings.Default, new Vector2F(x, y), color);
		}

		/// <summary>Draw the sprite at the specified position.</summary>
		public void DrawSprite(ISprite sprite, Vector2F position, Color color) {
			DrawSprite(sprite, SpriteSettings.Default, position, color);
		}

		/// <summary>Draw the sprite at the specified region.</summary>
		public void DrawSprite(ISprite sprite, Rectangle2F destination, Color color) {
			DrawSprite(sprite, SpriteSettings.Default, destination, color);
		}

		//-----------------------------------------------------------------------------
		// Animation Drawing
		//-----------------------------------------------------------------------------

		/// <summary>Draw an animation at the given time stamp and position.</summary>
		public void DrawAnimation(Animation animation, float time, Vector2F position) {
			DrawSprite(animation, new SpriteSettings(time), position);
		}

		/// <summary>Draw an animation at the given time stamp and position.</summary>
		public void DrawAnimation(Animation animation, float time, float x, float y) {
			DrawSprite(animation, new SpriteSettings(time), new Vector2F(x, y));
		}

		/// <summary>Draw the animation player at the specified position.</summary>
		public void DrawAnimation(AnimationPlayer animationPlayer, Vector2F position) {
			DrawSprite(animationPlayer.SpriteOrSubStrip,
				new SpriteSettings(animationPlayer.PlaybackTime), position);
		}


		/// <summary>Draw an animation at the given time stamp and position.</summary>
		public void DrawAnimation(Animation animation, float time, Vector2F position,
			Color color)
		{
			DrawSprite(animation, new SpriteSettings(time), position, color);
		}

		/// <summary>Draw an animation at the given time stamp and position.</summary>
		public void DrawAnimation(Animation animation, float time, float x, float y,
			Color color)
		{
			DrawSprite(animation, new SpriteSettings(time), new Vector2F(x, y), color);
		}

		/// <summary>Draw the animation player at the specified position.</summary>
		public void DrawAnimation(AnimationPlayer animationPlayer, Vector2F position,
			Color color)
		{
			DrawSprite(animationPlayer.SpriteOrSubStrip,
				new SpriteSettings(animationPlayer.PlaybackTime), position, color);
		}


		//-----------------------------------------------------------------------------
		// SpriteDrawSettings Drawing
		//-----------------------------------------------------------------------------

		/// <summary>Draw the sprite at the specified position.</summary>
		public void DrawSprite(ISprite sprite, SpriteSettings settings,
			float x, float y)
		{
			DrawSprite(sprite, settings, new Vector2F(x, y), Color.White);
		}

		/// <summary>Draw the sprite at the specified position.</summary>
		public void DrawSprite(ISprite sprite, SpriteSettings settings,
			Vector2F position)
		{
			DrawSprite(sprite, settings, position, Color.White);
		}

		/// <summary>Draw the sprite at the specified region.</summary>
		public void DrawSprite(ISprite sprite, SpriteSettings settings,
			Rectangle2F destination)
		{
			DrawSprite(sprite, settings, destination, Color.White);
		}


		/// <summary>Draw the sprite at the specified position.</summary>
		public void DrawSprite(ISprite sprite, SpriteSettings settings,
			float x, float y, Color color)
		{
			DrawSprite(sprite, settings, new Vector2F(x, y), color);
		}

		/// <summary>Draw the sprite at the specified position.</summary>
		public void DrawSprite(ISprite sprite, SpriteSettings settings,
			Vector2F position, Color color)
		{
			if (sprite == null) return;
			SpritePart part = sprite.GetParts(settings);
			while (part != null) {
				if (!part.Image.IsDisposed)
					spriteBatch.Draw(part.Image, NewPos(position) +
						part.DrawOffset.ToXnaVector2(), part.SourceRect.ToXnaRectangle(),
						color.ToXnaColor(), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
				part = part.NextPart;
			}
		}

		/// <summary>Draw the sprite at the specified region.</summary>
		public void DrawSprite(ISprite sprite, SpriteSettings settings,
			Rectangle2F destination, Color color)
		{
			if (sprite == null) return;
			SpritePart part = sprite.GetParts(settings);
			while (part != null) {
				destination.Point = NewPos(destination.Point).ToVector2F() +
					part.DrawOffset;
				if (!part.Image.IsDisposed)
					spriteBatch.Draw(part.Image, destination.ToXnaRectangle(),
						part.SourceRect.ToXnaRectangle(), color.ToXnaColor(), 0f,
						Vector2.Zero, SpriteEffects.None, 0f);
				part = part.NextPart;
			}
		}


		//-----------------------------------------------------------------------------
		// Text Drawing
		//-----------------------------------------------------------------------------
		
		// Real Fonts -----------------------------------------------------------------

		/// <summary>Draws the string at the specified position.</summary>
		public void DrawRealString(SpriteFont font, string text, Vector2F position,
			Align alignment, Color color)
		{
			spriteBatch.DrawString(font, text,
				NewStringPos(position, font, text, alignment), color.ToXnaColor());
		}

		/// <summary>Draws the string at the specified position.</summary>
		public void DrawRealString(SpriteFont font, string text, Vector2F position,
			Align alignment, Color color, Vector2F origin, float rotation, float scale,
			Flip flip = Flip.None)
		{
			spriteBatch.DrawString(font, text, NewStringPos(position, font, text, alignment),
				color.ToXnaColor(), rotation, origin.ToXnaVector2(), scale,
				(SpriteEffects) flip, 0f);
		}

		/// <summary>Draws the string at the specified position.</summary>
		public void DrawRealString(SpriteFont font, string text, Vector2F position,
			Align alignment, Color color, Vector2F origin, float rotation, Vector2F scale,
			Flip flip = Flip.None)
		{
			spriteBatch.DrawString(font, text, NewStringPos(position, font, text, alignment),
				color.ToXnaColor(), (float)rotation, origin.ToXnaVector2(),
				scale.ToXnaVector2(), (SpriteEffects) flip, 0f);
		}
		
		// Game Fonts -----------------------------------------------------------------

		/// <summary>Draws a game string of any type at the specified position.</summary>
		public void DrawString(GameFont font, DrawableString text, Vector2F position,
			ColorOrPalette color, Variables vars = null)
		{
			DrawString(font, text, position, color, Align.TopLeft, Vector2F.Zero,
				vars);
		}

		/// <summary>Draws a game string of any type at the specified position.</summary>
		public void DrawString(GameFont font, DrawableString text, Vector2F position,
			ColorOrPalette color, Align alignment, Variables vars = null)
		{
			DrawString(font, text, position, color, alignment, Vector2F.Zero, vars);
		}

		/// <summary>Draws a game string of any type at the specified position.</summary>
		public void DrawString(GameFont font, DrawableString text, Vector2F position,
			ColorOrPalette color, Align alignment, Vector2F area,
			Variables vars = null)
		{
			if (text.IsString)
				DrawLetterString(font, FormatCodes.FormatString(text.String, vars),
					position, color, alignment, area);
			else if (text.IsLetterString)
				DrawLetterString(font, text.LetterString, position, color, alignment,
					area);
			else if (text.IsWrappedLetterString)
				DrawWrappedLetterString(font, text.WrappedLetterString, position, color,
					alignment, area);
		}

		/// <summary>Draws a formatted game string at the specified position.</summary>
		public void DrawLetterString(GameFont font, LetterString letterString,
			Vector2F position, ColorOrPalette color)
		{
			DrawLetterString(font, letterString, position, color, Align.TopLeft,
				Vector2F.Zero);
		}

		/// <summary>Draws a formatted game string at the specified position.</summary>
		public void DrawLetterString(GameFont font, LetterString letterString,
			Vector2F position, ColorOrPalette color, Align alignment)
		{
			DrawLetterString(font, letterString, position, color, alignment,
				Vector2F.Zero);
		}

		/// <summary>Draws a formatted game string at the specified position.</summary>
		public void DrawLetterString(GameFont font, LetterString letterString,
			Vector2F position, ColorOrPalette color, Align alignment, Vector2F area)
		{
			Vector2F size = font.MeasureString(letterString);
			position = Alignment.AlignVector(position, area, size, alignment);
			for (int i = 0; i < letterString.Length; i++) {
				spriteBatch.Draw(
					font.SpriteSheet.Image,
					NewRect(new Rectangle2F(
						position.X + i * (font.CharacterWidth + font.CharacterSpacing),
						position.Y,
						font.CharacterSize
					)),
					font.GetCharacterCell(letterString[i].Char).ToXnaRectangle(),
					(letterString[i].Color == Letter.DefaultColor ? color.MappedColor :
						letterString[i].MappedColor).ToXnaColor(),
					0f, Vector2.Zero, SpriteEffects.None, 0f);
			}
		}
		
		/// <summary>Draws a wrapped game string of any type at the specified position.</summary>
		public void DrawWrappedString(GameFont font, DrawableString text, Vector2F position,
			ColorOrPalette color, Variables vars = null)
		{
			DrawWrappedString(font, text, position, color, Align.TopLeft, Vector2F.Zero, vars);
		}

		/// <summary>Draws a wrapped game string of any type at the specified position.</summary>
		public void DrawWrappedString(GameFont font, DrawableString text, Vector2F position,
			ColorOrPalette color, Align alignment, Variables vars = null)
		{
			DrawWrappedString(font, text, position, color, alignment, Vector2F.Zero, vars);
		}

		/// <summary>Draws a wrapped game string of any type at the specified position.</summary>
		public void DrawWrappedString(GameFont font, DrawableString text, Vector2F position,
			ColorOrPalette color, Align alignment, Vector2F area, Variables vars = null)
		{
			if (text.IsString)
				DrawWrappedLetterString(font, font.WrapString(text.String, (int) area.X),
					position, color, alignment, area);
			else if (text.IsLetterString)
				DrawLetterString(font, text.LetterString, position, color, alignment, area);
			else if (text.IsWrappedLetterString)
				DrawWrappedLetterString(font, text.WrappedLetterString, position, color,
					alignment, area);
		}

		/// <summary>Draws a formatted wrapped game string at the specified position.</summary>
		public void DrawWrappedLetterString(GameFont font, WrappedLetterString wrappedString,
			Vector2F position, ColorOrPalette color)
		{
			DrawWrappedLetterString(font, wrappedString, position, color, Align.TopLeft,
				Vector2F.Zero);
		}

		/// <summary>Draws a formatted wrapped game string at the specified position.</summary>
		public void DrawWrappedLetterString(GameFont font, WrappedLetterString wrappedString,
			Vector2F position, ColorOrPalette color, Align alignment)
		{
			DrawWrappedLetterString(font, wrappedString, position, color, alignment,
				Vector2F.Zero);
		}

		/// <summary>Draws a formatted wrapped game string at the specified position.</summary>
		public void DrawWrappedLetterString(GameFont font, WrappedLetterString wrappedString,
			Vector2F position, ColorOrPalette color, Align alignment, Vector2F area)
		{
			Vector2F size = font.MeasureString(wrappedString);
			Vector2F originalPosition = position;
			for (int j = 0; j < wrappedString.LineCount; j++) {
				LetterString letterString = wrappedString.Lines[j];
				position = Alignment.AlignVector(originalPosition, area,
					new Vector2F(wrappedString.LineLengths[j], size.Y), alignment);
				for (int i = 0; i < letterString.Length; i++) {
					spriteBatch.Draw(
						font.SpriteSheet.Image,
						NewRect(new Rectangle2F(
							position.X + i * (font.CharacterWidth + font.CharacterSpacing),
							position.Y + j * (font.CharacterHeight + font.LineSpacing),
							font.CharacterSize
						)),
						font.GetCharacterCell(letterString[i].Char).ToXnaRectangle(),
						(letterString[i].Color == Letter.DefaultColor ? color.MappedColor :
							letterString[i].MappedColor).ToXnaColor(),
						0f, Vector2.Zero, SpriteEffects.None, 0f);
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Vector Graphics
		//-----------------------------------------------------------------------------

		/// <summary>Draws the specified line.</summary>
		public void DrawLine(Line2F line, float thickness, ColorOrPalette color) {
			DrawImage(white1x1, line.Center + Vector2F.Half, Vector2F.Half,
				new Vector2F((line.Length + thickness), thickness),
				line.Direction, Flip.None, color.UnmappedColor);
		}

		/// <summary>Draws the specified rectangle.</summary>
		public void DrawRectangle(Rectangle2F rect, float thickness, ColorOrPalette color) {
			FillRectangle(new Rectangle2F(rect.Left, rect.Top, rect.Width, 1.0f), color);
			FillRectangle(new Rectangle2F(rect.Left, rect.Bottom - 1, rect.Width, 1.0f), color);
			FillRectangle(new Rectangle2F(rect.Left, rect.Top + 1, 1.0f, rect.Height - 2), color);
			FillRectangle(new Rectangle2F(rect.Right - 1, rect.Top + 1, 1.0f, rect.Height - 2), color);
		}

		/// <summary>Draws the specified filled rectangle.</summary>
		public void FillRectangle(Rectangle2F rect, ColorOrPalette color) {
			rect.Point = GMath.Round(rect.Point);
			DrawImage(white1x1, rect, color.MappedColor);
		}


		//-----------------------------------------------------------------------------
		// Debug Drawing
		//-----------------------------------------------------------------------------

		/// <summary>Draw a collision model as a solid color.</summary>
		public void DrawCollisionModel(CollisionModel model, Vector2F position, Color color) {
			for (int i = 0; i < model.Boxes.Count; i++)
				FillRectangle(Rectangle2F.Translate(model.Boxes[i], position), color);
		}


		//-----------------------------------------------------------------------------
		// Clearing
		//-----------------------------------------------------------------------------

		/// <summary>Clears the render target to black.</summary>
		public void Clear() {
			spriteBatch.GraphicsDevice.Clear(XnaColor.Black);
		}

		/// <summary>Clears the render target.</summary>
		public void Clear(ColorOrPalette color) {
			spriteBatch.GraphicsDevice.Clear(color.MappedColor.ToXnaColor());
		}


		//-----------------------------------------------------------------------------
		// Translation
		//-----------------------------------------------------------------------------
		
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
			translation = (translationStack.Any() ?
				translationStack.Peek() :
				Vector2F.Zero);
		}

		/// <summary>Pops the current translation from the translation stack.</summary>
		public void PopTranslation(int popCount) {
			for (int i = 0; i < popCount; i++) {
				translationStack.Pop();
			}
			translation = (translationStack.Any() ?
				translationStack.Peek() :
				Vector2F.Zero);
		}

		/// <summary>Resets the translation of the graphics object and clears
		/// the translation stack.</summary>
		public void ResetTranslation() {
			translation = Vector2F.Zero;
			translationStack.Clear();
		}


		//-----------------------------------------------------------------------------
		// Begin/End
		//-----------------------------------------------------------------------------

		/// <summary>Begins the drawing to the sprite batch.</summary>
		public void Begin() {
			spriteBatch.Begin();
			drawing = true;
		}

		/// <summary>Begins the drawing to the sprite batch.</summary>
		public void Begin(BlendState blendState) {
			spriteBatch.Begin(SpriteSortMode.Deferred, blendState);
			drawing = true;
		}

		/// <summary>Begins the drawing to the sprite batch.</summary>
		public void Begin(SpriteSortMode sortMode, BlendState blendState) {
			spriteBatch.Begin(sortMode, blendState);
			drawing = true;
		}

		/// <summary>Begins the drawing to the sprite batch.</summary>
		public void Begin(DrawMode mode) {
			spriteBatch.Begin(mode.SortMode, mode.BlendState,
				mode.SamplerState, mode.DepthStencilState,
				mode.RasterizerState, mode.Effect, mode.Transform);
			drawing = true;
		}

		/// <summary>Begins the drawing to the sprite batch with a different shader.</summary>
		public void Begin(DrawMode mode, Effect shader) {
			spriteBatch.Begin(mode.SortMode, mode.BlendState,
				mode.SamplerState, mode.DepthStencilState,
				mode.RasterizerState, shader, mode.Transform);
			drawing = true;
		}

		/// <summary>Ends the drawing to the sprite batch.</summary>
		public void End() {
			spriteBatch.End();
			drawing = false;
		}


		//-----------------------------------------------------------------------------
		// Render Targets
		//-----------------------------------------------------------------------------

		/// <summary>Gets the current render target.</summary>
		public RenderTarget GetRenderTarget() {
			return RenderTarget.Wrap(GraphicsDevice.GetRenderTarget());
		}

		/// <summary>Sets the render target to draw to.</summary>
		public void SetRenderTarget(RenderTarget2D renderTarget) {
			spriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Translates the position based on the translation and
		/// precision settings.</summary>
		private Vector2 NewPos(Vector2F position) {
			if (useTranslation)
				return (UseIntegerPrecision ? GMath.Round(position + translation) :
					(position + translation)).ToXnaVector2();
			else
				return position.ToXnaVector2();
		}

		/// <summary>Translates the position of the string based on the
		/// translation and precision settings.</summary>
		private Vector2 NewStringPos(Vector2F position, SpriteFont font, string text,
			Align alignment) {
			Vector2F stringSize = font.MeasureString(text).ToVector2F();
			bool intAlign = (alignment & Align.Int) != 0;
			if (((alignment & Align.Left) != 0) == ((alignment & Align.Right) != 0))
				position.X -= (intAlign ? (int) (stringSize.X / 2f) : (stringSize.X / 2f));
			else if ((alignment & Align.Right) != 0)
				position.X -= stringSize.X;
			if (((alignment & Align.Top) != 0) == ((alignment & Align.Bottom) != 0))
				position.Y -=(intAlign ? (int) (stringSize.Y / 2f) : (stringSize.Y / 2f));
			else if ((alignment & Align.Bottom) != 0)
				position.Y -= stringSize.Y;

			if (useTranslation)
				return (UseIntegerPrecision ? GMath.Floor(position + translation) :
					(position + translation)).ToXnaVector2();
			else
				return position.ToXnaVector2();
		}

		/// <summary>Translates the rectangle based on the translation and
		/// precision settings.</summary>
		private Rectangle NewRect(Rectangle2F destinationRect) {
			if (useTranslation)
				return (UseIntegerPrecision ? new Rectangle2F(GMath.Round(
					destinationRect.Point + translation), GMath.Round(destinationRect.Size)) :
					(destinationRect + translation)).ToXnaRectangle();
			else
				return destinationRect.ToXnaRectangle();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Containment ----------------------------------------------------------------

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

		/// <summary>Returns true if drawing is in progress.</summary>
		public bool IsDrawing {
			get { return drawing; }
		}

		// Drawing Settings -----------------------------------------------------------

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
	}
}
