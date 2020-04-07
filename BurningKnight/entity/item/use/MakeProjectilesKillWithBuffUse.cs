using System;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.events;
using BurningKnight.util;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class MakeProjectilesKillWithBuffUse : ItemUse {
		private Type buff;
		
		public override bool HandleEvent(Event e) {
			if (e is ProjectileCreatedEvent pce) {
				pce.Projectile.OnHurt += (p, w) => {
					if (w is Mob m && w.GetComponent<BuffsComponent>().Buffs.ContainsKey(buff)) {
						m.Kill(Item);
					}
				};
			}
			
			return base.HandleEvent(e);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			var b = settings["buff"].String("bk:frozen");

			if (BuffRegistry.All.TryGetValue(b, out var i)) {
				buff = i.Buff;
			}
		}

		public static void RenderDebug(JsonValue root) {
			root.InputText("Buff", "buff", "bk:frozen");
		}
	}
}