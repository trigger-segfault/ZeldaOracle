using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities
{
	public class GraphicsComponent {
		
		private	Point2I			drawOffset;
		private Entity			entity;				// The entity this component belongs to.
		private AnimationPlayer	animationPlayer;
		private DepthLayer		depthLayer;
		private DepthLayer		depthLayerInAir;
		private	bool			isVisible;
		private bool			isGrassEffectVisible;
		private bool			isRipplesEffectVisible;
		private bool			isShadowVisible;
		private Point2I			grassDrawOffset;
		private Point2I			ripplesDrawOffset;
		private Point2I			shadowDrawOffset;
		private int				grassAnimationTicks;
		private Sprite			sprite;
		private bool			isFlickering;
		private int				flickerAlternateDelay;
		private int				flickerTimer;
		private bool			flickerIsVisible;
		private bool			isAnimatedWhenPaused;
		private bool			isHurting;
		private int				imageVariant;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public GraphicsComponent(Entity entity) {
			this.entity					= entity;
			this.animationPlayer		= new AnimationPlayer();
			this.sprite					= null;
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
			this.imageVariant			= GameData.VARIANT_NONE;
			this.isAnimatedWhenPaused	= false;
			this.isHurting				= false;
		}
		

		//-----------------------------------------------------------------------------
		// Animation
		//-----------------------------------------------------------------------------

		public void PlayAnimation() {
			animationPlayer.Play();
		}

		public void PlaySprite(Sprite sprite) {
			this.sprite = sprite;
			animationPlayer.Animation = null;
		}

		public void PlayAnimation(SpriteAnimation sprite) {
			if (sprite.IsSprite) {
				this.sprite = sprite.Sprite;
				animationPlayer.Animation = null;
			}
			else {
				animationPlayer.Play(sprite.Animation);
			}
		}
		
		public void PlayAnimation(Animation animation) {
			animationPlayer.Play(animation);
		}
		
		public void PlayAnimation(string animationName) {
			Animation anim = Resources.GetAnimation(animationName);
			PlayAnimation(anim);
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


		//-----------------------------------------------------------------------------
		// Update/Draw
		//-----------------------------------------------------------------------------
		
		public void Update() {
			if ((entity.GameControl.UpdateRoom || isAnimatedWhenPaused) && entity.GameControl.AnimateRoom) {
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
		}

		// Draw the entity's graphics,
		public void Draw(RoomGraphics g) {
			Draw(g, CurrentDepthLayer);
		}

		// Draw the entity's graphics on the given depth layer.
		public void Draw(RoomGraphics g, DepthLayer layer) {
			if (!isVisible)
				return;

			// Draw the shadow.
			if (isShadowVisible && entity.ZPosition >= 1 && entity.GameControl.RoomTicks % 2 == 0) {
				g.DrawSprite(GameData.SPR_SHADOW, Entity.Position + shadowDrawOffset, DepthLayer.Shadows);
			}

			if (isFlickering && !flickerIsVisible)
				return;

			// Change the variant if hurting.
			int newImageVariant = imageVariant;
			if (isHurting && entity.GameControl.RoomTicks % 8 >= 4)
				newImageVariant = GameData.VARIANT_HURT;

			// Draw the sprite/animation.
			Vector2F drawPosition = Entity.Position - new Vector2F(0, Entity.ZPosition);
			if (animationPlayer.SubStrip != null)
				g.DrawAnimation(animationPlayer.SubStrip, newImageVariant, animationPlayer.PlaybackTime, drawPosition + drawOffset, layer);
			else if (sprite != null)
				g.DrawSprite(sprite, newImageVariant, drawPosition + drawOffset, layer);
			
			// Draw the ripples effect.
			if (isRipplesEffectVisible && entity.Physics.IsEnabled && entity.Physics.IsInPuddle)
				g.DrawAnimation(GameData.ANIM_EFFECT_RIPPLES, entity.GameControl.RoomTicks, entity.Position + ripplesDrawOffset, layer);
			
			// Draw the grass effect.
			if (isGrassEffectVisible && entity.Physics.IsEnabled &&entity.Physics.IsInGrass)
				g.DrawAnimation(GameData.ANIM_EFFECT_GRASS, grassAnimationTicks, entity.Position + grassDrawOffset, layer);
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Entity Entity {
			get { return entity; }
			set { entity = value; }
		}

		public int SubStripIndex {
			get { return animationPlayer.SubStripIndex; }
			set { animationPlayer.SubStripIndex = value; }
		}

		public bool IsAnimationPlaying {
			get { return animationPlayer.IsPlaying; }
			set { animationPlayer.IsPlaying = value; }
		}

		public bool IsAnimationDone {
			get { return animationPlayer.IsDone; }
		}

		public Animation Animation {
			get { return animationPlayer.Animation; }
			set { animationPlayer.Animation = value; }
		}

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

		public AnimationPlayer AnimationPlayer {
			get { return animationPlayer;  }
			set { animationPlayer = value; }
		}

		public Sprite Sprite {
			get { return sprite;  }
			set { sprite = value; }
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

		public int ImageVariant {
			get { return imageVariant; }
			set { imageVariant = value; }
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
	}
}
