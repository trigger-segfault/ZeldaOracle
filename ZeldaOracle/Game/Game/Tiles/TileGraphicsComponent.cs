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
		private Point2I			drawOffset;
		private	bool			syncPlaybackWithRoomTicks;
		private bool			isAnimatedWhenPaused;		// True if the tile updates its graphics while the room is paused.
		private Vector2F		absoluteDrawPosition;
		private bool			useAbsoluteDrawPosition;


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
			this.drawOffset					= Point2I.Zero;
			this.syncPlaybackWithRoomTicks	= true;
			this.isAnimatedWhenPaused		= false;
			this.absoluteDrawPosition		= Vector2F.Zero;
			this.useAbsoluteDrawPosition	= false;
		}

		
		//-----------------------------------------------------------------------------
		// Animation
		//-----------------------------------------------------------------------------

		public void PlaySprite(SpriteAnimation spriteAnimation) {
			if (spriteAnimation.IsSprite)
				PlaySprite(spriteAnimation.Sprite);
			else if (spriteAnimation.IsAnimation)
				PlayAnimation(spriteAnimation.Animation);
			else
				animationPlayer.Animation = null;
		}
		
		public void PlaySprite(Sprite sprite) {
			animationPlayer.Play(new Animation(sprite));
		}

		public void PlayAnimation(Animation animation) {
			animationPlayer.Play(animation);
		}
		

		//-----------------------------------------------------------------------------
		// Draw Settings
		//-----------------------------------------------------------------------------

		public void SetAbsoluteDrawPosition(Vector2F drawPosition) {
			absoluteDrawPosition = drawPosition;
			useAbsoluteDrawPosition = true;
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
			
			// Determine draw position.
			Vector2F drawPosition = (useAbsoluteDrawPosition ? absoluteDrawPosition : tile.Position);
			drawPosition += (raisedDrawOffset + drawOffset);
						
			// Draw sprite as object.
			if (tile.IsMoving && !tile.SpriteAsObject.IsNull) {
				g.DrawAnimation(tile.SpriteAsObject, imageVariant,
					tile.RoomControl.GameControl.RoomTicks,
					drawPosition, depthLayer, tile.Position);
			}
			// Draw animation player.
			else {
				float playbackTime;
				if (syncPlaybackWithRoomTicks)
					playbackTime = tile.RoomControl.GameControl.RoomTicks;
				else
					playbackTime = animationPlayer.PlaybackTime;

				if (animationPlayer.Animation != null) {
					g.DrawAnimation(animationPlayer.SubStrip, imageVariant,
						playbackTime, drawPosition, depthLayer, tile.Position);
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public AnimationPlayer AnimationPlayer {
			get { return animationPlayer; }
			set { animationPlayer = value; }
		}

		public Point2I DrawOffset {
			get { return drawOffset; }
			set { drawOffset = value; }
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
		
		public Vector2F AbsoluteDrawPosition {
			get { return absoluteDrawPosition; }
			set { absoluteDrawPosition = value; }
		}
		
		public bool UseAbsoluteDrawPosition {
			get { return useAbsoluteDrawPosition; }
			set { useAbsoluteDrawPosition = value; }
		}
		
		public bool SyncPlaybackWithRoomTicks {
			get { return syncPlaybackWithRoomTicks; }
			set { syncPlaybackWithRoomTicks = value; }
		}
	}
}
