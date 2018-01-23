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

namespace ConscriptDesigner.WinForms {

	public class ZeldaUniqueGraphicsDeviceControl : GraphicsDeviceControl {

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
		private Point2I trueHoverPoint;

		private bool isInitialized;

		private bool needsToInvalidate;

		private int scale;

		private Rectangle2I sourceRect;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ZeldaUniqueGraphicsDeviceControl() {
			this.isInitialized = false;
			this.spriteBatch = null;
			this.columns = 1;
			this.mouse = -Point2I.One;
			this.hoverPoint = -Point2I.One;
			this.trueHoverPoint = -Point2I.One;
			this.needsToInvalidate = false;
			this.scale = 1;
			sourceRect = new Rectangle2I(1, 1);

			this.ResizeRedraw = true;

			MouseMove += OnMouseMove;
			MouseLeave += OnMouseLeave;
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
		// Event Handlers
		//-----------------------------------------------------------------------------

		private void OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			mouse = (ScrollPosition + new Point2I(e.X, e.Y)) / scale;
			UpdateHoverSprite();
		}

		private void OnMouseLeave(object sender, EventArgs e) {
			mouse = -Point2I.One;
			UpdateHoverSprite();
		}

		private void OnPreviewReset(object sender, EventArgs e) {
			/*if (isInitialized) {

			}*/
		}

		private void OnPostReset(object sender, EventArgs e) {
			if (isInitialized) {
				Point2I oldRenderTargetSize = new Point2I(renderTarget.Width, renderTarget.Height);
				Point2I newRenderTargetSize = GMath.Max(oldRenderTargetSize, UnscaledClientSize);
				if (newRenderTargetSize != oldRenderTargetSize) {
					renderTarget.Dispose();
					renderTarget = new RenderTarget2D(GraphicsDevice, newRenderTargetSize.X, newRenderTargetSize.Y);
				}
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
			if (point != trueHoverPoint) {
				if (newHoverPoint != hoverPoint) {
					hoverPoint = newHoverPoint;
					if (HoverChanged != null)
						HoverChanged(this, EventArgs.Empty);
				}
				if (mouse >= Point2I.Zero)
					trueHoverPoint = point;
				else
					trueHoverPoint = -Point2I.One;
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
			newSize *= scale;
			sourceRect.Size = newSize;
			AutoScrollMinSize = new Size(newSize.X, newSize.Y);
			UpdateHoverSprite();
			needsToInvalidate = true;
		}

		public void UpdateScale(int scale) {
			this.scale = scale;
			UpdateHeight();
		}


		//-----------------------------------------------------------------------------
		// Drawing
		//-----------------------------------------------------------------------------

		protected sealed override void Draw() {
			SpriteDrawSettings settings = new SpriteDrawSettings(DesignerControl.PlaybackTime);
			Zone zone = DesignerControl.PreviewZone ?? defaultZone;
			Graphics2D g = new Graphics2D(spriteBatch);

			if (GameData.PaletteShader != null && !GameData.PaletteShader.Effect.IsDisposed) {
				GameData.PaletteShader.EntityPalette = GameData.PAL_ENTITIES_DEFAULT;
				GameData.PaletteShader.TilePalette = GameData.PAL_TILES_DEFAULT;
				if (zone.Palette != null)
					GameData.PaletteShader.TilePalette = zone.Palette;
				GameData.PaletteShader.ApplyPalettes();
			}
			else {
				g.Clear(BackgroundColor);
				return;
			}

			if (zone != null) {
				settings.VariantID = zone.ImageVariantID;
				settings.Styles = zone.StyleDefinitions;
			}

			g.Clear(BackgroundColor);
			//if (scale > 1) {
			g.SetRenderTarget(renderTarget);
			//}
			g.Clear(Color.White);
			g.PushTranslation(-ScrollPosition / scale);
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
			//if (scale > 1) {
				g.SetRenderTarget(null);
			g.Clear(BackgroundColor);
			g.Begin(GameSettings.DRAW_MODE_DEFAULT);
				g.DrawImage(renderTarget, Vector2F.Zero, sourceRect, Vector2F.Zero, new Vector2F(scale), 0.0);
				g.End();
			//}
		}


		//-----------------------------------------------------------------------------
		// Virtual Properties
		//-----------------------------------------------------------------------------

		protected virtual Point2I BaseSpriteSize {
			get { return new Point2I(GameSettings.TILE_SIZE); }
		}

		protected virtual Color BackgroundColor {
			get { return Color.White; }
		}

		//-----------------------------------------------------------------------------
		// Internal Properties
		//-----------------------------------------------------------------------------

		protected Point2I SpriteSize {
			get { return BaseSpriteSize * scale; }
		}

		protected Point2I SpriteSpacing {
			get { return (BaseSpriteSize + 1) * scale; }
		}

		protected Point2I SpriteOffset {
			get { return new Point2I(scale); }
		}

		public Point2I ScrollPosition {
			get { return new Point2I(HorizontalScroll.Value, VerticalScroll.Value); }
			set {
				AutoScrollPosition = new System.Drawing.Point(
					GMath.Clamp(value.X, HorizontalScroll.Minimum, HorizontalScroll.Maximum),
					GMath.Clamp(value.Y, VerticalScroll.Minimum, VerticalScroll.Maximum)
				);
			}
		}

		public Point2I UnscaledScrollPosition {
			get { return ScrollPosition / scale; }
		}

		public Point2I UnscaledClientSize {
			get { return new Point2I(ClientSize.Width, ClientSize.Height) / scale; }
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Point2I HoverPoint {
			get { return hoverPoint; }
		}

		public Point2I TrueHoverPoint {
			get { return trueHoverPoint; }
		}

		public new int Scale {
			get { return scale; }
		}
	}
}
