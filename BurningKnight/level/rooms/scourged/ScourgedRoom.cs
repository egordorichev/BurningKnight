using BurningKnight.assets.items;
using BurningKnight.entity.creature.npc.dungeon;
using BurningKnight.entity.item;
using BurningKnight.entity.item.stand;
using BurningKnight.level.entities.chest;
using BurningKnight.level.rooms.special;
using BurningKnight.level.tile;
using BurningKnight.state;
using BurningKnight.util.geometry;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.scourged {
	public class ScourgedRoom : SpecialRoom {
		public override void Paint(Level level) {
			var t = Tiles.Pick(Tile.Chasm, Tile.WallA, Tile.WallB, Tile.Planks);

			if (Rnd.Chance()) {
				Painter.Set(level, new Dot(Left + 1, Top + 1), t);
				Painter.Set(level, new Dot(Right - 1, Bottom - 1), t);
			}

			if (Rnd.Chance()) {
				Painter.Set(level, new Dot(Right - 1, Top + 1), t);
				Painter.Set(level, new Dot(Left + 1, Bottom - 1), t);
			}

			if (Rnd.Chance(10 + Run.Scourge * 5)) {
				var cn = GetCenter() * 16;
				var c = Rnd.Int(2, 4);

				for (var i = 0; i < c; i++) {
					var stand = new ScourgedStand();
					level.Area.Add(stand);
					stand.Center = cn + new Vector2(16 + (i - c / 2f) * 32, 8);

					stand.SetItem(Items.CreateAndAdd(Scourge.GenerateItemId(), level.Area), stand);
				}

				return;
			}
			
			var center = GetCenter() * 16 + new Vector2(8);

			if (Rnd.Chance(5)) {
				var chest = new ProtoChest();
				level.Area.Add(chest);
				chest.BottomCenter = center;

				return;
			}
			
			switch (Rnd.Int(6)) {
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
					stand.SetItem(Items.CreateAndAdd(Items.Generate(ItemPool.Treasure), level.Area), null);

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
				
				case 4: {
					for (var i = 0; i < Rnd.Int(6, 12); i++) {
						Items.CreateAndAdd("bk:coin", level.Area).Center = center;
					}

					break;
				}

				case 5: {
					Gobetta.Place(GetTileCenter() * 16 + new Vector2(8, 8), level.Area);
					break;
				}
			}
		}

		public override void SetupDoors(Level level) {
			foreach (var door in Connected.Values) {
				door.Type = DoorPlaceholder.Variant.Scourged;
			}
		}

		public override bool CanConnect(RoomDef R, Dot P) {
			if (P.X == Left || P.X == Right) {
				return false;
			}
		
			return base.CanConnect(R, P);
		}

		public override int GetMinWidth() {
			return 8;
		}

		public override int GetMinHeight() {
			return 8;
		}

		public override int GetMaxWidth() {
			return 12;
		}

		public override int GetMaxHeight() {
			return 12;
		}

		protected override bool Quad() {
			return Rnd.Chance();
		}
	}
}