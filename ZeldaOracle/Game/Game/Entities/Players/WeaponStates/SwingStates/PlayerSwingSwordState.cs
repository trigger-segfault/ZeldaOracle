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

		/// <summary>True if the player allowed to hold the sword after swinging. This
		/// will be set to false when the player slashes a monster.</summary>
		private bool allowHold;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PlayerSwingSwordState() {
			allowHold = true;
			limitTilesToDirection = true;
			
			InitStandardSwing(GameData.ANIM_SWORD_SWING,
				GameData.ANIM_SWORD_MINECART_SWING);
			AddTimedAction(SWING_SWORD_BEAM_DELAY, SpawnSwordBeam);
		}


		//-----------------------------------------------------------------------------
		// Sword Methods
		//-----------------------------------------------------------------------------
		
		private void SpawnSwordBeam() {
			ItemSword itemSword = Weapon as ItemSword;

			if (itemSword.BeamTracker.IsAvailable && player.IsAtFullHealth &&
				itemSword.Level > Item.Level1)
			{
				// Spawn a sword beam
				SwordBeam beam = new SwordBeam();
				beam.Owner		= player;
				beam.Position	= player.Center + player.Direction.ToVector(12.0f);
				beam.ZPosition	= player.ZPosition;
				beam.Direction	= player.Direction;
				beam.Physics.Velocity = player.Direction.ToVector(
					GameSettings.PROJECTILE_SWORD_BEAM_SPEED);

				// Adjust the beam spawn position based on player direction
				if (player.Direction.IsHorizontal)
					beam.Position += new Vector2F(0, 4);
				else if (player.Direction == Direction.Up)
					beam.Position -= new Vector2F(4, 0);
				else if (player.Direction == Direction.Down)
					beam.Position += new Vector2F(3, 0);
			
				player.RoomControl.SpawnEntity(beam);
				itemSword.BeamTracker.TrackEntity(beam);
				
				AudioSystem.PlaySound(GameData.SOUND_SWORD_BEAM);
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnSwingBegin() {
			base.OnSwingBegin();
			allowHold = !player.IsInMinecart;
			player.ToolSword.Interactions.InteractionType = InteractionType.Sword;
			AudioSystem.PlayRandomSound(
				GameData.SOUND_SWORD_SLASH_1,
				GameData.SOUND_SWORD_SLASH_2,
				GameData.SOUND_SWORD_SLASH_3);
		}

		public override void OnSwingEnd() {
			if (!player.IsInMinecart && allowHold &&
				Weapon.IsEquipped && Weapon.IsButtonDown())
			{
				// Begin holding the sword after swinging
				player.HoldSwordState.Weapon = Weapon;
				StateMachine.BeginState(player.HoldSwordState);
			}
			else {
				End();
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		/// <summary>True if the player allowed to hold the sword after swinging. This
		/// will be set to false when the player slashes a monster.</summary>
		public bool AllowSwordHold {
			get { return allowHold; }
			set { allowHold = false; }
		}
	}
}
