using System.Collections.Generic;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.events;
using Lens.entity;

namespace BurningKnight.entity.creature.pet {
	public class SpikedCookie : DiagonalPet {
		private List<Entity> Colliding = new List<Entity>();
		
		public override void PostInit() {
			AddGraphics("spiked_cookie", false);
			base.PostInit();
		}

		private float t;

		public override void Update(float dt) {
			base.Update(dt);
			t += dt;

			if (t >= 0.2f) {
				t = 0;
				
				foreach (var c in Colliding) {
					c.GetComponent<HealthComponent>().ModifyHealth(-3, this);
				}
			}
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent cse) {
				if (cse.Entity is Mob) {
					Colliding.Add(cse.Entity);
					cse.Entity.GetComponent<HealthComponent>().ModifyHealth(-3, this);
				}
			} else if (e is CollisionEndedEvent cee) {
				if (cee.Entity is Mob) {
					Colliding.Remove(cee.Entity);
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}