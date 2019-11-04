using System;
using System.Collections.Generic;
using BurningKnight.level.biome;
using BurningKnight.level.rooms.boss;
using BurningKnight.level.rooms.connection;
using BurningKnight.level.rooms.entrance;
using BurningKnight.level.rooms.regular;
using BurningKnight.level.rooms.secret;
using BurningKnight.level.rooms.shop;
using BurningKnight.level.rooms.special;
using BurningKnight.level.rooms.special.minigame;
using BurningKnight.level.rooms.trap;
using BurningKnight.level.rooms.treasure;
using BurningKnight.level.walls;
using Lens.util;
using Random = Lens.util.math.Random;

namespace BurningKnight.level.rooms {
	public static class RoomRegistry {
		public static List<RoomInfo> All = new List<RoomInfo>();
		public static Dictionary<RoomType, List<RoomInfo>> ByType = new Dictionary<RoomType, List<RoomInfo>>();

		public static readonly RoomType[] TypesByIndex = {
			RoomType.Regular,
			RoomType.Secret,
			RoomType.Connection,
			RoomType.Boss,
			RoomType.Exit,
			RoomType.Special,
			RoomType.Shop,
			RoomType.Treasure,
			RoomType.Entrance,
			RoomType.Trap
		};

		public static RoomType FromIndex(int i) {
			return TypesByIndex[i];
		}

		public static int FromType(RoomType type) {
			for (var i = 0; i < TypesByIndex.Length; i++) {
				if (TypesByIndex[i] == type) {
					return i;
				}
			}

			return 0;
		}
		
		static RoomRegistry() {
			RoomInfo[] infos = {
				// Secret
				// RoomInfo.New<SecretMachineRoom>(1f), - pretty bad
				RoomInfo.New<SecretChasmRoom>(1f),
				RoomInfo.New<GrannyRoom>(0.01f),

				// Regular
				RoomInfo.New<RegularRoom>(WallRegistry.Instance.Size),
				
				// Desert only room designs
				/*RoomInfo.New<TwoSidesRoom>(3f, Biome.Desert),
				RoomInfo.New<PlatformChaosRoom>(1f, Biome.Desert),
				RoomInfo.New<PlatformRingRoom>(1f, Biome.Desert),*/

				// Entrance
				RoomInfo.New<EntranceRoom>(1f),
				
				// Exit
				RoomInfo.Typed<EntranceRoom>(RoomType.Exit, 1f),
				
				// Treasure
				RoomInfo.New<HoleTreasureRoom>(1f),
				RoomInfo.New<PlatformTreasureRoom>(1f),
				RoomInfo.New<PadTreasureRoom>(1f),
				
				// Trap
				RoomInfo.New<RollingSpikesRoom>(1f),
				RoomInfo.New<SpikePassageRoom>(1f),
				RoomInfo.New<FollowingSpikeBallRoom>(1f),
				// RoomInfo.New<CageRoom>(1f), // I dont like it :(

				// Shop
				RoomInfo.New<ShopRoom>(1f),
				
				// Connection
				RoomInfo.New<TunnelRoom>(1f),
				RoomInfo.New<MazeConnectionRoom>(0.2f),
				RoomInfo.New<RingConnectionRoom>(1f),
				RoomInfo.New<IntersectionConnectionRoom>(0.5f),
				RoomInfo.New<HoleConnectionRoom>(0.3f),
				
				// Special
				RoomInfo.New<IdolTrapRoom>(1f),
				RoomInfo.New<WellRoom>(1f),
				RoomInfo.New<SafeRoom>(1f),
				RoomInfo.New<ChargerRoom>(1f),
				RoomInfo.New<ChestMinigameRoom>(1f + 1000000f),
				
				// Boss
				RoomInfo.New<ChasmBossRoom>(1f)
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

		public static RoomDef Generate(RoomType type, Biome biome) {
			if (!ByType.TryGetValue(type, out var types)) {
				Log.Error($"No rooms registered with type {type}");
				return null;
			}
			
			var length = types.Count;
			float sum = 0;

			foreach (var chance in types) {
				if (biome.IsPresent(chance.Biomes)) {
					sum += chance.Chance;
				}
			}

			float value = Random.Float(sum);
			sum = 0;

			for (int i = 0; i < length; i++) {
				var t = types[i];
				
				if (!biome.IsPresent(t.Biomes)) {
					continue;
				}

				sum += t.Chance;

				if (value < sum) {
					return (RoomDef) Activator.CreateInstance(t.Room);
				}
			}
			
			Log.Error($"Failed to generate a room with type {type}");

			return null;
		}
	}
}