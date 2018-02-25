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
			IsPhysicsEnabled = true;
		}

		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			base.OnInitialize();
		}

		public override void OnEquip() {
			base.OnEquip();
			
			AnimationPlayer.Play(GameData.ANIM_MONSTER_SWORD_HOLD);
			AnimationPlayer.Pause();
		}

		public override void Update() {
			// Set the collision box based on facing direction
			collisionBox = SWORD_BOXES[unit.Direction];
			collisionBox.Point += (Point2I) unit.CenterOffset;
			
			// Change depth based on facing direction
			DrawAboveUnit = (unit.Direction == Directions.Right ||
				unit.Direction == Directions.Down);

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
