using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control;
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

		private Message message;

		private NPCFlags flags;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public NPC() {
			// Physics.
			EnablePhysics(PhysicsFlags.Solid | PhysicsFlags.HasGravity);
			Physics.CollisionBox = new Rectangle2F(-8, -11, 16, 13);
			Physics.SoftCollisionBox = new Rectangle2F(-10, -15, 20, 19);

			// Graphics.
			Graphics.DepthLayer	= DepthLayer.PlayerAndNPCs;
			Graphics.DrawOffset = new Point2I(-8, -14);

			// General.
			centerOffset		= Graphics.DrawOffset + new Point2I(8, 8);
			actionAlignDistance	= 5;
			flags				= NPCFlags.FacePlayerOnTalk | NPCFlags.FacePlayerWhenNear;
			
			message = null;
			animationTalk = null;

			// Bounding box for talking is 4 pixels beyond the hard collision box (inclusive).
			// Alignment limit is a max 5 pixels in either direction (inclusive).
		}

		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			Graphics.PlaySpriteAnimation(animationDefault);

			sightDistance	= 2;
			faceDirection	= direction;

			Graphics.IsAnimatedWhenPaused	= flags.HasFlag(NPCFlags.AnimateOnTalk);
			Graphics.SubStripIndex			= faceDirection;
		}

		public override bool OnPlayerAction(int direction) {
			if (message != null) {
				if (!animationTalk.IsNull) {
					Graphics.PlaySpriteAnimation(animationTalk);
				}
				if (flags.HasFlag(NPCFlags.FacePlayerOnTalk)) {
					faceDirection = Directions.Reverse(direction);
					Graphics.SubStripIndex = faceDirection;
				}
				GameControl.DisplayMessage(message, delegate() {
					if (!animationTalk.IsNull)
						Graphics.PlaySpriteAnimation(animationDefault);
				});
				return true;
			}
			return false;
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

		public int Direction {
			get { return direction; }
			set { direction = value; }
		}

		public Message Message {
			get { return message; }
			set { message = value; }
		}

		public SpriteAnimation DefaultAnimation {
			get { return animationDefault; }
			set { animationDefault = value; }
		}

		public SpriteAnimation TalkAnimation {
			get { return animationTalk; }
			set { animationTalk = value; }
		}
	}
}
