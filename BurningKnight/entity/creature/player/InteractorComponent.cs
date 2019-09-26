using System;
using System.Collections.Generic;
using BurningKnight.assets.input;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.save;
using BurningKnight.ui.dialog;
using Lens.entity;
using Lens.entity.component;
using Lens.input;
using Lens.util;

namespace BurningKnight.entity.creature.player {
	public class InteractorComponent : Component {
		public Entity CurrentlyInteracting;
		public List<Entity> InteractionCandidates = new List<Entity>();

		public override void Update(float dt) {
			base.Update(dt);

			if (CurrentlyInteracting != null && Input.WasPressed(Controls.Interact, GetComponent<GamepadComponent>().Controller)) {
				GlobalSave.Put("control_interact", true);

				if (CurrentlyInteracting.GetComponent<InteractableComponent>().Interact(Entity)) {
					Send(new InteractedEvent {
						Who = Entity,
						With = CurrentlyInteracting
					});
					
					EndInteraction();
				}
			}
		}

		public void EndInteraction() {
			if (CurrentlyInteracting.TryGetComponent<InteractableComponent>(out var component)) {
				component.OnEnd?.Invoke(Entity);
				component.CurrentlyInteracting = null;
			}
			
			if (InteractionCandidates.Count == 0) {
				CurrentlyInteracting = null;
			} else {
				CurrentlyInteracting = InteractionCandidates[0];
				InteractionCandidates.RemoveAt(0);
				OnStart();
			}

			Entity.GetComponent<DialogComponent>().Close();
		}

		private void OnStart() {
			if (!CurrentlyInteracting.TryGetComponent<InteractableComponent>(out var component)) {
				return;
			}

			component.CurrentlyInteracting = Entity;
			component.OnStart?.Invoke(Entity);

			if (GlobalSave.IsFalse("control_interact")) {
				GetComponent<DialogComponent>().Dialog.Str.SetVariable("ctrl", Controls.Find(Controls.Interact, GamepadComponent.Current != null));
				Entity.GetComponent<DialogComponent>().Start("control_3");
			}
		}
		
		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent start) {
				if (CanInteract(start.Entity)) {
					var entity = start.Entity.GetComponent<InteractableComponent>().AlterInteraction?.Invoke() ?? start.Entity;

					if (CurrentlyInteracting != entity) {
						if (CurrentlyInteracting != null) {
							if (!InteractionCandidates.Contains(entity)) {
								InteractionCandidates.Add(entity);
							}
						} else {
							CurrentlyInteracting = entity;
							OnStart();
						}
					}
				}
			} else if (e is CollisionEndedEvent end) {
				if (CurrentlyInteracting == end.Entity) {
					EndInteraction();
				} else {
					InteractionCandidates.Remove(end.Entity);
				}
			}
			
			return base.HandleEvent(e);
		}

		public Func<Entity, bool> CanInteractCallback;
		
		public virtual bool CanInteract(Entity e) {
			return e.TryGetComponent<InteractableComponent>(out var component) && (component.CanInteract?.Invoke(Entity) ?? true) && (CanInteractCallback == null || CanInteractCallback(e));
		}
	}
}