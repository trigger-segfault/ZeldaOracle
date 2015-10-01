using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

using ZeldaImage = ZeldaOracle.Common.Graphics.Image;


namespace ZeldaEditor.Control {
	public class EditorGraphics {


		//-----------------------------------------------------------------------------
		// Sprite drawing
		//-----------------------------------------------------------------------------

		public static void DrawSprite(Graphics g, Sprite sprite, Point2I position) {
			DrawSprite(g, sprite, position.X, position.Y);
		}

		public static void DrawSprite(Graphics g, Sprite sprite, int variantID, Point2I position) {
			DrawSprite(g, sprite, variantID, position.X, position.Y);
		}

		public static void DrawSprite(Graphics g, Sprite sprite, int x, int y) {
			for (Sprite part = sprite; part != null; part = part.NextPart) {
				Bitmap bitmap = EditorResources.GetBitmap(part.Image);
				Rectangle sourceRect = new Rectangle(
					part.SourceRect.X, part.SourceRect.Y,
					part.SourceRect.Width, part.SourceRect.Height);
				g.DrawImage(bitmap, x + part.DrawOffset.X,
					y + part.DrawOffset.Y, sourceRect, GraphicsUnit.Pixel);
			}
		}
	
		public static void DrawSprite(Graphics g, Sprite sprite, int variantID, int x, int y) {
			for (Sprite part = sprite; part != null; part = part.NextPart) {
				Bitmap bitmap = EditorResources.GetBitmap(part.Image.GetVariant(variantID));
				Rectangle sourceRect = new Rectangle(
					part.SourceRect.X, part.SourceRect.Y,
					part.SourceRect.Width, part.SourceRect.Height);
				g.DrawImage(bitmap, x + part.DrawOffset.X,
					y + part.DrawOffset.Y, sourceRect, GraphicsUnit.Pixel);
			}
		}

		/*
		public void DrawSprite(Sprite sprite, int variantID, Vector2F position, Color color, int depth = 0.0f) {
			for (Sprite part = sprite; part != null; part = part.NextPart) {
				Image image = sprite.Image.GetVariant(variantID);
				spriteBatch.Draw(image, NewPos(position) + (Vector2)part.DrawOffset, (Rectangle)part.SourceRect,
					(XnaColor)color, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, depth);
			}
		}
		
		public void DrawSprite(Sprite sprite, int x, int y, Color color, int depth = 0.0f) {
			DrawSprite(sprite, new Vector2F(x, y), color, depth);
		}

		public void DrawSprite(Sprite sprite, int variantID, int x, int y, Color color, int depth = 0.0f) {
			DrawSprite(sprite, variantID, new Vector2F(x, y), color, depth);
		}

		public void DrawSprite(Sprite sprite, Vector2F position, Color color, int depth = 0.0f) {
			for (Sprite part = sprite; part != null; part = part.NextPart) {
				spriteBatch.Draw(part.Image, NewPos(position) + (Vector2)part.DrawOffset, (Rectangle)part.SourceRect,
					(XnaColor)color, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, depth);
			}
		}

		public void DrawSprite(Sprite sprite, Rectangle2F destination, Color color, int depth = 0.0f) {
			DrawSprite(sprite, 0, destination, color, depth);
		}

		public void DrawSprite(Sprite sprite, int variantID, Rectangle2F destination, Color color, int depth = 0.0f) {
			for (Sprite part = sprite; part != null; part = part.NextPart) {
				Image image = sprite.Image.GetVariant(variantID);
				destination.Point = NewPos(destination.Point) + (Vector2F)part.DrawOffset;
				spriteBatch.Draw(image, (Rectangle)destination, (Rectangle)part.SourceRect,
					(XnaColor)color, 0.0f, Vector2.Zero, SpriteEffects.None, depth);
			}
		}
		*/
	}
}
