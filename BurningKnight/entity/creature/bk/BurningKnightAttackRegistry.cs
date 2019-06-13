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
			typeof(SpinningHellAttack),
			typeof(MissileAttack)
		};

		public static BossAttack<BurningKnight> GetNext(BossPatternSet<BurningKnight> p) {
			if (Random.Chance(100f / (PatternRegistry.Count + 1))) {
				// fixme: enable
				// return (BossAttack<BurningKnight>) Activator.CreateInstance(Attacks[Random.Int(Attacks.Length)]);
			}

			return p.GetNext();
		}
		
		static BurningKnightAttackRegistry() {
			PatternRegistry.Register("bounce_0", new BossPattern<BurningKnight>(
				typeof(ArrowAttack),
				typeof(ShootAttack),
				typeof(MissileAttack)
			));
			
			PatternRegistry.Register("bounce_1", new BossPattern<BurningKnight>(
	typeof(FourArrowAttack),
				typeof(ShootAttack)
			));
			
			PatternRegistry.Register("bounce_2", new BossPattern<BurningKnight>(
				typeof(MissileAttack),
				typeof(ArrowAttack)
			));
			
			PatternRegistry.Register("spam_0", new BossPattern<BurningKnight>(
				typeof(MissileAttack),
				typeof(HugeSplittingBulletAttack),
				typeof(AutoSkullAttack)
			));
			
			PatternRegistry.Register("test", new BossPattern<BurningKnight>(
				typeof(HugeSplittingBulletAttack)
			));
			
			PatternSetRegistry.Register(new BossPatternSet<BurningKnight>(
				PatternRegistry, "test" //"bounce_0", "bounce_1", "bounce_2"
			), 1f, Biome.Castle);
		}
	}
}