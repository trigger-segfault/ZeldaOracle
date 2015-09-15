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
		
		private Entity			entity;				// The entity this component belongs to.
		private AnimationPlayer	animationPlayer;
		private int				subStripIndex;
		private bool			isShadowVisible;
		private bool			hasDynamicDepth;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public GraphicsComponent(Entity entity) {
			this.entity				= entity;
			this.animationPlayer	= new AnimationPlayer();
			this.subStripIndex		= 0;
			this.isShadowVisible	= true;
		}
		

		//-----------------------------------------------------------------------------
		// Methods
		//-----------------------------------------------------------------------------

		public void PlayAnimation() {
			animationPlayer.Play();
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

		public void Update(float ticks) {
			animationPlayer.SubStripIndex = subStripIndex;
			animationPlayer.Update(ticks);
		}

		public void Draw(Graphics2D g) {
			// Depth ranges:

			// Front [0.0 - 0.3][0.3 - 0.6][0.6 - 0.9][0.9    ][0.9 - 1.0] Back
			//       [???      ][Entities ][???      ][Shadows][???      ]

			// Draw the shadow.
			// TODO: Shadow draw offset.
			if (isShadowVisible && entity.ZPosition > 1) {
				float shadowDepth = 0.9f;
				g.DrawSprite(GameData.SPR_SHADOW, Entity.Position.X, Entity.Position.Y, shadowDepth);
			}

			// Draw the animation.
			float depth = 0.6f - 0.3f * (entity.Position.Y / (float) (entity.RoomControl.Room.Height * GameSettings.TILE_SIZE));
			Vector2F drawPosition = Entity.Position - new Vector2F(0, Entity.ZPosition);
			g.DrawAnimation(animationPlayer.SubStrip, animationPlayer.PlaybackTime, drawPosition, depth);
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

		public bool IsShadowVisible {
			get { return isShadowVisible;  }
			set { isShadowVisible = value; }
		}

		public AnimationPlayer AnimationPlayer {
			get { return animationPlayer;  }
			set { animationPlayer = value; }
		}
	}
}
