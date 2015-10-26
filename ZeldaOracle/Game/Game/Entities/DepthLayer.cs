using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Entities {

	public enum DepthLayer {
		None = 0,

		Collectibles,
		PlayerSubmerged,

		EffectSplash,
		EffectDirt,
		EffectTileBreak,
		EffectScentSeedPod,
		EffectSeed,
		EffectGale,
		EffectSomariaBlockPoof,

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
		RisingTile,
		
		InAirSeed,
		InAirMonsters,
		InAirPlayer,
		InAirCollectibles,
		
		ProjectileSwordBeam,
		ProjectileBomb,
		EffectSprintPuff,
		ProjectileCarriedTile,
		ItemSeedShooter,

		EffectCling,
		EffectMonsterBurnFlame,
		EffectPegasusDust,
		EffectBombExplosion,

		Count,
	}
}
