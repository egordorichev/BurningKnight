using System;
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

			foreach (var item in ItemRegistry.Items) {
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
			}

			/*if (Args.Length > 0 && Args.Length < 3) {
				string Name = Args[0];
				Item Item;

				try {
					ItemRegistry.Pair Clazz = ItemRegistry.Items.Get(Name);

					if (Clazz == null) {
						Console.Print("[red]Unknown item $name");

						return;
					}

					Item = (Item) Clazz.Type.NewInstance();

					if (Item.IsStackable()) Item.SetCount(Count);

					var ItemHolder = new ItemHolder();
					ItemHolder.SetItem(Item);
					Player.Instance.TryToPickup(ItemHolder);
					Console.Print("[green]Gave " + Name + " (" + Count + ")");
				}
				catch (Exception) {
					Console.Print("[red]Failed to create item");
				}
			}*/ // todo
		}
	}
}