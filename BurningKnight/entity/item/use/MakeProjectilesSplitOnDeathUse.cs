using System;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using Lens.entity;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.entity.item.use {
	public class MakeProjectilesSplitOnDeathUse : ItemUse {
		public override bool HandleEvent(Event e) {
			// Make sure that this is a new projectile, not created by this event
			if (e is ProjectileCreatedEvent pce && pce.Projectile.Parent == null) {
				ProjectileCallbacks.AttachDeathCallback(pce.Projectile,  (p, en, t) => {
					if (Rnd.Chance(20)) {
						return;
					}
				
					var v = p.GetAnyComponent<BodyComponent>().Velocity;

					if (p is Laser l) {
						var a = l.BodyComponent.Body.Rotation;
						var end = l.End - MathUtils.CreateVector(a, 5);
						
						for (var i = 0; i < 2; i++) {
							var laser = Laser.Make(p.Owner, a + (i == 0 ? -1 : 1) * (float) Math.PI * 0.5f, 0f, null, p.Damage, parent: l);

							laser.Position = end;
							laser.Recalculate();
						}
					} else {
						var c = p.HasComponent<CircleBodyComponent>();
						var s = v.Length();
						var a = v.ToAngle() - Math.PI;

						var builder = new ProjectileBuilder(pce.Owner, p.Slice) {
							Parent = p,
							Scale = p.Scale,
							RectHitbox = !c
						};

						builder.Shoot(a - (float) Math.PI * 0.5f, s).Build().Center = p.Center;
						builder.Shoot(a + (float) Math.PI * 0.5f, s).Build().Center = p.Center;
					}
				});
			}
			
			return false;
		}
	}
}