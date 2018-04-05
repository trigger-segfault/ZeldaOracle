using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XnaColor = Microsoft.Xna.Framework.Color;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Util;

namespace ZeldaOracle.Common.Graphics.Sprites {
	/// <summary>A database for palettizing and storing sprites in image grids.</summary>
	public partial class PalettedSpriteDatabase {

		/// <summary>A collection of palettized sprites of a single size.</summary>
		private class PalettedSpriteDatabaseSize : IDisposable {

			//-----------------------------------------------------------------------------
			// Constants
			//-----------------------------------------------------------------------------

			/// <summary>The maximum allowed size of an image. Except when a single
			/// cell is larger.</summary>
			private static readonly Point2I MaxImageSize = new Point2I(512, 512);


			//-----------------------------------------------------------------------------
			// Members
			//-----------------------------------------------------------------------------

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
				this.size		= size;
				spriteIndex		= 0;
				images			= new List<Image>();
				dimensions		= GMath.Max(Point2I.One, MaxImageSize / size);
				preloadCount	= -1;
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
			public BasicSprite RepaletteSprite(BasicSprite originalSprite,
				SpritePaletteArgs args)
			{
				// Skip all the busywork
				if (PreloadCheck())
					goto FinishSprite;

				// Modify the original sprite's colors based on the mapping
				Point2I rectSize = originalSprite.SourceRect.Size;
				XnaColor[] colorData =
					originalSprite.Image.GetData(originalSprite.SourceRect);

				for (int y = 0; y < rectSize.Y; y++) {
					for (int x = 0; x < rectSize.X; x++) {
						int index = x + y * rectSize.X;
						Color color = colorData[index].ToColor();

						// Don't palette ignored colors
						if (color.A == PaletteDictionary.AlphaIdentifier) {
							int subtype = (color.R | (color.G << 8)) % 4;
							colorData[index] =
								args.DefaultMapping[subtype].ToXnaColor();
						}
					}
				}

				// Save the mapping to the database image
				CurrentImage.SetData(colorData, CurrentSourceRect);

				// Skip all the busywork
				FinishSprite:

				// Create a new sprite from the database
				BasicSprite sprite = new BasicSprite(CurrentImage, CurrentSourceRect);

				spriteIndex++;
				return sprite;
			}

			/// <summary>Palettizes a new sprite.</summary>
			public BasicSprite AddSprite(SpritePaletteArgs args) {
				// Skip all the busywork
				if (PreloadCheck())
					goto FinishSprite;

				// Modify the original sprite's colors based on the mapping
				XnaColor[] colorData = args.Image.GetData(args.SourceRect);

				Point2I rectSize = args.SourceRect.Size;
				Point2I chunkSize = args.ChunkSize;
				if (chunkSize.X == 0) chunkSize.X = rectSize.X;
				if (chunkSize.Y == 0) chunkSize.Y = rectSize.Y;
				Point2I numChunks = GMath.CeilingDiv(rectSize, chunkSize);

				for (int chunkY = 0; chunkY < numChunks.Y; chunkY++) {
					for (int chunkX = 0; chunkX < numChunks.X; chunkX++) {
						PaletteChunk(args, colorData, new Point2I(chunkX, chunkY));
					}
				}

				// Save the mapping to the database image
				CurrentImage.SetData(colorData, CurrentSourceRect);

				// Skip all the busywork
				FinishSprite:

				// Create a new sprite from the database
				BasicSprite sprite = new BasicSprite(CurrentImage, CurrentSourceRect);

				spriteIndex++;
				return sprite;
			}

