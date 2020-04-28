using System;
using BurningKnight.assets.lighting;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;
using Lens.util.camera;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.pet {
	public class LampPet : FollowerPet {
		public LampPet(string spr) : base(spr) {
			
		}
		
		public override void AddComponents() {
			base.AddComponents();
			AddComponent(new LightComponent(this, 64f, new Color(1f, 0.8f, 0.2f, 1f)));
		}

		protected override void Follow() {
			if (Sprite == "bk:led") {
				if (!HasComponent<OrbitalComponent>()) {
					AddComponent(new OrbitalComponent());
					Owner.GetComponent<OrbitGiverComponent>().AddOrbiter(this);
				}
			} else {
				base.Follow();
			}
		}

		private float lastFlame;
		private float t;
		
		public override void Update(float dt) {
			base.Update(dt);
			t += dt * 0.5f;
			GetComponent<LightComponent>().Light.Radius = 38f + (float) Math.Cos(t) * 6;
			
			lastFlame += dt;

			if (lastFlame > 0.3f) {
				Area.Add(new FireParticle {
					X = CenterX,
					Y = Y + 1,
					Depth = Layers.Wall + 1
				});

				lastFlame = 0;
			}
		}
	}
}