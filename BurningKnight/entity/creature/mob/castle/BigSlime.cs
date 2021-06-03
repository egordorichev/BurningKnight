using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.prefabs;
using BurningKnight.entity.events;
using Lens.util.math;

namespace BurningKnight.entity.creature.mob.castle {
	public class BigSlime : Slime {
		protected override float GetJumpDelay() {
			return 1 + Rnd.Float(0.7f, 1.6f);
		}

		protected override void SetStats() {
			base.SetStats();
			
			AddComponent(new ZAnimationComponent(GetSprite()));
			SetMaxHp(4);

			var body = new RectBodyComponent(1, 15, 14, 1);
			AddComponent(body);

			body.Body.LinearDamping = 2;
			body.KnockbackModifier = 0.5f;
			
			AddComponent(new SensorBodyComponent(2, 7, 12, 9));
		}

		protected virtual string GetSprite() {
			return Events.Halloween ? "spooky_big_slime" : "big_slime";
		}

		protected override bool HandleDeath(DiedEvent d) {
			if (Rnd.Chance(30)) {
				var slime = new SimpleSlime();
				Area.Add(slime);
				slime.BottomCenter = BottomCenter;
			}
			
			return base.HandleDeath(d);
		}
	}
}