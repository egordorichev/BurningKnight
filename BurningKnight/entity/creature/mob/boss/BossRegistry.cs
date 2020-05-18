using System;
using System.Collections.Generic;
using BurningKnight.level.biome;
using BurningKnight.state;
using Lens.util.math;

namespace BurningKnight.entity.creature.mob.boss {
	public static class BossRegistry {
		public static List<BossInfo> All = new List<BossInfo>();
		
		static BossRegistry() {
			BossInfo[] infos = {
				BossInfo.New<OldKing>(new SpawnChance(1f, Biome.Castle)),
				BossInfo.New<Pharaoh>(new SpawnChance(1f, Biome.Desert)),
				BossInfo.New<QueenBee>(new SpawnChance(1f, Biome.Jungle)),
				BossInfo.New<IceQueen>(new SpawnChance(1f, Biome.Ice)),
				BossInfo.New<DM>(new SpawnChance(1f, Biome.Tech))
			};
			
			All.AddRange(infos);
		}
		
		public static Boss Generate() {
			var current = new List<BossInfo>();
			
			foreach (var info in All) {
				if (info.SpawnsIn(Run.Level.Biome.Id)) {
					current.Add(info);
				}
			}
			
			var chances = new float[current.Count];

			for (var i = 0; i < current.Count; i++) {
				chances[i] = current[i].GetChanceFor(Run.Level.Biome.Id).Chance;
			}

			var index = Rnd.Chances(chances);

			if (index == -1) {
				return null;
			}

			return (Boss) Activator.CreateInstance(current[index].Type);
		}
	}
}