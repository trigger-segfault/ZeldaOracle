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

			string text = Properties.Get<string>("text");

			// Spawn NPC entity.
			NPC npc = new NPC();
			npc.Properties			= properties;
			npc.Events				= Events;
			npc.Vars				= Variables;
			npc.Triggers			= Triggers;
			npc.Flags				= Properties.GetEnum("npc_flags", NPCFlags.Default);
			npc.Direction			= Properties.Get<int>("direction");
			npc.Message				= (text.Length > 0 ? new Message(text) : null);
			npc.DefaultAnimation	= Properties.GetResource<Animation>("animation");
			npc.TalkAnimation		= Properties.GetResource<Animation>("animation_talk");
			npc.Physics.Flags		= properties.GetEnum("physics_flags", npc.Physics.Flags);
			RoomControl.SpawnEntity(npc, position - npc.Graphics.DrawOffset);
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the action tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, ActionDataDrawArgs args) {
			int direction = args.Properties.Get<int>("direction", 0);
			ActionTile.DrawTileDataIndex(g, args, substripIndex: direction);
		}

		/// <summary>Initializes the properties and events for the action type.</summary>
		public static void InitializeTileData(ActionTileData data) {
			data.Properties.SetEnumInt("npc_flags", NPCFlags.FacePlayerWhenNear | NPCFlags.FacePlayerOnTalk)
				.SetDocumentation("NPC Options", "enum_flags", typeof(NPCFlags), "NPC", "The options for the NPC.");
			data.Properties.Set("direction", Direction.Right)
				.SetDocumentation("Direction", "direction", "", "NPC", "The default direction the NPC faces.");
			data.Properties.Set("text", "")
				.SetDocumentation("Text", "text_message", "", "NPC", "The text to display when the NPC is talked to.");
			data.Properties.Set("animation", "npc_shopkeeper")
				.SetDocumentation("Animation", "animation", "", "NPC", "The animation of the NPC.");
			data.Properties.Set("animation_talk", "")
				.SetDocumentation("Talk Animation", "animation", "", "NPC", "The animation of the NPC when being talked to.");
			data.Properties.Set("physics_flags", (int) (PhysicsFlags.Solid | PhysicsFlags.HasGravity))
				.SetDocumentation("Physics Flags", "enum_flags", typeof(PhysicsFlags), "Physics", "Physics properties of the entity.");

			data.Events.AddEvent("talk", "Talk", "NPC", "Triggered upon talking to the NPC.");
		}

		public enum EventTypes {
			Talk
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
	}
}
