using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using Lens;
using Lens.entity;
using Lens.util.file;

namespace BurningKnight.save {
	public class PlayerSave : EntitySaver {
		public override void Save(Area area, FileWriter writer) {
			SmartSave(area.Tags[Tags.PlayerSave], writer);
		}

		public override string GetPath(string path, bool old = false) {
			return $"{path}player.sv";
		}

		public override void Generate(Area area) {
			var player = new LocalPlayer();
			area.Add(player);

			if (Engine.Version.Dev) {
				// Simple inventory simulation
				var inventory = player.GetComponent<InventoryComponent>();
				inventory.Pickup(Items.CreateAndAdd("bk:sword", area));
			}

			//var bk = new entity.creature.BurningKnight();
			//area.Add(bk);
		}

		public PlayerSave() : base(SaveType.Player) {
			
		}
	}
}