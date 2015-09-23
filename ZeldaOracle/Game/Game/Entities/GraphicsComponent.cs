using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Collision;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities
{
	public class GraphicsComponent {
		
		private	Point2I			drawOffset;
		private Entity			entity;				// The entity this component belongs to.
		private AnimationPlayer	animationPlayer;
		private int				subStripIndex;
		private bool			hasDynamicDepth;
		private	bool			isVisible;
		private bool			isGrassEffectVisible;
		private bool			isRipplesEffectVisible;
		private bool			isShadowVisible;
		private Point2I			shadowDrawOffset;
		private int				grassAnimationTicks;
		private Sprite			sprite;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public GraphicsComponent(Entity entity) {
			this.entity				= entity;
			this.animationPlayer	= new AnimationPlayer();
			this.sprite				= null;
			this.subStripIndex		= 0;
			this.isVisible			= true;
			this.isShadowVisible	= true;
			this.shadowDrawOffset	= Point2I.Zero;
			this.isGrassEffectVisible	= true;
			this.isRipplesEffectVisible	= true;
			this.grassAnimationTicks	= 0;
			this.drawOffset				= new Point2I();
		}
		

		//-----------------------------------------------------------------------------
		// Methods
		//-----------------------------------------------------------------------------

		public void PlayAnimation() {
			animationPlayer.Play();
		}
		
		public void PlaySprite(Sprite sprite) {
			this.sprite = sprite;
			animationPlayer.Animation = null;
		}
		
		public void PlayAnimation(Animation animation) {
			animationPlayer.Play(animation);
		}
		
		public void StopAnimation() {
			animationPlayer.Stop();
		}


		//-----------------------------------------------------------------------------
		// Update/Draw
		//-----------------------------------------------------------------------------

		public void Update() {
			animationPlayer.SubStripIndex = subStripIndex;
			animationPlayer.Update();
			
			if (isGrassEffectVisible && entity.Physics.IsInGrass && entity.Physics.Velocity.Length > 0.1f) {
				grassAnimationTicks += 1;
			}
		}

		public void Draw(Graphics2D g) {
			// Depth ranges:

			if (!isVisible)
				return;

			// Front [0.0 - 0.3][0.3 - 0.6][0.6 - 0.9][0.9    ][0.9 - 1.0] Back
			//       [???      ][Entities ][???      ][Shadows][???      ]

			float shadowDepth	= 0.9f;
			float ripplesDepth	= 0.29f;
			float grassDepth	= 0.28f;

			// Draw the shadow.
			if (isShadowVisible && entity.ZPosition > 1 && entity.GameControl.RoomTicks % 2 == 0) {
				g.DrawSprite(GameData.SPR_SHADOW, Entity.Position + shadowDrawOffset, shadowDepth);
			}

			// Draw the sprite/animation.
			float depth = 0.6f - 0.3f * (entity.Origin.Y / (float) (entity.RoomControl.Room.Height * GameSettings.TILE_SIZE));
			Vector2F drawPosition = Entity.Position - new Vector2F(0, Entity.ZPosition);
			if (animationPlayer.SubStrip != null)
				g.DrawAnimation(animationPlayer.SubStrip, animationPlayer.PlaybackTime, drawPosition + drawOffset, depth);
			else if (sprite != null)
				g.DrawSprite(sprite, drawPosition + drawOffset, depth);
			
			// Draw the ripples effect.
			if (isRipplesEffectVisible && entity.Physics.IsEnabled && entity.Physics.IsInPuddle)
				g.DrawAnimation(GameData.ANIM_EFFECT_RIPPLES, entity.GameControl.RoomTicks, entity.Origin, ripplesDepth);
			
			// Draw the grass effect.
			if (isGrassEffectVisible && entity.Physics.IsEnabled &&entity.Physics.IsInGrass)
				g.DrawAnimation(GameData.ANIM_EFFECT_GRASS, grassAnimationTicks, entity.Origin, grassDepth);
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Entity Entity {
			get { return entity; }
			set { entity = value; }
		}

		public int SubStripIndex {
			get { return subStripIndex; }
			set { subStripIndex = value; }
		}

		public bool IsAnimationPlaying {
			get { return animationPlayer.IsPlaying; }
			set { animationPlayer.IsPlaying = value; }
		}

		public bool IsAnimationDone {
			get { return animationPlayer.IsDone; }
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

		public AnimationPlayer AnimationPlayer {
			get { return animationPlayer;  }
			set { animationPlayer = value; }
		}

		public Sprite Sprite {
			get { return sprite;  }
			set { sprite = value; }
		}

		public Point2I ShadowDrawOffset {
			get { return shadowDrawOffset;  }
			set { shadowDrawOffset = value; }
		}

		public Point2I DrawOffset {
			get { return drawOffset;  }
			set { drawOffset = value; }
		}
	}
}
