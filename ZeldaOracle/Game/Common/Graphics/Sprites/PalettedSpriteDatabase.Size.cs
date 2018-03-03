using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using XnaColor = Microsoft.Xna.Framework.Color;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;

namespace ZeldaOracle.Common.Graphics.Sprites {
	/// <summary>A database for palettizing and storing sprites in image grids.</summary>
	public partial class PalettedSpriteDatabase {

		//-----------------------------------------------------------------------------
		// Classes
		//-----------------------------------------------------------------------------

		/// <summary>A collection of palettized sprites of a single size.</summary>
		private class PalettedSpriteDatabaseSize {

			/// <summary>The maximum allowed size of an image. Except when a single cell is larger.</summary>
			private static readonly Point2I MaxImageSize = new Point2I(512, 512);

			/// <summary>The index for the next-defined sprite.</summary>
			private int spriteIndex;
			/// <summary>The spritesheet dimensions of the image.</summary>
			private Point2I dimensions;
			/// <summary>The size of the sprites.</summary>
			private Point2I size;
			/// <summary>The collection of images.</summary>
			private List<Image> images;
			/// <summary>The number of sprites preloaded.</summary>
			private int preloadCount;


			//-----------------------------------------------------------------------------
			// Constructor
			//-----------------------------------------------------------------------------

			/// <summary>Constructs the paletted sprite database image.</summary>
			public PalettedSpriteDatabaseSize(Point2I size) {
				this.spriteIndex    = 0;
				this.size           = size;
				this.images         = new List<Image>();
				this.dimensions     = GMath.Max(Point2I.One, MaxImageSize / size);
				this.preloadCount	= -1;
			}


			//-----------------------------------------------------------------------------
			// Disposing
			//-----------------------------------------------------------------------------

			/// <summary>Disposes of the images used for storing palettized sprites.</summary>
			public void Dispose() {
				foreach (Image image in images) {
					image.Dispose();
				}
			}


			//-----------------------------------------------------------------------------
			// Palettizing
			//-----------------------------------------------------------------------------

			/// <summary>Repalettes the already-paletted sprite.</summary>
			public BasicSprite RepaletteSprite(BasicSprite originalSprite, SpritePaletteArgs args) {
				if (IsPreloaded && spriteIndex >= preloadCount)
					throw new SpriteDatabaseException("Number of paletted images required " +
						"exceeds amount of preloaded sprites!");

				// Skip all the busywork
				Image currentImage;
				if (IsPreloaded) {
					currentImage = CurrentImage;
					goto FinishSprite;
				}

				// Do we need to create a new image
				if (ImageIndex == 0) {
					currentImage = new Image(Resources.GraphicsDevice, dimensions * size);
					images.Add(currentImage);
				}
				else {
					currentImage = CurrentImage;
				}

				// Modify the original sprite's colors based on the mapping
				XnaColor[] colorData = new XnaColor[originalSprite.SourceRect.Area];
				Point2I rectSize = originalSprite.SourceRect.Size;
				//XnaColor[] colorData = new XnaColor[currentImage.Width * currentImage.Height];
				XnaRectangle rect = (XnaRectangle) originalSprite.SourceRect;
				originalSprite.Image.Texture.GetData<XnaColor>(0, rect, colorData, 0, colorData.Length);

				for (int x = 0; x < rectSize.X; x++) {
					for (int y = 0; y < rectSize.Y; y++) {
						int index = x + y * rectSize.X;
						Color color = (Color)colorData[index];

						// Don't palette ignored colors
						if (color.A == PaletteDictionary.AlphaIdentifier) {
							int subtype = (color.R | (color.G << 8)) % 4;
							colorData[index] = args.DefaultMapping[subtype];
						}
					}
				}

				// Save the mapping to the database image
				rect = (XnaRectangle) CurrentSourceRect;
				currentImage.Texture.SetData<XnaColor>(0, rect, colorData, 0, colorData.Length);

				// Skip all the busywork
				FinishSprite:

				// Create a new sprite from the database
				BasicSprite sprite = new BasicSprite(currentImage, CurrentSourceRect);

				spriteIndex++;
				return sprite;
			}

