using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.state;
using BurningKnight.util;
using Lens.entity;
using Lens.lightJson;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.entity.item.use {
	public class ChanceToUseWeaponUse : ItemUse {
		private bool back;
		private bool up;
		private bool down;
		private float chance;

		public override bool HandleEvent(Event e) {
			if (e is ItemUsedEvent ite && !ite.Fake && ite.Item != Item) {
				if (Rnd.Chance(chance + Run.Luck * 10)) {
					var weapon = Item.Owner.GetComponent<ActiveWeaponComponent>().Item;

					if (weapon == null) {
						return base.HandleEvent(e);
					}

					var aim = Item.Owner.GetComponent<AimComponent>();
					var a = (aim.RealAim - aim.Center).ToAngle();

					if (up) {
						aim.RealAim = aim.Aim = MathUtils.CreateVector(a + Math.PI * 0.5f, 48) + aim.Center;
						weapon.Use(Item.Owner, true);
					}
					
					if (down) {
						aim.RealAim = aim.Aim = MathUtils.CreateVector(a + Math.PI * 1.5f, 48) + aim.Center;
						weapon.Use(Item.Owner, true);
					}
					
					if (back) {
						aim.RealAim = aim.Aim = MathUtils.CreateVector(a + Math.PI, 48) + aim.Center;
						weapon.Use(Item.Owner, true);
					}
				}
			}
			
			return base.HandleEvent(e);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			
			back = settings["back"].Bool(false);
			up = settings["up"].Bool(false);
			down = settings["down"].Bool(false);
			chance = settings["chance"].Number(10f);
		}

		public static void RenderDebug(JsonValue root) {
			root.Checkbox("Back", "back", false);
			root.Checkbox("Up", "up", false);
			root.Checkbox("Down", "down", false);
			root.InputFloat("Chance", "chance", 10);
		}
	}
}