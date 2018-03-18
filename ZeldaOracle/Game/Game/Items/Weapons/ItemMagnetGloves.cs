using ZeldaOracle.Game.Entities;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.Items.Weapons {
	
	public class ItemMagnetGloves : ItemWeapon {

		private ISprite[] polaritySprites;

		// TODO: Store these as properties for save format
		private Polarity polarity;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemMagnetGloves() {
			Flags =
				WeaponFlags.UsableWhileJumping |
				WeaponFlags.UsableWhileInHole;

			polarity = Polarity.South;
			polaritySprites = new ISprite[2];
			polaritySprites[(int) Polarity.North] =
				GameData.SPR_ITEM_ICON_MAGNET_GLOVES_NORTH;
			polaritySprites[(int) Polarity.South] =
				GameData.SPR_ITEM_ICON_MAGNET_GLOVES_SOUTH;
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

		/// <summary>Draws the item inside the inventory.</summary>
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
				SetSprite(polaritySprites[(int) polarity]);
			}
		}
	}
}
