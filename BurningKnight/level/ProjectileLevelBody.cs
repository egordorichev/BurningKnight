using BurningKnight.assets.particle;
using BurningKnight.entity;
using BurningKnight.entity.creature.pet;
using BurningKnight.entity.fx;
using BurningKnight.entity.projectile;
using BurningKnight.level.tile;
using BurningKnight.physics;
using BurningKnight.util.geometry;
using Lens;
using Lens.entity;
using Lens.util.camera;
using Microsoft.Xna.Framework;

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
			return entity is Projectile || entity is DiagonalPet;
		}

		public bool Break(float x, float y) {
			y += 8;
			var a = Check((int) (x / 16), (int) (y / 16));
			
			a = Check((int) (x / 16 - 0.5f), (int) (y / 16)) || a;
			a = Check((int) (x / 16 + 0.5f), (int) (y / 16)) || a;
			a = Check((int) (x / 16), (int) (y / 16 - 0.5f)) || a;
			a = Check((int) (x / 16), (int) (y / 16 + 0.5f)) || a;

			return a;
		}

		private bool Check(int x, int y) {
			if (Level.Get(x, y) == Tile.WallA) {
				Level.Set(x, y, Tile.Ice);
				Level.UpdateTile(x, y);
				Level.ReCreateBodyChunk(x, y);
				
				Level.Area.Add(new TileFx {
					X = x * 16,
					Y = y * 16 - 8
				});
			
				for (var i = 0; i < 3; i++) {
					var part = new ParticleEntity(Particles.Dust());
						
					part.Position = new Vector2(x * 16 + 8, y * 16 + 8);
					Level.Area.Add(part);
				}
		
				Engine.Instance.Freeze = 0.5f;

				return true;
			}

			return false;
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