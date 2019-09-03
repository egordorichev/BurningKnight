using BurningKnight.entity.component;
using BurningKnight.entity.projectile;
using BurningKnight.entity.projectile.pattern;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.orbital {
	public class SkullOrbital : Mob {
		private const float FireRate = 3f;
		private float t;

		protected override void HandleBreaking() {
			
		}

		public override void AddComponents() {
			base.AddComponents();

			AddAnimation("skull_orbital");

			Width = 14;
			Height = 23;
			
			SetMaxHp(3);
			AddComponent(new RectBodyComponent(0, 0, Width, Height));
			
			AddComponent(new OrbitalComponent {
				Radius = 32
			});
			
			RemoveTag(Tags.LevelSave);
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Target == null) {
				return;
			}
			
			t += dt;
			
			if (t >= FireRate) {
				t = 0;
				
				var pp = new ProjectilePattern(CircleProjectilePattern.Make(8f, 5)) {
					Position = Center
				};

				for (var j = 0; j < 5; j++) {
					var b = Projectile.Make(this, "small");
					pp.Add(b);
					b.AddLight(32f, Color.Red);
				}
				
				pp.Launch(AngleTo(Target), 30);
				Area.Add(pp);
			}
		}
	}
}