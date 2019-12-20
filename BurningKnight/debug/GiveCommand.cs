using System;
using BurningKnight.assets;
using BurningKnight.assets.items;
using BurningKnight.assets.mod;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using Lens.input;
using Lens.util;

namespace BurningKnight.debug {
	public class GiveCommand : ConsoleCommand {
		public GiveCommand() {
			_Init();
		}

		protected void _Init() {
			{
				ShortName = "gv";
				Name = "give";
			}
		}

		public override void Run(Console Console, string[] Args) {
			var player = LocalPlayer.Locate(Console.GameArea);

			for (var i = 0; i < Args.Length; i++) {
				var id = Args[i];
				var count = 1;
				var cursed = false;

				if (!id.Contains(":")) {
					id = $"{Mods.BurningKnight}:{id}";
				}

				if (id.EndsWith("_")) {
					id = id.Substring(0, id.Length - 1);
					cursed = true;
				}

				if (i < Args.Length - 1) {
					var c = Args[i + 1];

					if (int.TryParse(c, out count) && count > 0) {
						i++;
					} else {
						count = 1;
					}
				}

				if (id != "bk:coin" && !Items.Has(id)) {
					Console.Print($"Unknown item {id}");
					continue;
				}

				Log.Info($"Giving {id} x{count}");

				for (var j = 0; j < count; j++) {
					var item = Items.CreateAndAdd(id, Console.GameArea);

					if (cursed) {
						item.Scourged = true;
					}

					player?.GetComponent<InventoryComponent>().Pickup(item);
				}
			}
		}
	}
}