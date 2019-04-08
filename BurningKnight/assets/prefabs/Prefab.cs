using BurningKnight.entity.level;
using Lens.entity;
using Microsoft.Xna.Framework;

namespace BurningKnight.assets.prefabs {
	public class Prefab {
		public Area Area = new Area { NoInit = true };
		public Level Level;

		public void Place(Level level, int x, int y) {
			var pos = new Vector2(x * 16, y * 16);
			
			foreach (var e in Area.GetEntites()) {
				e.Position += pos;
				level.Area.Add(e);
			}

			for (int ty = 0; ty < Level.Height; ty++) {
				for (int tx = 0; tx < Level.Width; tx++) {
					var i = Level.ToIndex(tx, ty);
					var i2 = level.ToIndex(ty + x, tx + y);
					
					level.Tiles[i2] = Level.Tiles[i];
					level.Liquid[i2] = Level.Liquid[i];
				}
			}
		}
	}
}