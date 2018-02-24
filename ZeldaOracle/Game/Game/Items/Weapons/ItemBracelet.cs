using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Monsters;

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
		
		/// <summary>Grab, pull, and pickup objects.</summary>
		public override void OnButtonDown() {
			// Check for a tile to grab
			if (!Player.IsBeingKnockedBack) {
				Tile grabTile = Player.GrabState.GetGrabTile();
				if (grabTile != null) {
					grabTile.OnGrab(Player.Direction, this);
					return;
				}
				foreach (Entity entity in RoomControl.Entities) {
					if (entity.IsPickupable && Player.Physics.IsBraceletMeetingEntity(
						entity, GameSettings.PLAYER_BRACELET_BOXES[Player.Direction]))
					{
						Player.CarryState.SetCarryObject(entity);
						Player.BeginWeaponState(Player.CarryState);
						entity.RemoveFromRoom();
						return;
					}
					else if (entity is Monster && Player.Physics.IsBraceletMeetingEntity(
						entity, GameSettings.PLAYER_BRACELET_BOXES[Player.Direction]))
					{
						((Monster) entity).TriggerInteraction(InteractionType.Bracelet,
							Player, new WeaponInteractionEventArgs() {
								Weapon = this
						});
						return;
					}
				}
			}
		}
	}
}
