using System;

namespace BurningKnight.level.rooms {
	public class RoomInfo {
		public Type Room;
		public float Chance;
		public RoomType Type;
		public string[] Biomes;
		public Func<bool> CanAppear;

		public static RoomInfo New<T>(float chance, params string[] biomes) where T : RoomDef {
			return New<T>(chance, null, biomes);
		}

		public static RoomInfo New<T>(float chance, Func<bool> callback, params string[] biomes) where T : RoomDef {
			return new RoomInfo {
				Chance = chance,
				CanAppear = callback,
				Type = RoomDef.DecideType(null, typeof(T)),
				Room = typeof(T),
				Biomes = biomes
			};
		}
		
		public static RoomInfo Typed<T>(RoomType type, float chance, params string[] biomes) where T : RoomDef {
			return new RoomInfo {
				Chance = chance,
				Type = type,
				Room = typeof(T),
				Biomes = biomes
			};
		}
	}
}