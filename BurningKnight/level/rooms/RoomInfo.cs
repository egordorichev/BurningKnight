using System;

namespace BurningKnight.level.rooms {
	public class RoomInfo {
		public Type Room;
		public float Chance;
		public RoomType Type;
		public string[] Biomes;
		
		public static RoomInfo New<T>(float chance, params string[] biomes) where T : RoomDef {
			return new RoomInfo {
				Chance = chance,
				Type = RoomDef.DecideType(typeof(T)),
				Room = typeof(T),
				Biomes = biomes
			};
		}
	}
}