using System;
using BurningKnight.assets.items;
using BurningKnight.entity.item;
using BurningKnight.level.tile;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.level.rooms.secret {
	public class SecretChasmRoom : SecretRoom {
		public override void Paint(Level level) {
			Painter.Fill(level, this, 1, Tile.Chasm);
			Painter.Fill(level, this, (int) (Math.Min(GetWidth(), GetHeight()) / 2f) - 1, Tiles.RandomFloor());

			for (var i = 0; i < Random.Int(1, 5); i++) {
				var item = Items.CreateAndAdd(Items.Generate(ItemPool.Consumable), level.Area);
				item.Center = GetCenter() * 16 + new Vector2(Random.Float(-4, 4), Random.Float(-4, 4));
			}
		}
	}
}