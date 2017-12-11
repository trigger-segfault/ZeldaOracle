using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics.Sprites;
using XnaColor = Microsoft.Xna.Framework.Color;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;

namespace ZeldaOracle.Common.Graphics.Sprites {

	public struct ColorGroupSubtypePair {
		public string ColorGroup { get; set; }
		public LookupSubtypes Subtype { get; set; }
		public Color MappedColor { get; set; }

		public ColorGroupSubtypePair(string colorGroup, LookupSubtypes subtype, PaletteDictionary dictionary) {
			this.ColorGroup     = colorGroup;
			this.Subtype        = subtype;
			// Store the map color during creation.
			this.MappedColor	= dictionary.GetMappedColor(colorGroup, subtype);
		}
	}

	public struct SpritePaletteArgs {
		public Image Image { get; set; }
		public Rectangle2I SourceRect { get; set; }
		public Point2I DrawOffset { get; set; }
		public Dictionary<Color, ColorGroupSubtypePair> ColorMapping { get; set; }
		public HashSet<Color> IgnoreColors { get; set; }
		public PaletteDictionary Dictionary { get; set; }
	}

	public class UnspecifiedColorException : Exception {
		public Color Color { get; set; }

		public UnspecifiedColorException(Color color) {
			this.Color = color;
		}
	}

	public class PalettedSpriteDatabase {

		private class PalettedSpriteDatabaseImage {

			/// <summary>The maximum allowed size of an image. Except when a single cell is larger.</summary>
			private static readonly Point2I MaxImageSize = new Point2I(512, 512);

			/// <summary>The index for the next-defined sprite.</summary>
			private int index;
			/// <summary>The spritesheet dimensions of the image.</summary>
			private Point2I dimensions;
			/// <summary>The size of the sprites.</summary>
			private Point2I size;
			/// <summary>The collection of images.</summary>
			private List<Image> images;


			public PalettedSpriteDatabaseImage(Point2I size) {
				this.index          = 0;
				this.size           = size;
				this.images         = new List<Image>();
				this.dimensions     = GMath.Max(Point2I.One, MaxImageSize / size);
			}

			public BasicSprite AddSprite(SpritePaletteArgs args) {
				// Do we need to create a new image
				Image currentImage;
				if (index % IndeciesPerImage == 0) {
					currentImage = new Image(Resources.GraphicsDevice, dimensions * size);
					images.Add(currentImage);
				}
				else {
					currentImage = CurrentImage;
				}

				// Modify the original sprite's colors based on the mapping
				XnaColor[] colorData = new XnaColor[args.SourceRect.Area];
				//XnaColor[] colorData = new XnaColor[currentImage.Width * currentImage.Height];
				XnaRectangle rect = (XnaRectangle) args.SourceRect;
				args.Image.Texture.GetData<XnaColor>(0, rect, colorData, 0, colorData.Length);
				for (int i = 0; i < colorData.Length; i++) {
					Color color = (Color)colorData[i];
					if (args.ColorMapping.ContainsKey(color)) {
						colorData[i] = args.ColorMapping[color].MappedColor;
					}
					else if (!args.IgnoreColors.Contains(color)) {
						throw new UnspecifiedColorException(color);
					}
				}

				// Save the mapping to the database image
				rect = (XnaRectangle) CurrentSourceRect;
				currentImage.Texture.SetData<XnaColor>(0, rect, colorData, 0, colorData.Length);

				// Create a new sprite from the database
				BasicSprite sprite = new BasicSprite(currentImage, CurrentSourceRect, args.DrawOffset);

				index++;
				return sprite;
			}

			private int IndeciesPerImage {
				get { return dimensions.X * dimensions.Y; }
			}
			private Image CurrentImage {
				get { return images.Last(); }
			}
			private Rectangle2I CurrentSourceRect {
				get { return new Rectangle2I((index % dimensions.X) * size.X, (index / dimensions.Y) * size.Y, size); }
			}
		}

		private Dictionary<Point2I, PalettedSpriteDatabaseImage> spriteImages;

		public PalettedSpriteDatabase() {
			this.spriteImages   = new Dictionary<Point2I, PalettedSpriteDatabaseImage>();
		}

		public BasicSprite AddSprite(SpritePaletteArgs args) {
			PalettedSpriteDatabaseImage databaseImage;
			if (spriteImages.ContainsKey(args.SourceRect.Size)) {
				databaseImage = spriteImages[args.SourceRect.Size];
			}
			else {
				databaseImage = new PalettedSpriteDatabaseImage(args.SourceRect.Size);
				spriteImages[args.SourceRect.Size] = databaseImage;
			}
			return databaseImage.AddSprite(args);
		}
	}
}
