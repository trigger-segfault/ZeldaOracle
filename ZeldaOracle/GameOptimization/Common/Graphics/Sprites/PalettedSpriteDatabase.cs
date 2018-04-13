using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Util;

namespace ZeldaOracle.Common.Graphics.Sprites {
	/// <summary>A database for palettizing and storing sprites in image grids.</summary>
	public partial class PalettedSpriteDatabase : IDisposable {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------
		
		/// <summary>The path to the content directory.</summary>
		public static readonly string ContentDirectory = PathHelper.CombineExecutable(
				"Content");

		/// <summary>The path to the preloaded paletted sprite database file.</summary>
		public static readonly string DatabaseFile = Path.Combine(
				ContentDirectory, "PalettedSprites.dat");

		/// <summary>The directories to be checked for the checksum.</summary>
		public static readonly string[] ChecksumDirectories = {
			Path.Combine(ContentDirectory, "Images"),
			Path.Combine(ContentDirectory, "Sprites")
		};

		/// <summary>The file version for the database file.</summary>
		public const int DatabaseVersion = 1;


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The collection of different paletted sprite size groups.</summary>
		private Dictionary<Point2I, PalettedSpriteDatabaseSize> spriteSizes;
		/// <summary>True if the paletted sprite data base was preloaded.</summary>
		private bool isPreloaded;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the paletted sprite database.</summary>
		public PalettedSpriteDatabase() {
			spriteSizes = new Dictionary<Point2I, PalettedSpriteDatabaseSize>();
			isPreloaded = false;
		}


		//-----------------------------------------------------------------------------
		// Disposing
		//-----------------------------------------------------------------------------

		/// <summary>Disposes of the images in the paletted sprite database.</summary>
		public void Dispose() {
			foreach (var pair in spriteSizes) {
				pair.Value.Dispose();
			}
			spriteSizes.Clear();
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Clears the paletted sprite database.</summary>
		public void Clear() {
			spriteSizes.Clear();
		}


		//-----------------------------------------------------------------------------
		// Loading
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if the sprite database file exists and can be read.</summary>
		public bool DatabaseFileExists() {
			return File.Exists(DatabaseFile);
		}

		/// <summary>Calculates the checksum for the content and sprites directory.</summary>
		public int CalculateChecksum() {
			int checksum = 0;
			foreach (string directory in ChecksumDirectories) {
				CalculateChecksum(ref checksum, directory);
			}
			return checksum;
		}

		/// <summary>Calculates the checksum for a single directory.</summary>
		private void CalculateChecksum(ref int checksum, string directory) {
			foreach (string file in PathHelper.GetAllFiles(directory)) {
				string name = Path.GetFileName(file).ToLower();
				DateTime modified = File.GetLastWriteTimeUtc(file);
				checksum ^= name.GetHashCode() ^ modified.Ticks.GetHashCode();
			}
		}

		/// <summary>Loads the paletted sprite database from file.</summary>
		public bool Load() {
			try {
				spriteSizes.Clear();
				isPreloaded = false;

				using (Stream stream = File.OpenRead(DatabaseFile)) {
					BinaryReader reader = new BinaryReader(stream);
					int version = reader.ReadInt32();
					if (version == DatabaseVersion) {
						int checksum = reader.ReadInt32();
						if (checksum != CalculateChecksum())
							return false;

						int sizesCount = reader.ReadInt32();

						for (int i = 0; i < sizesCount; i++) {
							Point2I size = reader.ReadPoint2I();
							var spriteSize = new PalettedSpriteDatabaseSize(size);
							spriteSizes.Add(size, spriteSize);
							spriteSize.Read(reader);
						}
						
						isPreloaded = true;
						return true;
					}
					else {
						return false;
					}
				}
			}
			catch (Exception) {
				spriteSizes.Clear();
				return false;
			}
		}

		/// <summary>Save the paletted sprite database to file.</summary>
		public bool Save() {
			try {
				using (Stream stream = File.OpenWrite(DatabaseFile)) {
					BinaryWriter writer = new BinaryWriter(stream);
					stream.SetLength(0);

					writer.Write(DatabaseVersion);
					writer.Write(CalculateChecksum());

					writer.Write(spriteSizes.Count);
					foreach (var pair in spriteSizes) {
						writer.Write(pair.Key);
						pair.Value.Write(writer);
					}
					return true;
				}
			}
			catch (Exception) {
				return false;
			}
		}


		//-----------------------------------------------------------------------------
		// Palettizing
		//-----------------------------------------------------------------------------

		/// <summary>Palettizes the sprite and adds it to the database.</summary>
		public BasicSprite AddSprite(SpritePaletteArgs args) {
			PalettedSpriteDatabaseSize databaseImage;
			if (spriteSizes.ContainsKey(args.SourceRect.Size)) {
				databaseImage = spriteSizes[args.SourceRect.Size];
			}
			else {
				databaseImage = new PalettedSpriteDatabaseSize(args.SourceRect.Size);
				spriteSizes[args.SourceRect.Size] = databaseImage;
			}
			return databaseImage.AddSprite(args);
		}

		/// <summary>Repalettes the already-paletted sprite and adds it to the database.</summary>
		public BasicSprite RepaletteSprite(BasicSprite originalSprite, SpritePaletteArgs args) {
			PalettedSpriteDatabaseSize databaseImage;
			if (spriteSizes.ContainsKey(args.SourceRect.Size)) {
				databaseImage = spriteSizes[args.SourceRect.Size];
			}
			else {
				databaseImage = new PalettedSpriteDatabaseSize(args.SourceRect.Size);
				spriteSizes[args.SourceRect.Size] = databaseImage;
			}
			return databaseImage.RepaletteSprite(originalSprite, args);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		/// <summary>Gets if the paletted sprite data base was preloaded.</summary>
		public bool IsPreloaded {
			get { return isPreloaded; }
		}
	}
}
