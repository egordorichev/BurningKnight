using System;
using System.Collections.Generic;
using Lens.entity;
using Lens.entity.component;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.component {
	public class OrbitGiverComponent : Component {
		public List<Entity> Orbiting = new List<Entity>();
		public float RadiusMultiplier = 1;
		public float T;

		private float count;
		
		public void DestroyAll() {
			foreach (var o in Orbiting) {
				o.Done = true;
			}

			Orbiting.Clear();
		}
		
		public override void Update(float dt) {
			base.Update(dt);

			T += dt;
			count += (Orbiting.Count - count) * dt * 4;

			for (var i = Orbiting.Count - 1; i >= 0; i--) {
				var e = Orbiting[i];
				var component = e.GetComponent<OrbitalComponent>();
				var d = component.Radius * RadiusMultiplier;
				var a = i / count * Math.PI * 2 - T;

				if (e.Done) {
					Orbiting.RemoveAt(i);
					continue;
				}

				var target = Entity.Center + new Vector2((float) Math.Cos(a) * d, (float) Math.Sin(a) * d);

				if (component.Lerp) {
					e.Center += (target - e.Center) * (dt * 8);
				} else {
					e.Center = target;
				}
			}
		}

		public void AddOrbiter(Entity e) {
			if (!e.HasComponent<OrbitalComponent>()) {
				e.AddComponent(new OrbitalComponent());
			}

			e.GetComponent<OrbitalComponent>().Orbiting = Entity;
			Orbiting.Add(e);
		}

		public void RemoveOrbiter(Entity e) {
			Orbiting.Remove(e);
			e.GetComponent<OrbitalComponent>().Orbiting = null;
		}
	}
}