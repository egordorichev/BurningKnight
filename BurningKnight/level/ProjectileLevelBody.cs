using BurningKnight.entity;
using BurningKnight.entity.projectile;
using BurningKnight.level.tile;
using BurningKnight.physics;
using BurningKnight.util.geometry;
using Lens.entity;
using Lens.util.camera;

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

		public static void Mine(Level level, float x, float y) {
			y += 8;
			
			CheckMine(level, (int) (x / 16), (int) (y / 16));
			CheckMine(level, (int) (x / 16 - 0.5f), (int) (y / 16));
			CheckMine(level, (int) (x / 16 + 0.5f), (int) (y / 16));
			CheckMine(level, (int) (x / 16), (int) (y / 16 - 0.5f));
			CheckMine(level, (int) (x / 16), (int) (y / 16 + 0.5f));
		}

		private static void CheckMine(Level level, int x, int y) {
			if (level.Get(x, y) == Tile.Planks) {
				level.Set(x, y, Tile.Ember);
				level.UpdateTile(x, y);
				level.ReCreateBodyChunk(x, y);

				Camera.Instance.ShakeMax(3);

				Level.Animate(level.Area, x, y);
			} else if (level.Get(x, y, true).Matches(Tile.Rock, Tile.TintedRock, Tile.MetalBlock)) {
				level.Set(x, y, Tile.Ember);
				level.UpdateTile(x, y);
				level.ReCreateBodyChunk(x, y);

				Camera.Instance.ShakeMax(3);
				
				ExplosionMaker.BreakRock(level, new Dot(x * 16 + 8, y * 16 + 8), x, y, level.Get(x, y, true));
			}
		}
	}
}