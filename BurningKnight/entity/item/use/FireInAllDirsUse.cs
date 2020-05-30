using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using Lens.entity;
using Lens.util;

namespace BurningKnight.entity.item.use {
	public class FireInAllDirsUse : ItemUse {
		public override void Use(Entity entity, Item item) {
			var weapon = entity.GetComponent<ActiveWeaponComponent>().Item;

			if (weapon == null) {
				return;
			}

			var aim = entity.GetComponent<AimComponent>();
			

			for (var i = 0; i < 8; i++) {
				var angle = i / 4f * (float) Math.PI;
				aim.RealAim = aim.Aim = MathUtils.CreateVector(angle, 48) + aim.Center;

				weapon.Use(entity, true);
			}
		}
	}
}