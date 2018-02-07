using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Effects;

namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerSidescrollSwimState : PlayerState {


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerSidescrollSwimState() {
			IsNaturalState = true;
		}


		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------

		private void CreateSplashEffect() {
			Effect splash = new Effect(GameData.ANIM_EFFECT_WATER_SPLASH,
				DepthLayer.EffectSplash, true);
			splash.Position = player.Center + new Vector2F(0, 4);
			player.RoomControl.SpawnEntity(splash);
			AudioSystem.PlaySound(GameData.SOUND_PLAYER_WADE);
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override bool RequestStateChange(PlayerState newState) {
			return true;
		}

		public override void OnBegin(PlayerState previousState) {
			player.Movement.CanJump			= false;
			player.Movement.MoveSpeedScale	= 1.0f;
			player.Movement.AutoAccelerate	= false;
			player.Movement.OnlyFaceLeftOrRight	= true;
			player.MoveAnimation			= GameData.ANIM_PLAYER_MERMAID_SWIM;
			player.Physics.HasGravity		= false;
			player.Graphics.PlayAnimation(player.MoveAnimation);

			// Force facing left or right
			if (player.Direction == Directions.Up)
				player.Direction = Directions.Right;
			if (player.Direction == Directions.Down)
				player.Direction = Directions.Left;

			if (previousState.IsNaturalState)
				CreateSplashEffect();
		}
		
		public override void OnEnd(PlayerState newState) {
			player.Movement.CanJump			= true;
			player.Movement.MoveSpeedScale	= 1.0f;
			player.Movement.AutoAccelerate	= false;
			player.Movement.OnlyFaceLeftOrRight	= false;
			player.Graphics.DepthLayer		= DepthLayer.PlayerAndNPCs;
			player.MoveAnimation			= GameData.ANIM_PLAYER_DEFAULT;
			player.Physics.HasGravity		= true;
			
			if (!player.Movement.IsOnSideScrollLadder && newState.IsNaturalState)
			{
				// Jump out of the water, and create a splash effect
				player.Physics.ZVelocity = 1.5f;
				CreateSplashEffect();
			}
		}

		public override void Update() {

			// TODO: Code duplication with PlayerSwimState
			
			// Force facing left or right
			if (player.Direction == Directions.Up)
				player.Direction = Directions.Right;
			if (player.Direction == Directions.Down)
				player.Direction = Directions.Left;

			// Slow down movement over time from strokes
			if (player.Movement.MoveSpeedScale > 1.0f)
				player.Movement.MoveSpeedScale -= 0.025f;

			// Stroking scales the movement speed.
			// Press A to stroke, but this will not work if an item is usable
			// in slot A.
			//if (player.Movement.MoveSpeedScale <= 1.4f &&
			//	Controls.A.IsPressed() && 
			//	(player.EquippedUsableItems[Inventory.SLOT_A] == null || 
			//	!player.EquippedUsableItems[Inventory.SLOT_A].IsUsable()))
			//{
			//	AudioSystem.PlaySound(GameData.SOUND_PLAYER_SWIM);
			//	player.Movement.MoveSpeedScale = 2.0f;
			//}

			// Auto accelerate during the beginning of a stroke.
			player.Movement.AutoAccelerate = IsStroking;

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
