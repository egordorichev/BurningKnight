using System;
using BurningKnight.entity.creature;
using BurningKnight.entity.events;
using BurningKnight.entity.level;
using BurningKnight.state;
using Lens.entity.component;
using Lens.util;

namespace BurningKnight.entity.component {
	public class TileInteractionComponent : Component {
		public bool[] LastTouching;
		public bool[] Touching;

		public TileInteractionComponent() {
			Touching = new bool[(int) Tile.Total];
			LastTouching = new bool[(int) Tile.Total];
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Entity is Creature c && c.InAir()) {
				return;
			}
			
			var startX = (int) Math.Floor(Entity.X / 16f);
			var startY = (int) Math.Floor(Entity.Y / 16f);
			var endX = (int) Math.Floor(Entity.Right / 16f);
			var endY = (int) Math.Floor(Entity.Bottom / 16f);

			var level = Run.Level;

			for (int i = 0; i < Touching.Length; i++) {
				LastTouching[i] = Touching[i];
				Touching[i] = false;
			}
			
			for (int x = startX; x <= endX; x++) {
				for (int y = startY; y <= endY; y++) {
					var index = level.ToIndex(x, y);

					if (!level.IsInside(index)) {
						continue;
					}

					var tile = level.Tiles[index];

					if (tile > 0) {
						Touching[tile] = true;
					}

					var liquid = level.Liquid[index];

					if (liquid > 0) {
						Touching[liquid] = true;
					}
				}
			}

			for (int i = 0; i < Touching.Length; i++) {
				CheckTile(i);
			}
		}

		private void CheckTile(int tile) {
			if (!LastTouching[tile] && Touching[tile]) {
				Send(new TileCollisionStartEvent {
					Who = Entity,
					Tile = (Tile) tile
				});
			} else if (LastTouching[tile] && !Touching[tile]) {
				Send(new TileCollisionEndEvent {
					Who = Entity,
					Tile = (Tile) tile
				});
			}
		}
	}
}