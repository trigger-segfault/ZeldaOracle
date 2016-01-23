using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles;

namespace ZeldaOracle.Game.Entities.Players.States.SwingStates {

	public class PlayerSwingMagicRodState : PlayerSwingState {

		private const int SPAWN_FIRE_DELAY = 6;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PlayerSwingMagicRodState() {
			isReswingable			= true;
			lunge					= true;
			swingAnglePullBack		= 2;
			swingAngleDurations		= new int[] { 3, 3, 12 };
			weaponSwingAnimation	= GameData.ANIM_MAGIC_ROD_SWING;
			playerSwingAnimation	= GameData.ANIM_PLAYER_SWING;
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
				fire.Owner				= Player;
				fire.Position			= Player.Center + (Directions.ToVector(Player.Direction) * 16.0f);
				fire.ZPosition			= Player.ZPosition;
				fire.Direction			= Player.Direction;
				fire.Physics.Velocity	= Directions.ToVector(Player.Direction) * GameSettings.PROJECTILE_MAGIC_ROD_FIRE_SPEED;
				
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

		public override void OnBegin(PlayerState previousState) {
			if (player.IsInMinecart) {
				weaponSwingAnimation	= GameData.ANIM_MAGIC_ROD_MINECART_SWING;
				playerSwingAnimation	= GameData.ANIM_PLAYER_MINECART_SWING;
			}
			else {
				weaponSwingAnimation	= GameData.ANIM_MAGIC_ROD_SWING;
				playerSwingAnimation	= GameData.ANIM_PLAYER_SWING;
			}
			base.OnBegin(previousState);
		}

		public override void OnSwingBegin() {
			AudioSystem.PlaySound(GameData.SOUND_FIRE_ROD);
		}
	}
}
