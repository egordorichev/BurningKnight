using System;
using System.Linq;
using BurningKnight.assets.achievements;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.creature.npc.dungeon;
using BurningKnight.entity.door;
using BurningKnight.level.biome;
using BurningKnight.level.tile;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.util.geometry;
using Lens.util;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.special {
	public class NpcSaveRoom : SpecialRoom {
		private class NpcData {
			public NpcData(byte d, string i) {
				Depth = d;
				Id = i;
			}
			
			public byte Depth;
			public string Id;
		}
		
		private static NpcData[] npcs = {
			new NpcData(2, ShopNpc.HatTrader),
			new NpcData(2, ShopNpc.AccessoryTrader),
			new NpcData(3, ShopNpc.ActiveTrader),
			new NpcData(3, ShopNpc.WeaponTrader),
			new NpcData(4, ShopNpc.Snek),
			new NpcData(4, ShopNpc.Boxy),
			new NpcData(5, ShopNpc.Duck),
			new NpcData(6, ShopNpc.Vampire),
			new NpcData(7, ShopNpc.Elon),
			new NpcData(8, ShopNpc.Gobetta),
			new NpcData(9, ShopNpc.Nurse),
			new NpcData(10, ShopNpc.Roger),
			new NpcData(10, ShopNpc.Mike)
		};
		
		public override int GetMaxConnections(Connection Side) {
			return 1;
		}

		public override int GetMinConnections(Connection Side) {
			return Side == Connection.All ? 1 : 0;
		}

		public override bool CanConnect(RoomDef R) {
			return base.CanConnect(R) && !(R is NpcKeyRoom);
		}

		public override int GetMaxWidth() {
			return 10;
		}

		public override int GetMaxHeight() {
			return 10;
		}

		private static bool DefeatedBosses() {
			return Achievements.IsComplete("bk:democracy") && Achievements.IsComplete("bk:mummified") && Achievements.IsComplete("bk:ice_boss") && Achievements.IsComplete("bk:bk_no_more") && Achievements.IsComplete("bk:sting_operation");
		}

		private static string GenerateNpc() {
			var d = Run.Depth;

			foreach (var info in npcs) {
				if (info.Depth == d && GlobalSave.IsFalse(info.Id)) {
					if (info.Id == ShopNpc.Mike && !DefeatedBosses()) {
						continue;
					}

					Log.Info($"Npc {info.Id} should be generated");
					return info.Id;
				}
			}

			Log.Info($"No npc should be generated");
			return null;
		}

		public override void Paint(Level level) {
			var ice = LevelSave.BiomeGenerated is IceBiome;
		
			if (ice) {
				var clip = Painter.Clip;
				Painter.Clip = null;
				Painter.Rect(level, this, 0, Tile.WallB);
				Painter.Clip = clip;
			}
			
			var id = GenerateNpc();

			if (id == null) {
				return;
			}

			GameSave.Put("npc_appeared", true);

			var d = Connected.Values.First();
			var npc = ShopNpc.FromId(id);
			level.Area.Add(npc);

			var fl = Tiles.RandomFloorOrSpike();
			
			if (d.X == Left || d.X == Right) {
				var w = (int) (GetWidth() / 2f + Rnd.Int(-1, 1));
				var door = new Dot(Left + w, Rnd.Int(Top + 2, Bottom - 2));
				
				Painter.DrawLine(level, new Dot(Left + w, Top), new Dot(Left + w, Bottom), ice ? Tile.WallB : Tile.WallA);
				Painter.Set(level, door, fl);
				
				npc.Center = new Dot(Left + w + (d.X == Left ? 2 : -2), Rnd.Int(Top + 2, Bottom - 3)) * 16 + new Vector2(8);

				var dr = new CageDoor {
					Vertical = true
				};
				
				dr.Center = door * 16 + new Vector2(12, 0);
				level.Area.Add(dr);
				
				var v = (d.X == Left ? -1 : 1);
				
				Painter.DrawLine(level, new Dot(Left + w + v, Top + 1), new Dot(Left + w + v, Bottom - 1), Tiles.Pick(Tile.Chasm, Tile.Lava, Tile.SensingSpikeTmp));
				Painter.Set(level, door + new Dot(v, 0), fl);
			} else if (true) {
				var h = (int) (GetHeight() / 2f + Rnd.Int(-1, 1));
				var door = new Dot(Rnd.Int(Left + 2, Right - 2), Top + h);

				Painter.DrawLine(level, new Dot(Left, Top + h), new Dot(Right, Top + h), Tile.WallA);
				Painter.Set(level, door, fl);
				
				npc.Center = new Dot(Rnd.Int(Left + 2, Right - 2), Top + h + (d.Y == Top ? 2 : -2)) * 16 + new Vector2(8);
				
				var dr = new CageDoor();
				dr.Center = door * 16 + new Vector2(7, 8);
				level.Area.Add(dr);

				var v = (d.Y == Top ? -1 : 1);
				
				Painter.DrawLine(level, new Dot(Left + 1, Top + h + v), new Dot(Right - 1, Top + h + v), Tiles.Pick(Tile.Chasm, Tile.Lava, Tile.SensingSpikeTmp));
				Painter.Set(level, door + new Dot(0, v), fl);
			}
		}

		public static bool ShouldBeAdded() {
			if (Run.Type != RunType.Regular || GameSave.IsTrue("npc_appeared")) {
				return false;
			}

			return GenerateNpc() != null;
		}
	}
}