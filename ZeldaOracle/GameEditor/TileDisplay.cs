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
			MouseMove += OnMouseMove;
			MouseDown += OnMouseDown;

			// Start the timer to refresh the panel.
			Application.Idle += delegate { Invalidate(); };
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

		public Point2I GetTileLoc(Point2I point) {
			//
			return (point + new Point2I(HorizontalScroll.Value, VerticalScroll.Value)) / (Tileset.SpriteSheet.CellSize + Tileset.SpriteSheet.Spacing);
		}

		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------

		private void OnLeftClickTile() {

		}

		private void OnMouseDown(object sender, MouseEventArgs e) {
			Point2I mouseLoc = new Point2I(e.X, e.Y);
			Point2I newSelectedTile = GetTileLoc(mouseLoc);
			if (newSelectedTile >= Point2I.Zero && newSelectedTile < Tileset.Size) {
				SelectedTile = newSelectedTile;
				Invalidate();
			}
		}

		private void OnMouseMove(object sender, MouseEventArgs e) {
			
		}

		public void UpdateTileset() {
			this.AutoScrollMinSize = Tileset.Size * (Tileset.SpriteSheet.CellSize + Tileset.SpriteSheet.Spacing);
		}

		public void UpdateZone() {

		}

		//-----------------------------------------------------------------------------
		// Overriden methods
		//-----------------------------------------------------------------------------

		protected override void Draw() {

			Graphics2D g = new Graphics2D(spriteBatch);

			//g.SetRenderTarget(GameData.RenderTargetGame);

			g.Begin(GameSettings.DRAW_MODE_DEFAULT);
			g.Clear(Color.White);
			g.Translate(new Vector2F(-this.HorizontalScroll.Value, -this.VerticalScroll.Value));
			g.DrawImage(Tileset.SpriteSheet.Image.GetVariant(Zone.ImageVariantID), Point2I.Zero);
			g.DrawRectangle(new Rectangle2I(SelectedTile * (Tileset.SpriteSheet.CellSize + Tileset.SpriteSheet.Spacing), Tileset.SpriteSheet.CellSize), 1, Color.White);
			g.ResetTranslation();
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

		public Tileset Tileset {
			get { return editorControl.Tileset; }
		}
		public Zone Zone {
			get { return editorControl.Zone; }
		}

		public Point2I SelectedTile {
			get { return editorControl.SelectedTile; }
			set { editorControl.SelectedTile = value; }
		}
	}
}

