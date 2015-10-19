using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Players;

namespace ZeldaOracle.Game.Entities.Monsters {
	public class Monster : Unit {


		private Properties properties;
		private int contactDamage;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Monster() {
			// Physics.
			Physics.CollisionBox		= new Rectangle2I(3 - 8, 5 - 14, 10, 10);
			Physics.SoftCollisionBox	= new Rectangle2I(2 - 8, 3 - 14, 12, 13);
			Physics.CollideWithWorld	= true;
			Physics.CollideWithRoomEdge	= true;
			Physics.HasGravity			= true;

			// Graphics.
			Graphics.DrawOffset = new Point2I(-8, -14);
			centerOffset		= new Point2I(0, -6);

			// Monster settings.
			contactDamage = 1;
		}

		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();

			Graphics.PlayAnimation(GameData.ANIM_PLAYER_DEFAULT);
		}

		public override void Update() {

			// Check collisions with player.
			Player player = RoomControl.Player;
			if (physics.IsSoftMeetingEntity(player)) {
				player.Hurt(contactDamage, Center);
			}

			base.Update();
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Properties Properties {
			get { return properties; }
			set { properties = value; }
		}

		public int ContactDamage {
			get { return contactDamage; }
			set { contactDamage = value; }
		}
	}
}
