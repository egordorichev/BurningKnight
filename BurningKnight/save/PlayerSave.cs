using System;
using System.Collections.Generic;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using BurningKnight.entity.twitch;
using BurningKnight.level.challenge;
using BurningKnight.save.statistics;
using BurningKnight.state;
using Lens.entity;
using Lens.input;
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
			if (Run.Depth < -2) { // Cutscenes
				return;
			}
			
			var player = new LocalPlayer();
			area.Add(player);

			
			var input = player.GetComponent<InputComponent>();
			
			input.Index = 0;
			input.KeyboardEnabled = true;
			input.GamepadEnabled = false;

			input = area.Add(new LocalPlayer()).GetComponent<InputComponent>();
			
			input.Index = 1;
			input.KeyboardEnabled = false;
			input.GamepadEnabled = true;

			if (Run.Depth > 0) {
				if (Run.Type == RunType.Challenge) {
					var c = ChallengeRegistry.Get(Run.ChallengeId);

					try {
						c?.Apply(player);
					} catch (Exception e) {
						Log.Error(e);
					}
				} else if (Run.Type == RunType.Daily) {
					Rnd.Seed = Run.Seed;
					
					var count = Rnd.Int(1, 4);

					var pool = Items.GeneratePool(Items.GetPool(ItemPool.Treasure), i => i.Type == ItemType.Artifact);
					Log.Debug($"Pool count: {pool.Count}");
					Player.DailyItems = new List<string>();

					for (var i = 0; i < count; i++) {
						Player.DailyItems.Add(Items.GenerateAndRemove(pool));
					}

					var si = Rnd.Chance(70) ? Items.Generate(ItemType.Active) : null;
					var sw = Items.Generate(ItemPool.Weapon);
					var sl = Items.Generate(ItemType.Lamp);

					for (var i = 0; i < Player.MaxPlayers; i++) {
						Player.StartingItems[i] = si;
						Player.StartingWeapons[i] = sw;
						Player.StartingLamps[i] = sl;
					}
				}
				
				area.Add(new RunStatistics());
				area.Add(new entity.creature.bk.BurningKnight());
			} else if (Run.Depth == -2) {
				area.Add(new RunStatistics());
			}
		}

		public PlayerSave() : base(SaveType.Player) {
			
		}
	}
}