			/// <summary>Palettizes a new sprite.</summary>
			public BasicSprite AddSprite(SpritePaletteArgs args) {
				if (IsPreloaded && spriteIndex >= preloadCount)
					throw new SpriteDatabaseException("Number of paletted images required " +
						"exceeds amount of preloaded sprites!");

				// Skip all the busywork
				Image currentImage;
				if (IsPreloaded) {
					currentImage = CurrentImage;
					goto FinishSprite;
				}

				// Do we need to create a new image
				if (ImageIndex == 0) {
					currentImage = new Image(Resources.GraphicsDevice, dimensions * size);
					images.Add(currentImage);
				}
				else {
					currentImage = CurrentImage;
				}

				// The list of already scanned colors
				HashSet<Color> scannedColors = new HashSet<Color>();
				List<Color> focusColors = new List<Color>();
				List<Color> ignoredColors = new List<Color>();
				bool transparentOnly = false;

				Color defaultTransparent = Color.Transparent;
				Color defaultBlack = Color.Black;
				if (args.DefaultMapping != null) {
					defaultTransparent = args.DefaultMapping[(int) LookupSubtypes.Transparent];
					defaultBlack = args.DefaultMapping[(int) LookupSubtypes.Black];

				}

				// Modify the original sprite's colors based on the mapping
				XnaColor[] colorData = new XnaColor[args.SourceRect.Area];
				Point2I rectSize = args.SourceRect.Size;
				//XnaColor[] colorData = new XnaColor[currentImage.Width * currentImage.Height];
				XnaRectangle rect = (XnaRectangle) args.SourceRect;
				args.Image.Texture.GetData<XnaColor>(0, rect, colorData, 0, colorData.Length);
				Point2I chunkSize = args.ChunkSize;
				if (chunkSize.IsZero)
					chunkSize = rectSize;
				Point2I numChunks = (rectSize + chunkSize - 1) / chunkSize;
				for (int chunkX = 0; chunkX < numChunks.X; chunkX++) {
					for (int chunkY = 0; chunkY < numChunks.Y; chunkY++) {
						scannedColors.Clear();
						focusColors.Clear();
						ignoredColors.Clear();
						HashSet<int> possibleGroups = new HashSet<int>(args.IndexedPossibleColorGroups);

						for (int x = 0; x < chunkSize.X; x++) {
							int ix = chunkX * chunkSize.X + x;
							if (ix >= rectSize.X) break;
							for (int y = 0; y < chunkSize.Y; y++) {
								int iy = chunkY * chunkSize.Y + y;
								if (iy >= rectSize.Y) break;
								int index = ix + iy * rectSize.X;
								Color color = (Color) colorData[index];
								if (color.A == 0) {
									color = Color.Transparent;
									colorData[index] = color;
									// If no mapped colors have been encountered yet
									if (scannedColors.Count == ignoredColors.Count)
										transparentOnly = true;
								}
								if (!scannedColors.Contains(color)) {
									scannedColors.Add(color);
									Dictionary<int, ColorGroupSubtypePair> dict;
									if (args.IgnoreColors.Contains(color)) {
										ignoredColors.Add(color);
										// Carry on
									}
									else if (args.ColorMapping.TryGetValue(color, out dict)) {
										possibleGroups.RemoveWhere(s => !dict.ContainsKey(s));
										if (color.A != 0)
											transparentOnly = false;
										if (!possibleGroups.Any())
											throw new NoMatchingColorGroupsException(scannedColors, args.SourceRect.Point + new Point2I(ix, iy));
									}
									else if (color == Color.Black) {
										// All groups are valid here
									}
									else if (color.A == 0) {
										transparentOnly = (scannedColors.Count + 1 > ignoredColors.Count);
										if (args.Dictionary.PaletteType != PaletteTypes.Entity && !transparentOnly) {
											throw new UnspecifiedColorException(color, args.SourceRect.Point + new Point2I(ix, iy));
										}
									}
									else {
										throw new UnspecifiedColorException(color, args.SourceRect.Point + new Point2I(ix, iy));
									}
								}
							}
						}

						if (!transparentOnly) {
							int indexedFinalColorGroup = -1;
							string finalColorGroup = null;
							for (int i = 0; i < args.IndexedPossibleColorGroups.Length; i++) {
								int index = args.IndexedPossibleColorGroups[i];
								if (possibleGroups.Contains(index)) {
									indexedFinalColorGroup = index;
									finalColorGroup = args.PossibleColorGroups[i];
									break;
								}
							}

							Dictionary<Color, Color> finalColorMapping = new Dictionary<Color, Color>();
							bool transparentDefined = args.Dictionary.PaletteType != PaletteTypes.Entity;
							bool blackDefined = false;
							foreach (var pair in args.ColorMapping) {
								if (pair.Value.ContainsKey(indexedFinalColorGroup)) {
									ColorGroupSubtypePair colorGroupPair = pair.Value[indexedFinalColorGroup];
									if (!transparentDefined && colorGroupPair.Subtype == LookupSubtypes.Transparent)
										transparentDefined = true;
									else if (!blackDefined && colorGroupPair.Subtype == LookupSubtypes.Black)
										blackDefined = true;
									if (args.DefaultMapping != null)
										finalColorMapping.Add(pair.Key, args.DefaultMapping[(int) colorGroupPair.Subtype]);
									else
										finalColorMapping.Add(pair.Key, colorGroupPair.MappedColor);
								}
							}
							if (!transparentDefined)
								finalColorMapping.Add(Color.Transparent, args.Dictionary.GetMappedColor(finalColorGroup, LookupSubtypes.Transparent));
							if (!blackDefined)
								finalColorMapping.Add(Color.Black, args.Dictionary.GetMappedColor(finalColorGroup, LookupSubtypes.Black));

							for (int y = 0; y < chunkSize.Y; y++) {
								int iy = chunkY * chunkSize.Y + y;
								if (iy >= rectSize.Y) break;
								for (int x = 0; x < chunkSize.X; x++) {
									int ix = chunkX * chunkSize.X + x;
									if (ix >= rectSize.X) break;
									int index = ix + iy * rectSize.X;
									// Don't palette ignored colors or transparency
									Color color = (Color) colorData[index];
									if (color.A == 0)
										colorData[index] = Color.Transparent;
									else if (finalColorMapping.ContainsKey(color))
										colorData[index] = finalColorMapping[color];
								}
							}
						}
					}
				}

				// Save the mapping to the database image
				rect = (XnaRectangle) CurrentSourceRect;
				currentImage.Texture.SetData<XnaColor>(0, rect, colorData, 0, colorData.Length);

				// Skip all the busywork
				FinishSprite:

				// Create a new sprite from the database
				BasicSprite sprite = new BasicSprite(currentImage, CurrentSourceRect);

				spriteIndex++;
				return sprite;
			}


