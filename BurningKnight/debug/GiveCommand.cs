using System;
using BurningKnight.assets;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using Lens.input;

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

		public override string AutoComplete(string input) {
			var parts = input.Split(null);

			if (parts.Length != 2) {
				return input;
			}

			var name = parts[1];

			foreach (var item in Items.Datas) {
				if (item.Key.StartsWith(name)) {
					return $"{input}{item.Key.Substring(name.Length, item.Key.Length - name.Length)} ";
				}
			}
			
			return input;
		}

		public override void Run(Console Console, string[] Args) {
			var Count = 1;

			if (Args.Length == 2) {
				Count = Int32.Parse(Args[1]);

				if (Count == 0) {
					return;
				}
			}

			if (Args.Length > 0 && Args.Length < 3) {
				var id = Args[0];

				if (!id.Contains(":")) {
					id = $"{Mods.BurningKnight}:{id}";
				}
				
				var player = LocalPlayer.Locate(Console.GameArea);

				for (var i = 0; i < Count; i++) {
					var item = Items.Create(id);

					if (item == null) {
						Console.Print($"Unknown item {Args[0]}");
						return;
					}

					Console.GameArea.Add(item);
					player?.GetComponent<InventoryComponent>().Pickup(item);
				}
			}
		}
	}
}