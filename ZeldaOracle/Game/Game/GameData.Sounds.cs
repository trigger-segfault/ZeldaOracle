using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Graphics;


namespace ZeldaOracle.Game {
	public partial class GameData {
		
		//-----------------------------------------------------------------------------
		// Sound Groups
		//-----------------------------------------------------------------------------

		// TODO: Sound Groups


		//-----------------------------------------------------------------------------
		// Sound Effects
		//-----------------------------------------------------------------------------
		
		// Effects --------------------------------------------------------------------

		public static Sound SOUND_APPEAR_VANISH;
		public static Sound SOUND_EFFECT_CLING;
		public static Sound SOUND_ELECTROCUTE;
		public static Sound SOUND_FIRE;
		public static Sound SOUND_GALE_SEED;
		public static Sound SOUND_LEAVES;
		public static Sound SOUND_MYSTERY_SEED;
		public static Sound SOUND_OBJECT_FALL;
		public static Sound SOUND_ROCK_SHATTER;
		public static Sound SOUND_SCENT_SEED;
		public static Sound SOUND_SCENT_SEED_POD;

		// Items ----------------------------------------------------------------------

		public static Sound SOUND_BIGGORON_SWORD;
		public static Sound SOUND_BOMB_BOUNCE;
		public static Sound SOUND_BOMB_EXPLODE;
		public static Sound SOUND_BOOMERANG_LOOP;
		public static Sound SOUND_FIRE_ROD;
		public static Sound SOUND_KEY_BOUNCE;
		public static Sound SOUND_SEED_SHOOTER;
		public static Sound SOUND_SHIELD;
		public static Sound SOUND_SHIELD_DEFLECT;
		public static Sound SOUND_SHOOT_PROJECTILE;
		public static Sound SOUND_SHOVEL;
		public static Sound SOUND_SWITCH_HOOK_LOOP;
		public static Sound SOUND_SWITCH_HOOK_SWITCH;
		public static Sound SOUND_SWORD_BEAM;
		public static Sound SOUND_SWORD_CHARGE;
		public static Sound SOUND_SWORD_SLASH_1;
		public static Sound SOUND_SWORD_SLASH_2;
		public static Sound SOUND_SWORD_SLASH_3;
		public static Sound SOUND_SWORD_SPIN;

		// Misc -----------------------------------------------------------------------

		public static Sound SOUND_DUNGEON_SIGNAL;
		public static Sound SOUND_FANFARE_ITEM;
		public static Sound SOUND_FANFARE_ESSENCE;
		public static Sound SOUND_LOW_HEALTH_LOOP;
		public static Sound SOUND_MINECART_LOOP;
		public static Sound SOUND_ROOM_EXIT;
		public static Sound SOUND_SECRET;
		public static Sound SOUND_SECRET_LONG;

		// Monster --------------------------------------------------------------------

		public static Sound SOUND_MONSTER_DIE;
		public static Sound SOUND_MONSTER_HURT;
		public static Sound SOUND_MONSTER_JUMP;

		// Pickups --------------------------------------------------------------------

		public static Sound SOUND_GET_HEART;
		public static Sound SOUND_GET_ITEM;
		public static Sound SOUND_GET_RUPEE;
		public static Sound SOUND_GET_RUPEE_LOOP;
		public static Sound SOUND_HEART_CONTAINER;

		// Player ---------------------------------------------------------------------

		public static Sound SOUND_PLAYER_FALL;
		public static Sound SOUND_PLAYER_HURT;
		public static Sound SOUND_PLAYER_JUMP;
		public static Sound SOUND_PLAYER_LAND;
		public static Sound SOUND_PLAYER_PICKUP;
		public static Sound SOUND_PLAYER_SWIM;
		public static Sound SOUND_PLAYER_THROW;
		public static Sound SOUND_PLAYER_WADE;

		// Projectiles ----------------------------------------------------------------

		public static Sound SOUND_LASER;

		// Tiles ----------------------------------------------------------------------

		public static Sound SOUND_BARRIER;
		public static Sound SOUND_BLOCK_PUSH;
		public static Sound SOUND_BLUE_ROLLER;
		public static Sound SOUND_CHEST_OPEN;
		public static Sound SOUND_CROSSING_GATE;
		public static Sound SOUND_DUNGEON_DOOR;
		public static Sound SOUND_FLOOR_CRUMBLE;
		public static Sound SOUND_SWITCH;
		public static Sound SOUND_TURNSTILE;

		// UI -------------------------------------------------------------------------

		public static Sound SOUND_MENU_OPEN;
		public static Sound SOUND_MENU_CLOSE;
		public static Sound SOUND_MENU_NEXT;
		public static Sound SOUND_MENU_CURSOR_MOVE;
		public static Sound SOUND_MENU_SELECT;
		public static Sound SOUND_TEXT_CONTINUE;
		public static Sound SOUND_TEXT_LETTER;


		//-----------------------------------------------------------------------------
		// Sound Loading
		//-----------------------------------------------------------------------------
		
		// Loads the sound effects.
		private static void LoadSounds() {
			Resources.LoadSounds(Resources.SoundDirectory + "sounds.conscript");
			IntegrateResources<Sound>("SOUND_");
		}

		// Loads the music.
		private static void LoadMusic() {
			Resources.LoadMusic(Resources.MusicDirectory + "music.conscript");
		}
	}
}
