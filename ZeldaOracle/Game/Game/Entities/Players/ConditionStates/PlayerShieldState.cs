using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Util;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;

namespace ZeldaOracle.Game.Entities.Players.States {

	/// <summary>Player condition state which is active when the payer has the shield
	/// equipped. ItemShield is responsible for beginning and ending this PlayerState.
	/// </summary>
	public class PlayerShieldState : PlayerState {

		private enum SubState {
			NotBlocking,
			Blocking,
		}

		private ItemShield weapon;
		private GenericStateMachine<SubState> subStateMachine;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerShieldState() {
			subStateMachine = new GenericStateMachine<SubState>();
			subStateMachine.AddState(SubState.NotBlocking)
				.OnBegin(OnBeginNotBlockingState)
				.OnUpdate(OnUpdateNotBlockingState);
			subStateMachine.AddState(SubState.Blocking)
				.OnBegin(OnBeginBlockingState)
				.OnUpdate(OnUpdateBlockingState);
		}


		//-----------------------------------------------------------------------------
		// Sub State Callbacks
		//-----------------------------------------------------------------------------

		private void OnBeginNotBlockingState() {
			StateParameters.ProhibitPushing = false;

			// Set the player's default animation
			if (weapon.Level == Item.Level1)
				PlayerAnimations.Default = GameData.ANIM_PLAYER_SHIELD;
			else
				PlayerAnimations.Default = GameData.ANIM_PLAYER_SHIELD_LARGE;

			// Unequip the shield unit tool
			if (player.ToolShield.IsEquipped)
				player.UnequipTool(player.ToolShield);
		}

		private void OnUpdateNotBlockingState() {
			// Set the player's default animation
			if (weapon.Level == Item.Level1)
				PlayerAnimations.Default = GameData.ANIM_PLAYER_SHIELD;
			else
				PlayerAnimations.Default = GameData.ANIM_PLAYER_SHIELD_LARGE;
			
			// Check for beginning shield blocking
			if (weapon.IsButtonDown() &&
				!player.PressedActionButtons[(int) weapon.ActionButton] &&
				(player.WeaponState == null || player.WeaponState == player.PushState))
			{
				subStateMachine.BeginState(SubState.Blocking);
			}
		}

		private void OnBeginBlockingState() {
			StateParameters.ProhibitPushing = true;
			player.StopPushing();

			// Play the shield sound
			if (Player.WeaponState == null && Player.IsOnGround)
				AudioSystem.PlaySound(GameData.SOUND_SHIELD);

			// Set the player's default animation
			if (weapon.Level == Item.Level1)
				PlayerAnimations.Default = GameData.ANIM_PLAYER_SHIELD_BLOCK;
			else
				PlayerAnimations.Default = GameData.ANIM_PLAYER_SHIELD_LARGE_BLOCK;
		}

		private void OnUpdateBlockingState() {
			// Check if the button was released
			if (!weapon.IsButtonDown()) {
				subStateMachine.BeginState(SubState.NotBlocking);
				return;
			}

			// Set the player's default animation
			if (weapon.Level == Item.Level1)
				PlayerAnimations.Default = GameData.ANIM_PLAYER_SHIELD_BLOCK;
			else
				PlayerAnimations.Default = GameData.ANIM_PLAYER_SHIELD_LARGE_BLOCK;
			
			// Equip/unequip the unit tool for the shield when blocking
			// FIXME: there is a one-frame delay between when this becomes true
			if (Player.Graphics.Animation == GameData.ANIM_PLAYER_SHIELD_BLOCK ||
				Player.Graphics.Animation == GameData.ANIM_PLAYER_SHIELD_LARGE_BLOCK)
			{
				player.EquipTool(player.ToolShield);
			}
			else if (player.ToolShield.IsEquipped) {
				player.UnequipTool(player.ToolShield);
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			subStateMachine.InitializeOnState(SubState.NotBlocking);
		}

		public override void Update() {
			subStateMachine.Update();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public ItemShield Weapon {
			get { return weapon; }
			set { weapon = value; }
		}
	}
}
