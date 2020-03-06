using BurningKnight.entity.component;
using BurningKnight.entity.projectile;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.jungle {
	public class ManShooter : ManEater {
		protected override string GetAnimation() {
			return "manshooter";
		}

		private float last;

		public override void Init() {
			base.Init();
			last = Rnd.Float(2);
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Target == null || !OnScreen) {
				return;
			}
			
			last -= dt;

			if (last <= 0) {
				last = 2f;

				GetComponent<AudioEmitterComponent>().EmitRandomized("mob_fire_static");
				
				var a = GetComponent<MobAnimationComponent>().Angle;
				var p = Projectile.Make(this, "circle", a, 8f);

				p.Color = Color.Green;
				p.BreaksFromWalls = false;
				p.Spectral = true;
				p.Depth = Layers.FlyingMob;

				for (var i = 0; i < 6; i++) {
					Projectile.Make(this, "small", a + Rnd.Float(-1f, 1f), Rnd.Float(4, 16f), scale: Rnd.Float(0.5f, 1f));
				}
			}
		}
	}
}