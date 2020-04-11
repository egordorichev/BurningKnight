using System;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using Lens.entity;
using Lens.util;

namespace BurningKnight.entity.item.use {
	public class MakeProjectilesSplitUse : ItemUse {
		public override bool HandleEvent(Event e) {
			if (e is ProjectileCreatedEvent pce) {
				pce.Projectile.OnHurt += (p, en) => {
					var v = p.GetAnyComponent<BodyComponent>();
					var a = v.Velocity.ToAngle();
					var s = v.Velocity.Length();
					var c = p.HasComponent<CircleBodyComponent>();

					for (var i = 0; i < 2; i++) {
						var pr = Projectile.Make(pce.Owner, p.Slice, a + 0.2f * (i == 0 ? -1 : 1), s, c, -1, p, p.Scale);

						pr.EntitiesHurt.AddRange(p.EntitiesHurt);
						pr.Center = p.Center;
					}

					p.Break();
				};
			}
			
			return false;
		}
	}
}