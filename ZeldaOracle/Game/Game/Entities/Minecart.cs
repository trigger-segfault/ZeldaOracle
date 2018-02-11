using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities {

	public class Minecart : Entity {
		
		private TileMinecartTrack minecartTrack;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public Minecart(TileMinecartTrack minecartTrack) {
			this.minecartTrack = minecartTrack;

			// Physics.
			EnablePhysics(PhysicsFlags.Solid);
			Physics.CollisionBox		= new Rectangle2F(-1, 3, 18, 15);
			Physics.SoftCollisionBox	= new Rectangle2F(-1, 3, 18, 15);

			// Graphics.
			Graphics.DepthLayer	= DepthLayer.PlayerAndNPCs;
			Graphics.DrawOffset = new Point2I(0, 0);
			centerOffset		= new Point2I(8, 8);

			// General.
			actionAlignDistance	= 5;
		}

		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------
		/*
		public override bool OnPlayerAction(int direction) {
			// Hop in minecart.
			Player player = RoomControl.Player;
			player.EnterMinecart(this);
			return true;
		}*/

		public override void Initialize() {
			//Graphics.PlayAnimation(animationDefault);

			if (minecartTrack != null) {
				if (minecartTrack.IsHorizontal)
					Graphics.PlayAnimation(GameData.SPR_MINECART_HORIZONTAL);
				else
					Graphics.PlayAnimation(GameData.SPR_MINECART_VERTICAL);

				Position = minecartTrack.Position;
			}
			else
				Graphics.PlayAnimation(GameData.SPR_MINECART_HORIZONTAL);
		}

		public override void Update() {
			Player player = RoomControl.Player;

			if (player.IsOnGround && player.Movement.IsMoving &&
				!player.StateParameters.ProhibitEnteringMinecart &&
				!player.StateParameters.ProhibitMovementControlOnGround)
			{
				foreach (CollisionInfo collision in player.Physics.GetCollisions()) {
					if (collision.Entity == this &&
						player.Movement.IsMovingInDirection(collision.Direction))
					{
						player.JumpIntoMinecart(this);
						break;
					}
				}
			}

			base.Update();
		}



		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public TileMinecartTrack TrackTile {
			get { return minecartTrack; }
		}
		
		public Point2I TrackTileLocation {
			get {
				if (minecartTrack == null)
					return RoomControl.GetTileLocation(Center);
				return minecartTrack.Location;
			}
		}
	}
}
