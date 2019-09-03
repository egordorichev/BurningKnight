using System;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using BurningKnight.level;
using Lens.entity;
using Lens.util;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.summons {
	public class KingBlade : Mob {
		private float t;
		private bool stuck;
		private bool moving;
		
		public override void AddComponents() {
			base.AddComponents();

			var s = new SliceComponent("items", "bk:burning_king_sword") {
				ShadowZ = 2
			};
			
			AddComponent(s);
			s.AddShadow();

			// Width = 11;
			// Height = 26;

			AlwaysActive = true;
			AlwaysVisible = true;
			
			AddComponent(new RectBodyComponent(0, 0, Width, Height, center : true));
			SetMaxHp(5);
			
			AddComponent(new OrbitalComponent {
				Radius = 32
			});
			
			RemoveTag(Tags.LevelSave);
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (stuck) {
				t += dt;

				if (t > 1f && Target != null) {
					var an = AngleTo(Target);
					var p = Projectile.Make(this, "triangle", an, 10, false);
					// fixme: use own angle, not to target
					p.Center += MathUtils.CreateVector(an, 8f);

					t = 0;
				}
				
				return;
			} else if (moving) {
				return;
			} else 
			
			if (Target == null) {
				return;
			}

			var a = AngleTo(Target);
			var aa = a + (float) Math.PI / 2f;
			
			GetComponent<SliceComponent>().Angle = aa;
			GetComponent<RectBodyComponent>().Angle = aa;

			t += dt;
			
			if (t > 5) {
				t = 0;
				moving = true;
				
				GetComponent<OrbitalComponent>().Orbiting.GetComponent<OrbitGiverComponent>().RemoveOrbiter(this);
				GetComponent<RectBodyComponent>().Velocity = MathUtils.CreateVector(a, 64f);
			}
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent cse) {
				var en = cse.Entity;

				if (moving && (en is Level || en is DestroyableLevel)) {
					stuck = true;
					GetComponent<RectBodyComponent>().Velocity = Vector2.Zero;
				}
			}
			
			return base.HandleEvent(e);
		}

		public override bool ShouldCollide(Entity entity) {
			return (entity is Level || entity is DestroyableLevel);
		}
	}
}