using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Items.Weapons {
	public class ItemBracelet : ItemWeapon {


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemBracelet(int level = 0) {
			this.id				= "item_bracelet";
			this.name			= new string[] { "Power Bracelet", "Power Gloves" };
			this.description	= new string[] { "A strength booster.", "Used to lift large objects." };
			this.level			= level;
			this.maxLevel		= 1;
			this.flags			= ItemFlags.None;

			sprite = new ISprite[] {
				GameData.SPR_ITEM_ICON_BRACELET,
				GameData.SPR_ITEM_ICON_POWER_GLOVES
			};
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		// Grab, pull, and pickup objects.
		public override void OnButtonDown() {
			// Check for a tile to grab.
			if (!Player.IsBeingKnockedBack) {
				Tile grabTile = Player.Physics.GetFacingSolidTile(Player.Direction);
				if (grabTile != null && !grabTile.IsMoving)
					grabTile.OnGrab(Player.Direction, this);
			}
		}
	}
}
