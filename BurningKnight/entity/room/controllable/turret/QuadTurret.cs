using System;
using BurningKnight.entity.component;

namespace BurningKnight.entity.room.controllable.turret {
	public class QuadTurret : Turret {
		public override void AddComponents() {
			base.AddComponents();

			var a = GetComponent<AnimationComponent>();
			
			a.Animation.Tag = "four";
			a.Animation.Paused = true;
		}

		protected override void Fire(double angle) {
			for (var i = 0; i < 4; i++) {
				SendProjectile(angle + i * Math.PI / 2f);
			}
		}
	}
}