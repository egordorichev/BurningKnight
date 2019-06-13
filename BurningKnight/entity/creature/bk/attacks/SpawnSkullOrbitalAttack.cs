using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.creature.mob.orbital;

namespace BurningKnight.entity.creature.bk.attacks {
	public class SpawnSkullOrbitalAttack : BossAttack<BurningKnight> {
		private const float Delay = 3;
		
		public override void Init() {
			base.Init();

			var skull = new SkullOrbital();

			Self.Area.Add(skull);
			Self.GetComponent<OrbitGiverComponent>().AddOrbiter(skull);
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (T >= Delay) {
				Self.SelectAttack();
			}
		}
	}
}