using BurningKnight.entity.projectile;
using BurningKnight.level.tile;
using BurningKnight.physics;
using Lens.entity;

namespace BurningKnight.level {
	public class ProjectileLevelBody : Entity, CollisionFilterEntity {
		public Level Level;

		public override void AddComponents() {
			base.AddComponents();

			AlwaysActive = true;
			
			AddComponent(new ProjectileBodyComponent {
				Level = Level
			});
		}

		public bool ShouldCollide(Entity entity) {
			return entity is Projectile;
		}

		public void Break(float x, float y) {
			y += 8;
			
			Check((int) (x / 16), (int) (y / 16));
			Check((int) (x / 16 - 0.5f), (int) (y / 16));
			Check((int) (x / 16 + 0.5f), (int) (y / 16));
			Check((int) (x / 16), (int) (y / 16 - 0.5f));
			Check((int) (x / 16), (int) (y / 16 + 0.5f));
		}

		private void Check(int x, int y) {
			if (Level.Get(x, y) == Tile.WallA) {
				Level.Set(x, y, Tile.Ice);
				Level.UpdateTile(x, y);
				Level.ReCreateBodyChunk(x, y);
			}
		}
	}
}