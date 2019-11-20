using BurningKnight.assets.lighting;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.castle;
using BurningKnight.entity.events;
using Lens.graphics;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.desert {
	public class MegaSlime : SimpleSlime {
		private static readonly Color color = ColorUtils.FromHex("#0069aa");
		public static Color LightColor = new Color(0.5f, 0.5f, 1f, 1f);
		
		protected override Color GetBloodColor() {
			return color;
		}
		
		protected override void SetStats() {
			base.SetStats();
			
			AddComponent(new ZAnimationComponent("mega_slime"));
			SetMaxHp(5);

			AddComponent(new LightComponent(this, 64, LightColor));
		}

		protected override BodyComponent CreateBodyComponent() {
			return new RectBodyComponent(2, 7, 12, 9);
		}

		protected override BodyComponent CreateSensorBodyComponent() {
			return new SensorBodyComponent(2, 7, 12, 9);
		}

		protected override bool HandleDeath(DiedEvent d) {
			for (var i = 0; i < 2; i++) {
				var slime = new MotherSlime();
				Area.Add(slime);
				slime.Center = Center + new Vector2(Rnd.Float(-4, 4), Rnd.Float(-4, 4));
				slime.GetComponent<HealthComponent>().InvincibilityTimer = 0.1f;

				slime.GetAnyComponent<BodyComponent>().KnockbackFrom(d.From, Rnd.Float(1f, 2f), 2);
			}
			
			return base.HandleDeath(d);
		}
	}
}