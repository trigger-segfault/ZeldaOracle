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


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileGraphicsComponent(Tile tile) {
			this.tile					= tile;
			this.animationPlayer		= new AnimationPlayer();
			this.isVisible				= true;
			this.depthLayer				= DepthLayer.TileLayer1;
			this.imageVariant			= -1;
			this.raisedDrawOffset		= Point2I.Zero;
			this.syncPlaybackWithRoomTicks	= false;
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
			animationPlayer.Update();
		}

		public void Draw(RoomGraphics g) {
			if (!isVisible)
				return;
			
			int imageVariant = this.imageVariant;
			if (imageVariant < 0)
				imageVariant = tile.Zone.ImageVariantID;

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

			

			if (tile.AnimationPlayer != null) {
				g.DrawAnimation(tile.AnimationPlayer, imageVariant,
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

			/*
			if (!sprite.IsNull) {
				g.DrawAnimation(sprite, imageVariant,
					tile.RoomControl.GameControl.RoomTicks,
					tile.Position, depthLayer, tile.Position);
			}

			imageVariant = tile.Zone.ImageVariantID;

			// Draw the animation.
			if (animationPlayer.SubStrip != null) {
				g.DrawAnimation(animationPlayer.SubStrip, imageVariant,
					animationPlayer.PlaybackTime, tile.Position + drawOffset,
					depthLayer, tile.Position);
			}
			*/
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

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
	}
}
