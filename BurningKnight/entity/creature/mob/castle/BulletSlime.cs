using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.prefabs;
using BurningKnight.entity.projectile;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.castle {
	public class BulletSlime : Slime {
		private static readonly Color color = ColorUtils.FromHex("#ff0000");
		
		protected override Color GetColor() {
			return color;
		}
		
		protected override void SetStats() {
			base.SetStats();
			
			AddComponent(new ZAnimationComponent("bullet_slime"));
			SetMaxHp(1);

			var body = new RectBodyComponent(2, 7, 12, 9);
			AddComponent(body);

			body.Body.LinearDamping = 2;
			body.KnockbackModifier = 0.5f;
		}

		protected override void OnLand() {
			base.OnLand();

			if (Target == null) {
				return;
			}
			
			var am = 8;
			
			for (var i = 0; i < am; i++) {
				var a = Math.PI * 2 * (((float) i) / am);
				var projectile = Projectile.Make(this, "small", a, (float) (Math.Abs(Math.Cos(a)) + Math.Abs(Math.Sin(a))) * 20f);
					
				projectile.Range = 1f;
				projectile.AddLight(32f, Color.Red);
			}
		}

		public override float GetSpawnChance() {
			return 0.5f;
		}

		public override float GetWeight() {
			return 2;
		}
	}
}