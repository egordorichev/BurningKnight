using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using Lens.entity;
using Lens.util.math;

namespace BurningKnight.entity.item.use {
	public class MakeProjectileReshootUse : ItemUse {
		public override bool HandleEvent(Event e) {
			if (e is ProjectileCreatedEvent pce && !pce.Projectile.HasFlag(ProjectileFlags.Artificial)) {
				ProjectileCallbacks.AttachHurtCallback(pce.Projectile, (p, en) => {
					var room = p.Owner.GetComponent<RoomComponent>().Room;

					if (room == null || room.Tagged[Tags.MustBeKilled].Count == 0 || Rnd.Chance(20)) {
						return;
					}

					var target = room.FindClosest(p.Center, Tags.MustBeKilled, ent => ent != en);

					if (target != null) {
						var c = p.HasComponent<CircleBodyComponent>();
						var builder = new ProjectileBuilder(pce.Owner, p.Slice) {
							RectHitbox = !c,
							Parent = p,
							Scale = p.Scale,
						};

						var pr = builder.Shoot(p.AngleTo(target), 10).Build();

						pr.EntitiesHurt.AddRange(p.EntitiesHurt);
						pr.Center = p.Center;
					}
				});
			}
			
			return false;
		}
	}
}