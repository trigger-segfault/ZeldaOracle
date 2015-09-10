using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaColor		= Microsoft.Xna.Framework.Color;

using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Content;

namespace ZeldaOracle.Common.Graphics {
/** <summary>
 * The 2D graphics object class for simplifying drawing.
 * </summary> */
public class Graphics2D {

	//========== CONSTANTS ===========
	#region Constants

	#endregion
	//=========== MEMBERS ============
	#region Members

	// Containment
	/** <summary> The sprite batch of the graphics object. </summary> */
	private SpriteBatch spriteBatch;
	/** <summary> The previous render targets of the graphics object. </summary> */
	private Stack<RenderTargetBinding[]> preivousRenderTargets;

	// Drawing Settings
	/** <summary> The current translated position of the graphics object. </summary> */
	private Vector2F translation;
	/** <summary> True if the graphics translation will be taken into account. </summary> */
	private bool useTranslation;
	/** <summary> True if all draws will use integer precision. </summary> */
	private bool useIntPrecision;

	// Vector Graphics
	/** <summary> A white 1x1 texture used for drawing vector graphics. </summary> */
	private Texture2D white1x1;
	/** <summary> A white 2x2 texture used for drawing vector graphics. </summary> */
	private Texture2D white2x2;

	private XnaColor drawingColor;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs a 2D graphics object containing the sprite batch. </summary> */
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

	#endregion
	//========== OPERATORS ===========
	#region Operators

