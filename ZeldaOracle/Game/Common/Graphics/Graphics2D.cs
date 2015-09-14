using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaColor		= Microsoft.Xna.Framework.Color;

using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Translation;

namespace ZeldaOracle.Common.Graphics {

// The 2D graphics object class for simplifying drawing.
public class Graphics2D {

	// Containment:
	// The sprite batch of the graphics object.
	private SpriteBatch spriteBatch;
	// The previous render targets of the graphics object.
	private Stack<RenderTargetBinding[]> preivousRenderTargets;

	// Drawing Settings:
	// The current translated position of the graphics object.
	private Vector2F translation;
	// True if the graphics translation will be taken into account.
	private bool useTranslation;
	// True if all draws will use integer precision.
	private bool useIntPrecision;

	// Vector Graphics:
	// A white 1x1 texture used for drawing vector graphics.
	private Texture2D white1x1;
	// A white 2x2 texture used for drawing vector graphics.
	private Texture2D white2x2;

	private XnaColor drawingColor;

	
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
		this.useTranslation			= true;
		this.useIntPrecision		= false;

		// Vector Graphics
		this.white1x1				= new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
		this.white2x2				= new Texture2D(spriteBatch.GraphicsDevice, 2, 2);
		this.white1x1.SetData(new Color[] { Color.White });
		this.white2x2.SetData(new Color[] { Color.White, Color.White, Color.White, Color.White });

		this.drawingColor			= XnaColor.White;
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

	// Gets the sprite batch of the graphics object.
	public SpriteBatch SpriteBatch {
		get { return spriteBatch; }
	}
	// Gets the graphics device of the graphics object.
	public GraphicsDevice GraphicsDevice {
		get { return spriteBatch.GraphicsDevice; }
	}
	// Returns true if the sprite batch has been disposed.
	public bool IsDisposed {
		get { return spriteBatch.IsDisposed; }
	}

	//Drawing Settings.

	// Gets the current translation of the graphics object.
	public Vector2F Translation {
		get { return translation; }
	}
	// Gets or sets if the translation is used.
	public bool UseTranslation {
		get { return useTranslation; }
		set { useTranslation = value; }
	}
	// Gets or sets if integer precision is used with translations.
	public bool UseIntegerPrecision {
		get { return useIntPrecision; }
		set { useIntPrecision = value; }
	}

	
	//-----------------------------------------------------------------------------
	// Drawing Internal
	//-----------------------------------------------------------------------------

	// Translates the position based on the translation and precision settings.
	private Vector2 NewPos(Vector2F position) {
		if (useTranslation)
			return (UseIntegerPrecision ? (Vector2)GMath.Floor(position + translation) : (Vector2)(position + translation));
		else
			return (Vector2)position;
	}

	// Translates the position of the string based on the translation and precision settings.
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
			return (UseIntegerPrecision ? (Vector2)GMath.Floor(position + translation) : (Vector2)(position + translation));
		else
			return (Vector2)position;
	}
	// Translates the rectangle based on the translation and precision settings.
	private Rectangle NewRect(Rectangle2F destinationRect) {
		if (useTranslation)
			return (Rectangle)(destinationRect + translation);
		else
			return (Rectangle)destinationRect;
	}
	// Translates the rectangle based on the translation and precision settings.
	private Rectangle NewRect(Vector2F position, Vector2F size) {
		Rectangle2F destinationRect = new Rectangle2F(position, size);
		if (useTranslation)
			return (Rectangle)(destinationRect + translation);
		else
			return (Rectangle)destinationRect;
	}

	
	//-----------------------------------------------------------------------------
	// Image Drawing
	//-----------------------------------------------------------------------------

