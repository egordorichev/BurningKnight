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
			Set((int) Math.Floor(x / 16), (int) Math.Floor(y / 16));
			
			Set((int) Math.Floor(x / 16 - 0.5f), (int) Math.Floor(y / 16));
			Set((int) Math.Floor(x / 16 + 0.5f), (int) Math.Floor(y / 16));
			Set((int) Math.Floor(x / 16), (int) Math.Floor(y / 16 - 0.5f));
			Set((int) Math.Floor(x / 16), (int) Math.Floor(y / 16 + 0.5f));
		}

		private void Set(int tx, int ty) {

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