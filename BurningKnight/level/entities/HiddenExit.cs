using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.fx;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.ui.editor;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.util.camera;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.entities {
	public class HiddenExit : SaveableEntity, PlaceableEntity {
		private bool Interact(Entity entity) {
			foreach (var e in Area.Tagged[Tags.HiddenEntrance]) {
				if (e is HiddenEntrance) {
					var state = (InGameState) Engine.Instance.State;

					state.TransitionToBlack(Center, () => {
						entity.Center = e.Center;

						state.ResetFollowing();
						Camera.Instance.Jump();
						state.TransitionToOpen();
					});
					
					return true;
				}
			}
			
			return true;
		}

		private bool CanInteract(Entity e) {
			return true;
		}

		public override void AddComponents() {
			base.AddComponents();

			Width = 12;
			Height = 15;

			AddComponent(new InteractableComponent(Interact) {
				OnStart = entity => {
					if (entity is LocalPlayer) {
						Engine.Instance.State.Ui.Add(new InteractFx(this, Locale.Get("ascend")));
					}
				},
				
				CanInteract = CanInteract
			});
			
			AddTag(Tags.HiddenEntrance);
			
			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Static, true));
			AddComponent(new InteractableSliceComponent("props", "ladder"));
			AddComponent(new ShadowComponent());
		}
	}
}