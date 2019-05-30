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
		
		public override void Update(float dt) {
			base.Update(dt);

			T += dt;

			for (var i = 0; i < Orbiting.Count; i++) {
				var e = Orbiting[i];
				var d = e.GetComponent<OrbitalComponent>().Radius * RadiusMultiplier;
				var a = ((float) i) / Orbiting.Count * Math.PI * 2 + T;
				
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