			//-----------------------------------------------------------------------------
			// Loading
			//-----------------------------------------------------------------------------

			/// <summary>Reads the paletted sprite database size and all of its images.</summary>
			public void Read(BinaryReader reader) {
				preloadCount = reader.ReadInt32();
				int imageCount = reader.ReadInt32();
				for (int i = 0; i < imageCount; i++) {
					int pngSize = reader.ReadInt32();
					Texture2D texture = Texture2DLoader.FromStream(reader.BaseStream, pngSize);
					images.Add(new Image(texture));
				}
			}

			/// <summary>Writes the paletted sprite database size and all of its images.</summary>
			public void Write(BinaryWriter writer) {
				TextureLoader loader = new TextureLoader(Resources.GraphicsDevice);
				writer.Write(spriteIndex);
				writer.Write(images.Count);
				foreach (Image image in images) {
					// Write the png size later
					writer.Write((int) 0);
					int pngStart = (int) writer.BaseStream.Position;
					image.Texture.SaveAsPng(writer.BaseStream, image.Width, image.Height);
					int pngSize = (int) writer.BaseStream.Position - pngStart;
					writer.BaseStream.Position = pngStart - 4;
					writer.Write(pngSize);
					writer.BaseStream.Position += pngSize;
				}
			}


			//-----------------------------------------------------------------------------
			// Properties
			//-----------------------------------------------------------------------------

			/// <summary>True if the sprite images were preloaded.</summary>
			private bool IsPreloaded {
				get { return preloadCount != -1; }
			}

			/// <summary>The number of indecies to assign before making a new image.</summary>
			private int IndeciesPerImage {
				get { return dimensions.X * dimensions.Y; }
			}

			/// <summary>The current image to assign sprites to.</summary>
			private Image CurrentImage {
				get { return images[spriteIndex / IndeciesPerImage]; }
			}

			/// <summary>The index of the current image to assign sprites to.</summary>
			private int ImageIndex {
				get { return spriteIndex % IndeciesPerImage; }
			}

			/// <summary>The current source rect to assign sprites to in the image.</summary>
			private Rectangle2I CurrentSourceRect {
				get { return new Rectangle2I((ImageIndex % dimensions.X) * size.X, (ImageIndex / dimensions.X) * size.Y, size); }
			}
		}
	}
}
