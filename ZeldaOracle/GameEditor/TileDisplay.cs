using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsGraphicsDevice;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Common.Audio;
using ZeldaEditor.Control;

namespace ZeldaEditor {

	public class TileDisplay : GraphicsDeviceControl {

		private static ContentManager content;
		private static Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch;

		private EditorForm	editorForm;
		private EditorControl editorControl;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		protected override void Initialize() {
			content		= new ContentManager(Services, "Content");
			spriteBatch	= new Microsoft.Xna.Framework.Graphics.SpriteBatch(GraphicsDevice);

			editorControl.Initialize(content, GraphicsDevice);

			// Wire the events.
			MouseMove	+= OnMouseMove;
			MouseDown	+= OnMouseDown;
			MouseLeave	+= OnMouseLeave;
			MouseEnter	+= OnMouseEnter;

			// Start the timer to refresh the panel.
			//Application.Idle += delegate { Invalidate(); };
			this.ResizeRedraw = true;

			UpdateTileset();
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
			return (point / (Tileset.CellSize + Tileset.Spacing));
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
			return new Point2I(-1, -1);
		}


		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------

		private void OnLeftClickTile() {

		}

		private void OnMouseDown(object sender, MouseEventArgs e) {
			Point2I mousePos = ScrollPosition + e.Location;
			Point2I newSelectedTile = GetTileCoord(mousePos);

			if (newSelectedTile >= Point2I.Zero && newSelectedTile < Tileset.Size) {
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
			this.Focus();
		}

		public void UpdateTileset() {
			this.AutoScrollMinSize = Tileset.Size * (Tileset.CellSize + Tileset.Spacing);
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
			g.Begin(GameSettings.DRAW_MODE_DEFAULT);

			Point2I selectedTileLocation = GetSelectedTileLocation();

			// Draw the tileset.
			g.Clear(Color.White);
			g.Translate(-this.HorizontalScroll.Value, -this.VerticalScroll.Value);
			if (Tileset.SpriteSheet == null) {
				// Draw each tile's sprite seperately.
				for (int y = 0; y < Tileset.Height; y++) {
					for (int x = 0; x < Tileset.Width; x++) {
						BaseTileData tileData = Tileset.GetTileData(x, y);
						if (tileData != null) {
							int spacing = 1;
							Vector2F drawPos = new Vector2F(x, y) * (Tileset.CellSize + spacing);
							SpriteAnimation spr = tileData.Sprite;
							
							int imageVariantID = tileData.Properties.GetInteger("image_variant", Zone.ImageVariantID);
							if (imageVariantID < 0)
								imageVariantID = Zone.ImageVariantID;
							if (spr.IsAnimation) {
								int substripIndex = tileData.Properties.GetInteger("substrip_index", 0);
								spr.Animation = spr.Animation.GetSubstrip(substripIndex);
							}

							g.DrawAnimation(tileData.Sprite, imageVariantID, 0.0f, drawPos, Color.White);
						}
					}
				}
			}
			else {
				// Draw the spritesheet's image.
				g.Translate(-Tileset.SpriteSheet.Offset);
				g.DrawImage(Tileset.SpriteSheet.Image.GetVariant(Zone.ImageVariantID), Point2I.Zero);
				g.ResetTranslation();
			}


			// Draw the selection box.
			if (selectedTileLocation >= Point2I.Zero) {
				Point2I tilePoint = selectedTileLocation * (Tileset.CellSize + Tileset.Spacing);
				g.Translate(-this.HorizontalScroll.Value, -this.VerticalScroll.Value);
				g.DrawRectangle(new Rectangle2I(tilePoint, Tileset.CellSize + 1), 1, Color.White);
				g.DrawRectangle(new Rectangle2I(tilePoint + 1, Tileset.CellSize - 1), 1, Color.Black);
				g.DrawRectangle(new Rectangle2I(tilePoint - 1, Tileset.CellSize + 3), 1, Color.Black);
				g.ResetTranslation();
			}

			g.End();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public EditorForm EditorForm {
			get { return editorForm; }
			set { editorForm = value; }
		}

		public EditorControl EditorControl {
			get { return editorControl; }
			set { editorControl = value; }
		}

		public ITileset Tileset {
			get { return editorControl.Tileset; }
		}

		public Zone Zone {
			get { return editorControl.Zone; }
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

