using BurningKnight.ui.dialog;
using Lens.entity;
using Lens.entity.component;

namespace BurningKnight.entity.component {
	public class InteractDialogComponent : Component {
		private string dialog;
		private bool started;
		
		public InteractDialogComponent(string d) {
			dialog = d;
		}

		public override void Init() {
			base.Init();
			
			Entity.AddComponent(new InteractableComponent(Interact) {
				CanInteract = e => !started
			});
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

			var d = GetComponent<DialogComponent>();

			d.OnNext += OnNext;
			d.Start(dialog, e);
			
			started = true;
			
			return true;
		}
	}
}