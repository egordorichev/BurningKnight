using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.entity.orbital {
	public class Marshmallow : AnimatedOrbital {
		private float timer;
		
		public Marshmallow() : base("marshmallow") {
			timer = Rnd.Float(1);
		}

		public override void Update(float dt) {
			base.Update(dt);
			
			timer += dt;

			if (timer >= 1f) {
				timer = 0;

				if ((Owner.GetComponent<RoomComponent>().Room?.Tagged[Tags.MustBeKilled].Count ?? 0) == 0) {
					return;
				}
				
				Owner.GetComponent<AudioEmitterComponent>().EmitRandomizedPrefixed("item_meatguy", 4, 0.5f);

				var a = GetComponent<OrbitalComponent>().CurrentAngle;
				var projectile = Projectile.Make(Owner, "small", a, 6f);

				projectile.Color = ProjectileColor.White;
				projectile.Center = Center + MathUtils.CreateVector(a, 5f);
				projectile.AddLight(32f, ProjectileColor.White);

				Owner.HandleEvent(new ProjectileCreatedEvent {
					Projectile = projectile,
					Owner = Owner
				});
				
				projectile.Owner = this;
				GetComponent<AnimationComponent>().Animate();
			}
		}
	}
}