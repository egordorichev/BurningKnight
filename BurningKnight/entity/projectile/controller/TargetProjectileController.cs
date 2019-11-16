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

					foreach (var m in (p.Owner.TryGetComponent<RoomComponent>(out var c) ? c.Room.Tagged[Tags.Mob] : p.Area.Tagged[Tags.Mob])) {
						var dd = m.DistanceTo(p);

						if (dd < md) {
							md = dd;
							target = m;
						}
					}

					if (target == null) {
						b.Angle = b.Velocity.ToAngle();
						return;
					}
				}
				
				if (target.Done) {
					target = null;
					b.Angle = b.Velocity.ToAngle();
					return;
				}

				a = (float) MathUtils.LerpAngle(a, p.AngleTo(target), dt * speed * 4);
				b.Velocity = new Vector2((float) Math.Cos(a) * d, (float) Math.Sin(a) * d);
				b.Angle = a;
			};
		}

		public static ProjectileUpdateCallback MakeCursor(float speed = 1f) {
			return (p, dt) => {
				var b = p.GetAnyComponent<BodyComponent>();
				var d = b.Velocity.Length();
				var a = b.Velocity.ToAngle();

				a = (float) MathUtils.LerpAngle(a, p.AngleTo(Input.Mouse.GamePosition), dt * speed * 4);
				b.Velocity = new Vector2((float) Math.Cos(a) * d, (float) Math.Sin(a) * d);
				b.Angle = a;
			};
		}

		public static ProjectileUpdateCallback MakePoint(Vector2 point, float speed = 1f) {
			return (p, dt) => {
				var b = p.GetAnyComponent<BodyComponent>();
				var d = b.Velocity.Length();
				var a = b.Velocity.ToAngle();
				var dd = p.DistanceTo(point);
				
				if (dd < 3f) {
					p.Break();
					return;
				}
				
				a = (float) MathUtils.LerpAngle(a, p.AngleTo(point), dt * speed * 4 * Math.Min(1, d / 32));
				b.Velocity = new Vector2((float) Math.Cos(a) * d, (float) Math.Sin(a) * d);
				b.Angle = a;
			};
		}
	}
}