	public static explicit operator SpriteBatch(Graphics2D graphics) {
		return graphics.spriteBatch;
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties
	//--------------------------------
	#region Containment

	/** <summary> Gets the sprite batch of the graphics object. </summary> */
	public SpriteBatch SpriteBatch {
		get { return spriteBatch; }
	}
	/** <summary> Gets the graphics device of the graphics object. </summary> */
	public GraphicsDevice GraphicsDevice {
		get { return spriteBatch.GraphicsDevice; }
	}
	/** <summary> Returns true if the sprite batch has been disposed. </summary> */
	public bool IsDisposed {
		get { return spriteBatch.IsDisposed; }
	}

	#endregion
	//--------------------------------
	#region Drawing Settings

	/** <summary> Gets the current translation of the graphics object. </summary> */
	public Vector2F Translation {
		get { return translation; }
	}
	/** <summary> Gets or sets if the translation is used. </summary> */
	public bool UseTranslation {
		get { return useTranslation; }
		set { useTranslation = value; }
	}
	/** <summary> Gets or sets if integer precision is used with translations. </summary> */
	public bool UseIntegerPrecision {
		get { return useIntPrecision; }
		set { useIntPrecision = value; }
	}

	#endregion
	//--------------------------------
	#endregion
	//=========== DRAWING ============
	#region Drawing
	//--------------------------------
	#region Private

	/** <summary> Translates the position based on the translation and precision settings. </summary> */
	private Vector2 NewPos(Vector2F position) {
		if (useTranslation)
			return (UseIntegerPrecision ? (Vector2)GMath.Floor(position + translation) : (Vector2)(position + translation));
		else
			return (Vector2)position;
	}

	/** <summary> Translates the position of the string based on the translation and precision settings. </summary> */
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
	/** <summary> Translates the rectangle based on the translation and precision settings. </summary> */
	private Rectangle NewRect(Rectangle2F destinationRect) {
		if (useTranslation)
			return (Rectangle)(destinationRect + translation);
		else
			return (Rectangle)destinationRect;
	}
	/** <summary> Translates the rectangle based on the translation and precision settings. </summary> */
	private Rectangle NewRect(Vector2F position, Vector2F size) {
		Rectangle2F destinationRect = new Rectangle2F(position, size);
		if (useTranslation)
			return (Rectangle)(destinationRect + translation);
		else
			return (Rectangle)destinationRect;
	}

	#endregion
	//--------------------------------
	#region Image Drawing

	/** <summary> Draws the image at the specified position. </summary> */
	public void DrawImage(Texture2D texture, Vector2F position) {
		spriteBatch.Draw(texture, NewPos(position), XnaColor.White);
	}
	/** <summary> Draws the image at the specified position. </summary> */
	public void DrawImage(Texture2D texture, Vector2F position, Color color, double depth = 0.0) {
		spriteBatch.Draw(texture, NewPos(position), null, color, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, (float)depth);
	}
	/** <summary> Draws the image at the specified position. </summary> */
	public void DrawImage(Texture2D texture, Vector2F position, Vector2F origin, Vector2F scale, double rotation, Color? color = null, SpriteEffects flipEffect = SpriteEffects.None, double depth = 0.0) {
		spriteBatch.Draw(texture, NewPos(position), null, color ?? XnaColor.White, (float)GMath.ConvertToRadians(rotation), (Vector2)origin, (Vector2)scale, flipEffect, (float)depth);
	}
	/** <summary> Draws the image at the specified region. </summary> */
	public void DrawImage(Texture2D texture, Rectangle2F destinationRect, Vector2F origin, double rotation, Color? color = null, SpriteEffects flipEffect = SpriteEffects.None, double depth = 0.0) {
		spriteBatch.Draw(texture, NewRect(destinationRect), null, color ?? XnaColor.White, (float)GMath.ConvertToRadians(rotation), (Vector2)origin, flipEffect, (float)depth);
	}

	/** <summary> Draws part of the image at the specified position. </summary> */
	public void DrawImage(Texture2D texture, Vector2F position, Rectangle2F sourceRect) {
		spriteBatch.Draw(texture, NewPos(position), (Rectangle)sourceRect, XnaColor.White);
	}
	/** <summary> Draws part of the image at the specified position. </summary> */
	public void DrawImage(Texture2D texture, Vector2F position, Rectangle2F sourceRect, Color color, double depth = 0.0) {
		spriteBatch.Draw(texture, NewPos(position), (Rectangle)sourceRect, color, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, (float)depth);
	}
	/** <summary> Draws part of the image at the specified position. </summary> */
	public void DrawImage(Texture2D texture, Vector2F position, Rectangle2F sourceRect, Vector2F origin, Vector2F scale, double rotation, Color? color = null, SpriteEffects flipEffect = SpriteEffects.None, double depth = 0.0) {
		spriteBatch.Draw(texture, NewPos(position), (Rectangle)sourceRect, color ?? XnaColor.White, (float)GMath.ConvertToRadians(rotation), (Vector2)origin, (Vector2)scale, flipEffect, (float)depth);
	}
	/** <summary> Draws part of the image at the specified region. </summary> */
	public void DrawImage(Texture2D texture, Rectangle2F destinationRect, Rectangle2F sourceRect, Vector2F origin, double rotation, Color? color = null, SpriteEffects flipEffect = SpriteEffects.None, double depth = 0.0) {
		spriteBatch.Draw(texture, NewRect(destinationRect), (Rectangle)sourceRect, color ?? XnaColor.White, (float)GMath.ConvertToRadians(rotation), (Vector2)origin, flipEffect, (float)depth);
	}

	#endregion
	//--------------------------------
	#region Sprite Drawing

	/** <summary> Draws the sprite at the specified position. </summary> */
	public void DrawSprite(Sprite sprite, Vector2F position) {
		/*spriteBatch.Draw(sprite.Sheet.Image, NewRect(position, sprite.SourceSize), (Rectangle)sprite.FrameRect,
			XnaColor.White, 0.0f, (Vector2)(-sprite.Offset), SpriteEffects.None, 0.0f);*/
		spriteBatch.Draw(sprite.Sheet.Image, NewPos(position), (Rectangle)sprite.FrameRect,
			XnaColor.White, 0.0f, (Vector2)(-sprite.Offset), 1.0f, SpriteEffects.None, 0.0f);
	}
	/** <summary> Draws the sprite at the specified position. </summary> */
	public void DrawSprite(Sprite sprite, Vector2F position, Color color, double depth = 0.0) {
		/*spriteBatch.Draw(sprite.Sheet.Image, NewRect(position, sprite.SourceSize), (Rectangle)sprite.FrameRect,
			color, 0.0f, (Vector2)(-sprite.Offset), SpriteEffects.None, (float)depth);*/
		spriteBatch.Draw(sprite.Sheet.Image, NewPos(position), (Rectangle)sprite.FrameRect,
			color, 0.0f, (Vector2)(-sprite.Offset), 1.0f, SpriteEffects.None, (float)depth);
	}
	/** <summary> Draws the sprite at the specified position. </summary> */
	public void DrawSprite(Sprite sprite, Vector2F position, Vector2F origin, Vector2F scale, double rotation, Color color, SpriteEffects flipEffect = SpriteEffects.None, double depth = 0.0) {
		/*spriteBatch.Draw(sprite.Sheet.Image, NewRect(position, scale * sprite.SourceSize), (Rectangle)sprite.FrameRect,
			color, (float)GMath.ConvertToRadians(rotation), (Vector2)(origin - sprite.Offset), flipEffect, (float)depth);*/
		spriteBatch.Draw(sprite.Sheet.Image, NewPos(position), (Rectangle)sprite.FrameRect,
			color, (float)GMath.ConvertToRadians(rotation), (Vector2)(origin - sprite.Offset), (Vector2)scale, flipEffect, (float)depth);
	}
	/** <summary> Draws the sprite at the specified region. </summary> */
	public void DrawSprite(Sprite sprite, Rectangle2F destinationRect, Vector2F origin, double rotation, Color color, SpriteEffects flipEffect = SpriteEffects.None, double depth = 0.0) {
		spriteBatch.Draw(sprite.Sheet.Image, NewRect(destinationRect), (Rectangle)sprite.FrameRect,
			color, (float)GMath.ConvertToRadians(rotation), (Vector2)(origin - sprite.Offset), flipEffect, (float)depth);
	}

	#endregion
	//--------------------------------
	#region String Drawing

	/** <summary> Draws the string at the specified position. </summary> */
	public void DrawString(SpriteFont font, string text, Vector2F position, Align alignment, Color color) {
		spriteBatch.DrawString(font, text, NewStringPos(position, font, text, alignment), (XnaColor)color);
	}
	/** <summary> Draws the string at the specified position. </summary> */
	public void DrawString(SpriteFont font, string text, Vector2F position, Align alignment, Color color, Vector2F origin, double rotation, double scale, SpriteEffects flipEffect = SpriteEffects.None, double depth = 0.0) {
		spriteBatch.DrawString(font, text, NewStringPos(position, font, text, alignment), (XnaColor)color, (float)rotation, (Vector2)origin, (float)scale, flipEffect, (float)depth);
	}
	/** <summary> Draws the string at the specified position. </summary> */
	public void DrawString(SpriteFont font, string text, Vector2F position, Align alignment, Color color, Vector2F origin, double rotation, Vector2F scale, SpriteEffects flipEffect = SpriteEffects.None, double depth = 0.0) {
		spriteBatch.DrawString(font, text, NewStringPos(position, font, text, alignment), (XnaColor)color, (float)rotation, (Vector2)origin, (Vector2)scale, flipEffect, (float)depth);
	}

	#endregion
	//--------------------------------
	#region Vector Graphics

	/** <summary> Draws the specified line. </summary> */
	public void DrawLine(Line2F line, float thickness, Color color, float depth = 0.0f) {
		DrawImage(white1x1, line.Center + 0.5f, new Vector2F(0.5f, 0.5f), new Vector2F((line.Length + thickness), thickness), line.Direction, color, SpriteEffects.None, depth);
	}
	/** <summary> Draws the specified rectangle. </summary> */
	public void DrawRectangle(Rectangle2F rect, float thickness, Color color, float depth = 0.0f) {
		DrawLine(new Line2F(rect.Point, rect.Point + new Vector2F(rect.Width - 1, 0.0f)), thickness, color, depth);
		DrawLine(new Line2F(rect.Point, rect.Point + new Vector2F(0.0f, rect.Height - 1)), thickness, color, depth);
		DrawLine(new Line2F(rect.Point + rect.Size - 1, rect.Point + new Vector2F(rect.Width - 1, 0.0f)), thickness, color, depth);
		DrawLine(new Line2F(rect.Point + rect.Size - 1, rect.Point + new Vector2F(0.0f, rect.Height - 1)), thickness, color, depth);
	}
	/** <summary> Draws the specified filled rectangle. </summary> */
	public void FillRectangle(Rectangle2F rect, Color color, double depth = 0.0) {
		DrawImage(white1x1, rect, Vector2F.Zero, 0.0, color, SpriteEffects.None, depth);
		//DrawImage(white1x1, rect.Center + 0.5, new Vector2D(0.5, 0.5), rect.Size, 0, color, SpriteEffects.None, depth);
	}

	#endregion
	//--------------------------------
	#region Misc Drawing

	/** <summary> Clears the render target to black. </summary> */
	public void Clear() {
		spriteBatch.GraphicsDevice.Clear(XnaColor.Black);
	}
	/** <summary> Clears the render target. </summary> */
	public void Clear(Color color) {
		spriteBatch.GraphicsDevice.Clear(color);
	}

	#endregion
	//--------------------------------
	#endregion
	//========== MANAGEMENT ==========
	#region Management
	//--------------------------------
	#region Settings

	/** <summary> Translates the origin of the graphics object. </summary> */
	public void Translate(float x, float y) {
		translation += new Vector2F(x, y);
	}
	/** <summary> Translates the origin of the graphics object. </summary> */
	public void Translate(Vector2F distance) {
		translation += distance;
	}
	/** <summary> Resets the translation of the graphics object. </summary> */
	public void ResetTranslation() {
		translation = Vector2F.Zero;
	}

	#endregion
	//--------------------------------
	#region Begin/End

	/** <summary> Begins the drawing to the sprite batch. </summary> */
	public void Begin() {
		spriteBatch.Begin();
	}
	/** <summary> Begins the drawing to the sprite batch. </summary> */
	public void Begin(BlendState blendState) {
		spriteBatch.Begin(SpriteSortMode.Deferred, blendState);
	}
	/** <summary> Begins the drawing to the sprite batch. </summary> */
	public void Begin(SpriteSortMode sortMode, BlendState blendState) {
		spriteBatch.Begin(sortMode, blendState);
	}
	/** <summary> Begins the drawing to the sprite batch. </summary> */
	public void Begin(DrawMode mode) {
		spriteBatch.Begin(mode.SortMode, mode.BlendState,
			mode.SamplerState, mode.DepthStencilState,
			mode.RasterizerState, mode.Effect, mode.Transform);
	}
	/** <summary> Ends the drawing to the sprite batch. </summary> */
	public void End() {
		spriteBatch.End();
	}

	#endregion
	//--------------------------------
	#region Render Target

	/** <summary> Sets the render target to draw to. </summary> */
	public void SetRenderTarget(RenderTarget2D renderTarget) {
		if (renderTarget != null)
			preivousRenderTargets.Push(spriteBatch.GraphicsDevice.GetRenderTargets());
		else
			preivousRenderTargets.Clear();
		spriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);
	}
	/** <summary> Resets the render target back to the previous one. </summary> */
	public void ResetRenderTarget() {
		spriteBatch.GraphicsDevice.SetRenderTargets(preivousRenderTargets.Pop());
	}

	#endregion
	//--------------------------------
	#region Dispose

	/** <summary> Immediately releases the unmanaged resources used by the sprite batch. </summary> */
	public void Dispose() {
		spriteBatch.Dispose();
	}

	#endregion
	//--------------------------------
	#endregion
}
} // End namespace
