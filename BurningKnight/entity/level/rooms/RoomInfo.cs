using System;
using BurningKnight.entity.level.rooms.treasure;
using Lens.util;

namespace BurningKnight.entity.level.rooms {
	public class RoomInfo {
		public Type Room;
		public float Chance;
		public RoomType Type;
		public string[] Biomes;
		
		public static RoomInfo New<T>(float chance, params string[] biomes) where T : RoomDef {
			if (typeof(T) == typeof(TreasureRoom)) {
				Log.Debug(RoomDef.DecideType(typeof(T)));
				Log.Debug(RoomDef.DecideType(typeof(T)));
			}
			
			return new RoomInfo {
				Chance = chance,
				Type = RoomDef.DecideType(typeof(T)),
				Room = typeof(T),
				Biomes = biomes
			};
		}
	}
}