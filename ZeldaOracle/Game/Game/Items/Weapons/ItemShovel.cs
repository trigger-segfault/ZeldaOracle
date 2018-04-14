using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Players.States;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.Items.Weapons {
	public class ItemShovel : ItemWeapon {
		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemShovel() {
			Flags = WeaponFlags.UsableWhileInHole;
		}


		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------

		private void PerformDig(PlayerState state) {
			// Look for tiles to dig up
			float distance = 6.5f;
			Vector2F center = Player.Center;
			if (Player.Direction.IsVertical)
				distance = 7.5f;
			else
				center.Y += 4.0f;
			Vector2F hotspot = GMath.Round(center) +
				Player.Direction.ToVector(distance);
			Point2I tileLoc = RoomControl.GetTileLocation(hotspot);
			if (!RoomControl.IsTileInBounds(tileLoc))
				return;

			Tile tile = RoomControl.GetTopTile(tileLoc);

			if (tile != null && tile.OnDig(Player.Direction)) {
				// Spawn dirt effect
				Effect effect = new Effect();
				effect.Graphics.DepthLayer = DepthLayer.EffectDirt;
				effect.CreateDestroyTimer(15);
				effect.Physics.Enable(PhysicsFlags.HasGravity);
				effect.Physics.Velocity = Player.Direction.ToVector(0.5f);
				effect.Graphics.IsShadowVisible = false;
				effect.Graphics.PlayAnimation(GameData.ANIM_EFFECT_DIRT);
				effect.Graphics.SubStripIndex = Player.Direction;
				if (Player.Direction.IsHorizontal) {
					effect.Physics.ZVelocity	= 3.0f;
					effect.Physics.Gravity		= 0.5f;
				}
				else {
					effect.Physics.ZVelocity	= 2.5f;
					effect.Physics.Gravity		= 0.4f;
				}
				RoomControl.SpawnEntity(effect, tile.Center);
				

				AudioSystem.PlaySound(GameData.SOUND_SHOVEL);
			}
			else {
				AudioSystem.PlaySound(GameData.SOUND_EFFECT_CLING);
			}

			// Trigger shovel reactions
			// Note that these happens even if a tile is dug
			Rectangle2F shovelHitBox = new Rectangle2F(-4, -4, 8, 8);
			shovelHitBox.ExtendEdge(Player.Direction, 7);
			shovelHitBox.Point += Player.CenterOffset;
			EventArgs actionArguments = new WeaponInteractionEventArgs() {
				Weapon = this
			};
			HitBox hitBox = new HitBox(shovelHitBox,
				Player.Interactions.InteractionZRange);
			RoomControl.InteractionManager.TriggerReaction(Player,
				InteractionType.Shovel, hitBox, actionArguments);
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Called when the items button is pressed (A or B)
		public override bool OnButtonPress() {
			Player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_DIG);
			Player.BeginBusyState(GameData.ANIM_PLAYER_DIG.Duration)
				.AddDelayedAction(4, PerformDig);
			return true;
		}

		// Update the item.
		public override void Update() { }

		// Draws under link's sprite.
		public override void DrawUnder() { }

		// Draws over link's sprite.
		public override void DrawOver() { }
	}
}
