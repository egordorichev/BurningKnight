using System;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.projectile;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.bk.attacks {
	public class FourArrowAttack : BossAttack<BurningKnight> {
		private const float Delay = 3f;
		
		public override void Init() {
			base.Init();

			for (var i = 0; i < 4; i++) {
				ProjectileTemplate.Make(Self, "green_small", Self.Center, 
					i * (float) Math.PI * 0.5f, 5, 2, 0,

					p => p.AddLight(16f, ProjectileColor.Green),
					
					"  x ",
					"xxxx",
					"  x ");
			}
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (T >= Delay) {
				Self.SelectAttack();
			}
		}
	}
}