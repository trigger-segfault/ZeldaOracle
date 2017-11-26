using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Common.Audio;

namespace ZeldaOracle.Game.Tiles {

	public class TileButton : Tile, ZeldaAPI.Button {

		private bool isPressed;
		private bool isReleasable;
		private HashSet<Tile> tilesCovering; // List of tiles covering this button
		private bool isCovered;
		private int uncoverTimer;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileButton() {
			tilesCovering = new HashSet<Tile>();
		}


		//-----------------------------------------------------------------------------
		// Button Methods
		//-----------------------------------------------------------------------------

		// Set the pressed state of the button.
		public void SetPressed(bool isPressed) {
			if (this.isPressed != isPressed) {
				this.isPressed = isPressed;

				// Set the pressed property.
				Properties.Set("pressed", isPressed);

				// Fire the event.
				if (isPressed) {
					//Graphics.PlaySprite(GameData.SPR_TILE_BUTTON_DOWN);
					GameControl.FireEvent(this, "press", this);
				}
				else {
					//Graphics.PlaySprite(GameData.SPR_TILE_BUTTON_UP);
					GameControl.FireEvent(this, "release", this);
				}

				Graphics.PlaySpriteAnimation(SpriteList[isPressed ? 1 : 0]);
				AudioSystem.PlaySound(GameData.SOUND_PLAYER_WADE);
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------
		
		public override void OnInitialize() {
			isPressed		= Properties.Get("pressed", false);
			isReleasable	= Properties.GetBoolean("releasable", true);
			isCovered		= false;
			uncoverTimer	= 0;
			Graphics.PlaySpriteAnimation(SpriteList[isPressed ? 1 : 0]);
		}

		public override void OnCoverBegin(Tile tile) {
			tilesCovering.Add(tile);
		}

		public override void OnCoverComplete(Tile tile) {
			isCovered = true;
			tilesCovering.Add(tile);
			tile.Graphics.RaisedDrawOffset = Point2I.Zero;
		}

		public override void OnUncoverBegin(Tile tile) {
			isCovered = false;
			if (tile.IsMoving)
				uncoverTimer = GameSettings.TILE_BUTTON_UNCOVER_RELEASE_DELAY;
			else
				uncoverTimer = 0;
			tilesCovering.Remove(tile);
		}

		public override void Update() {
			base.Update();

			bool isDown = isCovered;

			// Update the uncovered state.
			if (!isCovered) {
				// There is a small delay between being uncovered and becoming unpressed.
				uncoverTimer--;
				if (uncoverTimer > 0) {
					isDown = true;
				}
				else {
					// Raise certain tiles (pots) that are partially covering this button.
					foreach (Tile tile in tilesCovering) {
						if (tile.Bounds.Contains(Center) && tile.Properties.GetBoolean("raised_on_buttons", false)) {
							tile.Graphics.RaisedDrawOffset = new Point2I(0, -GameSettings.TILE_BUTTON_TILE_RAISE_AMOUNT);
						}
					}
				}
			}

			// Check if the player is on top of this button.
			if (!isDown) {
				Rectangle2F pressRect = Rectangle2F.Translate(
					GameSettings.TILE_BUTTON_PLAYER_PRESS_AREA, Position);
				if (pressRect.Contains(RoomControl.Player.Position))
					isDown = true;
			}

			// Check if the pressed state needs to be changed.
			if (isPressed != isDown && (isDown || isReleasable))
				SetPressed(isDown);
		}
	}
}
