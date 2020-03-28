using System;
using BurningKnight.assets.items;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.item;
using BurningKnight.entity.item.stand;
using BurningKnight.level.biome;
using BurningKnight.level.entities;
using BurningKnight.level.rooms.boss;
using BurningKnight.level.rooms.special;
using BurningKnight.level.tile;
using BurningKnight.save;
using BurningKnight.util.geometry;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.oldman {
	public class OldManRoom : SpecialRoom {
		public override void PaintFloor(Level level) {
			
		}

		public override void SetupDoors(Level level) {
			foreach (var door in Connected.Values) {
				door.Type = DoorPlaceholder.Variant.Hidden;
			}
		}

		public override void Paint(Level level) {
			Painter.Rect(level, this, 1, Tile.EvilWall);
			Painter.Fill(level, this, 2, Tile.EvilFloor);
			
			PaintTunnel(level, Tile.EvilFloor, GetCenterRect(), false, false, false);
			
			var dm = new DarkMage();
			level.Area.Add(dm);

			var w = new Vector2(GetCenterVector().X, (Top + 3) * 16);
			dm.BottomCenter = w;

			for (var i = 0; i < 2; i++) {
				var campfire = new Campfire();
				level.Area.Add(campfire);
				campfire.BottomCenter = w + new Vector2(24 * (i == 0 ? -1 : 1), 0);
			}

			var count = Math.Ceiling((GetWidth() - 6) / 2f);
			var pool = Items.GeneratePool(Items.GetPool(ItemPool.OldMan));

			for (var i = 0; i < count; i++) {
				var stand = new DarkMageStand();
				level.Area.Add(stand);
				stand.Center = new Vector2(Left + 3.5f + i * 2, Top + 4.5f) * 16;
				
				stand.SetItem(Items.CreateAndAdd(Items.GenerateAndRemove(pool), level.Area, false), null);
			}
		}

		public override bool CanConnect(RoomDef R) {
			return R is BossRoom;
		}

		public override int GetMinHeight() {
			return 9;
		}

		public override int GetMaxHeight() {
			return 11;
		}

		public override int GetMinWidth() {
			return 9;
		}

		public override int GetMaxWidth() {
			return 16;
		}

		protected override int ValidateWidth(int W) {
			return W % 2 == 0 ? W : W - 1;
		}
	}
}