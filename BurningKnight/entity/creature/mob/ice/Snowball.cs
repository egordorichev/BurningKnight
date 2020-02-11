using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.prefabs;
using Lens.graphics;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.ice {
	public class Snowball : Slime {
		private static readonly Color color = ColorUtils.FromHex("#92a1b9");
		
		protected override Color GetBloodColor() {
			return color;
		}

		protected override float GetJumpDelay() {
			return Rnd.Float(0.6f, 1f);
		}

		protected override float GetJumpAngle() {
			return Target == null || Rnd.Chance(80) ? Rnd.AnglePI() : AngleTo(Target) + Rnd.Float(-0.1f, 0.1f);
		}

		protected override void SetStats() {
			base.SetStats();
			
			AddComponent(new ZAnimationComponent("snowball") {
				ShadowOffset = 2
			});

			SetMaxHp(4);

			var body = CreateBodyComponent();
			AddComponent(body);

			body.Body.LinearDamping = 2;
			body.KnockbackModifier = 0.7f;
			
			AddComponent(CreateSensorBodyComponent());

			JumpForce = 40;
			ZVelocity = 3;

			GetComponent<RectBodyComponent>().Body.LinearDamping = 1;
		}

		protected virtual BodyComponent CreateBodyComponent() {
			return new RectBodyComponent(1, 11, 11, 1);
		}

		protected virtual BodyComponent CreateSensorBodyComponent() {
			return new SensorBodyComponent(1, 2, 11, 10);
		}
	}
}