using System;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.projectile;
using Lens.util;

namespace BurningKnight.entity.creature.bk.forms.king {
	public class JumpAttack : BossAttack<BurningKing> {
		public override void Update(float dt) {
			base.Update(dt);

			if (T > 3f) {
				for (var i = 0; i < 16; i++) {
					var a = (float) Math.PI * i / 8f;
					var p = Projectile.Make(Self, "small", a, 10);

					p.Center += MathUtils.CreateVector(a, 8f);
				}
				
				Self.SelectAttack();
			}
		}
	}
}