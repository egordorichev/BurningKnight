using System;
using System.Collections.Generic;
using BurningKnight.entity.creature.npc;
using BurningKnight.level.biome;
using BurningKnight.level.entities.statue;
using BurningKnight.level.rooms.boss;
using BurningKnight.level.rooms.connection;
using BurningKnight.level.rooms.entrance;
using BurningKnight.level.rooms.granny;
using BurningKnight.level.rooms.oldman;
using BurningKnight.level.rooms.regular;
using BurningKnight.level.rooms.secret;
using BurningKnight.level.rooms.shop;
using BurningKnight.level.rooms.shop.sub;
using BurningKnight.level.rooms.special;
using BurningKnight.level.rooms.special.minigame;
using BurningKnight.level.rooms.special.npc;
using BurningKnight.level.rooms.special.shop;
using BurningKnight.level.rooms.special.statue;
using BurningKnight.level.rooms.trap;
using BurningKnight.level.rooms.treasure;
using BurningKnight.level.walls;
using BurningKnight.save;
using Lens.util;
using Lens.util.math;

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
			RoomType.Spiked,
			RoomType.Challenge,
			RoomType.Scourged,
			RoomType.Payed,
			RoomType.DarkMarket,
			RoomType.Treasure,
			RoomType.Entrance,
			RoomType.Trap,
			RoomType.Granny,
			RoomType.OldMan,
			RoomType.SubShop
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
				RoomInfo.New<SecretMachineRoom>(1f),
				RoomInfo.New<SecretChasmRoom>(1f),
				RoomInfo.New<SecretItemRoom>(1f),
				RoomInfo.New<GrannySecretRoom>(0.01f),

				// Regular
				RoomInfo.New<RegularRoom>(WallRegistry.Instance.Size),
				RoomInfo.New<VerticalRegularRoom>(WallRegistry.Instance.Size),
				RoomInfo.New<ItemTrollRoom>(0.2f),
				
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
				RoomInfo.New<TwoDiagonalTreasureRoom>(1.5f),
				RoomInfo.New<AcrossTreasureRoom>(1.5f),
				
				// Trap
				RoomInfo.New<RollingSpikesRoom>(1f),
				RoomInfo.New<SpikePassageRoom>(1f),
				RoomInfo.New<TurretTrapRoom>(1f),
				RoomInfo.New<SpikeMazeRoom>(1f),
				RoomInfo.New<DangerousPadsRoom>(1f),
				RoomInfo.New<TurretPassageRoom>(1f),
				RoomInfo.New<VerticalTurretPassageRoom>(1f),
				RoomInfo.New<CrossTurretPassageRoom>(1f),
				RoomInfo.New<FollowingSpikeBallRoom>(1f),

				// Shop
				RoomInfo.New<ShopRoom>(1f),
				
				// Sub shop
				RoomInfo.New<StorageRoom>(1f),
				RoomInfo.New<SnekShopRoom>(1f, () => GlobalSave.IsTrue(ShopNpc.Snek)),
				RoomInfo.New<VampireShopRoom>(1f, () => GlobalSave.IsTrue(ShopNpc.Vampire)),
				
				// Connection
				RoomInfo.New<TunnelRoom>(1f),
				RoomInfo.New<MazeConnectionRoom>(0.2f),
				RoomInfo.New<RingConnectionRoom>(1f),
				RoomInfo.New<IntersectionConnectionRoom>(0.5f),
				RoomInfo.New<HoleConnectionRoom>(0.3f),
				
				// Special
				RoomInfo.New<IdolTrapRoom>(1f),
				RoomInfo.New<SafeRoom>(1f),
				RoomInfo.New<ChargerRoom>(1f),
				RoomInfo.New<ChestMinigameRoom>(1f),
				RoomInfo.New<VendingRoom>(1f),
				
				RoomInfo.New<ChestStatueRoom>(0.8f),
				RoomInfo.New<DiceStatueRoom>(1f),
				RoomInfo.New<ScourgeStatueRoom>(0.5f),
				RoomInfo.New<StoneStatueRoom>(1f),
				RoomInfo.New<SwordStatueRoom>(1f),
				RoomInfo.New<WarriorStatueRoom>(1f),
				RoomInfo.New<WellRoom>(0.3f),
				RoomInfo.New<ProtoChestRoom>(0.1f),
				
				RoomInfo.New<RogerShopRoom>(1f, () => GlobalSave.IsTrue(ShopNpc.Roger)),
				RoomInfo.New<BoxyShopRoom>(1f, () => GlobalSave.IsTrue(ShopNpc.Boxy)),
				RoomInfo.New<TrashGoblinRoom>(1f),
				RoomInfo.New<DuckRoom>(1f, () => GlobalSave.IsTrue(ShopNpc.Duck)),
				RoomInfo.New<NurseRoom>(1f, () => GlobalSave.IsTrue(ShopNpc.Nurse)),
				RoomInfo.New<ElonRoom>(1f, () => GlobalSave.IsTrue(ShopNpc.Elon)),
				RoomInfo.New<GobettaShopRoom>(1f, () => GlobalSave.IsTrue(ShopNpc.Gobetta)),
				RoomInfo.New<SnekRoom>(1f, () => GlobalSave.IsTrue(ShopNpc.Snek)),
				RoomInfo.New<VampireRoom>(1f, () => GlobalSave.IsTrue(ShopNpc.Vampire)),

				// Boss
				RoomInfo.New<BossRoom>(1f),
				RoomInfo.New<ChasmBossRoom>(1f),
				
				// Granny
				RoomInfo.New<GrannyRoom>(1f),
				
				// Old Man
				RoomInfo.New<OldManRoom>(1f),
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

			var list = new List<RoomInfo>();
			float sum = 0;

			foreach (var t in types) {
				if (biome.IsPresent(t.Biomes) && (t.CanAppear == null || t.CanAppear())) {
					sum += t.Chance;
					list.Add(t);
				}
			}
			
			var length = list.Count;

			float value = Rnd.Float(sum);
			sum = 0;

			for (int i = 0; i < length; i++) {
				var t = list[i];
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