using System;
using System.Collections.Generic;
using Size = System.Drawing.Size;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game;
using System.Windows.Threading;
using System.Diagnostics;
using ZeldaOracle.Game.Worlds;
using ConscriptDesigner.Control;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Forms;

namespace ConscriptDesigner.WinForms {
	
	public class ZeldaGraphicsDeviceControl : GraphicsDeviceControl {
		
		// Static
		private static RenderTarget2D renderTarget;
		private static Zone defaultZone;

		private DispatcherTimer dispatcherTimer;
		private SpriteBatch spriteBatch;
		
		protected int columns;

		// Hover
		protected Point2I mouse;
		protected Point2I hoverPoint;
		private Point2I hoverSize;

		private bool isInitialized;

		private bool needsToInvalidate;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ZeldaGraphicsDeviceControl() {
			this.isInitialized = false;
			this.spriteBatch = null;
			this.columns = 1;
			this.mouse = -Point2I.One;
			this.hoverPoint = -Point2I.One;
			this.needsToInvalidate = false;

			this.ResizeRedraw = true;

			MouseMove += OnMouseMove;
			MouseLeave += OnMouseLeave;
			MouseWheel += OnMouseWheel;
			//ClientSizeChanged += OnClientSizeChanged;
			PreviewReset += OnPreviewReset;
			PostReset += OnPostReset;
		}

		protected override void Initialize() {
			this.spriteBatch = new SpriteBatch(GraphicsDevice);
			if (renderTarget == null)
				renderTarget = new RenderTarget2D(GraphicsDevice, 1, 1);
			if (defaultZone == null)
				defaultZone = new Zone();

			isInitialized = true;

			this.dispatcherTimer = new DispatcherTimer(
				TimeSpan.FromMilliseconds(15),
				DispatcherPriority.Render,
				delegate { TimerUpdate(); },
				System.Windows.Application.Current.Dispatcher);
		}


		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------

		public event EventHandler HoverChanged;


		//-----------------------------------------------------------------------------
		// WndProc Override
		//-----------------------------------------------------------------------------

		protected override void WndProc(ref Message m) {
			Point2I scrollPositionBefore = ScrollPosition;
			base.WndProc(ref m);
			// 0x115 and 0x20a both tell the control to scroll. If either one comes 
			// through, you can handle the scrolling before any repaints take place
			if (ModifierKeys.HasFlag(Keys.Control) && (m.Msg == 0x115 || m.Msg == 0x20a)) {
				ScrollPosition = scrollPositionBefore;
			}
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		private void OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			mouse = (ScrollPosition + new Point2I(e.X, e.Y)) / DesignerControl.PreviewScale;
			UpdateHoverSprite();
		}

		private void OnMouseLeave(object sender, EventArgs e) {
			mouse = -Point2I.One;
			UpdateHoverSprite();
		}

		private void OnMouseWheel(object sender, System.Windows.Forms.MouseEventArgs e) {
			if (ModifierKeys.HasFlag(Keys.Control)) {
				if (e.Delta > 0 && DesignerControl.PreviewScale < 3)
					DesignerControl.PreviewScale++;
				else if (e.Delta < 0 && DesignerControl.PreviewScale > 1)
					DesignerControl.PreviewScale--;
			}
		}

		/*private void OnClientSizeChanged(object sender, EventArgs e) {
			if (isInitialized) {
				Point2I oldRenderTargetSize = new Point2I(renderTarget.Width, renderTarget.Height);
				Point2I newRenderTargetSize = GMath.Max(oldRenderTargetSize, new Point2I(ClientSize.Width, ClientSize.Height));
				renderTarget.Dispose();
				renderTarget = new RenderTarget2D(GraphicsDevice, newRenderTargetSize.X, newRenderTargetSize.Y);
				ScrollPosition = Point2I.Zero;
			}
		}*/

		private void OnPreviewReset(object sender, EventArgs e) {
			/*if (isInitialized) {

			}*/
		}

		private void OnPostReset(object sender, EventArgs e) {
			if (isInitialized) {
				Point2I oldRenderTargetSize = new Point2I(renderTarget.Width, renderTarget.Height);
				Point2I newRenderTargetSize = GMath.Max(oldRenderTargetSize, new Point2I(ClientSize.Width, ClientSize.Height));
				renderTarget.Dispose();
				renderTarget = new RenderTarget2D(GraphicsDevice, newRenderTargetSize.X, newRenderTargetSize.Y);
				ScrollPosition = Point2I.Zero;
			}
		}


		//-----------------------------------------------------------------------------
		// Virtual Methods
		//-----------------------------------------------------------------------------

		protected virtual void TimerUpdate() {
			if (DesignerControl.PlayAnimations || needsToInvalidate) {
				needsToInvalidate = false;
				Invalidate();
			}
		}

		protected virtual bool IsValidHoverPoint(ref Point2I point, out Point2I hoverSize) {
			hoverSize = Point2I.Zero;
			return false;
		}
		
