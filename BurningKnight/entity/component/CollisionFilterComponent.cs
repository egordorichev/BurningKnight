using System;
using System.Collections.Generic;
using BurningKnight.physics;
using Lens.entity;
using Lens.entity.component;

namespace BurningKnight.entity.component {
	public class CollisionFilterComponent : Component {
		public List<Func<Entity, Entity, CollisionResult>> Filter = new List<Func<Entity, Entity, CollisionResult>>();

		public CollisionResult Invoke(Entity colliding) {
			foreach (var f in Filter) {
				var v = f(Entity, colliding);

				if (v != CollisionResult.Default) {
					return v;
				}
			}

			return CollisionResult.Default;
		}

		public static void Add(Entity e, Func<Entity, Entity, CollisionResult> filter) {
			CollisionFilterComponent c;

			if (!e.TryGetComponent(out c)) {
				c = new CollisionFilterComponent();
				e.AddComponent(c);
			}
			
			c.Filter.Add(filter);
		}
	}
}