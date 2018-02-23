using System;
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
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Common.Audio;

namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerJumpToState : PlayerState {

		private Vector2F destination;
		private float destinationZPosition;
		private int jumpDuration;
		private Action<PlayerJumpToState> endAction;

		private int timer;
		private Vector2F startPosition;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerJumpToState() {
			destination				= Vector2F.Zero;
			destinationZPosition	= 0.0f;
			jumpDuration			= 26;
			endAction				= null;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			StateParameters.EnableAutomaticRoomTransitions	= true;
			StateParameters.EnableStrafing					= true;
			StateParameters.DisableSolidCollisions			= true;
			StateParameters.DisableInteractionCollisions	= true;
			StateParameters.DisableSurfaceContact			= true;
			StateParameters.DisablePlayerControl			= true;

			player.Physics.ZVelocity = GameSettings.PLAYER_JUMP_SPEED;

			// Play the jump animation
			if (player.WeaponState == null)
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_JUMP);
			else if (player.WeaponState == player.CarryState)
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_CARRY);
			else if (player.Graphics.Animation == player.MoveAnimation)
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_JUMP);
			
			AudioSystem.PlaySound(GameData.SOUND_PLAYER_JUMP);

			timer = 0;
			startPosition = player.Position;
		}
		
		public override void OnEnd(PlayerState newState) {
		}

		public override void Update() {
			base.Update();	

			timer++;
			float percent = GMath.Min(1.0f, (float) timer / (float) jumpDuration);
			player.Position = Vector2F.Lerp(startPosition, destination, percent);
			
			player.Physics.Velocity = Vector2F.Zero;

			if (timer >= jumpDuration && player.ZPosition <= destinationZPosition) {
				player.ZPosition = destinationZPosition;
				player.Position  = destination;
				if (endAction != null)
					endAction(this);
				else
					End(null);
			}

			// TODO: bug with knockback and this state.
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Vector2F DestinationPosition {
			get { return destination; }
			set { destination = value; }
		}
		
		public float DestinationZPosition {
			get { return destinationZPosition; }
			set { destinationZPosition = value; }
		}
		
		public int JumpDuration {
			get { return jumpDuration; }
			set { jumpDuration = value; }
		}
		
		public Action<PlayerJumpToState> EndAction {
			get { return endAction; }
			set { endAction = value; }
		}
	}
}
