using System;
using System.Collections.Generic;
using BurningKnight.entity.level.rooms.regular;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.level.rooms {
	public static class RoomRegistry {
		public static List<RoomInfo> All = new List<RoomInfo>();
		public static Dictionary<RoomType, List<RoomInfo>> ByType = new Dictionary<RoomType, List<RoomInfo>>();
		
		static RoomRegistry() {
			RoomInfo[] infos = {
				RoomInfo.New<RegularRoom>(1f, RoomType.Regular)
			};

			foreach (var info in infos) {
				Add(info);
			}
		}

		public static void Add(RoomInfo info) {
			All.Add(info);

			if (ByType.TryGetValue(info.Type, out var rooms)) {
				rooms.Add(info);
			} else {
				rooms = new List<RoomInfo>();
				rooms.Add(info);
				
				ByType[info.Type] = rooms;
			}
		}

		public static void Remove(RoomInfo info) {
			All.Remove(info);
			ByType[info.Type].Remove(info);
		}

		public static RoomDef Generate(RoomType type) {
			if (!ByType.TryGetValue(type, out var types)) {
				return null;
			}
			
			var length = types.Count;
			float sum = 0;

			foreach (var chance in types) {
				sum += chance.Chance;
			}

			float value = Random.Float(sum);
			sum = 0;

			for (int i = 0; i < length; i++) {
				sum += types[i].Chance;

				if (value < sum) {
					return (RoomDef) Activator.CreateInstance(types[i].Room);
				}
			}

			return null;
		}
	}
}