using System;
using System.Collections.Generic;
using Size = System.Drawing.Size;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game;
using System.Windows.Threading;
using System.Diagnostics;
using ZeldaOracle.Game.Tiles;
using ConscriptDesigner.Control;
using ZeldaOracle.Game.Worlds;
using MouseButtons = System.Windows.Forms.MouseButtons;
using System.Windows.Forms;
using ConscriptDesigner.Anchorables.TilesetEditorTools;
using ZeldaOracle.Common.Util;

namespace ConscriptDesigner.WinForms {
	public class TilesetEditorDisplay : ZeldaUniqueGraphicsDeviceControl {

		public readonly Color FadeColor = new Color(200, 200, 200, 100);

		private Tileset tileset;

		private static List<TilesetEditorTool> tools;
		private static ToolPlace toolPlace;
		private static ToolSelection toolSelection;
		private static ToolEyedrop toolEyedrop;

		private static int currentToolIndex;

		private static bool overwrite;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		static TilesetEditorDisplay() {
			try {
				tools = new List<TilesetEditorTool>();

				tools = new List<TilesetEditorTool>();
				tools.Add(toolPlace = new ToolPlace());
				tools.Add(toolSelection = new ToolSelection());
				tools.Add(toolEyedrop = new ToolEyedrop());

				currentToolIndex = 0;

				overwrite = false;
			}
			catch (Exception) {

			}
		}

		public TilesetEditorDisplay() {
			this.tileset = null;

			MouseEnter += OnMouseEnter;
			MouseLeave += OnMouseLeave;
			MouseDown += OnMouseDown;
			MouseUp += OnMouseUp;
			MouseMove += OnMouseMove;
			MouseDoubleClick += OnMouseDoubleClick;
		}
		
		protected override void Initialize() {
			base.Initialize();

			foreach (TilesetEditorTool tool in tools) {
				tool.Initialize(this);
			}
		}


		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------

		public event EventHandler Modified;
		public event EventHandler ToolChanged;


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		/*private void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			if (tileset == null) return;
			if (e.Button == MouseButtons.Middle) {
				if (HoverTileData != null) {
					DesignerControl.SelectedTileData = HoverTileData;
					DesignerControl.SelectedTileset = null;
					DesignerControl.SelectedTileLocation = -Point2I.One;
				}
			}
			else if (e.Button == MouseButtons.Left) {
				if (DesignerControl.SelectedTileData != null) {
					tileset.SetTileData(hoverPoint, DesignerControl.SelectedTileData);
					if (Modified != null)
						Modified(this, EventArgs.Empty);
				}
			}
			else {
				OnMouseMove(sender, e);
			}
		}

		private void OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			if (tileset == null) return;
			if (hoverPoint != -Point2I.One && e.Button == MouseButtons.Right) {
				bool modified = (tileset.GetTileData(hoverPoint) != null);
				tileset.RemoveTileData(hoverPoint);
				if (modified && Modified != null)
					Modified(this, EventArgs.Empty);
			}
		}*/

		public void SelectAll() {
			/*if (CurrentTool != toolSelection) {
				CurrentTool = toolSelection;
			}
			toolSelection.SelectAll();*/
		}


		//-----------------------------------------------------------------------------
		// Form Events
		//-----------------------------------------------------------------------------

		private void OnMouseEnter(object sender, EventArgs e) {

		}
		private void OnMouseLeave(object sender, EventArgs e) {
			
		}

		private void OnMouseDown(object sender, MouseEventArgs e) {
			Cursor = CurrentTool.MouseCursor;

			if (tileset != null) {
				Point2I mousePos = ScrollPosition + e.Location.ToPoint2I();
				
				// Notify the current tool.
				e = new MouseEventArgs(e.Button, e.Clicks, mousePos.X, mousePos.Y, e.Delta);
				CurrentTool.MouseDown(e);
			}
			this.Focus();
		}

		private void OnMouseUp(object sender, MouseEventArgs e) {
			Cursor = CurrentTool.MouseCursor;

			if (tileset != null) {
				Point2I mousePos = ScrollPosition + e.Location.ToPoint2I();

				// Notify the current tool.
				e = new MouseEventArgs(e.Button, e.Clicks, mousePos.X, mousePos.Y, e.Delta);
				CurrentTool.MouseUp(e);
			}
		}

		private void OnMouseMove(object sender, MouseEventArgs e) {
			Cursor = CurrentTool.MouseCursor;

			if (tileset != null) {
				Point2I mousePos = ScrollPosition + e.Location.ToPoint2I();
				
				// Notify the current tool.
				e = new MouseEventArgs(e.Button, e.Clicks, mousePos.X, mousePos.Y, e.Delta);
				CurrentTool.MouseMove(e);
			}
		}

