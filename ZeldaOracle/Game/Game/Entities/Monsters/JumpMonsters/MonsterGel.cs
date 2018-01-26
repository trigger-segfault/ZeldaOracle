using System;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Players;

namespace ZeldaOracle.Game.Entities.Monsters.JumpMonsters {

	public class MonsterGel : JumpMonster {

		private bool isAttached;
		private int attachTimer;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterGel() {
			// Monster
			color			= MonsterColor.Red;
			MaxHealth		= 1;
			ContactDamage	= 0;

			// Jump monster
			crawlTime				= new RangeI(12);
			stopTime				= new RangeI(8);
			prepareJumpTime			= new RangeI(56);
			shakeDuringPrepareJump	= false;
			jumpOdds				= 6; // Needs confirmation
			crawlSpeed				= 0.25f;
			jumpSpeed				= new RangeF(2.0f);
			moveSpeed				= 1.0f;
			stopAnimation			= GameData.ANIM_MONSTER_GEL;
			prepareJumpAnimation	= GameData.ANIM_MONSTER_GEL_PREPARE_JUMP;
			jumpAnimation			= GameData.ANIM_MONSTER_GEL_JUMP;

			// Physics and graphics
			Physics.CollisionBox		= new Rectangle2F(-6, -3 - 6, 12, 10);
			Physics.SoftCollisionBox	= new Rectangle2F(-4, -3 - 6, 8, 8);
			
			// Weapon interactions
			SetReaction(InteractionType.Sword,			Reactions.Kill);
			SetReaction(InteractionType.SwordSpin,		Reactions.Kill);
			SetReaction(InteractionType.BiggoronSword,	Reactions.Kill);
			// Projectile interactions
			SetReaction(InteractionType.Arrow,			SenderReactions.Intercept, Reactions.Kill);
			SetReaction(InteractionType.SwordBeam,		SenderReactions.Intercept, Reactions.Kill);
			SetReaction(InteractionType.SwitchHook,		SenderReactions.Intercept, Reactions.Kill);
			SetReaction(InteractionType.Boomerang,		SenderReactions.Intercept, Reactions.Kill);
			// Misc interactions
			SetReaction(InteractionType.Block,			Reactions.Kill);
			SetReaction(InteractionType.ThrownObject,	Reactions.Kill);
		}

		
		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		private void Attach() {
			attachTimer					= 0;
			isAttached					= true;
			Physics.HasGravity			= false;
			Physics.CollideWithWorld	= false;
			Physics.IsDestroyedInHoles	= false;
			Physics.Velocity			= Vector2F.Zero;
			physics.ZVelocity			= 0.0f;
			position					= RoomControl.Player.Position;
			zPosition					= 0;
			IsPassable					= true;
			Graphics.DepthLayer			= DepthLayer.PlayerAndNPCs;
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_GEL_ATTACH);
		}
		
		private void Detach() {
			attachTimer					= 60;
			isAttached					= false;
			Physics.HasGravity			= true;
			Physics.CollideWithWorld	= true;
			Physics.IsDestroyedInHoles	= true;
			IsPassable					= false;
			Graphics.DepthLayer			= DepthLayer.Monsters;

			Jump();

			Physics.Velocity = RoomControl.Player.Physics.Velocity;

			if (Physics.Velocity.Length > 0.01f) {
				// If the player is moving, then jump away from him
				Physics.Velocity = Physics.Velocity.Normalized * moveSpeed * -0.5f;
			}
			else {
				// Otherwise, jump in a random angle
				float randomAngle = GMath.TwoPi * GRandom.NextFloat();
				Physics.Velocity = new Vector2F(
					(float) Math.Cos(randomAngle),
					(float) Math.Sin(randomAngle)) * moveSpeed;
			}
		}

				
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			isAttached = false;
			attachTimer = 0;
		}

		public override void OnFallInHole() {
			if (!isAttached)
				base.OnFallInHole();
		}

		public override void OnFallInWater() {
			if (!isAttached)
				base.OnFallInWater();
		}

		public override void OnFallInLava() {
			if (!isAttached)
				base.OnFallInWater();
		}

		public override void OnTouchPlayer(Entity sender, EventArgs args) {
			if (!isAttached && attachTimer == 0) {
				Player player = (Player) sender;
				Attach();
			}
		}

		public override void UpdateAI() {
			if (isAttached) {
				Player player = RoomControl.Player;
				attachTimer++;
				position = player.Position;
				zPosition = 0;
				if (attachTimer > GameSettings.MONSTER_GEL_ATTACH_TIME)
					Detach();
				//else if (player.CurrentState != player.DisarmedState)
				// Detach();
			}
			else {
				if (attachTimer > 0)
					attachTimer--;
				base.UpdateAI();
			}
		}
	}
}
