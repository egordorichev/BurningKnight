using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.prefabs;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using Lens.graphics;
using Lens.util.math;
using Microsoft.Xna.Framework;

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

		protected override void OnLand() {
			base.OnLand();

			if (Target == null) {
				return;
			}

			// DoSpit();
		}

		protected virtual void DoSpit() {
			var am = 8;
			GetComponent<AudioEmitterComponent>().EmitRandomized("mob_fire");

			for (var i = 0; i < am; i++) {
				var a = Math.PI * 2 * (((float) i) / am);
				var projectile = Projectile.Make(this, "small", a, 5f);
					
				projectile.Center = BottomCenter;
				projectile.AddLight(32f, Projectile.RedLight);
			}
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