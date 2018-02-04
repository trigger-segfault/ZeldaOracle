
namespace ZeldaOracle.Game.Entities.Monsters {
	
	public class MonsterGibdo : BasicMonster {


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public MonsterGibdo() {
			// General
			MaxHealth		= 4;
			ContactDamage	= 2;
			Color			= MonsterColor.Red;
			IsKnockbackable	= false;

			// Movement
			moveSpeed					= 0.375f;
			syncAnimationWithDirection	= false;
			animationMove				= GameData.ANIM_MONSTER_GIBDO;
			stopTime.Set(0, 0);
			moveTime.Set(50, 90);
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		private void TransformIntoStalfos() {
			// TODO: the stalfos needs to carry some properties?
			// if the stalfos dies, then the Gibdo wont respawn.
			// Same goes for Red Gel and Biri.
			MonsterStalfos stalfos = new MonsterStalfos();
			stalfos.Color = MonsterColor.Orange;
			RoomControl.SpawnEntity(stalfos, position);
			DestroyAndTransform(stalfos);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnBurn() {
			// Prevent self from dying from the burn so we can transform
			IsDamageable = false;
			base.OnBurn();
		}
		public override void OnBurnComplete() {
			TransformIntoStalfos();
		}
	}
}
