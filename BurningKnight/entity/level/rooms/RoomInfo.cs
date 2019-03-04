using System;

namespace BurningKnight.entity.level.rooms {
	public class RoomInfo {
		public Type Room;
		public float Chance;
		public RoomType Type;
		public string[] Biomes;
		
		public static RoomInfo New<T>(float chance, RoomType type = RoomType.Regular, params string[] biomes) where T : RoomDef {
			return new RoomInfo {
				Chance = chance,
				Type = type,
				Biomes = biomes
			};
		}
	}
}