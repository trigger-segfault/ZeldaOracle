using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.Entities {
	public class NPC : Entity {

		// The direction to face when not near the player.
		private int direction;
		// The current direction the NPC is facing.
		private int faceDirection;
		// The radius of the diamond shape of tiles where the NPC will face the
		// player (excluding the center tile).
		private int sightDistance;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public NPC() {
			// Graphics.
			Graphics.DrawOffset = new Point2I(-8, -14);

			// Physics.
			EnablePhysics(PhysicsFlags.Solid | PhysicsFlags.HasGravity);
			Physics.CollisionBox = new Rectangle2F(-8, -11, 16, 15);
			Physics.SoftCollisionBox = new Rectangle2F(-10, -15, 20, 19);

			// General.
			centerOffset = Graphics.DrawOffset + new Point2I(8, 8);
			actionAlignDistance = 5;

			// Bounding box for talking is 4 pixels beyond the hard collision box (inclusive).
			// Alignment limit is a max 5 pixels in either direction (inclusive).
		}

		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			Graphics.AnimationPlayer.Play(GameData.ANIM_PLAYER_DEFAULT);

			sightDistance	= 2;
			direction		= Directions.Down;
			faceDirection	= direction;
		}

		public override bool OnPlayerAction(int direction) {
			GameControl.DisplayMessage("Hello, my friend!");
			return true;
		}

		public override void Update() {
			// Face the player if he is nearby.
			Point2I a = RoomControl.GetTileLocation(Center);
			Point2I b = RoomControl.GetTileLocation(RoomControl.Player.Center);

			if (Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) <= sightDistance) {
				Vector2F lookVector = RoomControl.Player.Center - Center;
				lookVector.Y *= -1;
				faceDirection = Directions.RoundFromRadians(
					GMath.ConvertToRadians(lookVector.Direction)
				);
			}
			else {
				faceDirection = direction;
			}
			
			Graphics.SubStripIndex = faceDirection;

			base.Update();
		}
	}
}