		protected virtual void UpdateHoverSprite() {
			int column = mouse.X / (BaseSpriteSize.X + 1);
			int row = mouse.Y / (BaseSpriteSize.Y + 1);
			Point2I point = new Point2I(column, row);
			Point2I newHoverPoint = -Point2I.One;
			if (mouse >= Point2I.Zero && column < columns && IsValidHoverPoint(ref point, out hoverSize)) {
				newHoverPoint = point;
			}
			if (newHoverPoint != hoverPoint) {
				hoverPoint = newHoverPoint;
				if (HoverChanged != null)
					HoverChanged(this, EventArgs.Empty);
				if (!DesignerControl.PlayAnimations)
					Invalidate();
			}
		}

		protected virtual void UpdateHeight() {
			columns = 1;
		}

		protected virtual void Draw(Graphics2D g, SpriteDrawSettings settings, Zone zone) { }

		//-----------------------------------------------------------------------------
		// Updating
		//-----------------------------------------------------------------------------

		protected void UpdateSize(Point2I newSize) {
			newSize *= DesignerControl.PreviewScale;
			AutoScrollMinSize = new Size(newSize.X, newSize.Y);
			UpdateHoverSprite();
			needsToInvalidate = true;
		}


		//-----------------------------------------------------------------------------
		// Drawing
		//-----------------------------------------------------------------------------

		protected sealed override void Draw() {
			SpriteDrawSettings settings = new SpriteDrawSettings(DesignerControl.PlaybackTime);
			Zone zone = DesignerControl.PreviewZone ?? defaultZone;
			Graphics2D g = new Graphics2D(spriteBatch);

			if (GameData.PaletteShader != null && !GameData.PaletteShader.Effect.IsDisposed) {
				//GameData.PaletteShader.EntityPalette = GameData.PAL_ENTITIES_DEFAULT;
				//GameData.PaletteShader.TilePalette = GameData.PAL_TILES_DEFAULT;
				//if (zone.Palette != null)
				//	GameData.PaletteShader.TilePalette = zone.Palette;
				GameData.PaletteShader.TilePalette = DesignerControl.PreviewTilePalette;
				GameData.PaletteShader.EntityPalette = DesignerControl.PreviewEntityPalette;
				GameData.PaletteShader.ApplyPalettes();
			}
			else {
				g.Clear(Color.White);
				return;
			}

			if (zone != null) {
				settings.Styles = zone.StyleDefinitions;
			}

			if (DesignerControl.PreviewScale > 1) {
				g.SetRenderTarget(renderTarget);
			}
			g.Clear(Color.White);
			g.PushTranslation(-ScrollPosition / DesignerControl.PreviewScale);
			g.Begin(GameSettings.DRAW_MODE_DEFAULT);

			Draw(g, settings, zone);

			if (hoverPoint != -Point2I.One) {
				Rectangle2I selectRect = new Rectangle2I(
					hoverPoint * (BaseSpriteSize + 1),
					hoverSize * BaseSpriteSize + 2);
				g.DrawRectangle(selectRect, 1, Color.Black);
				g.DrawRectangle(selectRect.Inflated(1, 1), 1, Color.White);
				g.DrawRectangle(selectRect.Inflated(2, 2), 1, Color.Black);
			}

			g.End();
			g.PopTranslation();
			if (DesignerControl.PreviewScale > 1) {
				g.SetRenderTarget(null);
				g.Begin(GameSettings.DRAW_MODE_DEFAULT);
				g.DrawImage(renderTarget, Vector2F.Zero, Vector2F.Zero, new Vector2F(DesignerControl.PreviewScale), 0.0);
				g.End();
			}
		}


		//-----------------------------------------------------------------------------
		// Virtual Properties
		//-----------------------------------------------------------------------------
		
		protected virtual Point2I BaseSpriteSize {
			get { return new Point2I(GameSettings.TILE_SIZE); }
		}


		//-----------------------------------------------------------------------------
		// Internal Properties
		//-----------------------------------------------------------------------------

		protected Point2I SpriteSize {
			get { return BaseSpriteSize * DesignerControl.PreviewScale; }
		}

		protected Point2I SpriteSpacing {
			get { return (BaseSpriteSize + 1) * DesignerControl.PreviewScale; }
		}

		protected Point2I SpriteOffset {
			get { return new Point2I(DesignerControl.PreviewScale); }
		}

		protected Point2I ScrollPosition {
			get { return new Point2I(HorizontalScroll.Value, VerticalScroll.Value); }
			set {
				AutoScrollPosition = new System.Drawing.Point(
					GMath.Clamp(value.X, HorizontalScroll.Minimum, HorizontalScroll.Maximum),
					GMath.Clamp(value.Y, VerticalScroll.Minimum, VerticalScroll.Maximum)
				);
			}
		}

		protected Point2I UnscaledScrollPosition {
			get { return ScrollPosition / DesignerControl.PreviewScale; }
		}

		protected Point2I UnscaledClientSize {
			get { return new Point2I(ClientSize.Width, ClientSize.Height) / DesignerControl.PreviewScale; }
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Point2I HoverPoint {
			get { return hoverPoint; }
		}
	}
}
