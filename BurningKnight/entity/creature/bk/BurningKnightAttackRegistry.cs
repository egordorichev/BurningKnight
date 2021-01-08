using System;
using BurningKnight.entity.creature.bk.attacks;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.level.biome;
using BurningKnight.state;
using Lens.util.math;

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
			typeof(BulletRingAttack)
		};

		public static BossAttack<BurningKnight> GetNext(BossPatternSet<BurningKnight> p) {
			if (Rnd.Chance(100f / (PatternRegistry.Count + 1))) {
				return (BossAttack<BurningKnight>) Activator.CreateInstance(Attacks[Rnd.Int(Attacks.Length)]);
			}

			return p.GetNext();
		}
		
		static BurningKnightAttackRegistry() {
			PatternRegistry.Register("bounce_0", new BossPattern<BurningKnight>(
				typeof(ArrowAttack),
				typeof(ShootAttack)
			));
			
			PatternRegistry.Register("bounce_1", new BossPattern<BurningKnight>(
				typeof(FourArrowAttack),
				typeof(ShootAttack)
			));
			
			PatternRegistry.Register("bounce_2", new BossPattern<BurningKnight>(
				typeof(ArrowAttack)
			));
			
			PatternRegistry.Register("spam_0", new BossPattern<BurningKnight>(
				typeof(HugeSplittingBulletAttack),
				typeof(AutoSkullAttack)
			));
			
			PatternRegistry.Register("spam_1", new BossPattern<BurningKnight>(
				typeof(BulletRingAttack),
				typeof(FourArrowAttack)
			));
			
			PatternRegistry.Register("spam_2", new BossPattern<BurningKnight>(
				typeof(BulletRingAttack),
				typeof(AutoSkullAttack)
			));
			
			PatternRegistry.Register("test", new BossPattern<BurningKnight>(
				typeof(SpawnSkullOrbitalAttack),
				typeof(BulletRingAttack),
				typeof(AutoSkullAttack)
			));
			
			PatternSetRegistry.Register(new BossPatternSet<BurningKnight>(
				PatternRegistry, "bounce_0", "bounce_1", "bounce_2"
			), 1f, Biome.Castle);
		}
	}
}