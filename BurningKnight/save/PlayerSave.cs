using System;
using BurningKnight.entity.creature.player;
using BurningKnight.level.challenge;
using BurningKnight.save.statistics;
using BurningKnight.state;
using Lens.entity;
using Lens.util;
using Lens.util.file;

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
				}
			}
		}

		public PlayerSave() : base(SaveType.Player) {
			
		}
	}
}