	// Draws the image at the specified position.
	public void DrawImage(Texture2D texture, Vector2F position) {
		spriteBatch.Draw(texture, NewPos(position), XnaColor.White);
	}
	// Draws the image at the specified position.
	public void DrawImage(Texture2D texture, Vector2F position, Color color, double depth = 0.0) {
		spriteBatch.Draw(texture, NewPos(position), null, color, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, (float)depth);
	}
	// Draws the image at the specified position.
	public void DrawImage(Texture2D texture, Vector2F position, Vector2F origin, Vector2F scale, double rotation, Color? color = null, SpriteEffects flipEffect = SpriteEffects.None, double depth = 0.0) {
		spriteBatch.Draw(texture, NewPos(position), null, color ?? XnaColor.White, (float)GMath.ConvertToRadians(rotation), (Vector2)origin, (Vector2)scale, flipEffect, (float)depth);
	}
	// Draws the image at the specified region.
	public void DrawImage(Texture2D texture, Rectangle2F destinationRect, Vector2F origin, double rotation, Color? color = null, SpriteEffects flipEffect = SpriteEffects.None, double depth = 0.0) {
		spriteBatch.Draw(texture, NewRect(destinationRect), null, color ?? XnaColor.White, (float)GMath.ConvertToRadians(rotation), (Vector2)origin, flipEffect, (float)depth);
	}

	// Draws part of the image at the specified position.
	public void DrawImage(Texture2D texture, Vector2F position, Rectangle2F sourceRect) {
		spriteBatch.Draw(texture, NewPos(position), (Rectangle)sourceRect, XnaColor.White);
	}
	// Draws part of the image at the specified position.
	public void DrawImage(Texture2D texture, Vector2F position, Rectangle2F sourceRect, Color color, double depth = 0.0) {
		spriteBatch.Draw(texture, NewPos(position), (Rectangle)sourceRect, color, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, (float)depth);
	}
	// Draws part of the image at the specified position.
	public void DrawImage(Texture2D texture, Vector2F position, Rectangle2F sourceRect, Vector2F origin, Vector2F scale, double rotation, Color? color = null, SpriteEffects flipEffect = SpriteEffects.None, double depth = 0.0) {
		spriteBatch.Draw(texture, NewPos(position), (Rectangle)sourceRect, color ?? XnaColor.White, (float)GMath.ConvertToRadians(rotation), (Vector2)origin, (Vector2)scale, flipEffect, (float)depth);
	}
	// Draws part of the image at the specified region.
	public void DrawImage(Texture2D texture, Rectangle2F destinationRect, Rectangle2F sourceRect, Vector2F origin, double rotation, Color? color = null, SpriteEffects flipEffect = SpriteEffects.None, double depth = 0.0) {
		spriteBatch.Draw(texture, NewRect(destinationRect), (Rectangle)sourceRect, color ?? XnaColor.White, (float)GMath.ConvertToRadians(rotation), (Vector2)origin, flipEffect, (float)depth);
	}


	//-----------------------------------------------------------------------------
	// Sprite drawing
	//-----------------------------------------------------------------------------

	public void DrawSprite(Sprite sprite, float x, float y) {
		DrawSprite(sprite, new Vector2F(x, y));
	}

