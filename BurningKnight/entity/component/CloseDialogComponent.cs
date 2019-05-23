using BurningKnight.ui.dialog;
using Lens.entity;
using Lens.entity.component;
using Lens.util.math;

namespace BurningKnight.entity.component {
	public class CloseDialogComponent : Component {
		private const int Radius = 64 * 64;
		private const int RadiusMax = 72 * 72;
		
		private string[] variants;
		private Entity trigger;

		public CloseDialogComponent(params string[] vars) {
			variants = vars;
		}

		public override void Update(float dt) {
			if (!Entity.OnScreen) {
				return;
			}

			if (!Entity.TryGetComponent<DialogComponent>(out var d)) {
				return;
			}

			if (d.Current != null) {
				if (trigger != null && trigger.DistanceTo(Entity) > RadiusMax) {
					trigger = null;
					d.Dialog.Finish();
				}

				return;
			}

			foreach (var p in Entity.Area.Tags[Tags.Player]) {
				if (p.DistanceTo(Entity) <= Radius) {
					d.Start(variants[Random.Int(variants.Length)]);
					trigger = p;
					return;
				}
			}
		}
	}
}