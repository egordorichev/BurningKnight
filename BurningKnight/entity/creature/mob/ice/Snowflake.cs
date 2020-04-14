using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.creature.mob.prefabs;
using BurningKnight.level;
using Lens.entity;
using Lens.util;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.ice {
	public class Snowflake : Mob {
		protected const float DefaultZ = 2;
		
		protected override Color GetBloodColor() {
			return Snowball.BloodColor;
		}
		
		protected override void SetStats() {
			base.SetStats();

			SetMaxHp(6);
			GetComponent<HealthComponent>().Unhittable = true;
			
			var body = new SensorBodyComponent(1, 1, 14, 15);
			AddComponent(body);
			body.Body.LinearDamping = 0.3f;
			
			AddComponent(new ZAnimationComponent("snowflake"));
			AddComponent(new ZComponent() {
				Float = true
			});
			
			AddComponent(new OrbitalComponent {
				Radius = 32,
				Lerp = true
			});

			Depth = Layers.Wall;
			Become<IdleState>();
			
			RemoveTag(Tags.MustBeKilled);
		}

		public override bool InAir() {
			return true;
		}

		public override void Update(float dt) {
			base.Update(dt);
			var rm = GetComponent<RoomComponent>().Room;

			if (rm == null || rm.Tagged[Tags.Player].Count == 0 || rm.Tagged[Tags.MustBeKilled].Count > 0) {
				return;
			}

			var h = GetComponent<HealthComponent>();
			h.Unhittable = false;
			h.Kill(this);
		}

		#region Snowflake States
		public class IdleState : SmartState<Snowflake> {
			private bool searched;
			private Entity target;

			public override void Init() {
				base.Init();
				
				var component = Self.GetComponent<ZComponent>();
				Tween.To(0, component.Z, x => component.Z = x, 0.4f, Ease.BackOut);
			}

			public override void Update(float dt) {
				base.Update(dt);
				
				if (target != null) {
					if (target.Done) {
						target = null;
						return;
					}
					
					var dx = Self.DxTo(target);
					var dy = Self.DyTo(target);
					var d = MathUtils.Distance(dx, dy);

					if (d <= 32) {
						if (!target.HasComponent<OrbitGiverComponent>()) {
							target.AddComponent(new OrbitGiverComponent());
						}
					
						target.GetComponent<OrbitGiverComponent>().AddOrbiter(Self);
						Become<OrbitingState>();

						return;
					}

					var body = Self.GetComponent<SensorBodyComponent>();
					var s = dt * 200;
					
					body.Velocity += new Vector2(dx / d * s, dy / d * s);
					return;
				}

				if (!searched) {
					searched = true;
					target = Self.GetComponent<RoomComponent>().Room?.FindClosest(Self.Center, Tags.Mob, e => !e.HasComponent<OrbitalComponent>() && !(e is WallWalker || e is Boss));

					if (target == null) {
						Self.Kill(Self);
						// target = Self.GetComponent<RoomComponent>().Room?.FindClosest(Self.Center, Tags.Player);
					}
				}

				if (T >= 1f && searched) {
					searched = false;
				}
				
				// fixme: pursue first, make unhittable but autodying (add tag for autodeath)
			}
		}
		
		public class OrbitingState : SmartState<Snowflake> {
			public override void Init() {
				base.Init();
				
				var component = Self.GetComponent<ZComponent>();
				Tween.To(DefaultZ, component.Z, x => component.Z = x, 0.4f, Ease.BackOut);
			}

			public override void Update(float dt) {
				base.Update(dt);

				var orbiting = Self.GetComponent<OrbitalComponent>().Orbiting;
				
				if (orbiting == null) {
					Become<IdleState>();
					return;
				}
				
				if (orbiting.TryGetComponent<ZComponent>(out var z)) {
					Self.GetComponent<ZComponent>().Z = z.Z + DefaultZ;
				}
			}
		}
		#endregion

		public override bool ShouldCollide(Entity entity) {
			return !(entity is Level) && base.ShouldCollide(entity);
		}
	}
}