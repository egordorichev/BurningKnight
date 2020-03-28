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
			Rnd.Seed = Run.Seed = Rnd.GenerateSeed(8, Run.DailyId);
			Log.Debug($"1 {Rnd.Int(128)}");

			var player = new LocalPlayer();
			area.Add(player);

			if (Run.Depth > 0) {
				if (Run.Type == RunType.Challenge) {
					var c = ChallengeRegistry.Get(Run.ChallengeId);

					try {
						c?.Apply(player);
					} catch (Exception e) {
						Log.Error(e);
					}
				} else if (Run.Type == RunType.Daily) {
					Log.Debug($"2 {Rnd.Int(128)}");
					
					var count = Rnd.Int(1, 4);
					var inventory = player.GetComponent<InventoryComponent>();
					var pool = Items.GeneratePool(Items.GetPool(ItemPool.Treasure), i => i.Type == ItemType.Artifact);

					for (var i = 0; i < count; i++) {
						var id = Items.GenerateAndRemove(pool);
						
						Log.Info($"Giving {id}");
						inventory.Pickup(Items.CreateAndAdd(id, area), false);
					}

					Player.StartingItem = Rnd.Chance(70) ? Items.Generate(ItemType.Active) : null;
					Player.StartingWeapon = Items.Generate(ItemPool.Weapon);
				}
				
				area.Add(new RunStatistics());
				area.Add(new entity.creature.bk.BurningKnight());
			}
			
			Log.Debug($"3 {Rnd.Int(128)}");
		}

		public PlayerSave() : base(SaveType.Player) {
			
		}
	}
}