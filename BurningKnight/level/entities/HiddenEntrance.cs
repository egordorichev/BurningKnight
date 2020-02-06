using BurningKnight.entity;
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
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.entities {
	public class HiddenEntrance : SaveableEntity, PlaceableEntity {
		private bool Interact(Entity entity) {
			foreach (var e in Area.Tagged[Tags.HiddenEntrance]) {
				if (e is HiddenExit) {
					var state = (InGameState) Engine.Instance.State;

					state.TransitionToBlack(Center, () => {
						entity.BottomCenter = e.BottomCenter + new Vector2(0, 2);

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

			Depth = Layers.Entrance;
			AddTag(Tags.HiddenEntrance);
			
			AddComponent(new InteractableComponent(Interact) {
				OnStart = entity => {
					if (entity is LocalPlayer) {
						Engine.Instance.State.Ui.Add(new InteractFx(this, Locale.Get("descend")));
					}
				},
				
				CanInteract = CanInteract
			});
			
			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Static, true));
			AddComponent(new InteractableSliceComponent("props", "dark_market"));
		}
	}
}