	public void DrawSprite(Sprite sprite, Vector2F position) {
		for (Sprite part = sprite; part != null; part = part.NextPart) {
			spriteBatch.Draw(part.Image, NewPos(position) + (Vector2) part.DrawOffset, (Rectangle) part.SourceRect,
				XnaColor.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
		}
	}

	

	//-----------------------------------------------------------------------------
	// OLD Sprite drawing
	//-----------------------------------------------------------------------------

	// Draws the sprite at the specified position.
	public void DrawSpriteEx(SpriteEx sprite, Vector2F position) {
		/*spriteBatch.Draw(sprite.Sheet.Image, NewRect(position, sprite.SourceSize), (Rectangle)sprite.FrameRect,
			XnaColor.White, 0.0f, (Vector2)(-sprite.Offset), SpriteEffects.None, 0.0f);*/
		spriteBatch.Draw(sprite.Sheet.Image, NewPos(position), (Rectangle)sprite.FrameRect,
			XnaColor.White, 0.0f, (Vector2)(-sprite.Offset), 1.0f, SpriteEffects.None, 0.0f);
	}

	// Draws the sprite at the specified position.
	public void DrawSpriteEx(SpriteEx sprite, Vector2F position, Color color, double depth = 0.0) {
		/*spriteBatch.Draw(sprite.Sheet.Image, NewRect(position, sprite.SourceSize), (Rectangle)sprite.FrameRect,
			color, 0.0f, (Vector2)(-sprite.Offset), SpriteEffects.None, (float)depth);*/
		spriteBatch.Draw(sprite.Sheet.Image, NewPos(position), (Rectangle)sprite.FrameRect,
			color, 0.0f, (Vector2)(-sprite.Offset), 1.0f, SpriteEffects.None, (float)depth);
	}

	// Draws the sprite at the specified position.
	public void DrawSpriteEx(SpriteEx sprite, Vector2F position, Vector2F origin, Vector2F scale, double rotation, Color color, SpriteEffects flipEffect = SpriteEffects.None, double depth = 0.0) {
		/*spriteBatch.Draw(sprite.Sheet.Image, NewRect(position, scale * sprite.SourceSize), (Rectangle)sprite.FrameRect,
			color, (float)GMath.ConvertToRadians(rotation), (Vector2)(origin - sprite.Offset), flipEffect, (float)depth);*/
		spriteBatch.Draw(sprite.Sheet.Image, NewPos(position), (Rectangle)sprite.FrameRect,
			color, (float)GMath.ConvertToRadians(rotation), (Vector2)(origin - sprite.Offset), (Vector2)scale, flipEffect, (float)depth);
	}

	// Draws the sprite at the specified region.
	public void DrawSpriteEx(SpriteEx sprite, Rectangle2F destinationRect, Vector2F origin, double rotation, Color color, SpriteEffects flipEffect = SpriteEffects.None, double depth = 0.0) {
		spriteBatch.Draw(sprite.Sheet.Image, NewRect(destinationRect), (Rectangle)sprite.FrameRect,
			color, (float)GMath.ConvertToRadians(rotation), (Vector2)(origin - sprite.Offset), flipEffect, (float)depth);
	}


	//-----------------------------------------------------------------------------
	// Animation drawing
	//-----------------------------------------------------------------------------
	
	// Draw an animation during at the given time stamp and position.
	public void DrawAnimation(Animation animation, float time, Vector2F position) {
		DrawAnimation(animation, time, position.X, position.Y);
	}

	// Draw an animation during at the given time stamp and position.
	public void DrawAnimation(Animation animation, float time, float x, float y) {
		x = GMath.Round(x); 
		y = GMath.Round(y);

		for (int i = 0; i < animation.Frames.Count; ++i) {
			AnimationFrame frame = animation.Frames[i];
			if (time < frame.StartTime)
				return;
			if (time < frame.StartTime + frame.Duration || (time >= animation.Duration && frame.StartTime + frame.Duration == animation.Duration))
				DrawSprite(frame.Sprite, x, y);
		}
	}
	

	//-----------------------------------------------------------------------------
	// Text Drawing
	//-----------------------------------------------------------------------------

	// Draws the string at the specified position.
	public void DrawString(SpriteFont font, string text, Vector2F position, Align alignment, Color color) {
		spriteBatch.DrawString(font, text, NewStringPos(position, font, text, alignment), (XnaColor)color);
	}

	// Draws the string at the specified position.
	public void DrawString(SpriteFont font, string text, Vector2F position, Align alignment,
		Color color, Vector2F origin, double rotation, double scale, SpriteEffects flipEffect = SpriteEffects.None, float depth = 0.0f)
	{
		spriteBatch.DrawString(font, text, NewStringPos(position, font, text, alignment),
			(XnaColor)color, (float)rotation, (Vector2)origin, (float)scale, flipEffect, depth);
	}

	// Draws the string at the specified position.
	public void DrawString(SpriteFont font, string text, Vector2F position, Align alignment,
			Color color, Vector2F origin, double rotation, Vector2F scale,
			SpriteEffects flipEffect = SpriteEffects.None, float depth = 0.0f)
	{
		spriteBatch.DrawString(font, text, NewStringPos(position, font, text, alignment), (XnaColor)color, (float)rotation, (Vector2)origin, (Vector2)scale, flipEffect, depth);
	}

	public void DrawGameString(GameFont font, string text, Point2I position, Color color, float depth = 0.0f) {
		DrawFormattedGameString(font, StringCodes.FormatText(text), position, color, depth);
	}
	public void DrawWrappedGameString(GameFont font, string text, int width, Point2I position, Color color, float depth = 0.0f) {
		WrappedString wrappedString = font.MeasureWrappedString(text, width);
		for (int i = 0; i < wrappedString.Lines.Length; i++) {
			DrawFormattedGameString(font, wrappedString.Lines[i], position + new Point2I(0, font.LineSpacing * i), color, depth);
		}
	}
	public void DrawFormattedGameString(GameFont font, FormattedString formattedString, Point2I position, Color color, float depth = 0.0f) {
		for (int i = 0; i < formattedString.Text.Length; i++) {
			spriteBatch.Draw(
				font.SpriteSheet.Image,
				(Rectangle)new Rectangle2I(
					position.X + i * (font.CharacterSize.X + font.CharacterSpacing),
					position.Y,
					font.CharacterSize.X,
					font.CharacterSize.Y
				),
				(Rectangle)new Rectangle2I(
					font.SpriteSheet.Offset.X + ((int)formattedString.Text[i] % 16) * (font.SpriteSheet.CellSize.X + font.SpriteSheet.Spacing.X),
					font.SpriteSheet.Offset.Y + ((int)formattedString.Text[i] / 16) * (font.SpriteSheet.CellSize.Y + font.SpriteSheet.Spacing.Y),
					font.SpriteSheet.CellSize.X,
					font.SpriteSheet.CellSize.Y
				),
				(formattedString.Colors[i] == Color.Black ? color : formattedString.Colors[i]), 0.0f, Vector2.Zero, SpriteEffects.None, depth
			);
		}
	}


	//-----------------------------------------------------------------------------
	// Vector graphics.
	//-----------------------------------------------------------------------------

	// Draws the specified line.
	public void DrawLine(Line2F line, float thickness, Color color, float depth = 0.0f) {
		DrawImage(white1x1, line.Center + 0.5f, new Vector2F(0.5f, 0.5f),
			new Vector2F((line.Length + thickness), thickness), 
			line.Direction, color, SpriteEffects.None, depth);
	}

	// Draws the specified rectangle.
	public void DrawRectangle(Rectangle2F rect, float thickness, Color color, float depth = 0.0f) {
		DrawLine(new Line2F(rect.Point, rect.Point + new Vector2F(rect.Width - 1, 0.0f)), thickness, color, depth);
		DrawLine(new Line2F(rect.Point, rect.Point + new Vector2F(0.0f, rect.Height - 1)), thickness, color, depth);
		DrawLine(new Line2F(rect.Point + rect.Size - 1, rect.Point + new Vector2F(rect.Width - 1, 0.0f)), thickness, color, depth);
		DrawLine(new Line2F(rect.Point + rect.Size - 1, rect.Point + new Vector2F(0.0f, rect.Height - 1)), thickness, color, depth);
	}

	// Draws the specified filled rectangle.
	public void FillRectangle(Rectangle2F rect, Color color, double depth = 0.0) {
		DrawImage(white1x1, rect, Vector2F.Zero, 0.0, color, SpriteEffects.None, depth);
		//DrawImage(white1x1, rect.Center + 0.5, new Vector2D(0.5, 0.5), rect.Size, 0, color, SpriteEffects.None, depth);
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
	public void Translate(float x, float y) {
		translation += new Vector2F(x, y);
	}

	// Translates the origin of the graphics object.
	public void Translate(Vector2F distance) {
		translation += distance;
	}

	// Resets the translation of the graphics object.
	public void ResetTranslation() {
		translation = Vector2F.Zero;
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
