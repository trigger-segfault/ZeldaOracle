
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Monsters {

	public class MonsterSpinningBladeTrap : Monster {
		
		private bool isMoving;
		private Point2I tileLocation;
		private float moveSpeed;
		private float moveDistance;
		private int timer;

		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterSpinningBladeTrap() {
			// General.
			ContactDamage	= 2;
			isDamageable	= false;
			isBurnable		= false;
			isStunnable		= false;
			isGaleable		= false;
			IsKnockbackable	= false;
			
			// Weapon Reactions
			Reactions[InteractionType.Sword]
				.SetBegin(MonsterReactions.ParryWithClingEffect);
			Reactions[InteractionType.SwordStrafe]
				.SetBegin(MonsterReactions.ParryWithClingEffect);
			Reactions[InteractionType.SwordSpin]
				.SetBegin(MonsterReactions.ParryWithClingEffect);
			Reactions[InteractionType.BiggoronSword]
				.SetBegin(MonsterReactions.ClingEffect);
			Reactions[InteractionType.Shield]
				.SetBegin(MonsterReactions.ParryWithClingEffect);
			Reactions[InteractionType.Shovel]
				.SetBegin(MonsterReactions.ClingEffect);
			// Projectile Reactions
			Reactions[InteractionType.Arrow]
				.Set(SenderReactions.Intercept);
			Reactions[InteractionType.SwordBeam]
				.Set(SenderReactions.Intercept);
			Reactions[InteractionType.Boomerang]
				.Set(MonsterReactions.ParryWithClingEffect);
			Reactions[InteractionType.SwitchHook]
				.Set(MonsterReactions.ParryWithClingEffect);
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		private bool CanMoveToNextTile(int direction) {
			Point2I nextTileLocation = RoomControl.GetTileLocation(Center) +
				Directions.ToPoint(direction);

			// Check if the tile is outside of the room
			if (!RoomControl.IsTileInBounds(nextTileLocation))
				return false;

			// Check if the tile is solid or hazardous
			Tile tile = RoomControl.GetTopTile(nextTileLocation);
			if (tile != null && (tile.IsSolid || tile.IsHoleWaterOrLava))
				return false;

			return true;
		}

		private void StartMoving(int moveDirection) {
			Direction = moveDirection;
			timer = 0;
			moveDistance = 0.0f;
			moveSpeed = GameSettings.MONSTER_SPINNING_BLADE_TRAP_SLOW_SPEED;
			tileLocation = RoomControl.GetTileLocation(Center);
			isMoving = true;
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_SPINNING_BLADE_TRAP);
			AudioSystem.PlaySound(GameData.SOUND_BLOCK_PUSH);
		}
		
		private void StopMoving() {
			isMoving = false;
			Physics.Velocity = Vector2F.Zero;
			tileLocation = RoomControl.GetTileLocation(Center);
			SetPositionByCenter((tileLocation *
				GameSettings.TILE_SIZE) + new Vector2F(8, 8));
			Graphics.PlayAnimation(
				GameData.ANIM_MONSTER_SPINNING_BLADE_TRAP_SLEEP);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_SPINNING_BLADE_TRAP_SLEEP);
		}

		public override void UpdateAI() {
			if (isMoving) {
				// After a delay, speed up
				timer++;
				if (timer > GameSettings.MONSTER_SPINNING_BLADE_TRAP_SPEED_UP_DELAY) {
					moveSpeed = GameSettings.MONSTER_SPINNING_BLADE_TRAP_FAST_SPEED;
				}

				Physics.Velocity = Directions.ToVector(direction) * moveSpeed;
				moveDistance += moveSpeed;

				// After moving one tile, check if we can move one more
				if (moveDistance > GameSettings.TILE_SIZE) {
					moveDistance = moveSpeed;
					tileLocation = RoomControl.GetTileLocation(Center);

					if (!CanMoveToNextTile(Direction))
						StopMoving();
				}
			}
			else {
				int directionToPlayer = Directions.NearestFromVector(
					RoomControl.Player.Center - Center);

				// Check if player is aligned 
				if (Entity.AreEntitiesAligned(this,
						RoomControl.Player, directionToPlayer,
						GameSettings.MONSTER_SPINNING_BLADE_TRAP_AGGRO_RANGE) &&
					CanMoveToNextTile(directionToPlayer))
				{
					StartMoving(directionToPlayer);
				}
			}
		}
	}
}
