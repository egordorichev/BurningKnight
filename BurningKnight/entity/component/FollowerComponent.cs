using System;
using Lens.entity;
using Lens.entity.component;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.component {
	public class FollowerComponent : Component {
		public Entity Following;
		public Entity Follower;
		public float maxDistance = 16;
		
		public override void Update(float dt) {
			base.Update(dt);

			if (Following != null) {
				var body = Entity.GetAnyComponent<BodyComponent>();

				if (body == null) {
					return;
				}

				var dx = Following.DxTo(Entity);
				var dy = Following.DyTo(Entity);
				var d = Math.Sqrt(dx * dx + dy * dy);
				var sp = dt * 16f;
				
				body.Velocity -= body.Velocity * (sp * 0.4f);

				if (d > maxDistance) {
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