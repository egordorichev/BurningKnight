using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.creature.mob.orbital;
using BurningKnight.entity.creature.mob.summons;
using Lens.util.math;

namespace BurningKnight.entity.creature.bk.forms.king {
	public class BladeAttack : BossAttack<BurningKing> {
		// Summons 3-4 blades around him (mobs), then raises his hand and sends them flying into you
		// Blades on death get stuck in the walls, and start firing bullets at you, until you kill em

		private int count;

		public override void Init() {
			base.Init();
			count = Rnd.Int(3, 5);
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (T > 2f && count > 0) {
				T = 0;
				count--;

				var blade = new KingBlade();
				Self.Area.Add(blade);
				
				Self.GetComponent<OrbitGiverComponent>().AddOrbiter(blade);
			}
		}
	}
}