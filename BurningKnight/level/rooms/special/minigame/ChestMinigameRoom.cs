using BurningKnight.entity.creature.npc;
using BurningKnight.level.entities.chest;
using BurningKnight.level.floors;
using BurningKnight.level.tile;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.special.minigame {
	public class ChestMinigameRoom : SpecialRoom {
		public override void PaintFloor(Level level) {
			FloorRegistry.Paint(level, this, -1, true);
		}
		
		public override void Paint(Level level) {
			/*
			 * todo: a rare variant that spawns below certaint depth with rainbow chests and the cost set to constant 69
			 */
			
			if (Rnd.Chance()) {
				if (Rnd.Chance()) {
					var tt = Tiles.RandomFloor();
					
					Painter.Set(level, Left + 2, Top + 2, tt, true);
					Painter.Set(level, Right - 2, Top + 2, tt, true);
				}
				
				var t = Tiles.Pick(Tile.WallA, Tile.Chasm, Tile.SensingSpikeTmp, Tile.Planks);
				
				Painter.Set(level, Left + 2, Top + 2, t);
				Painter.Set(level, Right - 2, Top + 2, t);
			}

			var maanex = new Maanex();
			level.Area.Add(maanex);
			maanex.BottomCenter = new Vector2(Left + 4.5f, Top + 2) * 16;

			var prize = Rnd.Int(4);
			
			for (var i = 0; i < 3; i++) {
				var chest = new WoodenChest();
				level.Area.Add(chest);

				chest.Empty = i != prize;
				chest.BottomCenter = new Vector2(Left + 2.5f + i * 2, Bottom - 1.5f) * 16;
			}
		}

		public override int GetMinWidth() {
			return 9;
		}

		public override int GetMaxWidth() {
			return 10;
		}

		public override int GetMinHeight() {
			return 7;
		}

		public override int GetMaxHeight() {
			return 8;
		}
	}
}