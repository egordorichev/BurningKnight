using BurningKnight.entity.creature.npc.dungeon;
using BurningKnight.level.entities;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.special.shop {
	public class TrashGoblinRoom : NpcShopRoom {
		private static string[] infos = {
			"box_a",
			"box_b",
			"box_c",
			"box_d",
			"crate_a",
			"crate_b"
		};
		
		public override void Paint(Level level) {
			base.Paint(level);
			TrashGoblin.Place(GetTileCenter() * 16 + new Vector2(8, 8), level.Area);
			
			for (var x = Left + 1; x < Right - 1; x++) {
				for (var y = Top + 1; y < Bottom - 1; y++) {
					if (Rnd.Chance(50)) {
						continue;
					}
					
					var prop = new BreakableProp {
						Sprite = infos[Rnd.Int(infos.Length)]
					};

					level.Area.Add(prop);
					prop.BottomCenter = new Vector2(x + 0.5f, y + 1f) * 16 + Rnd.Vector(-4, 4);
				}
			}
		}
	}
}