﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Players {
	public class PlayerSwimState : PlayerState {

		private bool	isSubmerged;
		private int		submergedTimer;
		private int		submergedDuration;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerSwimState() {
			submergedDuration	= 128;
			isSubmerged			= false;
			submergedTimer		= 0;
			isNaturalState		= true;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			base.OnBegin();

			player.Movement.MoveSpeedScale = 1.0f;
			player.Movement.AutoAccelerate = false;

			isSubmerged = false;
			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_SWIM);
			
			// Create a splash effect.
			Effect splash = new Effect(GameData.ANIM_EFFECT_WATER_SPLASH);
			splash.Position = player.Position - new Vector2F(0, 4);
			player.RoomControl.SpawnEntity(splash);
		}
		
		public override void OnEnd() {
			isSubmerged = false;
			player.Movement.MoveSpeedScale = 1.0f;
			player.Movement.AutoAccelerate = false;
			base.OnEnd();
		}

		public override void Update() {
			// Slow down movement over time from strokes
			if (player.Movement.MoveSpeedScale > 1.0f)
				player.Movement.MoveSpeedScale -= 0.025f;
			
			// Stroking scales the movement speed.
			if (player.Movement.MoveSpeedScale <= 1.4f && Controls.A.IsPressed()) {
				//Sounds.PLAYER_SWIM.play();
				player.Movement.MoveSpeedScale = 2.0f;
			}

			// Auto accelerate during the beginning of a stroke.
			player.Movement.AutoAccelerate = IsStroking;

			// Update the submerge state.
			if (isSubmerged) {
				submergedTimer--;
				if (submergedTimer <= 0 || Controls.B.IsPressed()) {
					isSubmerged = false;
					player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_SWIM);
				}
			}
			else if (Controls.B.IsPressed()) {
				isSubmerged = true;
				submergedTimer = submergedDuration;
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_DIVE);

				// Create a splash effect.
				Effect splash = new Effect(GameData.ANIM_EFFECT_WATER_SPLASH);
				splash.Position = player.Position - new Vector2F(0, 4);
				player.RoomControl.SpawnEntity(splash);

				//Sounds.PLAYER_WADE.play();

				// TODO: Change player depth to lowest.
			}

			base.Update();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		// This is the threshhold of movement speed scale to be considered stroking.
		public bool IsStroking {
			get { return (player.Movement.MoveSpeedScale > 1.3f); }
		}

	}
}