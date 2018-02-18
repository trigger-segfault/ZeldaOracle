using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Entities {

	public enum DepthLayer {
		None = 0,

		Shadows,

		TileLayer1,
		TileLayer2,
		TileLayer3,

		Collectibles,
		PlayerSubmerged,

		EffectSplash,
		EffectDirt,
		EffectTileBreak,
		EffectScentSeedPod,
		Traps,
		EffectSeed,
		EffectGale,
		
		// Used by crossing gate.
		// Draws above player/monsters when player is above and vice-versa.
		DynamicDepthBelowTileLayer2,
		DynamicDepthBelowTileLayer3,
		DynamicDepthBelowEntity,

		Monsters,

		EffectMonsterExplosion,
		EffectFire,
		EffectFallingObject,
		EffectOwlSparkles,
		ProjectileBoomerang,
		ProjectileBombchu,
		ProjectileArrow,
		ProjectileRodFire,
		ProjectileSwitchHook,

		PlayerSwingItem,
		PlayerAndNPCs,
		// Used by crossing gate.
		// Draws above player/monsters when player is above and vice-versa.
		DynamicDepthAboveTileLayer2,
		DynamicDepthAboveTileLayer3,
		DynamicDepthAboveEntity,
		RisingTile,
		
		InAirSeed,
		InAirMonsters,
		InAirPlayer,
		InAirCollectibles,
		
		ProjectileSwordBeam,
		ProjectileBeam,
		ProjectileBomb,
		EffectMagnetGloves,
		EffectSprintDustParticle,
		EffectCrackedFloorCrumble,
		EffectSomariaBlockPoof,
		ProjectileCarriedTile,
		ItemSeedShooter,

		EffectCling,
		EffectMonsterBurnFlame,
		EffectPegasusDust,
		EffectBombExplosion,

		TileAbove,

		Count,
	}
}
