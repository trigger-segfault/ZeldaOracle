using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Tiles.ActionTiles {

	public class NPCAction : ActionTile {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public NPCAction() {
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		protected override void Initialize() {
			base.Initialize();

			// Spawn NPC entity.
			NPC npc = new NPC();
			npc.Flags				= (NPCFlags) Properties.GetInteger("npc_flags");
			npc.Direction			= Properties.GetInteger("direction");
			npc.Message				= new Message(Properties.GetString("text"));
			npc.DefaultAnimation	= Properties.GetResource<Animation>("animation");
			npc.TalkAnimation		= Properties.GetResource<Animation>("animation_talk");
			RoomControl.SpawnEntity(npc, position - npc.Graphics.DrawOffset);
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the action tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, ActionTileDataDrawArgs args) {
			int direction = args.Properties.GetInteger("direction", 0);
			ActionTile.DrawTileDataIndex(g, args, substripIndex: direction);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
	}
}
