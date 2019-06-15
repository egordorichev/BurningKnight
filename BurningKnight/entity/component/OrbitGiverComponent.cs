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
			count += (Orbiting.Count - count) * dt;

			for (var i = Orbiting.Count - 1; i >= 0; i--) {
				var e = Orbiting[i];
				var d = e.GetComponent<OrbitalComponent>().Radius * RadiusMultiplier;
				var a = i / count * Math.PI * 2 + T;

				if (e.Done) {
					Orbiting.RemoveAt(i);
					continue;
				}
				
				e.Center = Entity.Center + new Vector2((float) Math.Cos(a) * d, (float) Math.Sin(a) * d);
			}
		}

		public void AddOrbiter(Entity e) {
			if (!e.HasComponent<OrbitalComponent>()) {
				e.AddComponent(new OrbitalComponent());
			}
			
			Orbiting.Add(e);
		}
	}
}