using System;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using BurningKnight.level;
using Lens.entity;
using Lens.util;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.creature.mob.summons {
	public class KingBlade : Mob {
		private float t;
		private bool stuck;
		private bool moving;
		private float angle;
		
		public override void AddComponents() {
			base.AddComponents();

			var s = new ScalableSliceComponent("items", "bk:burning_king_sword") {
				ShadowZ = 2
			};
			
			AddComponent(s);
			s.AddShadow();

			AlwaysActive = true;
			AlwaysVisible = true;
			
			AddComponent(new RectBodyComponent(0, 0, 11, 26, center : true) {
				KnockbackModifier = 0
			});
			
			SetMaxHp(5);
			
			AddComponent(new OrbitalComponent {
				Radius = 32
			});
			
			RemoveTag(Tags.LevelSave);

			s.Scale.X = 0;
			s.Scale.Y = 0;
			
			Tween.To(1, s.Scale.X, x => s.Scale.X = x, 0.4f, Ease.QuadOut);
			Tween.To(1, s.Scale.Y, x => s.Scale.Y = x, 0.4f, Ease.QuadOut);
			
			Height = 26;
			// Width = 11;
		}

		private bool spawned;
		private bool launched;
		private float tt;
		
		public override void Update(float dt) {
			base.Update(dt);

			if (stuck) {
				if (!spawned) {
					spawned = true;
					var v =  MathUtils.CreateVector(angle, 13);
					
					for (var i = 0; i < 16; i++) {
						var aan = (float) Math.PI * i / 8f;
						var p = Projectile.Make(this, "small_triangle", aan, 5, false, scale : Random.Float(0.5f, 1f));

						p.Center += v;
						p.AddLight(32f, Color.Red);
					}
				}
				
				t += dt;

				if (t > 3f && Target != null) {
					var an = AngleTo(Target);
					var s = GetComponent<ScalableSliceComponent>();
					
					Tween.To(2f, s.Scale.X, x => s.Scale.X = x, 0.3f).OnEnd = () => {
						s.Scale.X = 1;
					};

					Tween.To(0.5f, s.Scale.Y, x => s.Scale.Y = x, 0.3f).OnEnd = () => {
						s.Scale.X = 1;

						var p = Projectile.Make(this, "triangle", an, 10, false);
						p.Center += MathUtils.CreateVector(angle, 13);
					};
					
					t = 0;
				}
				
				return;
			}
			
			if (moving) {
				t += dt;
				tt += dt;

				if (tt > 0.1f) {
					tt = 0;
					
					var p = Projectile.Make(this, "triangle", angle + Random.Float(-0.8f, 0.8f), 10, false, scale: Random.Float(0.5f, 1f));
					p.Center += MathUtils.CreateVector(angle, 13);
				}

				Position += GetComponent<RectBodyComponent>().Velocity * (dt * 0.2f);
				
				if (!launched) {
					if (t < 0.5f) {
						return;
					}
					
					GetComponent<RectBodyComponent>().Velocity = MathUtils.CreateVector(angle + Math.PI, (t - 0.5f) * 128f);

					if (t >= 1.5f) {
						launched = true;
						t = 0;
					}
				}
			}
			
			if (Target == null) {
				return;
			}

			var a = AngleTo(Target);
			var aa = a + (float) Math.PI / 2f;
			
			GetComponent<ScalableSliceComponent>().Angle = aa;
			GetComponent<RectBodyComponent>().Angle = aa;

			t += dt;
			
			if (t > 5) {
				t = 0;
				moving = true;

				GetComponent<OrbitalComponent>().Orbiting.GetComponent<OrbitGiverComponent>().RemoveOrbiter(this);
				angle = a - (float) Math.PI;
			}
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent cse) {
				var en = cse.Entity;

				if (moving && (en is Level)) {
					stuck = true;
					GetComponent<RectBodyComponent>().Velocity = Vector2.Zero;
				}
			}
			
			return base.HandleEvent(e);
		}

		public override bool ShouldCollide(Entity entity) {
			return (entity is Level);
		}
	}
}