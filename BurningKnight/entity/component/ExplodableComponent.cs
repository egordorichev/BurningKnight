using System;
using BurningKnight.entity.events;
using Lens.entity;
using Lens.entity.component;

namespace BurningKnight.entity.component {
	public class ExplodableComponent : Component {
		public Action OnExplosion;

		public void HandleExplosion(Entity entity, Entity origin, float damage) {
			OnExplosion?.Invoke();

			Send(new ExplodedEvent {
				Who = entity,
				Origin = origin ?? entity,
				Damage = damage
			});
		}
	}
}