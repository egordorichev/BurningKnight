using System;
using Lens.entity;
using Lens.entity.component;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.component {
	public class FollowerComponent : Component {
		public Entity Following;
		public Entity Follower;
		public float MaxDistance = 16;

		public void DestroyAll() {
			if (Follower != null) {
				Follower.GetComponent<FollowerComponent>().DestroyAll();
				Follower.Done = true;
			}
		}
		
		public override void Update(float dt) {
			base.Update(dt);

			if (Follower != null && Follower.Done) {
				Follower = null;
			}
			
			if (Following != null) {
				if (Following.Done) {
					var f = Following.GetComponent<FollowerComponent>();

					if (f.Following != null) {
						f.Following.GetComponent<FollowerComponent>().Follower = Entity;
						Following = f.Following;
					} else {
						Following = null;
					}
				}

				if (Following == null) {
					return;
				}
				
				var body = Entity.GetAnyComponent<BodyComponent>();

				if (body == null) {
					return;
				}

				var dx = Following.DxTo(Entity);
				var dy = Following.DyTo(Entity);
				var d = Math.Sqrt(dx * dx + dy * dy);

				if (d > 1024f) {
					Entity.Center = Following.Center;
					return;
				}

				var sp = dt * 16f;
				
				body.Velocity -= body.Velocity * (sp * 0.4f);

				if (d > MaxDistance) {
					body.Velocity -= new Vector2(dx * sp, dy * sp);
				}
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