using System;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using BurningKnight.level.challenge;
using BurningKnight.save.statistics;
using BurningKnight.state;
using Lens.entity;
using Lens.util;
using Lens.util.file;
using Lens.util.math;

namespace BurningKnight.save {
	public class PlayerSave : EntitySaver {
		public override void Save(Area area, FileWriter writer, bool old) {
			SmartSave(area.Tagged[Tags.PlayerSave], writer);
		}

		public override string GetPath(string path, bool old = false) {
			return $"{path}player.sv";
		}

		public override void Generate(Area area) {
			var player = new LocalPlayer();
			area.Add(player);

			if (Run.Depth > 0) {
				area.Add(new RunStatistics());
				area.Add(new entity.creature.bk.BurningKnight());

				if (Run.Type == RunType.Challenge) {
					var c = ChallengeRegistry.Get(Run.ChallengeId);

					try {
						c?.Apply(player);
					} catch (Exception e) {
						Log.Error(e);
					}
				} else if (Run.Type == RunType.Daily) {
					var count = Rnd.Int(1, 4);
					var inventory = player.GetComponent<InventoryComponent>();
					var pool = Items.GeneratePool(Items.GetPool(ItemPool.Treasure));

					for (var i = 0; i < count; i++) {
						var id = Items.GenerateAndRemove(pool);
						
						Log.Info($"Giving {id}");
						inventory.Add(Items.CreateAndAdd(id, area));
					}

					if (Rnd.Chance(70)) {
						player.GetComponent<ActiveItemComponent>().Set(Items.CreateAndAdd(Items.Generate(ItemType.Active), area), false);
					}
				}
			}
		}

		public PlayerSave() : base(SaveType.Player) {
			
		}
	}
}