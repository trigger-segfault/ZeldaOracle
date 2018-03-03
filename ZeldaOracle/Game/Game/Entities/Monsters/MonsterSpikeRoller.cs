using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Monsters {
	public class MonsterSpikeRoller : Monster {

		private bool        vertical;
		private int         length;
		private int[]       movePositions;
		private int         destination;

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public MonsterSpikeRoller(Tile tile) {
			ContactDamage	= 2;
			isDamageable	= false;
			isBurnable		= false;
			isStunnable		= false;
			isGaleable		= false;
			IsKnockbackable	= false;

			// Spike Roller Settings
			int tileLocation    = tile.Location[vertical];
			vertical            = tile.Properties.GetBoolean("vertical", false);
			length              = tile.Size[!vertical];
			movePositions       = new int[2];
			movePositions[0]    = tile.Properties.GetInteger("move_distance_1");
			movePositions[1]    = tile.Properties.GetInteger("move_distance_2");
			destination = 0;

			// Prevent rogue spike roller movement
			if (GMath.Sign(movePositions[0]) == GMath.Sign(movePositions[1]))
				movePositions[1] = 0;

			movePositions[0]    += tileLocation;
			movePositions[0]    *= GameSettings.TILE_SIZE;
			movePositions[1]    += tileLocation;
			movePositions[1]    *= GameSettings.TILE_SIZE;

			// Graphics
			centerOffset        = Point2I.FromBoolean(!vertical, length, 1) *
				GameSettings.TILE_SIZE / 2;
			Graphics.DrawOffset	= Point2I.Zero;

			// Physics
			Physics.Flags =	PhysicsFlags.DestroyedOutsideRoom;

			// Interactions
			Interactions.InteractionBox = new Rectangle2F(
				2, 3,
				(Point2I.FromBoolean(!vertical, length, 1) *
					GameSettings.TILE_SIZE) - 4);
			// Weapon Reactions
			Interactions.SetReaction(InteractionType.Sword,			SenderReactions.Intercept, Reactions.ParryWithClingEffect);
			Interactions.SetReaction(InteractionType.SwordSpin,		Reactions.ParryWithClingEffect);
			Interactions.SetReaction(InteractionType.BiggoronSword,	Reactions.ClingEffect);
			Interactions.SetReaction(InteractionType.Shovel,			Reactions.ClingEffect);
			// Seed Reactions
			Interactions.SetReaction(InteractionType.EmberSeed,		SenderReactions.Intercept);
			Interactions.SetReaction(InteractionType.ScentSeed,		SenderReactions.Intercept);
			Interactions.SetReaction(InteractionType.PegasusSeed,	SenderReactions.Intercept);
			Interactions.SetReaction(InteractionType.GaleSeed,		SenderReactions.Intercept);
			Interactions.SetReaction(InteractionType.MysterySeed,	Reactions.MysterySeed);
			// Projectile Reactions
			Interactions.SetReaction(InteractionType.Arrow,			SenderReactions.Intercept);
			Interactions.SetReaction(InteractionType.SwordBeam,		SenderReactions.Intercept);
			Interactions.SetReaction(InteractionType.RodFire,		SenderReactions.Intercept);
			Interactions.SetReaction(InteractionType.Boomerang,		SenderReactions.Intercept);
			Interactions.SetReaction(InteractionType.SwitchHook,		Reactions.None);
			// Environment Reactions
			Interactions.SetReaction(InteractionType.Fire,			Reactions.None);
			Interactions.SetReaction(InteractionType.Gale,			Reactions.None);
			Interactions.SetReaction(InteractionType.BombExplosion,	Reactions.None);
			Interactions.SetReaction(InteractionType.ThrownObject,	Reactions.None);
			Interactions.SetReaction(InteractionType.MineCart,		Reactions.None);
			Interactions.SetReaction(InteractionType.Block,			Reactions.None);
		}


		//-----------------------------------------------------------------------------
		// Overrides
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			
			int currentPosition = (int) GMath.Round(CurrentPosition);
			if (currentPosition == movePositions[0] && currentPosition != movePositions[1]) {
				destination = 1;
			}
			CurrentSpeed =
				GMath.Sign(movePositions[destination] - currentPosition) *
				GameSettings.MONSTER_SPIKE_ROLLER_MOVE_SPEED;
		}

		public override void UpdateAI() {
			float currentPosition = CurrentPosition;
			float currentSpeed =
				GMath.Sign(movePositions[destination] - movePositions[1 - destination]) *
				GameSettings.MONSTER_SPIKE_ROLLER_MOVE_SPEED;
			if (currentSpeed != 0) {
				if ((currentSpeed > 0 && currentPosition + currentSpeed >= movePositions[destination]) ||
					(currentSpeed < 0 && currentPosition + currentSpeed <= movePositions[destination])) {
					CurrentSpeed = (movePositions[destination] - currentPosition) * 2 - currentSpeed;
					// Switch the destination
					destination = 1 - destination;
				}
				else {
					CurrentSpeed = currentSpeed;
				}
			}
		}

		public override void OnTouchPlayer(Entity sender, EventArgs args) {
			Player player = (Player) sender;

			// Treat each square in the spike roller as a center
			float relativePosition = (Position - player.Center)[!vertical];
			int centerIndex = (int) (GMath.Round(relativePosition) / GameSettings.TILE_SIZE);
			centerIndex = GMath.Clamp(centerIndex, 0, length - 1);
			Vector2F center = Position + (GameSettings.TILE_SIZE / 2) +
				Vector2F.FromBoolean(!vertical, centerIndex * GameSettings.TILE_SIZE);
			DamageInfo damageInfo = new DamageInfo(ContactDamage, center);
			player.Hurt(damageInfo);
		}

		public override void Draw(RoomGraphics g) {
			Animation[] spriteList = GameData.ANIM_TRAP_SPIKE_ROLLERS[vertical ? 1 : 0];
			for (int i = 0; i < length; i++) {
				ISprite sprite = spriteList[0];
				if (i == 0) {
					if (i + 1 < length)
						sprite = spriteList[1];
				}
				else if (i + 1 < length)
					sprite = spriteList[2];
				else
					sprite = spriteList[3];
				g.DrawSprite(sprite, new SpriteSettings(RoomControl.CurrentRoomTicks),
					DrawPosition + Point2I.FromBoolean(!vertical, i * GameSettings.TILE_SIZE),
					DepthLayer.Traps);
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		private float CurrentPosition {
			get { return Position[vertical]; }
		}

		private float CurrentSpeed {
			get { return Physics.Velocity[vertical]; }
			set {
				Vector2F velocity = Physics.Velocity;
				velocity[vertical] = value;
				Physics.Velocity = velocity;
			}
		}
	}
}
