using System;
using BurningKnight.entity.component;
using BurningKnight.entity.projectile;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.entity.creature.pet {
	public class Shooty : DiagonalPet {
		public override void PostInit() {
			AddGraphics("shooty", false);
			base.PostInit();
			t = Rnd.Float(2f);
		}

		private float t;

		public override void Update(float dt) {
			base.Update(dt);
			t += dt;

			if (t >= 2f) {
				t = 0;

				var r = GetComponent<RoomComponent>().Room;

				if (r == null) {
					return;
				}

				if (r.Tagged[Tags.MustBeKilled].Count > 0) {
					var o = Owner;
						
					GetComponent<AudioEmitterComponent>().EmitRandomizedPrefixed("item_meatguy", 4, 0.5f);

					var builder = new ProjectileBuilder(o, "small") {
						Color = ProjectileColor.Yellow,
						Range = 32f
					};

					builder.AddFlags(ProjectileFlags.FlyOverStones);
					
					for (var i = 0; i < 4; i++) {
						var a = i * Math.PI * 0.5f;
						var projectile = builder.Shoot(a, 4f).Build();

						projectile.Center = Center + MathUtils.CreateVector(a, 5f);
						projectile.Owner = this;
					}
					
					GetComponent<AnimationComponent>().Animate();
				}
			}
		}
	}
}