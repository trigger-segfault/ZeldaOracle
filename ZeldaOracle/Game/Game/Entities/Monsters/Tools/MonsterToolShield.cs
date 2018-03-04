using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Units;

namespace ZeldaOracle.Game.Entities.Monsters.Tools {

	public class MonsterToolShield : UnitTool {
		
		private Rectangle2F[] hitboxes;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterToolShield() {
			toolType = UnitToolType.Shield;
			syncAnimationWithDirection = true;

			hitboxes = new Rectangle2F[Direction.Count];

			// Interactions
			Interactions.Enable();
			// Player Interactions
			Reactions[InteractionType.PlayerContact]
				.Add(SenderReactions.Damage2);
			// Weapon Interactions
			Reactions[InteractionType.Sword]
				.SetProtectParent(true)
				.SetBegin(MonsterReactions.ParryWithClingEffect);
			Reactions[InteractionType.SwordSpin]
				.SetProtectParent(true)
				.SetBegin(MonsterReactions.ParryWithClingEffect);
			Reactions[InteractionType.SwordStrafe]
				.SetProtectParent(true)
				.SetBegin(MonsterReactions.ParryWithClingEffect);
			Reactions[InteractionType.BiggoronSword]
				.SetProtectParent(true)
				.SetBegin(MonsterReactions.ParryWithClingEffect);
			Reactions[InteractionType.Shield]
				.SetProtectParent(true)
				.Add(MonsterReactions.Bump)
				.Add(SenderReactions.Bump);
			Reactions[InteractionType.Shovel]
				.SetProtectParent(true)
				.Add(MonsterReactions.ClingEffect)
				.Add(MonsterReactions.Bump);
			// Seed Reactions
			Reactions[InteractionType.EmberSeed]
				.SetProtectParent(true)
				.Set(SenderReactions.Intercept);
			Reactions[InteractionType.PegasusSeed]
				.SetProtectParent(true)
				.Set(SenderReactions.Intercept);
			Reactions[InteractionType.GaleSeed]
				.SetProtectParent(true)
				.Set(SenderReactions.Intercept);
			Reactions[InteractionType.ScentSeed]
				.SetProtectParent(true)
				.Set(MonsterReactions.ParryWithClingEffect);
			// Projectile Reactions
			Reactions[InteractionType.Boomerang]
				.SetProtectParent(true)
				.Set(MonsterReactions.ParryWithClingEffect);
			Reactions[InteractionType.SwitchHook]
				.SetProtectParent(true)
				.Set(MonsterReactions.ParryWithClingEffect);
			Reactions[InteractionType.Arrow]
				.SetProtectParent(true)
				.Set(MonsterReactions.ParryWithClingEffect);
			Reactions[InteractionType.SwordBeam]
				.SetProtectParent(true)
				.Set(MonsterReactions.ParryWithClingEffect);
			Reactions[InteractionType.RodFire]
				.SetProtectParent(true)
				.Set(SenderReactions.Intercept);
			Reactions[InteractionType.RodFire]
				.SetProtectParent(true)
				.Set(SenderReactions.Intercept);
			// Environment Reactions
			Reactions[InteractionType.Fire]
				.SetProtectParent(true)
				.Set(SenderReactions.Intercept);
			Reactions[InteractionType.Gale]
				.SetProtectParent(true)
				.Set(SenderReactions.Intercept);
		}

		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
		}

		public override void OnEquip() {
			Interactions.InteractionBox = Rectangle2F.Translate(
				hitboxes[unit.Direction], unit.CenterOffset);
		}

		public override void Update() {
			// Set the collision box based on facing direction
			Interactions.InteractionBox = Rectangle2F.Translate(
				hitboxes[unit.Direction], unit.CenterOffset);
			
			base.Update();
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Monster Monster {
			get { return (unit as Monster); }
		}

		public Rectangle2F[] Hitboxes {
			get { return hitboxes; }
		}
	}
}
