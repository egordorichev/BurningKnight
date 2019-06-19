using System;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using Lens.entity;
using Lens.input;
using Lens.util;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.projectile.controller {
	public static class TargetProjectileController {
		public static ProjectileUpdateCallback Make(Entity target, float speed = 1f) {
			return (p, dt) => {
				var b = p.GetAnyComponent<BodyComponent>();
				var d = b.Velocity.Length();
				var a = b.Velocity.ToAngle();
				
				if (target == null) {
					var md = 320000f;

					foreach (var m in (p.Owner.TryGetComponent<RoomComponent>(out var c) ? c.Room.Tagged[Tags.Mob] : p.Area.Tags[Tags.Mob])) {
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

		public static ProjectileUpdateCallback MakeCursor(float speed = 1f) {
			return (p, dt) => {
				var b = p.GetAnyComponent<BodyComponent>();
				var d = b.Velocity.Length();
				var a = b.Velocity.ToAngle();

				if (p.DistanceTo(Input.Mouse.GamePosition) < 3f) {
					p.Break();
				}
				
				a = (float) MathUtils.LerpAngle(a, p.AngleTo(Input.Mouse.GamePosition), dt * speed * 4);
				b.Velocity = new Vector2((float) Math.Cos(a) * d, (float) Math.Sin(a) * d);
			};
		}
	}
}