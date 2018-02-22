using ZeldaOracle.Game.Entities;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Items.Weapons {
	
	public class ItemMagnetGloves : ItemWeapon {

		private Polarity polarity;
		private ISprite[] polaritySprites;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemMagnetGloves() {
			this.id				= "item_magnet_gloves";
			this.name			= new string[] { "Magnetic Glovess" };
			this.description	= new string[] { "Magnetically attractive!" };
			this.maxLevel		= Item.Level1;
			this.flags = 
				ItemFlags.UsableWhileJumping | 
				ItemFlags.UsableWhileInHole;

			this.sprite = new ISprite[] {
				GameData.SPR_ITEM_ICON_MAGNET_GLOVES_SOUTH
			};

			polarity = Polarity.South;
			polaritySprites = new ISprite[2];
			polaritySprites[(int) Polarity.North] = GameData.SPR_ITEM_ICON_MAGNET_GLOVES_NORTH;
			polaritySprites[(int) Polarity.South] = GameData.SPR_ITEM_ICON_MAGNET_GLOVES_SOUTH;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		/// <summary>Called when the items button is pressed (A or B).</summary>
		public override bool OnButtonPress() {
			Player.MagnetGlovesState.Weapon = this;
			Player.BeginWeaponState(Player.MagnetGlovesState);
			return true;
		}

		// Draws the item inside the inventory.
		public override void DrawSlot(Graphics2D g, Point2I position) {
			DrawSprite(g, position);
			g.DrawString(GameData.FONT_SMALL, (polarity == Polarity.North ? "N" : "S"),
				position + new Point2I(8, 8), EntityColors.Black);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Polarity Polarity {
			get { return polarity; }
			set {
				polarity = value;
				sprite[0] = polaritySprites[(int) polarity];
			}
		}
	}
}
