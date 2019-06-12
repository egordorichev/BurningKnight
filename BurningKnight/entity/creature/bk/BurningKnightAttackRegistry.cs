using BurningKnight.entity.creature.bk.attacks;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.level.biome;

namespace BurningKnight.entity.creature.bk {
	public static class BurningKnightAttackRegistry {
		public static BossPatternRegistry<BurningKnight> PatternRegistry = new BossPatternRegistry<BurningKnight>();
		public static BossPatternSetRegistry<BurningKnight> PatternSetRegistry = new BossPatternSetRegistry<BurningKnight>();

		static BurningKnightAttackRegistry() {
			PatternRegistry.Register("bk_0", new BossPattern<BurningKnight>(
				typeof(AutoSkullAttack)//,
				// typeof(ArrowAttack)
			));
			
			PatternSetRegistry.Register(new BossPatternSet<BurningKnight>(
				PatternRegistry, "bk_0"
			), 1f, Biome.Castle);
		}
	}
}