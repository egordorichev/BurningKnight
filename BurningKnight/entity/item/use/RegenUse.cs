using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.level.entities.chest;
using BurningKnight.util;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class RegenUse : ItemUse {
		private int speed;
		private int count;
		private bool onKills;
		private bool onChests;
		private bool onPurchase;

		public override void Use(Entity entity, Item item) {
			base.Use(entity, item);
			entity.Subscribe<Chest.OpenedEvent>();
		}

		public override bool HandleEvent(Event e) {
			if (e is KilledEvent) {
				if (onKills) {
					count++;

					if (count >= speed) {
						count = 0;
						Item.Owner.GetComponent<HealthComponent>().ModifyHealth(1, Item);
					}
				}
			} else if (e is Chest.OpenedEvent) {
				if (onChests) {
					Item.Owner.GetComponent<HealthComponent>().ModifyHealth(1, Item);
				}
			} else if (e is ItemBoughtEvent) {
				if (onPurchase) {
					Item.Owner.GetComponent<HealthComponent>().ModifyHealth(1, Item);
				}
			}
			
			return base.HandleEvent(e);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			
			speed = settings["speed"].Int(10);
			onKills = settings["kills"].Bool(true);
			onChests = settings["chests"].Bool(false);
			onPurchase = settings["purchase"].Bool(false);
		}

		public static void RenderDebug(JsonValue root) {
			if (root.Checkbox("On Kills", "kills", true)) {
				root.InputInt("Speed", "speed", 10);
			}

			root.Checkbox("On Chests", "chests", false);
			root.Checkbox("On Purchase", "purchase", false);
		}
	}
}