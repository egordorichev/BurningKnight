using System;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.save;
using BurningKnight.util;
using Lens.entity;
using Lens.util.tween;

namespace BurningKnight.entity.orbital {
	public delegate void OrbitalCollisionHandler(Orbital o, Entity e);
	
	public class Orbital : Entity {
		public OrbitalCollisionHandler OnCollision;
		public Entity Owner => GetComponent<OrbitalComponent>().Orbiting;
		public Action<float> Controller; 
		
		public override void AddComponents() {
			base.AddComponents();
			AddComponent(new OrbitalComponent());
		}

		private bool animated;

		public override void Update(float dt) {
			base.Update(dt);
			
			if (!animated) {
				animated = true;
				
				if (TryGetComponent<ScalableSliceComponent>(out var g)) {
					AnimationUtil.Poof(Center);
					g.Animate();
				}
			}
			
			Controller?.Invoke(dt);
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent cse) {
				OnCollision?.Invoke(this, cse.Entity);
			}

			return base.HandleEvent(e);
		}
	}
}