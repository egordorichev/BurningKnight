using BurningKnight.entity.events;
using BurningKnight.entity.item.util;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class ModEachAttackUse : ItemUse {
		private int count;
		
		public override bool HandleEvent(Event e) {
			if (e is ItemUsedEvent iue && iue.Item.Type == ItemType.Weapon) {
				count = (count + 1) % 5;
			} else if (count == 0) {
				if (e is ProjectileCreatedEvent pce) {
					pce.Projectile.Scale *= 2;
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}