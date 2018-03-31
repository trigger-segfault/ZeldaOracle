using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Entities.Objects;

namespace ZeldaOracle.Game.Tiles.ActionTiles {

	/// <summary>Action tile used to spawn an entity.</summary>
	public class PullHandleAction : EntityActionTile<PullHandle> {
		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PullHandleAction() {
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the action tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, ActionDataDrawArgs args) {
			Direction direction =
				args.Properties.GetInteger("direction", Direction.Down);
			g.DrawSprite(
				GameData.ANIM_TILE_PULL_HANDLE.GetSubstrip(direction.Index),
				args.SpriteSettings,
				args.Position,
				args.Color);
		}
		
		/// <summary>Initializes the properties and events for the action type.</summary>
		public static void InitializeTileData(ActionTileData data) {
			data.Properties.Set("direction", Direction.Down)
				.SetDocumentation("Direction", "direction", "", "Pull Handle",
				"The direction the handle extends in.");

			data.Events.AddEvent("retracting", "Retracting", "Pull Handle",
				"Occurs every step that the pull handle is retracting into the wall.");
			data.Events.AddEvent("extending", "Extending", "Pull Handle",
				"Occurs every step that the pull handle is extending from the wall.");
			data.Events.AddEvent("fully_retract", "Fully Retracted", "Pull Handle",
				"Occurs when the handle is fully retracted into the wall.");
			data.Events.AddEvent("fully_extend", "Fully Extended", "Pull Handle",
				"Occurs when the handle is fully extended from the wall.");
		}
	}
}
