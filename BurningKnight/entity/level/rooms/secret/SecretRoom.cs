using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.level.features;
using BurningKnight.entity.level.painters;
using BurningKnight.entity.level.rooms.connection;
using BurningKnight.entity.level.save;
using BurningKnight.entity.pool;
using BurningKnight.entity.pool.room;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.secret {
	public class SecretRoom : Room {
		public override void Paint(Level Level) {
			Hidden = true;
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, this, 1, Terrain.FLOOR_D);

			foreach (LDoor Door in Connected.Values()) Door.SetType(LDoor.Type.SECRET);
		}

		private float SpawnMob(Mob Mob, Room Room, float Weight) {
			Weight -= Mob.GetWeight();
			Point Point;
			var I = 0;

			do {
				Point = Room.GetRandomCell();

				if (I++ > 40) {
					Log.Error("Failed to place " + Mob.GetClass() + " in room " + Room.GetClass());

					break;
				}
			} while (!Dungeon.Level.CheckFor((int) Point.X, (int) Point.Y, Terrain.PASSABLE));

			if (I <= 40) {
				Mob.Generate();
				Dungeon.Area.Add(Mob);
				LevelSave.Add(Mob);
				Mob.Tp(Point.X * 16, Point.Y * 16);
			}

			return Weight;
		}

		protected void AddEnemies() {
			if (Random.Chance(30) && GameSave.RunId > 10) {
				MobPool.Instance.InitForFloor();
				Log.Info("Spawn modifier is x" + Player.MobSpawnModifier);
				var Weight = (Random.NewFloat(1f, 3f) + GetWidth() * GetHeight() / 128) * Player.MobSpawnModifier;

				while (Weight > 0) {
					var Mobs = MobPool.Instance.Generate();

					foreach (Class Mob in Mobs.Types) {
						try {
							Weight = SpawnMob((Mob) Mob.NewInstance(), this, Weight);
						}
						catch (InstantiationException |

						IllegalAccessException) {
							E.PrintStackTrace();
						}
					}
				}
			}
		}

		public override int GetMinWidth() {
			return 8;
		}

		public int GetMaxWidth() {
			return 12;
		}

		public override int GetMinHeight() {
			return 8;
		}

		public int GetMaxHeight() {
			return 12;
		}

		public override int GetMaxConnections(Connection Side) {
			return 1;
		}

		public override int GetMinConnections(Connection Side) {
			if (Side == Connection.ALL) return 1;

			return 0;
		}

		public static SecretRoom Create() {
			return SecretRoomPool.Instance.Generate();
		}

		public override bool CanConnect(Room R) {
			return !(R is ConnectionRoom) && base.CanConnect(R);
		}
	}
}