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

	public class PlayerSwingSwordState : PlayerBaseSwingSwordState {

		
		private const int SWING_SWORD_BEAM_DELAY = 6;

		// True if the player allowed to hold the sword after swinging.
		// This turns false when the player slashes a monster.
		private bool allowHold;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PlayerSwingSwordState() {
			allowHold				= true;
			limitTilesToDirection	= true;
			isReswingable			= true;
			lunge					= true;
			swingAnglePullBack		= 2;
			swingAngleDurations		= new int[] { 3, 3, 12 };
			weaponSwingAnimation	= GameData.ANIM_SWORD_SWING;
			playerSwingAnimation	= GameData.ANIM_PLAYER_SWING;
			AddTimedAction(SWING_SWORD_BEAM_DELAY, SpawnSwordBeam);
		}


		//-----------------------------------------------------------------------------
		// Sword Methods
		//-----------------------------------------------------------------------------
		
		private void SpawnSwordBeam() {
			ItemSword itemSword = Weapon as ItemSword;

			if (itemSword.BeamTracker.IsAvailable && player.IsAtFullHealth && itemSword.Level > Item.Level1) {
				// Spawn a sword beam.
				SwordBeam beam = new SwordBeam();
				beam.Owner				= Player;
				beam.Position			= Player.Center + (Directions.ToVector(Player.Direction) * 12.0f);
				beam.ZPosition			= Player.ZPosition;
				beam.Direction			= Player.Direction;
				beam.Physics.Velocity	= Directions.ToVector(Player.Direction) * GameSettings.PROJECTILE_SWORD_BEAM_SPEED;

				// Adjust the beam spawn position based on player direction.
				if (Directions.IsHorizontal(player.Direction))
					beam.Position += new Vector2F(0, 4);
				else if (player.Direction == Directions.Up)
					beam.Position -= new Vector2F(4, 0);
				else if (player.Direction == Directions.Down)
					beam.Position += new Vector2F(3, 0);
			
				player.RoomControl.SpawnEntity(beam);
				itemSword.BeamTracker.TrackEntity(beam);
				
				AudioSystem.PlaySound(GameData.SOUND_SWORD_BEAM);
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			if (player.IsInMinecart) {
				weaponSwingAnimation	= GameData.ANIM_SWORD_MINECART_SWING;
				playerSwingAnimation	= GameData.ANIM_PLAYER_MINECART_SWING;
			}
			else {
				weaponSwingAnimation	= GameData.ANIM_SWORD_SWING;
				playerSwingAnimation	= GameData.ANIM_PLAYER_SWING;
			}
			base.OnBegin(previousState);
		}

		public override void OnSwingBegin() {
			base.OnSwingBegin();
			allowHold = true;
			AudioSystem.PlayRandomSound(
				GameData.SOUND_SWORD_SLASH_1,
				GameData.SOUND_SWORD_SLASH_2,
				GameData.SOUND_SWORD_SLASH_3);
		}

		public override void OnSwingEnd() {
			// Begin holding the sword after swinging.
			if (!player.IsInMinecart && allowHold && Weapon.IsEquipped && Weapon.IsButtonDown()) {
				player.HoldSwordState.Weapon = Weapon;
				player.BeginState(player.HoldSwordState);
			}
			else
				base.OnSwingEnd();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool AllowSwordHold {
			get { return allowHold; }
			set { allowHold = false; }
		}
	}
}
