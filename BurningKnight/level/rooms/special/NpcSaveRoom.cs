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
		
		private static string[] townNpcs = {
			ShopNpc.HatTrader, ShopNpc.AccessoryTrader, ShopNpc.ActiveTrader, ShopNpc.WeaponTrader, ShopNpc.Mike
		};

		private static bool DefeatedBosses() {
			return Achievements.IsComplete("bk:democracy") && Achievements.IsComplete("bk:mummified") && Achievements.IsComplete("bk:ice_boss") && Achievements.IsComplete("bk:bk_no_more") && Achievements.IsComplete("bk:sting_operation");
		}

		public override void Paint(Level level) {
			if (LevelSave.BiomeGenerated is IceBiome) {
				var clip = Painter.Clip;
				Painter.Clip = null;
				Painter.Rect(level, this, 0, Tile.WallB);
				Painter.Clip = clip;
			}
			
			GameSave.Put("npc_appeared", true);
			var d = Connected.Values.First();

			ShopNpc npc = null;
			
			foreach (var s in townNpcs) {
				if (GlobalSave.IsFalse(s) && (s != ShopNpc.Mike || DefeatedBosses())) {
					npc = ShopNpc.FromId(s);
					break;
				}
			}

			if (npc == null) {
				foreach (var s in shopNpcs) {
					if (GlobalSave.IsFalse(s)) {
						npc = ShopNpc.FromId(s);
						break;
					}
				}
			}

			if (npc == null) {
				return;
			}
			
			level.Area.Add(npc);

			var fl = Tiles.RandomFloorOrSpike();
			
			if (d.X == Left || d.X == Right) {
				var w = (int) (GetWidth() / 2f + Rnd.Int(-1, 1));
				var door = new Dot(Left + w, Rnd.Int(Top + 2, Bottom - 2));
				
				Painter.DrawLine(level, new Dot(Left + w, Top), new Dot(Left + w, Bottom), Tile.WallA);
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

		private static string[] shopNpcs = {
			ShopNpc.Snek, ShopNpc.Boxy, ShopNpc.Duck, ShopNpc.Vampire,
			ShopNpc.Elon, ShopNpc.Gobetta, ShopNpc.Nurse, ShopNpc.Roger
		};

		public static bool ShouldBeAdded() {
			if (Run.Type != RunType.Regular || GameSave.IsTrue("npc_appeared") || Run.CustomSeed) {
				return false;
			}

			string npc = null;

			switch (Run.Depth) {
				case 1: {
					if (GlobalSave.IsFalse(ShopNpc.HatTrader)) {
						npc = ShopNpc.HatTrader;
					}
					
					break;
				}
				
				case 2: {
					if (GlobalSave.IsFalse(ShopNpc.AccessoryTrader)) {
						npc = ShopNpc.AccessoryTrader;
					}
					
					break;
				}

				case 3: {
					if (GlobalSave.IsFalse(ShopNpc.ActiveTrader)) {
						npc = ShopNpc.ActiveTrader;
					}
					
					break;
				}
				
				case 4: {
					if (GlobalSave.IsFalse(ShopNpc.WeaponTrader)) {
						npc = ShopNpc.WeaponTrader;
					}
					
					break;
				}
				
				case 5: {
					if (GlobalSave.IsFalse(ShopNpc.Mike) && DefeatedBosses()) {
						npc = ShopNpc.Mike;
					}
					
					break;
				}
			}

			if (npc != null) {
				Log.Error($"Npc to save: {npc}");
				return true;
			}

			var i = 0;
			foreach (var s in shopNpcs) {
				if (Run.Depth == i / 2 + 2 && GlobalSave.IsFalse(s)) {
					return true;
				}

				i++;
			}
			
			return false;
		}
	}
}