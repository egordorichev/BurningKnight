using System;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using Lens.entity;
using Lens.util;

namespace BurningKnight.entity.item.use {
	public class MakeProjectilesSplitOnDeathUse : ItemUse {
		public override bool HandleEvent(Event e) {
			// Make sure that this is a new projectile, not created by this event
			if (e is ProjectileCreatedEvent pce && pce.Projectile.Parent == null) {
				pce.Projectile.OnDeath += (p, en, t) => {
					var v = p.GetAnyComponent<BodyComponent>().Velocity;

					if (p is Laser l) {
						var a = l.BodyComponent.Body.Rotation;
						var end = l.End - MathUtils.CreateVector(a, 10);
						
						for (var i = 0; i < 2; i++) {
							var laser = Laser.Make(p.Owner, a + (i == 0 ? -1 : 1) * (float) Math.PI * 0.5f, 0f, null, p.Damage, parent: l);

							laser.Position = end;
							laser.Recalculate();
						}
					} else {
						var c = p.HasComponent<CircleBodyComponent>();
						var s = v.Length();
						var a = v.ToAngle() - Math.PI;

						Projectile.Make(pce.Owner, p.Slice, a - (float) Math.PI * 0.5f, s, c, -1, p, p.Scale).Center = p.Center;
						Projectile.Make(pce.Owner, p.Slice, a + (float) Math.PI * 0.5f, s, c, -1, p, p.Scale).Center = p.Center;	
					}
				};
			}
			
			return false;
		}
	}
}