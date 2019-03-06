using System.Collections.Generic;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using Lens.entity;
using Lens.entity.component;

namespace BurningKnight.entity.creature.player {
	public class InteractorComponent : Component {
		public Entity CurrentlyInteracting;
		public List<Entity> InteractionCandidates = new List<Entity>();
		
		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent start) {
				if (CanInteract(start.Entity)) {
					if (CurrentlyInteracting != null) {
						InteractionCandidates.Add(start.Entity);
					} else {
						CurrentlyInteracting = start.Entity;
					}
				}
			} else if (e is CollisionEndedEvent end) {
				if (CurrentlyInteracting == end.Entity) {
					if (InteractionCandidates.Count == 0) {
						CurrentlyInteracting = null;						
					} else {
						CurrentlyInteracting = InteractionCandidates[0];
						InteractionCandidates.RemoveAt(0);
					}
				}
			}
			
			return base.HandleEvent(e);
		}

		public virtual bool CanInteract(Entity e) {
			return e.HasComponent(typeof(InteractableComponent));
		}
	}
}