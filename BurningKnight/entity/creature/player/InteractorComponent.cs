using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using BurningKnight.assets;
using BurningKnight.assets.input;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.fx;
using BurningKnight.entity.item;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using Lens;
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

			if (CurrentlyInteracting != null && Input.WasPressed(Controls.Interact, GetComponent<GamepadComponent>().Controller) && !GetComponent<InventoryComponent>().Busy) {
				if (Run.Depth == -2 && GlobalSave.IsFalse("control_interact")) {
					GlobalSave.Put("control_interact", true);
					Entity.GetComponent<DialogComponent>().Close();
				}

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
		}

		private void OnStart() {
			if (!CurrentlyInteracting.TryGetComponent<InteractableComponent>(out var component)) {
				return;
			}

			component.CurrentlyInteracting = Entity;
			component.OnStart?.Invoke(Entity);

			if (Run.Depth == -2) {
				var hasGamepad = GamepadComponent.Current != null && GamepadComponent.Current.Attached;
				
				var region = CommonAse.Ui.GetSlice(Controls.FindSlice(Controls.Interact, false));
				Engine.Instance.State.Ui.Add(new InteractFx(CurrentlyInteracting, null, region, hasGamepad ? -5 : 0));

				if (hasGamepad) {
					region = CommonAse.Ui.GetSlice(Controls.FindSlice(Controls.Interact, true));
					Engine.Instance.State.Ui.Add(new InteractFx(CurrentlyInteracting, null, region, 5));
				}
			}
		}
		
		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent start) {
				if (start.Body.CanCollide && CanInteract(start.Entity)) {
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
			} else if (e is CollisionEndedEvent end && end.Body.CanCollide) {
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