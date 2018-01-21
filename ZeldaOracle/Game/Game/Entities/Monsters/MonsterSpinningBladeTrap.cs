
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Monsters {

	public class MonsterSpinningBladeTrap : Monster {
		
		private bool isMoving;
		private Point2I tileLocation;
		private float moveSpeed;
		private float moveDistance;

		
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
			
			// Weapon interactions
			SetReaction(InteractionType.Sword,			Reactions.ParryWithClingEffect);
			SetReaction(InteractionType.SwordSpin,		Reactions.ParryWithClingEffect);
			SetReaction(InteractionType.BiggoronSword,	Reactions.ClingEffect);
			SetReaction(InteractionType.Shovel,			Reactions.ClingEffect);
			// Projectile interactions
			SetReaction(InteractionType.Arrow,			SenderReactions.Intercept);
			SetReaction(InteractionType.SwordBeam,		SenderReactions.Intercept);
			SetReaction(InteractionType.Boomerang,		SenderReactions.Intercept, Reactions.ClingEffect);
			SetReaction(InteractionType.SwitchHook,		SenderReactions.Intercept, Reactions.ClingEffect);
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		private bool CanMoveToNextTile() {
			Point2I nextTileLocation = RoomControl.GetTileLocation(Center) +
				Directions.ToPoint(direction);

			if (!RoomControl.IsTileInBounds(nextTileLocation))
				return false;

			Tile tile = RoomControl.GetTopTile(nextTileLocation);
			if (tile != null && (tile.IsSolid || tile.IsHoleWaterOrLava))
				return false;

			return true;
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
				moveSpeed += GameSettings.MONSTER_SPINNING_BLADE_TRAP_ACCELERATION;
				moveSpeed = GMath.Min(moveSpeed,
					GameSettings.MONSTER_SPINNING_BLADE_TRAP_MAX_SPEED);
				Physics.Velocity = Directions.ToVector(direction) * moveSpeed;
				moveDistance += moveSpeed;

				if (moveDistance > GameSettings.TILE_SIZE) {
					moveDistance = moveSpeed;
					tileLocation = RoomControl.GetTileLocation(Center);

					if (!CanMoveToNextTile()) {
						isMoving = false;
						Physics.Velocity = Vector2F.Zero;
						SetPositionByCenter((tileLocation *
							GameSettings.TILE_SIZE) + new Vector2F(8, 8));
						Graphics.PlayAnimation(GameData.ANIM_MONSTER_SPINNING_BLADE_TRAP_SLEEP);
					}
				}
			}
			else {
				int directionToPlayer = Directions.NearestFromVector(
					RoomControl.Player.Center - Center);

				// Check if player is aligned 
				if (Entity.AreEntitiesAligned(this,
					RoomControl.Player, directionToPlayer,
					GameSettings.MONSTER_SPINNING_BLADE_TRAP_AGGRO_RANGE))
				{
					Direction = directionToPlayer;
					
					if (CanMoveToNextTile()) {
						moveDistance = 0.0f;
						moveSpeed = GameSettings.MONSTER_SPINNING_BLADE_TRAP_INITIAL_SPEED;
						tileLocation = RoomControl.GetTileLocation(Center);
						isMoving = true;
						Graphics.PlayAnimation(GameData.ANIM_MONSTER_SPINNING_BLADE_TRAP);
						AudioSystem.PlaySound(GameData.SOUND_BLOCK_PUSH);
					}
				}
			}
		}
	}
}
