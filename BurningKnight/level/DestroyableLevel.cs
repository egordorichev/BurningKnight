using System;
using BurningKnight.level.tile;
using Lens.entity;

namespace BurningKnight.level {
	public class DestroyableLevel : Entity {
		public Level Level;
		
		public override void AddComponents() {
			base.AddComponents();
			AddComponent(new DestroyableBodyComponent());
		}

		public void Break(float x, float y) {
			var tx = (int) Math.Floor(x / 16);
			var ty = (int) Math.Floor(y / 16);

			if (!Level.IsInside(tx, ty)) {
				return;
			}

			var index = Level.ToIndex(tx, ty);
			var tile = Level.Tiles[index];

			if ((Tile) tile != Tile.Planks) {
				return;
			}

			Level.Tiles[index] = (byte) Tile.FloorA;
			Level.UpdateTile(tx, ty);
			
			GetComponent<DestroyableBodyComponent>().CreateBody();
		}
	}
}