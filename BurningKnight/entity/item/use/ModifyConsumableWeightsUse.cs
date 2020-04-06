using System;
using BurningKnight.entity.events;
using BurningKnight.util;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class ModifyConsumableWeightsUse : ItemUse {
		private bool keys;
		private bool bombs;
		private bool coins;
		private float amount;

		public override bool HandleEvent(Event e) {
			if (e is ConsumableAddedEvent cae) {
				var t = cae.Type;

				if ((t == ItemType.Coin && coins) || (t == ItemType.Bomb && bombs) || (t == ItemType.Key && keys)) {
					cae.TotalNow -= cae.Amount;
					cae.Amount = (int) Math.Round(cae.Amount * amount);
					cae.TotalNow += cae.Amount;
				}
			}
			
			return base.HandleEvent(e);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			
			amount = settings["amount"].Int(1);
			keys = settings["keys"].Bool(false);
			bombs = settings["bombs"].Bool(false);
			coins = settings["coins"].Bool(true);
		}
		
		public static void RenderDebug(JsonValue root) {
			root.InputInt("Modifier", "amount");
			root.Checkbox("Coins", "coins", true);
			root.Checkbox("Keys", "keys", false);
			root.Checkbox("Bombs", "bombs", false);
		}
	}
}