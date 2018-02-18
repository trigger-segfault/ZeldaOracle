using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles;
using ZeldaOracle.Game.Items.Weapons;

namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerMagnetGlovesState : PlayerState {

		private ItemMagnetGloves weapon;
		private AnimationPlayer effectAnimation;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerMagnetGlovesState() {
			StateParameters.ProhibitJumping	= true;
			StateParameters.EnableStrafing	= true;
			PlayerAnimations.Default		= GameData.ANIM_PLAYER_AIM_WALK;
			effectAnimation					= new AnimationPlayer();
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			Animation animation;
			if (Polarity == Polarity.North)
				animation = GameData.ANIM_EFFECT_MAGNET_GLOVES_RED;
			else
				animation = GameData.ANIM_EFFECT_MAGNET_GLOVES_BLUE;
			effectAnimation.Play(animation);
			effectAnimation.SubStripIndex = player.Direction;
		}
		
		public override void OnEnd(PlayerState newState) {
			// Reverse polarity of the item
			if (weapon.Polarity == Polarity.North)
				weapon.Polarity = Polarity.South;
			else
				weapon.Polarity = Polarity.North;
		}

		public override void Update() {

			// Udpate the magnet effect
			effectAnimation.SubStripIndex = player.Direction;
			effectAnimation.Update();

			// Check if the button was released
			if (!weapon.IsEquipped || !weapon.IsButtonDown())
				End();
		}

		public override void DrawOver(RoomGraphics g) {
			// Draw the magnet effect
			g.DrawAnimationPlayer(effectAnimation, player.Position,
				DepthLayer.EffectMagnetGloves);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public ItemMagnetGloves Weapon {
			get { return weapon; }
			set { weapon = value; }
		}

		public Polarity Polarity {
			get { return weapon.Polarity; }
		}
	}
}
