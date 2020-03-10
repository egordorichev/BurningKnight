using System;
using BurningKnight.assets.items;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.item;
using BurningKnight.entity.item.stand;
using BurningKnight.level.biome;
using BurningKnight.level.rooms.boss;
using BurningKnight.level.rooms.special;
using BurningKnight.level.tile;
using BurningKnight.save;
using BurningKnight.util.geometry;
using Lens.util;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.granny {
	public class GrannyRoom : SpecialRoom {
		public override void PaintFloor(Level level) {
			
		}

		public override void SetupDoors(Level level) {
			foreach (var door in Connected.Values) {
				door.Type = DoorPlaceholder.Variant.Hidden;
			}
		}

		public override void Paint(Level level) {
			Painter.Rect(level, this, 1, Tile.GrannyWall);
			Painter.Fill(level, this, 2, Tile.GrannyFloor);
			
			PaintTunnel(level, Tile.GrannyFloor, GetCenterRect(), false, false, false);
			
			var granny = new Granny();
			level.Area.Add(granny);
			granny.BottomCenter = new Vector2(Left + GetWidth() / 2 + 0.5f, Top + 3) * 16;

			var count = Math.Ceiling((GetWidth() - 6) / 2f);
			var pool = Items.GeneratePool(Items.GetPool(ItemPool.Granny));

			for (var i = 0; i < count; i++) {
				var stand = new GrannyStand();
				level.Area.Add(stand);
				stand.Center = new Vector2(Left + 3.5f + i * 2, Top + 4.5f) * 16;
				
				stand.SetItem(Items.CreateAndAdd(Items.GenerateAndRemove(pool), level.Area), null);
			}
		}

		public override bool CanConnect(RoomDef R) {
			return R is BossRoom;
		}

		public override bool CanConnect(RoomDef r, Dot p) {
			if (p.X == Left + 1 || p.X == Right - 1 || p.Y == Top + 1 || p.Y == Bottom - 1) {
				return false;
			}
			
			return base.CanConnect(r, p);
		}

		public override int GetMinHeight() {
			return 8;
		}

		public override int GetMaxHeight() {
			return 11;
		}

		public override int GetMinWidth() {
			return 7;
		}

		public override int GetMaxWidth() {
			return 14;
		}

		protected override int ValidateWidth(int W) {
			return W % 2 == 0 ? W : W - 1;
		}
	}
}