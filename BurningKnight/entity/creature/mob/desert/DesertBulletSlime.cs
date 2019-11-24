using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.castle;
using BurningKnight.entity.projectile;
using Lens.graphics;
using Lens.util;
using Lens.util.math;
using Lens.util.timer;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.desert {
	public class DesertBulletSlime : BulletSlime {
		private static readonly Color color = ColorUtils.FromHex("#ffeb57");
		
		protected override Color GetBloodColor() {
			return color;
		}

		protected override void SetStats() {
			base.SetStats();
			SetMaxHp(8);
		}

		protected override void OnJump() {
			base.OnJump();

			for (var i = 0; i < 3; i++) {
				Timer.Add(() => {
					if (Target == null) {
						return;
					}

					var a = AngleTo(Target) + Rnd.Float(-0.1f, 0.1f);
					var projectile = Projectile.Make(this, "small", a, 9f);

					projectile.Spectral = true;
					projectile.Center = Center + MathUtils.CreateVector(a, 5f) - new Vector2(0, GetComponent<ZComponent>().Z);
					projectile.AddLight(32f, Projectile.RedLight);
				}, i * 0.3f);
			}
		}

		protected override string GetSprite() {
			return "desert_bullet_slime";
		}
	}
}