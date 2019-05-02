using BurningKnight.entity.component;
using BurningKnight.entity.events;
using Lens.entity;
using Lens.graphics;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.creature.mob.castle {
	public class MotherSlime : SimpleSlime {
		private static readonly Color color = ColorUtils.FromHex("#0069aa");
		
		protected override Color GetBloodColor() {
			return color;
		}
		
		protected override void SetStats() {
			base.SetStats();
			
			AddComponent(new ZAnimationComponent("mother_slime"));
			SetMaxHp(1);

			var body = new RectBodyComponent(2, 7, 12, 9);
			AddComponent(body);

			body.Body.LinearDamping = 2;
			body.KnockbackModifier = 0.5f;
		}

		public override bool HandleEvent(Event e) {
			if (e is DiedEvent ev) {
				for (var i = 0; i < 2; i++) {
					var slime = new BabySlime();
					Area.Add(slime);
					slime.Center = Center + new Vector2(Random.Float(-4, 4), Random.Float(-4, 4));
					slime.GetComponent<HealthComponent>().InvincibilityTimer = 0.1f;

					slime.GetAnyComponent<BodyComponent>().KnockbackFrom(ev.From, 2f, 1);
				}
			}
			
			return base.HandleEvent(e);
		}

		public override float GetSpawnChance() {
			return 0.25f;
		}

		public override float GetWeight() {
			return 2;
		}
	}
}