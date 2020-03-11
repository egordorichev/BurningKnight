using BurningKnight.assets.items;
using BurningKnight.entity.item;
using BurningKnight.entity.item.stand;
using BurningKnight.level.biome;
using BurningKnight.level.entities.chest;
using BurningKnight.level.rooms.special;
using BurningKnight.level.tile;
using BurningKnight.save;
using BurningKnight.util.geometry;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.spiked {
	public class SpikedRoom : SpecialRoom {
		public override void PaintFloor(Level level) {
			
		}

		public override void Paint(Level level) {
			if (LevelSave.BiomeGenerated is IceBiome) {
				var clip = Painter.Clip;
				Painter.Clip = null;
				Painter.Rect(level, this, 0, Tile.WallB);
				Painter.Clip = clip;
			}
			
			Painter.Fill(level, this, 1, Tile.EvilWall);
			Painter.Fill(level, this, 2, Tile.EvilFloor);
			
			PaintTunnel(level, Tile.EvilFloor);

			var center = GetCenter() * 16 + new Vector2(8);

			switch (Rnd.Int(4)) {
				case 0: {
					for (var i = 0; i < Rnd.Int(1, 3); i++) {
						Items.CreateAndAdd("bk:heart", level.Area).Center = center;
					}
					
					break;
				}

				case 1: {
					for (var i = 0; i < Rnd.Int(1, 4); i++) {
						Items.CreateAndAdd("bk:shield", level.Area).Center = center;
					}

					break;
				}

				case 2: {
					var stand = new ItemStand();
					level.Area.Add(stand);
					stand.BottomCenter = center;
					stand.SetItem(Items.CreateAndAdd(Items.Generate(ItemPool.SpikedRoom), level.Area), null);
					
					break;
				}

				case 3: {
					var a = Rnd.Chance();
					var c = Rnd.Int(1, a ? 3 : 4);

					for (var i = 0; i < c; i++) {
						var chest = a ? (Chest) new StoneChest() : new RedChest();
						level.Area.Add(chest);
						chest.BottomCenter = center - new Vector2((c / 2f - i) * 20, 0);
					}

					break;
				}
			}
		}

		public override void SetupDoors(Level level) {
			foreach (var door in Connected.Values) {
				door.Type = DoorPlaceholder.Variant.Spiked;
			}
		}

		public override bool CanConnect(RoomDef R, Dot P) {
			if (P.X == Left || P.X == Right || P.X == Left + 1 || P.X == Right - 1) {
				return false;
			}
			
			return base.CanConnect(R, P);
		}
	}
}