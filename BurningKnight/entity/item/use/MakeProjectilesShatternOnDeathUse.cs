using System;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using Lens.entity;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.entity.item.use {
	public class MakeProjectilesShatternOnDeathUse : ItemUse {
		public override bool HandleEvent(Event e) {
			// Make sure that this is a new projectile, not created by this event
			if (e is ProjectileCreatedEvent pce && pce.Projectile.Parent == null) {
				pce.Projectile.OnDeath += (p, en, t) => {
					var v = p.GetAnyComponent<BodyComponent>().Velocity;
					var a = v.ToAngle() - (float) Math.PI;
					var s = v.Length();
					var c = p.HasComponent<CircleBodyComponent>();

					for (var i = 0; i < Rnd.Int(3, 5); i++) {
						Projectile.Make(pce.Owner, p.Slice, a + Rnd.Float(-1.4f, 1.4f), s * Rnd.Float(0.3f, 0.7f), c, -1, p, p.Scale * Rnd.Float(0.4f, 0.8f)).Center = p.Center;
					}
				};
			}
			
			return false;
		}
	}
}