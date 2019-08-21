using System.Collections.Generic;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.events;
using Lens.entity;
using Lens.entity.component.logic;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.room.input {
	public class PreasurePlate : RoomInput {
		private List<Entity> colliding = new List<Entity>();
		
		public override void AddComponents() {
			base.AddComponents();

			Depth = Layers.Entrance;
			
			AddComponent(new AnimationComponent("preasure_plate"));
			AddComponent(new RectBodyComponent(1, 1, 14, 14, BodyType.Static, true));
			AddComponent(new StateComponent());
			
			UpdateState();
		}

		protected override void UpdateState() {
			base.UpdateState();
			
			var s = GetComponent<StateComponent>();
			
			if (On) {
				s.Become<OnState>();
			} else {
				s.Become<OffState>();
			}
		}

		public override void TurnOff() {
			
		}

		public override void TurnOn() {
			
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent cse) {
				colliding.Add(cse.Entity);

				if (!On) {
					On = true;
					UpdateState();
					SendEvents();
				}
			} else if (e is CollisionEndedEvent cee) {
				colliding.Remove(cee.Entity);
				
				if (On && colliding.Count == 0) {
					On = false;
					UpdateState();
					SendEvents();
				}
			}
			
			return base.HandleEvent(e);
		}

		#region Plate States
		private class OnState : SmartState<PreasurePlate> {
			
		}
		
		private class OffState : SmartState<PreasurePlate> {
			
		}
		#endregion
	}
}