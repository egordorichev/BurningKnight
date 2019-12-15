using System;
using BurningKnight.util;
using Lens.entity;
using Lens.entity.component;
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
			var f = Following.GetComponent<FollowerComponent>();
			
			f.Follower = Follower;

			if (Follower != null) {
				f.Follower.GetComponent<FollowerComponent>().Following = Following;
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
				
				body.Velocity -= new Vector2(dx * sp, dy * sp);
			}
		}

		public void AddFollower(Entity e) {
			if (!e.HasComponent<FollowerComponent>()) {
				e.AddComponent(new FollowerComponent());
			}

			if (Follower != null) {
				Follower.GetComponent<FollowerComponent>().AddFollower(e);
			} else {
				Follower = e;
				e.GetComponent<FollowerComponent>().Following = Entity;
			}
		}
	}
}