using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles;

namespace ZeldaOracle.Game.Entities.Players.States.SwingStates {

	public class PlayerSwingMagicRodState : PlayerSwingState {

		private const int SPAWN_FIRE_DELAY = 6;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PlayerSwingMagicRodState() {
			InitStandardSwing(GameData.ANIM_MAGIC_ROD_SWING,
				GameData.ANIM_MAGIC_ROD_MINECART_SWING);
			AddTimedAction(SPAWN_FIRE_DELAY, SpawnFireProjectile);
		}


		//-----------------------------------------------------------------------------
		// Magic Rod Methods
		//-----------------------------------------------------------------------------
		
		private void SpawnFireProjectile() {
			ItemMagicRod itemMagicRod = Weapon as ItemMagicRod;

			if (itemMagicRod.FireTracker.IsAvailable) {
				// Spawn the fire projectile.
				MagicRodFire fire = new MagicRodFire();
				fire.Owner				= player;
				fire.Position			= player.Center + (Directions.ToVector(player.Direction) * 16.0f);
				fire.ZPosition			= player.ZPosition;
				fire.Direction			= player.Direction;
				fire.Physics.Velocity	= Directions.ToVector(player.Direction) * GameSettings.PROJECTILE_MAGIC_ROD_FIRE_SPEED;
				
				// Adjust the projectile spawn position based on player direction.
				if (Directions.IsHorizontal(player.Direction))
					fire.Position += new Vector2F(0, 4);
				else if (player.Direction == Directions.Up)
					fire.Position -= new Vector2F(4, 0);
				else if (player.Direction == Directions.Down)
					fire.Position += new Vector2F(3, 0);

				player.RoomControl.SpawnEntity(fire);
				itemMagicRod.FireTracker.TrackEntity(fire);
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnSwingBegin() {
			AudioSystem.PlaySound(GameData.SOUND_FIRE_ROD);
		}
	}
}
