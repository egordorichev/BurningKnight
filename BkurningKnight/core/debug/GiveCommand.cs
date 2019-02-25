using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.item;

namespace BurningKnight.core.debug {
	public class GiveCommand : ConsoleCommand {
		protected void _Init() {
			{
				ShortName = "/gv";
				Name = "/give";
			}
		}

		public override Void Run(Console Console, string Args) {
			int Count = 1;

			if (Args.Length == 2) {
				Count = Integer.ValueOf(Args[1]);
			} 

			if (Args.Length > 0 && Args.Length < 3) {
				string Name = Args[0];
				Item Item;

				try {
					ItemRegistry.Pair Clazz = ItemRegistry.Items.Get(Name);

					if (Clazz == null) {
						Console.Print("[red]Unknown item $name");

						return;
					} 

					Item = (Item) Clazz.Type.NewInstance();

					if (Item.IsStackable()) {
						Item.SetCount(Count);
					} 

					ItemHolder ItemHolder = new ItemHolder();
					ItemHolder.SetItem(Item);
					Player.Instance.TryToPickup(ItemHolder);
					Console.Print("[green]Gave " + Name + " (" + Count + ")");
				} catch (Exception) {
					Console.Print("[red]Failed to create item");
					E.PrintStackTrace();
				}
			} 
		}

		public GiveCommand() {
			_Init();
		}
	}
}
