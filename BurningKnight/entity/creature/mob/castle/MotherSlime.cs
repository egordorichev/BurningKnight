using BurningKnight.entity.component;
using BurningKnight.entity.events;
using Lens.entity;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.castle {
	public class MotherSlime : SimpleSlime {
		protected override void SetStats() {
			base.SetStats();
			
			// todo: different sprite
			AddComponent(new ZAnimationComponent("slime"));
			SetMaxHp(3);

			var body = new RectBodyComponent(2, 7, 12, 9);
			AddComponent(body);

			body.Body.LinearDamping = 2;
			body.KnockbackModifier = 0.5f;
		}

		public override bool HandleEvent(Event e) {
			if (e is DiedEvent) {
				for (var i = 0; i < Random.Int(3, 4); i++) {
					var slime = new SimpleSlime();
					Area.Add(slime);
					slime.Center = Center + new Vector2(Random.Float(-4, 4), Random.Float(-4, 4));
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}