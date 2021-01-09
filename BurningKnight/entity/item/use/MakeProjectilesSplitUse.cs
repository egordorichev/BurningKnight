using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using Lens.entity;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.entity.item.use {
	public class MakeProjectilesSplitUse : ItemUse {
		public override bool HandleEvent(Event e) {
			if (e is ProjectileCreatedEvent pce) {
				if (pce.Projectile.Parent?.Parent != null || Rnd.Chance(20)) {
					return false;
				}

				ProjectileCallbacks.AttachHurtCallback(pce.Projectile, (p, en) => {
					var v = p.GetAnyComponent<BodyComponent>();
					var a = v.Velocity.ToAngle();
					var s = v.Velocity.Length();
					var c = p.HasComponent<CircleBodyComponent>();

					var builder = new ProjectileBuilder(pce.Owner, p.Slice) {
						Scale = p.Scale,
						RectHitbox = !c,
						Parent = p
					};

					for (var i = 0; i < 2; i++) {
						var pr = builder.Shoot( a + 0.2f * (i == 0 ? -1 : 1), s).Build();

						pr.EntitiesHurt.AddRange(p.EntitiesHurt);
						pr.Center = p.Center;
					}

					p.Break();
				});
			}
			
			return false;
		}
	}
}