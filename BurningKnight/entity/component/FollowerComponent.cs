using System;
using BurningKnight.assets.achievements;
using BurningKnight.entity.creature.player;
using BurningKnight.util;
using Lens.entity;
using Lens.entity.component;
using Lens.util;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.component {
	public class FollowerComponent : Component {
		public Entity Following;
		public Entity Follower;
		public float MaxDistance = 16;
		public float Pause = -1;
		public bool Paused;

		public void DestroyAll() {
			if (Follower != null) {
				Follower.GetComponent<FollowerComponent>().DestroyAll();
				Follower.Done = true;
			}
		}

		public void Remove() {
			if (Following != null) {
				var f = Following.GetComponent<FollowerComponent>();

				f.Follower = Follower;

				if (Follower != null) {
					f.Follower.GetComponent<FollowerComponent>().Following = Following;
				}
			}

			Follower = null;
			Following = null;
		}
		
		public override void Update(float dt) {
			base.Update(dt);

			if (Follower != null && Follower.Done) {
				Follower = null;
			}
			
			if (Following != null) {
				if (Following.Done) {
					Following.GetComponent<FollowerComponent>().Remove();
					Following = null;
				}

				if (Following == null) {
					return;
				}
				
				var body = Entity.GetAnyComponent<BodyComponent>();

				if (body == null) {
					return;
				}

				var sp = dt * 16f;
				body.Velocity -= body.Velocity * (sp * 0.4f);

				if (Paused) {
					return;
				}
				
				if (Pause > 0) {
					Pause -= dt;
					return;
				}

				var dx = Following.DxTo(Entity);
				var dy = Following.DyTo(Entity);
				var d = Math.Sqrt(dx * dx + dy * dy);

				if (d > 1024f || !Entity.OnScreen) {
					AnimationUtil.Poof(Entity.Center);
					Entity.Center = Following.Center + Rnd.Vector(-16, 16);
					AnimationUtil.Poof(Entity.Center);
					return;
				}

				if (d < 12) {
					sp *= -1;
				} else if (d < MaxDistance) {
					return;
				}

				var s = 3;

				if (Following.TryGetComponent<RectBodyComponent>(out var rb)) {
					body.Velocity = body.Velocity.Lerp(rb.Velocity, dt * s);
				} else if (Following.TryGetComponent<CircleBodyComponent>(out var cb)) {
					body.Velocity = body.Velocity.Lerp(cb.Velocity, dt * s);
				} else if (Following.TryGetComponent<SensorBodyComponent>(out var sb)) {
					body.Velocity = body.Velocity.Lerp(sb.Velocity, dt * s);
				}
				
				body.Velocity -= new Vector2(dx * sp, dy * sp);
			}
		}

		public void AddFollower(Entity e) {
			if (e == Entity) {
				return;
			}
			
			if (!e.HasComponent<FollowerComponent>()) {
				e.AddComponent(new FollowerComponent());
			}

			if (Follower != null) {
				Follower.GetComponent<FollowerComponent>().AddFollower(e);
			} else {
				Follower = e;
				e.GetComponent<FollowerComponent>().Following = Entity;
			}

			if (Entity is Player) {
				var depth = 0;
				var entity = Entity;

				while (entity != null) {
					entity = entity.GetComponent<FollowerComponent>().Follower;

					if (entity != null) {
						depth++;
					}
				}

				if (depth >= 3) {
					Achievements.Unlock("bk:family");
				}
			}
		}
	}
}