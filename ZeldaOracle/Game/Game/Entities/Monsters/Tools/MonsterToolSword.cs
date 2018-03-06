using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Units;

namespace ZeldaOracle.Game.Entities.Monsters.Tools {

	public class MonsterToolSword : UnitTool {
		
		private readonly Rectangle2I[] SWORD_BOXES = new Rectangle2I[] {
			new Rectangle2I( 12 - 8,   4 - 8 + 7, 11, 2),
			new Rectangle2I(  4 - 8 + 7, -12 - 8 + 3 + 2, 2, 11),
			new Rectangle2I(-12 - 8 + 3 + 2,   4 - 8 + 7, 11, 2),
			new Rectangle2I( -4 - 8 + 7,  14 - 8, 2, 8),
		};


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterToolSword() {
			toolType = UnitToolType.Sword;
			syncAnimationWithDirection = true;

			// Interactions
			Interactions.Enable();
			// Player Interactions
			Reactions[InteractionType.PlayerContact]
				.Set(SenderReactions.Damage2);
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
				.SetBegin(MonsterReactions.ParryWithClingEffect);
		}

		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
		}

		public override void OnEquip() {
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_SWORD_HOLD);
			Graphics.PauseAnimation();

			Interactions.InteractionBox = Rectangle2F.Translate(
				SWORD_BOXES[unit.Direction], (Point2I) unit.CenterOffset);
		}

		public override void Update() {
			// Set the collision box based on facing direction
			Interactions.InteractionBox = Rectangle2F.Translate(
				SWORD_BOXES[unit.Direction], (Point2I) unit.CenterOffset);
			
			// Change depth based on facing direction
			DrawAboveUnit = (unit.Direction == Direction.Right ||
				unit.Direction == Direction.Down);

			Graphics.ColorDefinitions = Unit.Graphics.ColorDefinitions;

			base.Update();
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Monster Monster {
			get { return (unit as Monster); }
		}
	}
}
