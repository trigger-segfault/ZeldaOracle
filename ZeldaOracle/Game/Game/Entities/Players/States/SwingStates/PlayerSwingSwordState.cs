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

namespace ZeldaOracle.Game.Entities.Players.States.SwingStates {

	public class PlayerSwingSwordState : PlayerBaseSwingSwordState {

		
		private const int SWING_SWORD_BEAM_DELAY = 6;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PlayerSwingSwordState() {
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
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnSwingBegin() {
			base.OnSwingBegin();
			AudioSystem.PlayRandomSound("Items/slash_1", "Items/slash_2", "Items/slash_3");
		}

		public override void OnSwingEnd() {
			player.HoldSwordState.Weapon = Weapon;
			player.BeginState(player.HoldSwordState);
		}
	}
}
