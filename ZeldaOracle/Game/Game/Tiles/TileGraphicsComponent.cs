using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities;

namespace ZeldaOracle.Game.Tiles {

	public class TileGraphicsComponent {
		
		private Tile			tile;				// The tile this component belongs to.
		private AnimationPlayer	animationPlayer;
		private	bool			isVisible;
		private DepthLayer		depthLayer;
		private int				imageVariant;
		private Point2I			raisedDrawOffset;			// Offset to draw tiles that are slightly raised (EX: pushing pots onto a button)
		private	bool			syncPlaybackWithRoomTicks;
		private bool			isAnimatedWhenPaused;		// True if the tile updates its graphics while the room is paused.


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileGraphicsComponent(Tile tile) {
			this.tile						= tile;
			this.animationPlayer			= new AnimationPlayer();
			this.isVisible					= true;
			this.depthLayer					= DepthLayer.TileLayer1;
			this.imageVariant				= -1;
			this.raisedDrawOffset			= Point2I.Zero;
			this.syncPlaybackWithRoomTicks	= false;
			this.isAnimatedWhenPaused		= false;
		}

		
		//-----------------------------------------------------------------------------
		// Animation
		//-----------------------------------------------------------------------------

		public void PlayAnimation(Animation animation) {
			animationPlayer.Play(animation);
		}

		
		//-----------------------------------------------------------------------------
		// Update/Draw
		//-----------------------------------------------------------------------------

		public void Update() {
			if ((tile.GameControl.UpdateRoom || isAnimatedWhenPaused) && tile.GameControl.AnimateRoom) {
				animationPlayer.Update();
			}
		}

		public void Draw(RoomGraphics g) {
			if (!isVisible)
				return;
			
			// Determine the image variant.
			int imageVariant = this.imageVariant;
			if (imageVariant < 0)
				imageVariant = tile.Zone.ImageVariantID;

			// Determine the depth layer based on the tile grid layer.
			if (tile.Layer == 0)
				depthLayer = DepthLayer.TileLayer1;
			else if (tile.Layer == 1)
				depthLayer = DepthLayer.TileLayer2;
			else if (tile.Layer == 2)
				depthLayer = DepthLayer.TileLayer3;

			SpriteAnimation sprite = (!tile.CustomSprite.IsNull ? tile.CustomSprite : tile.CurrentSprite);
			if (tile.IsMoving && !tile.SpriteAsObject.IsNull)
				sprite = tile.SpriteAsObject;
			
			Vector2F drawPosition = tile.Position + raisedDrawOffset;

			// Draw the tile.
			if (animationPlayer.Animation != null) {
				g.DrawAnimation(animationPlayer, imageVariant,
					drawPosition, depthLayer, tile.Position);
			}
			else if (sprite.IsAnimation) {
				g.DrawAnimation(sprite.Animation, imageVariant,
					tile.RoomControl.GameControl.RoomTicks,
					drawPosition, depthLayer, tile.Position);
			}
			else if (sprite.IsSprite) {
				g.DrawSprite(sprite.Sprite, imageVariant,
					drawPosition, depthLayer, tile.Position);
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public AnimationPlayer AnimationPlayer {
			get { return animationPlayer; }
			set { animationPlayer = value; }
		}

		public Point2I RaisedDrawOffset {
			get { return raisedDrawOffset; }
			set { raisedDrawOffset = value; }
		}

		public DepthLayer DepthLayer {
			get { return depthLayer; }
			set { depthLayer = value; }
		}

		public int SubStripIndex {
			get { return animationPlayer.SubStripIndex; }
			set { animationPlayer.SubStripIndex = value; }
		}

		public bool IsAnimationDone {
			get { return animationPlayer.IsDone; }
		}
		
		public bool IsAnimatedWhenPaused {
			get { return isAnimatedWhenPaused; }
			set { isAnimatedWhenPaused = value; }
		}
	}
}
