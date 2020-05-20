using System.Collections.Generic;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.room.controllable;
using Lens.entity;
using Lens.entity.component;

namespace BurningKnight.entity.component {
	public class SupportableComponent : Component {
		public List<Support> Supports = new List<Support>();

		public override void Destroy() {
			base.Destroy();
			Supports.Clear();
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent cse && cse.Entity is Support ss) {
				if (!Supports.Contains(ss) && (!(Entity is Player) || !(cse.Body is SensorBodyComponent))) {
					Supports.Add(ss);
					ss.Supporting.Add(Entity);

					ss.HandleEvent(new Support.StartedSupportingEvent {
						Support = ss,
						Entity = Entity
					});
				}
			} else if (e is CollisionEndedEvent cee && cee.Entity is Support se) {
				if (Supports.Contains(se)) {
					Supports.Remove(se);
					se.Supporting.Remove(Entity);

					se.HandleEvent(new Support.EndedSupportingEvent {
						Support = se,
						Entity = Entity
					});
				}
			}
			
			return base.HandleEvent(e);
		}

		public override void Update(float dt) {
			base.Update(dt);
			
			foreach (var s in Supports) {
				s.Apply(Entity, dt);
			}
		}

		public bool HasAnotherSupportBesides(Support support) {
			foreach (var s in Supports) {
				if (s != support) {
					return true;
				}
			}

			return false;
		}
	}
}