using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class MakeProjectileReshootUse : ItemUse {
		public override bool HandleEvent(Event e) {
			if (e is ProjectileCreatedEvent pce && !pce.Projectile.Artificial) {
				pce.Projectile.OnHurt += (p, en) => {
					var room = p.Owner.GetComponent<RoomComponent>().Room;

					if (room == null || room.Tagged[Tags.MustBeKilled].Count == 0) {
						return;
					}

					var target = room.FindClosest(p.Center, Tags.MustBeKilled, ent => ent != en);

					if (target != null) {
						var c = p.HasComponent<CircleBodyComponent>();
						var pr = Projectile.Make(pce.Owner, p.Slice, p.AngleTo(target), 10, c, -1, p, p.Scale);

						pr.EntitiesHurt.AddRange(p.EntitiesHurt);
						pr.Center = p.Center;
					}
				};
			}
			
			return false;
		}
	}
}