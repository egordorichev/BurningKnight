using System;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using Lens.entity;
using Lens.input;
using Lens.util;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.projectile.controller {
	public static class TargetProjectileController {
		public static ProjectileUpdateCallback Make(Entity target, float speed = 1f) {
			return (p, dt) => {
				var b = p.GetAnyComponent<BodyComponent>();
				var d = b.Velocity.Length();
				var a = b.Velocity.ToAngle();

				var from = p.Center;

				if (p.Owner.TryGetComponent<AimComponent>(out var aim)) {
					from = aim.RealAim;
				}
				
				if (target == null) {
					var md = 320000f;

					foreach (var m in (p.Owner.TryGetComponent<RoomComponent>(out var c) ? c.Room.Tagged[Tags.Mob] : p.Area.Tagged[Tags.Mob])) {
						if (m.GetComponent<HealthComponent>().Unhittable) {
							continue;
						}
						
						var dd = m.DistanceTo(from);

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

				a = (float) MathUtils.LerpAngle(a, p.AngleTo(p.Owner.GetComponent<CursorComponent>().Cursor.GamePosition) + Rnd.Float(-2, 2), dt * speed * 4);
				b.Velocity = new Vector2((float) Math.Cos(a) * d, (float) Math.Sin(a) * d);
				b.Angle = a;
			};
		}

		public static ProjectileUpdateCallback MakeBetter(float speed = 1f) {
			return (p, dt) => {
				var b = p.GetAnyComponent<BodyComponent>();
				var d = b.Velocity.Length();
				var a = b.Velocity.ToAngle();
				Entity target = null;
				
				var md = 320000f;
				var from = p.Center;

				if (p.Owner.TryGetComponent<AimComponent>(out var aim)) {
					from = aim.RealAim;
				}
				
				foreach (var m in (p.Owner.TryGetComponent<RoomComponent>(out var c) ? c.Room.Tagged[Tags.Mob] : p.Area.Tagged[Tags.Mob])) {
					if (m.Done || m.GetComponent<HealthComponent>().Unhittable) {
						continue;
					}
					
					var dd = m.DistanceTo(from);

					if (dd < md) {
						md = dd;
						target = m;
					}
				}

				if (target == null) {
					b.Angle = b.Velocity.ToAngle();
					return;
				}
				
				a = (float) MathUtils.LerpAngle(a, p.AngleTo(target), dt * speed * 16 * Math.Max(0.3f, 1 - Math.Min(1, md / 96)));
				b.Velocity = new Vector2((float) Math.Cos(a) * d, (float) Math.Sin(a) * d);
				b.Angle = a;
			};
		}
	}
}