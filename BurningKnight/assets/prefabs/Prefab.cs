using BurningKnight.level;
using BurningKnight.level.rooms;
using Lens;
using Lens.entity;
using Lens.util;
using Microsoft.Xna.Framework;

namespace BurningKnight.assets.prefabs {
	public class Prefab {
		public Area Area = new Area { NoInit = true };
		public Level Level;

		public void Place(Level level, int x, int y) {
			var pos = new Vector2(x * 16, y * 16);
			
			foreach (var e in Area.GetEntites()) {
				if (e is Level || e is Room) {
					continue;
				}

				var ee = Cloner.DeepClone(e);
				ee.Position += pos;
				level.Area.Add(ee);
			}

			for (int ty = 0; ty < Level.Height && ty + y < level.Height; ty++) {
				for (int tx = 0; tx < Level.Width && tx + x < level.Width; tx++) {
					var i = Level.ToIndex(tx, ty);
					var i2 = level.ToIndex(tx + x, ty + y);
					
					level.Tiles[i2] = Level.Tiles[i];
					level.Liquid[i2] = Level.Liquid[i];
				}
			}
		}
	}
}