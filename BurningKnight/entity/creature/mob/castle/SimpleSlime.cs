using BurningKnight.assets.lighting;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.prefabs;
using BurningKnight.state;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.castle {
	public class SimpleSlime : Slime {
		private static readonly Color color = ColorUtils.FromHex("#33984b");
		
		protected override Color GetBloodColor() {
			return color;
		}
		
		protected override void SetStats() {
			base.SetStats();
			
			AddComponent(new ZAnimationComponent(Events.Halloween ? "spooky_slime" : "slime"));
			SetMaxHp(1 + Run.Depth);

			var body = CreateBodyComponent();
			AddComponent(body);

			body.Body.LinearDamping = 2;
			body.KnockbackModifier = 0.5f;
			
			AddComponent(CreateSensorBodyComponent());
			AddComponent(new LightComponent(this, 24, new Color(0.5f, 1f, 0.4f)));
		}

		protected virtual BodyComponent CreateBodyComponent() {
			return new RectBodyComponent(2, 12, 10, 1);
		}

		protected virtual BodyComponent CreateSensorBodyComponent() {
			return new SensorBodyComponent(2, 5, 12, 9);
		}
	}
}