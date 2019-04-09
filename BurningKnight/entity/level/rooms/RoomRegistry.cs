using System;
using System.Collections.Generic;
using BurningKnight.entity.level.rooms.boss;
using BurningKnight.entity.level.rooms.connection;
using BurningKnight.entity.level.rooms.entrance;
using BurningKnight.entity.level.rooms.regular;
using BurningKnight.entity.level.rooms.secret;
using BurningKnight.entity.level.rooms.shop;
using BurningKnight.entity.level.rooms.treasure;
using Lens.util;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.level.rooms {
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
			RoomType.Entrance
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
				RoomInfo.New<RegularRoom>(1f),
				RoomInfo.New<EntranceRoom>(1f),
				RoomInfo.New<ExitRoom>(1f),
				// RoomInfo.New<BossRoom>(1f),
				RoomInfo.New<SecretRoom>(1f),
				RoomInfo.New<TreasureRoom>(1f),
				RoomInfo.New<ShopRoom>(1f),
				
				RoomInfo.New<TunnelRoom>(1f),
				RoomInfo.New<WayOverChasmRoom>(1f)
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
				Log.Error($"No rooms registered with type {type}");
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
			
			Log.Error($"Failed to generate a room with type {type}");

			return null;
		}
	}
}