using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Common.Audio;
using ZeldaEditor.Control;
using SpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;
using ZeldaOracle.Common.Graphics.Sprites;
using System.Windows.Threading;

namespace ZeldaEditor.WinForms {

	public class TileDisplay : GraphicsDeviceControl {

		private static ContentManager content;
		private static SpriteBatch spriteBatch;

		private EditorWindow	editorWindow;
		private EditorControl	editorControl;

		private DispatcherTimer dispatcherTimer;

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		protected override void Initialize() {
			content		= new ContentManager(Services, "Content");
			spriteBatch	= new SpriteBatch(GraphicsDevice);

			editorControl.SetGraphics(GraphicsDevice, content);

			// Wire the events.
			MouseMove	+= OnMouseMove;
			MouseDown	+= OnMouseDown;
			MouseLeave	+= OnMouseLeave;
			MouseEnter	+= OnMouseEnter;

			// Start the timer to refresh the panel.
			//Application.Idle += delegate { Invalidate(); };
			this.ResizeRedraw = true;

			//UpdateTileset();

			dispatcherTimer = new DispatcherTimer(
				TimeSpan.FromMilliseconds(15),
				DispatcherPriority.Render,
				delegate { Invalidate(); },
				System.Windows.Application.Current.Dispatcher);
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				content.Unload();
			}

			base.Dispose(disposing);
		}

		
		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		public Point2I GetTileCoord(Point2I point) {
			return (point / (GameSettings.TILE_SIZE + Tileset.Spacing));
		}

		public Point2I GetSelectedTileLocation() {
			for (int x = 0; x < Tileset.Width; x++) {
				for (int y = 0; y < Tileset.Height; y++) {
					BaseTileData tileData = Tileset.GetTileData(x, y);
					if (tileData != null && tileData == editorControl.SelectedTilesetTileData) {
						return new Point2I(x, y);
					}
				}
			}
			return -Point2I.One;
		}


		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------
		
		private void OnMouseDown(object sender, MouseEventArgs e) {
			Point2I mousePos = ScrollPosition + e.Location;
			Point2I newSelectedTile = GetTileCoord(mousePos);

			if (newSelectedTile >= Point2I.Zero && newSelectedTile < Tileset.Dimensions) {
				BaseTileData tileData = Tileset.GetTileData(newSelectedTile);

				if (tileData != null) {
					SelectedTile = newSelectedTile;
					editorControl.SelectedTilesetTileData = Tileset.GetTileData(newSelectedTile);
					Invalidate();
				}
			}

			this.Focus();
		}

		private void OnMouseMove(object sender, MouseEventArgs e) {
			
		}
		private void OnMouseLeave(object sender, EventArgs e) {
			
		}
		private void OnMouseEnter(object sender, EventArgs e) {
			
		}

		public void UpdateTileset() {
			this.AutoScrollMinSize = Tileset.Dimensions * (GameSettings.TILE_SIZE + Tileset.Spacing);
			Invalidate();
		}

		public void UpdateZone() {
			Invalidate();
		}


		//-----------------------------------------------------------------------------
		// Overriden methods
		//-----------------------------------------------------------------------------

		protected override void Draw() {
			Graphics2D g = new Graphics2D(spriteBatch);
			//g.SetRenderTarget(GameData.RenderTargetGame);
			GameData.PaletteShader.TilePalette = Zone.Palette;
			GameData.PaletteShader.ApplyPalettes();
			g.Begin(GameSettings.DRAW_MODE_DEFAULT);

			Point2I selectedTileLocation = GetSelectedTileLocation();

			// Draw the tileset.
			g.Clear(Color.White);
			g.PushTranslation(-ScrollPosition);
			for (int y = 0; y < Tileset.Height; y++) {
				for (int x = 0; x < Tileset.Width; x++) {
					BaseTileData tileData = Tileset.GetTileData(x, y);
					if (tileData != null) {
						TileDataDrawing.RewardManager = editorControl.RewardManager;
						TileDataDrawing.Level = editorControl.Level;
						TileDataDrawing.Extras = false;
						TileDataDrawing.PlaybackTime = editorControl.Ticks;
						int spacing = 1;
						Point2I drawPos = new Point2I(x, y) * (GameSettings.TILE_SIZE + spacing) + spacing;
						/*ISprite spr = tileData.Sprite;

						int imageVariantID = tileData.Properties.GetInteger("image_variant", Zone.ImageVariantID);
						if (imageVariantID < 0)
							imageVariantID = Zone.ImageVariantID;
						if (spr is Animation) {
							int substripIndex = tileData.Properties.GetInteger("substrip_index", 0);
							spr = ((Animation) spr).GetSubstrip(substripIndex);
						}

						g.DrawSprite(spr, new SpriteDrawSettings(Zone.StyleDefinitions, imageVariantID), drawPos, Color.White);*/

						TileDataDrawing.DrawTilePreview(g, tileData, drawPos, Zone);
					}
				}
			}


			// Draw the selection box.
			if (selectedTileLocation >= Point2I.Zero) {
				Point2I tilePoint = selectedTileLocation * (GameSettings.TILE_SIZE + Tileset.Spacing);
				//g.Translate(-this.HorizontalScroll.Value, -this.VerticalScroll.Value);
				g.DrawRectangle(new Rectangle2I(tilePoint, (Point2I) GameSettings.TILE_SIZE + 1), 1, Color.White);
				g.DrawRectangle(new Rectangle2I(tilePoint + 1, (Point2I) GameSettings.TILE_SIZE - 1), 1, Color.Black);
				g.DrawRectangle(new Rectangle2I(tilePoint - 1, (Point2I) GameSettings.TILE_SIZE + 3), 1, Color.Black);
				//g.ResetTranslation();
			}

			g.PopTranslation();

			g.End();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public EditorWindow EditorWindow {
			get { return editorWindow; }
			set { editorWindow = value; }
		}

		public EditorControl EditorControl {
			get { return editorControl; }
			set { editorControl = value; }
		}

		public ITileset Tileset {
			get { return editorControl.Tileset; }
		}

		public Zone Zone {
			get { return editorControl.Zone ?? GameData.ZONE_DEFAULT; }
		}

		public Point2I SelectedTile {
			get { return editorControl.SelectedTilesetTile; }
			set { editorControl.SelectedTilesetTile = value; }
		}

		public Point2I ScrollPosition {
			get { return new Point2I(HorizontalScroll.Value, VerticalScroll.Value); }
		}
	}
}

