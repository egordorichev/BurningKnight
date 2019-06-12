using System;
using BurningKnight.entity.creature.bk.attacks;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.level.biome;
using BurningKnight.state;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.creature.bk {
	public static class BurningKnightAttackRegistry {
		public static BossPatternRegistry<BurningKnight> PatternRegistry = new BossPatternRegistry<BurningKnight>();
		public static BossPatternSetRegistry<BurningKnight> PatternSetRegistry = new BossPatternSetRegistry<BurningKnight>();

		public static Type[] Attacks = {
			typeof(AutoSkullAttack),
			typeof(ArrowAttack),
			typeof(FourArrowAttack),
			typeof(HugeSplittingBulletAttack),
			typeof(ShootAttack),
			typeof(SpinningHellAttack)
		};

		public static BossAttack<BurningKnight> GetNext(BossPatternSet<BurningKnight> p) {
			if (Random.Chance(100f / (PatternRegistry.Count + 1))) {
				return (BossAttack<BurningKnight>) Activator.CreateInstance(Attacks[Random.Int(Attacks.Length)]);
			}

			return p.GetNext();
		}
		
		static BurningKnightAttackRegistry() {
			PatternRegistry.Register("bk_0", new BossPattern<BurningKnight>(
				typeof(AutoSkullAttack),
				typeof(ArrowAttack),
				typeof(ShootAttack)
			));
			
			PatternRegistry.Register("bk_1", new BossPattern<BurningKnight>(
				typeof(HugeSplittingBulletAttack),
				typeof(AutoSkullAttack),
				typeof(ShootAttack)
			));
			
			PatternRegistry.Register("bk_2", new BossPattern<BurningKnight>(
				typeof(SpinningHellAttack),
				typeof(HugeSplittingBulletAttack),
				typeof(ArrowAttack)
			));
			
			PatternSetRegistry.Register(new BossPatternSet<BurningKnight>(
				PatternRegistry, "bk_0", "bk_1", "bk_2"
			), 1f, Biome.Castle);
		}
	}
}