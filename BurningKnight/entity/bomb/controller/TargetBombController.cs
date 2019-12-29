using System;
using BurningKnight.entity.component;
using Lens.entity;
using Lens.util;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.bomb.controller {
	public static class TargetBombController {
		public static BombUpdateCallback Make(Entity target, float speed = 1f) {
			return (p, dt) => {
				var b = p.GetAnyComponent<BodyComponent>();
				var d = Math.Max(100, b.Velocity.Length());
				var a = b.Velocity.ToAngle();
				
				if (target == null) {
					var md = 320000f;

					foreach (var m in (p.Owner.TryGetComponent<RoomComponent>(out var c) ? c.Room.Tagged[Tags.Mob] : p.Area.Tagged[Tags.Mob])) {
						if (m.GetComponent<HealthComponent>().Unhittable) {
							continue;
						}
						
						var dd = m.DistanceTo(p);

						if (dd < md) {
							md = dd;
							target = m;
						}
					}

					if (target == null) {
						return;
					}
				}
				
				if (target.Done) {
					target = null;
					return;
				}

				a = (float) MathUtils.LerpAngle(a, p.AngleTo(target), dt * speed * 4);
				b.Velocity = new Vector2((float) Math.Cos(a) * d, (float) Math.Sin(a) * d);
			};
		}
	}
}