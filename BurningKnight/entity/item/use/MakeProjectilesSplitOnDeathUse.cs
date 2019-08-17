using System;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using Lens.entity;
using Lens.util;
using MonoGame.Extended;

namespace BurningKnight.entity.item.use {
	public class MakeProjectilesSplitOnDeathUse : ItemUse {
		public override bool HandleEvent(Event e) {
			// Make sure that this is a new projectile, not created by this event
			if (e is ProjectileCreatedEvent pce && pce.Projectile.Parent == null) {
				pce.Projectile.OnDeath += (p, t) => {
					var v = p.GetAnyComponent<BodyComponent>().Velocity;
					var a = v.ToAngle();
					var s = v.Length();
					var c = p.HasComponent<CircleBodyComponent>();

					Projectile.Make(pce.Owner, p.Slice, a - (float) Math.PI * 0.5f, s, c, -1, p, p.Scale).Center = p.Center;
					Projectile.Make(pce.Owner, p.Slice, a + (float) Math.PI * 0.5f, s, c, -1, p, p.Scale).Center = p.Center;
				};
			}
			
			return false;
		}
	}
}