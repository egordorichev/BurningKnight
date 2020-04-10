using System;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.creature.mob.prefabs;
using BurningKnight.entity.events;
using Lens.entity;
using Lens.util;

namespace BurningKnight.entity.item.use {
	public class PokemonUse : ItemUse {
		private Type type;

		public override void Use(Entity entity, Item item) {
			base.Use(entity, item);

			if (type == null) {
				return;
			}

			try {
				var m = (Mob) Activator.CreateInstance(type);
				entity.Area.Add(m);

				m.GetComponent<BuffsComponent>().Add(new CharmedBuff {
					Infinite = true
				});

				var h = m.GetComponent<HealthComponent>();
				h.InitMaxHealth = (int) (h.Health * 3);
				
				m.Center = entity.Center;
			} catch (Exception e) {
				Log.Error(e);
			}
		}

		public override bool HandleEvent(Event e) {
			if (e is KilledEvent k && k.KilledBy == Item.Owner && k.Who is Mob m && !(m is WallWalker || m is Boss)) {
				type = k.Who.GetType();
			}
			
			return base.HandleEvent(e);
		}
	}
}