using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.prefabs;
using BurningKnight.entity.projectile;
using BurningKnight.state;
using Lens.graphics;
using Lens.util;
using Color = Microsoft.Xna.Framework.Color;

namespace BurningKnight.entity.creature.mob.desert {
	public class DesertSlime : Slime {
		private static readonly Color color = ColorUtils.FromHex("#ffeb57");
		
		protected override Color GetBloodColor() {
			return color;
		}
		
		protected override void SetStats() {
			base.SetStats();
			
			AddComponent(new ZAnimationComponent("desert_slime"));
			SetMaxHp(1 + (int) Math.Round(Run.Depth * 1.5f));

			var body = new RectBodyComponent(2, 12, 12, 1);
			AddComponent(body);

			body.Body.LinearDamping = 2;
			body.KnockbackModifier = 0.5f;
			
			AddComponent(new SensorBodyComponent(2, 7, 12, 9));
		}
		
		protected override void OnLand() {
			base.OnLand();

			if (Target == null) {
				return;
			}

			var a = AngleTo(Target);
			var projectile = Projectile.Make(this, "small", a, 5f);

			projectile.Center = Center + MathUtils.CreateVector(a, 5f);
			projectile.AddLight(32f, Projectile.RedLight);
			
			GetComponent<RectBodyComponent>().KnockbackFrom(a - (float) Math.PI, 0.3f);
		}
	}
}