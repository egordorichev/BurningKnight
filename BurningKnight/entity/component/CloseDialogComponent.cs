using System;
using BurningKnight.ui.dialog;
using Lens.entity;
using Lens.entity.component;
using Lens.util.math;

namespace BurningKnight.entity.component {
	public class CloseDialogComponent : Component {
		public int Radius = 32 * 32;
		public int RadiusMax = 64 * 64;

		public string[] Variants;
		private Entity trigger;

		public Func<Entity, bool> CanTalk;
		public Func<Entity, string> DecideVariant;

		public CloseDialogComponent(params string[] vars) {
			Variants = vars;
		}

		public override void Update(float dt) {
			if (!Entity.OnScreen || Variants.Length == 0) {
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

			foreach (var p in Entity.Area.Tagged[Tags.Player]) {
				if (p.DistanceToSquared(Entity) <= Radius) {
					if (CanTalk == null || CanTalk(Entity)) {
						d.Start(DecideVariant?.Invoke(p) ?? Variants[Rnd.Int(Variants.Length)]);
						trigger = p;
						return;
					}
				}
			}
		}
	}
}