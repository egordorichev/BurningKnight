using BurningKnight.ui.dialog;
using Lens.entity;
using Lens.entity.component;

namespace BurningKnight.entity.component {
	public class InteractDialogComponent : Component {
		private string dialog;
		private bool started;
		private Entity toStart;
		
		public InteractDialogComponent(string d) {
			dialog = d;
		}

		public override void Init() {
			base.Init();
			
			Entity.AddComponent(new InteractableComponent(Interact) {
				CanInteract = e => !started
			});
		}

		public override void Update(float dt) {
			base.Update(dt);

			// Delayed setup to avoid interaction flowing into this dialog
			if (toStart != null) {
				var d = GetComponent<DialogComponent>();

				d.OnNext += OnNext;
				d.Start(dialog, toStart);
			
				started = true;
				toStart = null;
			}
		}

		private void OnNext(DialogComponent d) {
			if (d.Current == null) {
				d.OnNext -= OnNext;
				started = false;
			}			
		}

		private bool Interact(Entity e) {
			if (started) {
				return false;
			}

			toStart = e;
			
			return true;
		}
	}
}