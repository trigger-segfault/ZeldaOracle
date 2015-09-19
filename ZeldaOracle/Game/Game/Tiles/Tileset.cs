using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using Microsoft.Xna.Framework;

namespace ZeldaOracle.Game.Tiles {

	public class Tileset {
		
		private TileData[,]	tileData;
		private Point2I		size;
		private SpriteSheet	sheet;
		private Point2I		defaultTile;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Tileset(SpriteSheet sheet, int width, int height) {
			this.sheet			= sheet;
			this.size			= new Point2I(width, height);
			this.defaultTile	= Point2I.Zero;
			this.tileData		= new TileData[width, height];

			// Create default tiledata.
			for (int x = 0; x < size.X; x++) {
				for (int y = 0; y < size.Y; y++) {
					tileData[x, y] = new TileData();
					tileData[x, y].Tileset = this;
					tileData[x, y].SheetLocation = new Point2I(x, y);
				}
			}
		}
		

		//-----------------------------------------------------------------------------
		// Functions
		//----------------------------------------------------------------------------

		public Tile CreateTile(Point2I sheetLocation) {
			TileData data = tileData[sheetLocation.X, sheetLocation.Y];

			Tile tile;
			if (data.Type == null)
				tile = new Tile();
			else
				tile = (Tile) data.Type.GetConstructor(Type.EmptyTypes).Invoke(null);


			// TODO: needs to instantiate other tile sub-class types.
			
			tile.Tileset			= this;
			tile.TileSheetLocation	= sheetLocation;
			tile.Flags				= data.Flags;
			tile.Sprite				= data.Sprite;
			tile.CollisionModel		= data.CollisionModel;
			tile.Size				= data.Size;
			tile.AnimationPlayer.Animation = data.Animation;

			return tile;
		}

		public void LoadConfig(string filename) {
			// Create default tile data.
			for (int x = 0; x < size.X; x++) {
				for (int y = 0; y < size.Y; y++) {
					tileData[x, y] = new TileData();
					tileData[x, y].Tileset			= this;
					tileData[x, y].SheetLocation	= new Point2I(x, y);
					tileData[x, y].Sprite			= new Sprite(sheet, x, y, 0, 0);
				}
			}
			
			// Read the character grid.
			try {
				Stream stream = TitleContainer.OpenStream(filename);
				StreamReader reader = new StreamReader(stream, Encoding.ASCII);
				
				for (int y = 0; y < size.Y; y++) {
					string line = reader.ReadLine();

					for (int x = 0; x < size.X && x < line.Length; x++) {
						TileData data = tileData[x, y];
						char c = line[x];

						switch (c) {
							case 'S':
								data.Flags |= TileFlags.Solid;
								data.CollisionModel = GameData.MODEL_BLOCK;
								break;
							case 'F':
								data.Flags |= TileFlags.Waterfall | TileFlags.Solid | TileFlags.Ledge;
								data.CollisionModel = GameData.MODEL_BLOCK;
								break;
							case 'L': data.Flags |= TileFlags.Ledge;		break;
							case 'G': data.Flags |= TileFlags.Diggable;		break;
							case 'H': data.Flags |= TileFlags.Hole;			break;
							case 'V': data.Flags |= TileFlags.Lava;			break;
							case 'W': data.Flags |= TileFlags.Water;		break;
							case 'I': data.Flags |= TileFlags.Ice;			break;
							case 'R': data.Flags |= TileFlags.Stairs;		break;
							case 'D': data.Flags |= TileFlags.Ladder;		break;
							case 'A': data.Flags |= TileFlags.HalfSolid;	break;
							case 'P': data.Flags |= TileFlags.Puddle;		break;
							case 'O': data.Flags |= TileFlags.Water | TileFlags.Ocean; break;
						}
					}

					stream.Close();
				}
			}
			catch (FileNotFoundException) {
				Console.WriteLine("Error loading tileset config \"" + filename + "\"");
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public TileData[,] TileData {
			get { return tileData; }
		}
		
		public Point2I Size {
			get { return size; }
			set { size = value; }
		}
		
		public int Width {
			get { return size.X; }
			set { size.X = value; }
		}
		
		public int Height {
			get { return size.Y; }
			set { size.Y = value; }
		}
		
		public Point2I DefaultTile {
			get { return defaultTile; }
			set { defaultTile = value; }
		}

		public SpriteSheet SpriteSheet {
			get { return sheet; }
			set { sheet = value; }
		}
	}
}
