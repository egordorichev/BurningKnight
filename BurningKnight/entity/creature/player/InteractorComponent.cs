using System.Collections.Generic;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using Lens.entity;
using Lens.entity.component;
using Lens.input;

namespace BurningKnight.entity.creature.player {
	public class InteractorComponent : Component {
		public Entity CurrentlyInteracting;
		public List<Entity> InteractionCandidates = new List<Entity>();

		public override void Update(float dt) {
			base.Update(dt);

			if (CurrentlyInteracting != null && Input.WasPressed(Controls.Interact)) {
				if (CurrentlyInteracting.GetComponent<InteractableComponent>().Interact(Entity)) {
					EndInteraction();
				}
			}
		}

		private void EndInteraction() {
			CurrentlyInteracting.GetComponent<InteractableComponent>().OnEnd(Entity);
			
			if (InteractionCandidates.Count == 0) {
				CurrentlyInteracting = null;
			} else {
				CurrentlyInteracting = InteractionCandidates[0];
				InteractionCandidates.RemoveAt(0);
				CurrentlyInteracting.GetComponent<InteractableComponent>().OnStart(Entity);
			}
		}
		
		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent start) {
				if (CanInteract(start.Entity)) {
					if (CurrentlyInteracting != null) {
						InteractionCandidates.Add(start.Entity);
					} else {
						CurrentlyInteracting = start.Entity;
						CurrentlyInteracting.GetComponent<InteractableComponent>().OnStart(Entity);
					}
				}
			} else if (e is CollisionEndedEvent end) {
				if (CurrentlyInteracting == end.Entity) {
					EndInteraction();
				}
			}
			
			return base.HandleEvent(e);
		}

		public virtual bool CanInteract(Entity e) {
			return e.HasComponent(typeof(InteractableComponent));
		}
	}
}