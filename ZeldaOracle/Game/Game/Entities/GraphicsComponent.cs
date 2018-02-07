using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Entities
{
	public class GraphicsComponent {
		
		private Entity			entity;				// The entity this component belongs to.
		private AnimationPlayer	animationPlayer;

		private	bool			isVisible;
		private DepthLayer		depthLayer;
		private DepthLayer		depthLayerInAir;
		private	Point2I			drawOffset;
		private bool			isGrassEffectVisible;
		private bool			isRipplesEffectVisible;
		private bool			isShadowVisible;
		private Point2I			grassDrawOffset;
		private Point2I			ripplesDrawOffset;
		private Point2I			shadowDrawOffset;
		private int				grassAnimationTicks;
		private bool			isFlickering;
		private int				flickerAlternateDelay;
		private int				flickerTimer;
		private bool			flickerIsVisible;
		private bool			isAnimatedWhenPaused;
		private bool			isHurting;
		private ColorDefinitions colorDefinitions;
		private bool            unmapped;
		private UnmappedSprite	unmappedSprite;
		private Palette			unmappedPalette;
		/// <summary>Draws above the player/monsters when
		/// the player is above and vice-versa.</summary>
		private bool			useDynamicDepth;
		/// <summary>The y offset from the position of the tile to the origin.
		/// Only needed for dynamic depth.</summary>
		private int                 dynamicOriginY;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public GraphicsComponent(Entity entity) {
			this.entity					= entity;
			this.animationPlayer		= new AnimationPlayer();
			this.depthLayer				= DepthLayer.None;
			this.depthLayerInAir		= DepthLayer.None;
			this.isVisible				= true;
			this.isShadowVisible		= true;
			this.grassDrawOffset		= Point2I.Zero;
			this.ripplesDrawOffset		= Point2I.Zero;
			this.shadowDrawOffset		= Point2I.Zero;
			this.isGrassEffectVisible	= true;
			this.isRipplesEffectVisible	= true;
			this.grassAnimationTicks	= 0;
			this.drawOffset				= Point2I.Zero;
			this.isFlickering			= false;
			this.flickerAlternateDelay	= 2;
			this.flickerTimer			= 0;
			this.flickerIsVisible		= true;
			this.isAnimatedWhenPaused	= false;
			this.isHurting				= false;
			this.colorDefinitions		= new ColorDefinitions();
			this.unmapped				= false;
			this.unmappedSprite			= null;
			this.unmappedPalette		= null;
			this.useDynamicDepth		= false;
			this.dynamicOriginY			= 0;
		}


		//-----------------------------------------------------------------------------
		// Animation & Sprite Interface
		//-----------------------------------------------------------------------------

		public void CreateUnmappedSprite() {
			Palette palette = unmappedPalette;
			if (palette == null)
				palette = Entity.RoomControl.Zone.Palette;

			Graphics2D g2d = new Graphics2D(Resources.SpriteBatch);
			unmappedSprite = Unmapping.UnmapSprite(g2d, animationPlayer.SpriteOrSubStrip,
				new SpriteDrawSettings(colorDefinitions, animationPlayer.PlaybackTime),
				palette, Entity.RoomControl.EntityPalette);
		}

		public void PlayAnimation() {
			animationPlayer.Play();
		}

		public void PlayAnimation(ISprite sprite) {
			animationPlayer.Play(sprite);
		}
		
		public void PlayAnimation(Animation animation) {
			animationPlayer.Play(animation);
		}

		public void SetAnimation(Animation animation) {
			animationPlayer.SetAnimation(animation);
		}
		
		public void StopAnimation() {
			animationPlayer.Stop();
		}
		
		public void PauseAnimation(bool isPaused) {
			animationPlayer.Pause(isPaused);
		}
		
		public void PauseAnimation() {
			animationPlayer.Pause();
		}
		
		public void ResumeAnimation() {
			animationPlayer.Resume();
		}
		
		public void ClearAnimation() {
			animationPlayer.Clear();
		}


		//-----------------------------------------------------------------------------
		// Update/Draw
		//-----------------------------------------------------------------------------
		
		public void Update() {
			if (entity.GameControl.AnimateRoom && (entity.GameControl.UpdateRoom || isAnimatedWhenPaused)) {
				// Update the animation player.
				animationPlayer.Update();

				// Update flickering.
				if (isFlickering) {
					flickerTimer++;
					if (flickerTimer >= flickerAlternateDelay) {
						flickerIsVisible = !flickerIsVisible;
						flickerTimer = 0;
					}
				}
				else {
					flickerTimer = 0;
					flickerIsVisible = true;
				}

				// Update grass effect animation.
				if (isGrassEffectVisible && entity.Physics.IsInGrass && entity.Physics.Velocity.Length > 0.1f) {
					grassAnimationTicks += 1;
				}
			}

			if (unmapped) {
				ColorDefinitions finalColorDefinitions = colorDefinitions;

				// Change the color if hurting.
				if (isHurting && entity.GameControl.RoomTicks % 8 >= 4) {
					finalColorDefinitions = ColorDefinitions.All("hurt");
				}

				Palette palette = Entity.RoomControl.TilePaletteOverride ?? unmappedPalette;

				Graphics2D g2d = new Graphics2D(Resources.SpriteBatch);
				unmappedSprite = Unmapping.UnmapSprite(g2d, animationPlayer.SpriteOrSubStrip,
					new SpriteDrawSettings(finalColorDefinitions, animationPlayer.PlaybackTime),
					palette, Entity.RoomControl.EntityPalette);
			}
		}

		// Draw the entity's graphics,
		public void Draw(RoomGraphics g) {
			Draw(g, CurrentDepthLayer);
		}

		// Draw the entity's graphics on the given depth layer.
		public void Draw(RoomGraphics g, DepthLayer layer) {
			if (!isVisible)
				return;

			if (layer == CurrentDepthLayer && useDynamicDepth) {
				float playerY = Entity.RoomControl.Player.Position.Y;
				if (Math.Round(playerY) < Math.Round(Entity.Position.Y + dynamicOriginY))
					depthLayer = DepthLayer.DynamicDepthAboveEntity;
				else
					depthLayer = DepthLayer.DynamicDepthBelowEntity;
			}

			// Draw the shadow.
			if (isShadowVisible && entity.ZPosition >= 1 &&
				entity.GameControl.RoomTicks % 2 == 0 && !entity.RoomControl.IsSideScrolling)
			{
				g.DrawSprite(GameData.SPR_SHADOW, Entity.Position + shadowDrawOffset, DepthLayer.Shadows);
			}

			if (isFlickering && !flickerIsVisible)
				return;

			ColorDefinitions finalColorDefinitions = colorDefinitions;

			// Change the color if hurting.
			if (isHurting && entity.GameControl.RoomTicks % 8 >= 4) {
				finalColorDefinitions = ColorDefinitions.All("hurt");
			}

			// Draw the sprite/animation.
			Vector2F drawPosition = Entity.Position - new Vector2F(0, Entity.ZPosition);
			if (unmapped) {
				if (unmappedSprite != null)
					g.DrawSprite(unmappedSprite, drawPosition + drawOffset, layer, entity.Position);
			}
			else {
				g.DrawSprite(animationPlayer.SpriteOrSubStrip, new SpriteDrawSettings(
					finalColorDefinitions, animationPlayer.PlaybackTime),
					drawPosition + drawOffset, layer, entity.Position);
			}

			// Draw the ripples effect.
			if (isRipplesEffectVisible && entity.Physics.IsEnabled && entity.Physics.IsInPuddle) {
				g.DrawSprite(GameData.ANIM_EFFECT_RIPPLES,
					new SpriteDrawSettings((float) entity.GameControl.RoomTicks), entity.Position +
					ripplesDrawOffset, layer, entity.Position);
			}
			
			// Draw the grass effect.
			if (isGrassEffectVisible && entity.Physics.IsEnabled && entity.Physics.IsInGrass) {
				g.DrawSprite(GameData.ANIM_EFFECT_GRASS,
					new SpriteDrawSettings((float) grassAnimationTicks), entity.Position +
					grassDrawOffset, layer, entity.Position + new Vector2F(0, 1));
			}
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Entity Entity {
			get { return entity; }
			set { entity = value; }
		}

		// Animation Player -----------------------------------------------------------

		public AnimationPlayer AnimationPlayer {
			get { return animationPlayer; }
		}

		public int SubStripIndex {
			get { return animationPlayer.SubStripIndex; }
			set { animationPlayer.SubStripIndex = value; }
		}

		public bool IsAnimationPlaying {
			get { return animationPlayer.IsPlaying; }
		}

		public bool IsAnimationDone {
			get { return animationPlayer.IsDone; }
		}
		
		public ISprite Sprite {
			get { return animationPlayer.Sprite; }
		}
		
		public Animation Animation {
			get { return animationPlayer.Animation; }
		}

		// Graphics Settings ----------------------------------------------------------

		public bool IsVisible {
			get { return isVisible; }
			set { isVisible = value; }
		}

		public bool IsShadowVisible {
			get { return isShadowVisible;  }
			set { isShadowVisible = value; }
		}

		public bool IsGrassEffectVisible {
			get { return isGrassEffectVisible;  }
			set { isGrassEffectVisible = value; }
		}

		public bool IsRipplesEffectVisible {
			get { return isRipplesEffectVisible;  }
			set { isRipplesEffectVisible = value; }
		}

		public bool IsFlickering {
			get { return isFlickering;  }
			set {
				if (!isFlickering && value) {
					flickerTimer = 0;
					flickerIsVisible = false;
				}
				isFlickering = value;
			}
		}

		public int FlickerAlternateDelay {
			get { return flickerAlternateDelay;  }
			set { flickerAlternateDelay = value; }
		}

		public bool IsAnimatedWhenPaused {
			get { return isAnimatedWhenPaused; }
			set { isAnimatedWhenPaused = value; }
		}

		public Point2I GrassDrawOffset {
			get { return grassDrawOffset; }
			set { grassDrawOffset = value; }
		}

		public Point2I RipplesDrawOffset {
			get { return ripplesDrawOffset; }
			set { ripplesDrawOffset = value; }
		}

		public Point2I ShadowDrawOffset {
			get { return shadowDrawOffset;  }
			set { shadowDrawOffset = value; }
		}

		public Point2I DrawOffset {
			get { return drawOffset;  }
			set { drawOffset = value; }
		}

		public bool IsHurting {
			get { return isHurting; }
			set { isHurting = value; }
		}

		public DepthLayer DepthLayer {
			get { return depthLayer; }
			set { depthLayer = value; }
		}

		public DepthLayer DepthLayerInAir {
			get { return depthLayerInAir; }
			set { depthLayerInAir = value; }
		}

		public DepthLayer CurrentDepthLayer {
			get {
				if (depthLayerInAir != DepthLayer.None && entity.IsInAir)
					return depthLayerInAir;
				return depthLayer;
			}
		}

		public bool IsUnmapped {
			get { return unmapped; }
			set { unmapped = value; }
		}

		/*public UnmappedSprite UnmappedSprite {
			get { return unmappedSprite; }
			set { unmappedSprite = value; }
		}*/

		public Palette UnmappedPalette {
			get { return unmappedPalette; }
			set { unmappedPalette = value; }
		}

		public ColorDefinitions ColorDefinitions {
			get { return colorDefinitions; }
			set { colorDefinitions = null; }
		}
		
		public ColorDefinitions ModifiedColorDefinitions {
			get {
				// Change color if hurting.
				if (isHurting && entity.GameControl.RoomTicks % 8 >= 4)
					return ColorDefinitions.All("hurt");
				else
					return colorDefinitions;
			}
		}

		/// <summary>Gets if the t draws above the player/monsters
		/// when the player is above and vice-versa.</summary>
		public bool UseDynamicDepth {
			get { return useDynamicDepth; }
			set { useDynamicDepth = value; }
		}

		/// <summary>The y offset from the position of the tile to the origin.
		/// Only needed for dynamic depth.</summary>
		public int DynamicOriginY {
			get { return dynamicOriginY; }
			set { dynamicOriginY = value; }
		}
	}
}
