using System;
using BurningKnight.entity.room;
using BurningKnight.level;
using BurningKnight.level.rooms;
using BurningKnight.save;
using Lens.util.file;
using Microsoft.Xna.Framework;

namespace BurningKnight.assets.prefabs {
	public class Prefab {
		public PrefabData[] Datas;
		public Level Level;

		public void Place(Level level, int x, int y) {
			var pos = new Vector2(x * 16, y * 16);
			var reader = new FileReader(null);
			
			foreach (var d in Datas) {
				if (d.Type == typeof(Level)) {
					continue;
				}

				var e = (SaveableEntity) Activator.CreateInstance(d.Type);
				
				level.Area.Add(e, false);
				reader.SetData(d.Data);

				e.Load(reader);

				if (e is Room r) {
					r.MapX += x;
					r.MapY += y;
				} else {
					e.Position += pos;
				}
				
				e.PostInit();
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