			/// <summary>Palettes a single chunk of a sprite.</summary>
			private void PaletteChunk(SpritePaletteArgs args, XnaColor[] colorData,
				Point2I chunk)
			{
				// The list of already scanned colors
				HashSet<Color> scannedColors = new HashSet<Color>();
				// The indexes of the possible color groups
				// Lower indexes have priority
				HashSet<int> possibleGroups = new HashSet<int>();
				//	new HashSet<int>(args.IndexedPossibleColorGroups);
				for (int i = 0; i < args.PossibleColorGroups.Length; i++)
					possibleGroups.Add(i);

				// Transparency checks
				int ignoredColorsCount = 0;
				bool transparentOnly = false;

				// Constant
				bool isEntityPalette =
					(args.Dictionary.PaletteType == PaletteTypes.Entity);

				Point2I rectSize = args.SourceRect.Size;
				Point2I chunkSize = args.ChunkSize;
				if (chunkSize.X == 0) chunkSize.X = rectSize.X;
				if (chunkSize.Y == 0) chunkSize.Y = rectSize.Y;
				Point2I chunkStart = chunk * chunkSize;

				// Eliminate all color groups that don't match
				for (int y = 0; y < chunkSize.Y; y++) {
					int iy = chunkStart.Y + y;
					if (iy >= rectSize.Y) break;
					for (int x = 0; x < chunkSize.X; x++) {
						int ix = chunkStart.X + x;
						if (ix >= rectSize.X) break;

						int index = ix + iy * rectSize.X;
						Color color = colorData[index].ToColor();

						if (color.IsTransparent) {
							// Force the color's RGB channels to 0
							color = Color.Transparent;
							colorData[index] = color.ToXnaColor();
							// Check if no mapped colors have been encountered yet
							if (scannedColors.Count == ignoredColorsCount)
								transparentOnly = true;
						}

						// Skip this color if its already been scanned and added
						if (scannedColors.Add(color)) {
							Dictionary<int, ColorGroupSubtypePair> dict;
							if (args.IgnoreColors.Contains(color)) {
								ignoredColorsCount++;
								// Carry on
							}
							else if (args.ColorMapping.TryGetValue(color, out dict)) {
								possibleGroups.RemoveWhere(s => !dict.ContainsKey(s));
								if (!color.IsTransparent)
									transparentOnly = false;

								// Check if this new color eliminates
								// all possible color groups
								if (!possibleGroups.Any())
									throw new NoMatchingColorGroupsException(
										scannedColors, args.SourceRect.Point +
										new Point2I(ix, iy));
							}
							/*else if (color == Color.Black) {
								// All groups are valid here
							}*/
							else if (color.IsTransparent) {
								// Check if no mapped colors have been encountered yet
								transparentOnly =
									(scannedColors.Count + 1 > ignoredColorsCount);
								if (!isEntityPalette && !transparentOnly) {
									throw new UnspecifiedColorException(color,
										args.SourceRect.Point + new Point2I(ix, iy));
								}
							}
							else {
								// This color is not present in
								// any of the valid color groups!
								throw new UnspecifiedColorException(color,
									args.SourceRect.Point + new Point2I(ix, iy));
							}
						}
					}
				}

				// Skip all the extra work if it's just transparency
				if (!transparentOnly) {
					int colorGroupIndex = possibleGroups.First();
					string colorGroup = args.PossibleColorGroups[colorGroupIndex];
					/*int colorGroupIndex = -1;
					string colorGroup = null;
					for (int i = 0; i < args.IndexedPossibleColorGroups.Length; i++) {
						int index = args.IndexedPossibleColorGroups[i];
						if (possibleGroups.Contains(index)) {
							colorGroupIndex = index;
							colorGroup = args.PossibleColorGroups[i];
							break;
						}
					}*/

					var colorMapping = new Dictionary<Color, Color>();
					
					/*bool transparentDefined = !isEntityPalette;
					bool blackDefined = false;*/
					foreach (var pair in args.ColorMapping) {
						ColorGroupSubtypePair colorGroupPair;
						if (pair.Value.TryGetValue(colorGroupIndex, out colorGroupPair)) {
							/*if (!transparentDefined &&
								colorGroupPair.Subtype == LookupSubtypes.Transparent)
								transparentDefined = true;
							else if (!blackDefined &&
								colorGroupPair.Subtype == LookupSubtypes.Black)
								blackDefined = true;*/

							// Override with default mappings if they are defined
							if (args.DefaultMapping != null)
								colorMapping.Add(pair.Key,
									args.DefaultMapping[(int) colorGroupPair.Subtype]);
							else
								colorMapping.Add(pair.Key,
									colorGroupPair.MappedColor);
						}
					}

					/*if (!transparentDefined)
						colorMapping.Add(Color.Transparent,
							args.Dictionary.GetMappedColor(
								colorGroup, LookupSubtypes.Transparent));

					if (!blackDefined)
						colorMapping.Add(Color.Black,
							args.Dictionary.GetMappedColor(
								colorGroup, LookupSubtypes.Black));*/

					for (int y = 0; y < chunkSize.Y; y++) {
						int iy = chunkStart.Y + y;
						if (iy >= rectSize.Y) break;
						for (int x = 0; x < chunkSize.X; x++) {
							int ix = chunkStart.X + x;
							if (ix >= rectSize.X) break;

							int index = ix + iy * rectSize.X;
							Color color = colorData[index].ToColor();

							// Don't palette ignored colors or transparency
							if (color.IsTransparent)
								colorData[index] = XnaColor.Transparent;
							// If the color is not present in the final
							// mapping, then it was an ignored color.
							else if (colorMapping.TryGetValue(color, out color))
								colorData[index] = color.ToXnaColor();
						}
					}
				}
			}


			//-----------------------------------------------------------------------------
			// Preload Checks
			//-----------------------------------------------------------------------------

			/// <summary>Checks if preloading should be used and also throws an exception
			/// if the sprite index surpasses the preloaded sprite count.</summary>
			private bool PreloadCheck() {
				if (IsPreloaded && spriteIndex >= preloadCount)
					throw new SpriteDatabaseException("Number of paletted images " +
						"required exceeds amount of preloaded sprites!");

				// Skip all the busywork
				return IsPreloaded;
			}


			//-----------------------------------------------------------------------------
			// Loading
			//-----------------------------------------------------------------------------

			/// <summary>Reads the paletted sprite database size and all of its images.</summary>
			public void Read(BinaryReader reader) {
				preloadCount = reader.ReadInt32();
				int imageCount = reader.ReadInt32();
				for (int i = 0; i < imageCount; i++) {
					images.Add(Image.FromStreamAndSize(reader.BaseStream));
				}
			}

			/// <summary>Writes the paletted sprite database size and all of its images.</summary>
			public void Write(BinaryWriter writer) {
				writer.Write(spriteIndex);
				writer.Write(images.Count);
				foreach (Image image in images) {
					image.SaveAsPngAndSize(writer.BaseStream);
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
			private int IndexesPerImage {
				get { return dimensions.X * dimensions.Y; }
			}

			/// <summary>The current image to assign sprites to.</summary>
			private Image CurrentImage {
				get {
					int index = spriteIndex / IndexesPerImage;
					if (images.Count == index) {
						Image image = new Image(dimensions * size);
						images.Add(image);
					}
					return images[index];
				}
			}

			/// <summary>The index of the sprite in the current image.</summary>
			private int ImageIndex {
				get { return spriteIndex % IndexesPerImage; }
			}

			/// <summary>The current source rect to assign sprites to in the image.</summary>
			private Rectangle2I CurrentSourceRect {
				get {
					return new Rectangle2I(
						(ImageIndex % dimensions.X) * size.X,
						(ImageIndex / dimensions.X) * size.Y, size);
				}
			}
		}
	}
}
