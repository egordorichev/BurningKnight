using System.Collections.Generic;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.events;
using BurningKnight.save;
using Lens.entity;
using Lens.entity.component.graphics;
using Lens.entity.component.logic;

namespace BurningKnight.entity.level.entities {
	public class Door : SaveableEntity {
		private const float W = 16;
		private const float H = 4;
		private const float CloseTimer = 1f;
		
		public bool FacingSide;

		private List<Entity> colliding = new List<Entity>();
		private float LastCollisionTimer;
		
		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new RectBodyComponent(0, 0, FacingSide ? H : W, FacingSide ? W : H));
			AddComponent(new AnimationComponent(FacingSide ? "side_door" : "regular_door"));
			AddComponent(new StateComponent());
			
			GetComponent<StateComponent>().Become<ClosedState>();
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent start) {
				if (start.Entity is Creature) {
					colliding.Add(start.Entity);

					if (colliding.Count == 1) {
						GetComponent<StateComponent>().Become<OpeningState>();	
					}
				}
			} else if (e is CollisionEndedEvent end) {
				if (end.Entity is Creature) {
					colliding.Remove(end.Entity);
					
					if (colliding.Count == 0) {
						LastCollisionTimer = CloseTimer;
					}
				}
			}
			
			return base.HandleEvent(e);
		}

		public override void Update(float dt) {
			base.Update(dt);
			var state = GetComponent<StateComponent>();
			
			if (state.StateInstance is OpenState && colliding.Count == 0) {
				LastCollisionTimer -= dt;

				if (LastCollisionTimer <= 0) {
					state.Become<ClosingState>();
				}
			}
		}

		public class ClosedState : EntityState {
			
		}

		public class ClosingState : EntityState {
			// fixme: figure out a way to find when animation is ended and go to close state
		}

		public class OpenState : EntityState {
			
		}

		public class OpeningState : EntityState {
			
		}
	}
}