using System.Linq;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.door;
using BurningKnight.level.tile;
using BurningKnight.save;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.special {
	public class NpcSaveRoom : SpecialRoom {
		/*
		 * todo:
		 *
		 * dialog about save me
		 * dialog about thanks for saving and dissappear, gifting some emeralds
		 */
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

		public override void Paint(Level level) {
			var d = Connected.Values.First();
			
			ShopNpc npc;

			if (GlobalSave.IsFalse(ShopNpc.HatTrader)) {
				npc = new HatTrader();
			} else if (GlobalSave.IsFalse(ShopNpc.WeaponTrader)) {
				npc = new WeaponTrader();
			} else if (GlobalSave.IsFalse(ShopNpc.AccessoryTrader)) {
				npc = new AccessoryTrader();
			} else {
				npc = new ActiveTrader();
			}

			level.Area.Add(npc);

			var fl = Tiles.RandomFloorOrSpike();
			
			if (d.X == Left || d.Y == Right) {
				var w = (int) (GetWidth() / 2f + Random.Int(-1, 1));
				var door = new Vector2(Left + w, Random.Int(Top + 2, Bottom - 2));
				
				Painter.DrawLine(level, new Vector2(Left + w, Top), new Vector2(Left + w, Bottom), Tile.WallA);
				Painter.Set(level, door, fl);
				
				npc.Center = new Vector2(Left + w + (d.X == Left ? 2 : -2), Random.Int(Top + 2, Bottom - 3)) * 16 + new Vector2(8);

				var dr = new CageDoor {
					FacingSide = true
				};
				
				dr.Center = door * 16 + new Vector2(12, 0);
				level.Area.Add(dr);
				
				var v = (d.X == Left ? -1 : 1);
				
				Painter.DrawLine(level, new Vector2(Left + w + v, Top + 1), new Vector2(Left + w + v, Bottom - 1), Tiles.Pick(Tile.Chasm, Tile.Lava, Tile.SensingSpikeTmp));
				Painter.Set(level, door + new Vector2(v, 0), fl);
			} else if (true) {
				var h = (int) (GetHeight() / 2f + Random.Int(-1, 1));
				var door = new Vector2(Random.Int(Left + 2, Right - 2), Top + h);

				Painter.DrawLine(level, new Vector2(Left, Top + h), new Vector2(Right, Top + h), Tile.WallA);
				Painter.Set(level, door, fl);
				
				npc.Center = new Vector2(Random.Int(Left + 2, Right - 2), Top + h + (d.Y == Top ? 2 : -2)) * 16 + new Vector2(8);
				
				var dr = new CageDoor();
				dr.Center = door * 16 + new Vector2(8, 8);
				level.Area.Add(dr);

				var v = (d.Y == Top ? -1 : 1);
				
				Painter.DrawLine(level, new Vector2(Left + 1, Top + h + v), new Vector2(Right - 1, Top + h + v), Tiles.Pick(Tile.Chasm, Tile.Lava, Tile.SensingSpikeTmp));
				Painter.Set(level, door + new Vector2(0, v), fl);
			}
		}

		public static bool ShouldBeAdded() {
			return GlobalSave.IsFalse(ShopNpc.AccessoryTrader) || GlobalSave.IsFalse(ShopNpc.ActiveTrader) ||
			       GlobalSave.IsFalse(ShopNpc.WeaponTrader) || GlobalSave.IsFalse(ShopNpc.HatTrader);
		}
	}
}