		private void OnMouseDoubleClick(object sender, MouseEventArgs e) {
			if (tileset != null) {
				Point2I mousePos = ScrollPosition + e.Location.ToPoint2I();

				// Notify the current tool.
				e = new MouseEventArgs(e.Button, e.Clicks, mousePos.X, mousePos.Y, e.Delta);
				CurrentTool.MouseDoubleClick(e);
			}
		}


		//-----------------------------------------------------------------------------
		// Loading/Updating
		//-----------------------------------------------------------------------------

		public void UpdateList(Tileset tileset) {
			this.tileset = tileset;
			UpdateHeight();
		}

		public void UpdateScale() {
			UpdateHeight();
		}

		public void Unload() {
			tileset = null;
			UpdateSize(Point2I.One);
		}

		public void Modfied() {
			if (Modified != null)
				Modified(this, EventArgs.Empty);
		}


		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		protected override void TimerUpdate() {
			base.TimerUpdate();
		}

		protected override void UpdateHeight() {
			if (tileset != null) {
				columns = tileset.Width;
				UpdateSize(tileset.Dimensions * (BaseSpriteSize + 1) + 1);
			}
			else {
				columns = 1;
				UpdateSize(Point2I.One);
			}
		}

		protected override bool IsValidHoverPoint(ref Point2I point, out Point2I hoverSize) {
			hoverSize = Point2I.One;
			if (tileset != null && point < tileset.Dimensions) {
				Point2I origin = tileset.GetTileDataOrigin(point);
				if (origin != -Point2I.One) {
					if (!tileset.UsePreviewSprites)
						hoverSize = tileset.GetTileDataAtOrigin(origin).Size;
					point = origin;
				}
				return true;
			}
			return false;
		}

		protected override void Draw(Graphics2D g, SpriteSettings settings, Zone zone) {
			if (tileset == null)
				return;
			TileDataDrawing.Extras = false;
			TileDataDrawing.Level = null;
			TileDataDrawing.PlaybackTime = DesignerControl.PlaybackTime;
			TileDataDrawing.RewardManager = DesignerControl.RewardManager;

			g.Clear(Color.White);
			g.FillRectangle(new Rectangle2F(tileset.Dimensions * (GameSettings.TILE_SIZE + 1) + 1), Color.White);

			for (int indexX = 0; indexX < tileset.Width; indexX++) {
				for (int indexY = 0; indexY < tileset.Height; indexY++) {
					BaseTileData tile = tileset.GetTileDataAtOrigin(indexX, indexY);
					if (tile != null) {
						int x = 1 + indexX * (BaseSpriteSize.X + 1);
						int y = 1 + indexY * (BaseSpriteSize.Y + 1);

						try {
							if (tileset.UsePreviewSprites)
								TileDataDrawing.DrawTilePreview(g, tile, new Point2I(x, y), zone);
							else
								TileDataDrawing.DrawTile(g, tile, new Point2I(x, y), zone);
						}
						catch (Exception) {

						}
					}
				}
			}

			if (hoverPoint != -Point2I.One && DesignerControl.SelectedTileData != null) {
				BaseTileData tile = DesignerControl.SelectedTileData;
				Point2I point = hoverPoint * (BaseSpriteSize + 1) + 1;
				//if (tileset.UsePreviewSprites)
				//	TileDataDrawing.DrawTilePreview(g, tile, point, zone, FadeColor);
				//else
				//	TileDataDrawing.DrawTile(g, tile, point, zone, FadeColor);
			}

			CurrentTool.DrawTiles(g, zone);
		}


		//-----------------------------------------------------------------------------
		// Override Properties
		//-----------------------------------------------------------------------------

		protected override Point2I BaseSpriteSize {
			get { return new Point2I(GameSettings.TILE_SIZE); }
		}

		protected override Color BackgroundColor {
			get { return new Color(150, 150, 150); }
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public BaseTileData HoverTileData {
			get {
				if (hoverPoint == -Point2I.One)
					return null;
				return tileset.GetTileData(hoverPoint.X, hoverPoint.Y);
			}
		}

		public Tileset Tileset {
			get { return tileset; }
		}

		public TilesetEditorTool CurrentTool {
			get { return tools[currentToolIndex]; }
			set {
				CurrentTool.End();
				currentToolIndex = tools.IndexOf(value);
				CurrentTool.Begin();
				if (ToolChanged != null)
					ToolChanged(this, EventArgs.Empty);
				Invalidate();
			}
		}

		public ToolPlace ToolPlace {
			get { return toolPlace; }
		}

		public ToolSelection ToolSelection {
			get { return toolSelection; }
		}

		public ToolEyedrop ToolEyedrop {
			get { return toolEyedrop; }
		}

		public List<TilesetEditorTool> Tools {
			get { return tools; }
		}

		public bool Overwrite {
			get { return overwrite; }
			set { overwrite = value; }
		}

	}
}
