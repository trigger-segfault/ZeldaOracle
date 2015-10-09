using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.Entities {
	[Flags]
	public enum NPCFlags {
		AlwaysFacePlayer	= 0x1,
		FacePlayerWhenNear	= 0x2,		// Face player when he is nearby.
		FacePlayerOnTalk	= 0x4,		// Face player when talked to.
		AnimateOnTalk		= 0x8,		// Animate when a different room event is playing.
		OnlyFaceHorizontal	= 0x10,		// Only face horizontally (left or right).
		OnlyFaceVertical	= 0x20,		// Only face vertically (up or down).

		Default				= FacePlayerOnTalk | FacePlayerWhenNear,
	};

	public class NPC : Entity {

		// The direction to face when not near the player.
		private int direction;
		// The current direction the NPC is facing.
		private int faceDirection;
		// The radius of the diamond shape of tiles where the NPC will face the
		// player (excluding the center tile).
		private int sightDistance;
		// The default animation to play.
		private SpriteAnimation animationDefault;
		// The animation to play when being talked to.
		private SpriteAnimation animationTalk;

		private NPCFlags flags;


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
			centerOffset		= Graphics.DrawOffset + new Point2I(8, 8);
			actionAlignDistance	= 5;
			flags				= NPCFlags.FacePlayerOnTalk | NPCFlags.FacePlayerWhenNear;
			
			// Bounding box for talking is 4 pixels beyond the hard collision box (inclusive).
			// Alignment limit is a max 5 pixels in either direction (inclusive).
		}

		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			//Graphics.PlayAnimation(GameData.ANIM_PLAYER_DEFAULT);
			Graphics.PlayAnimation("npc_shopkeeper");

			sightDistance	= 2;
			direction		= Directions.Down;
			faceDirection	= direction;

			Graphics.IsAnimatedWhenPaused = flags.HasFlag(NPCFlags.AnimateOnTalk);
		}

		public override bool OnPlayerAction(int direction) {
			//GameControl.DisplayMessage("Hello, my friend!");
			GameControl.DisplayMessage("Welcome, sir! Bring me any item you wish to purchase.");
			if (flags.HasFlag(NPCFlags.FacePlayerOnTalk))
				faceDirection = Directions.Reverse(direction);
			return true;
		}

		public override void Update() {
			faceDirection = direction;

			bool facePlayer = flags.HasFlag(NPCFlags.AlwaysFacePlayer);

			// Check if the player is nearby.
			if (!facePlayer && flags.HasFlag(NPCFlags.FacePlayerWhenNear)) {
				Point2I a = RoomControl.GetTileLocation(Center);
				Point2I b = RoomControl.GetTileLocation(RoomControl.Player.Center);
				if (Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) <= sightDistance)
					facePlayer = true;
			}

			if (facePlayer) {
				if (flags.HasFlag(NPCFlags.OnlyFaceHorizontal)) {
					// Face the player horizontally.
					faceDirection = Directions.Right;
					if (RoomControl.Player.Center.X < Center.X)
						faceDirection = Directions.Left;
				}
				else if (flags.HasFlag(NPCFlags.OnlyFaceVertical)) {
					// Face the player vertically.
					faceDirection = Directions.Down;
					if (RoomControl.Player.Center.Y < Center.Y)
						faceDirection = Directions.Up;
				}
				else {
					// Face the player in all directions.
					Vector2F lookVector = RoomControl.Player.Center - Center;
					lookVector.Y *= -1;
					faceDirection = Directions.RoundFromRadians(
						GMath.ConvertToRadians(lookVector.Direction));
				}
			}
			
			Graphics.SubStripIndex = faceDirection;

			base.Update();
		}



		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public NPCFlags Flags {
			get { return flags; }
			set { flags = value; }
		}
	}
}
