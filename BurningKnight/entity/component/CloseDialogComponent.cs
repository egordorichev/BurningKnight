using BurningKnight.ui.dialog;
using Lens.entity;
using Lens.entity.component;
using Lens.util.math;

namespace BurningKnight.entity.component {
	public class CloseDialogComponent : Component {
		private const int Radius = 64 * 64;
		private const int RadiusMax = 80 * 80;

		public string[] Variants;
		private Entity trigger;

		public CloseDialogComponent(params string[] vars) {
			Variants = vars;
		}

		public override void Update(float dt) {
			if (!Entity.OnScreen) {
				return;
			}

			if (!Entity.TryGetComponent<DialogComponent>(out var d)) {
				return;
			}

			if (d.Current != null) {
				if (trigger != null && trigger.DistanceToSquared(Entity) > RadiusMax) {
					trigger = null;
					d.Close();
				}

				return;
			}

			foreach (var p in Entity.Area.Tags[Tags.Player]) {
				if (p.DistanceToSquared(Entity) <= Radius) {
					d.Start(Variants[Random.Int(Variants.Length)]);
					trigger = p;
					return;
				}
			}
		}
	}
}