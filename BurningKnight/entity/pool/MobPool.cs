using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.mob.blood;
using BurningKnight.entity.creature.mob.common;
using BurningKnight.entity.creature.mob.desert;
using BurningKnight.entity.creature.mob.forest;
using BurningKnight.entity.creature.mob.hall;
using BurningKnight.entity.creature.mob.ice;
using BurningKnight.entity.creature.mob.library;
using BurningKnight.entity.creature.mob.tech;
using BurningKnight.entity.level.blood;
using BurningKnight.entity.level.levels.desert;
using BurningKnight.entity.level.levels.forest;
using BurningKnight.entity.level.levels.hall;
using BurningKnight.entity.level.levels.ice;
using BurningKnight.entity.level.levels.library;
using BurningKnight.entity.level.levels.tech;
using BurningKnight.util;

namespace BurningKnight.entity.pool {
	public class MobPool {
		public static MobPool Instance = new MobPool();
		protected List<Float> Chances = new List<>();
		protected List<MobHub> Classes = new List<>();
		protected List<Float> Dchances = new List<>();
		protected List<MobHub> Dclasses = new List<>();

		public void InitForRoom() {
			for (var I = 0; I < Dchances.Size(); I++) {
				MobHub Cl = Dclasses.Get(I);
				Classes.Add(Cl);
				Chances.Add(Dchances.Get(I));
				Cl.MaxMatches = Cl.MaxMatchesInitial;
			}

			Dchances.Clear();
			Dclasses.Clear();
		}

		public MobHub Generate() {
			var I = Random.Chances(Chances.ToArray(new Float[0]));

			if (I == -1) {
				Log.Error("-1 as pool result!");

				return null;
			}

			MobHub Hub = Classes.Get(I);

			if (Hub != null) {
				Hub.MaxMatches -= 1;

				if (Hub.MaxMatches == 0) {
					if (!Hub.Once) {
						Dchances.Add(Chances.Get(Classes.IndexOf(Hub)));
						Dclasses.Add(Hub);
					}

					Chances.Remove(Classes.IndexOf(Hub));
					Classes.Remove(Hub);
				}
			}

			return Hub;
		}

		public void Add(MobHub Type, float Chance) {
			Classes.Add(Type);
			Chances.Add(Chance);
		}

		public void Clear() {
			Classes.Clear();
			Chances.Clear();
		}

		private void Add(float Chance, int Max, params Class[] Classes) {
			Add(new MobHub(Chance, Max, Classes), Chance);
		}

		public void InitForFloor() {
			Clear();
			var Chaos = Random.GetSeed().Equals("CHAOS");
			Add(0.2f, -1, MovingFly.GetType());
			Add(0.2f, 1, DiagonalFly.GetType());
			Add(0.05f, 1, SupplyMan.GetType());
			Add(0.05f, 1, CoinMan.GetType());
			Add(0.05f, 1, BombMan.GetType());
			var D = Dungeon.Depth;

			if (Chaos || Dungeon.Level is IceLevel) Add(0.15f, 1, BurningMan.GetType());

			if (Chaos || D > 1) {
				Add(0.1f, -1, MovingFly.GetType(), MovingFly.GetType(), MovingFly.GetType(), MovingFly.GetType());
				Add(0.1f, -1, DiagonalShotFly.GetType());
			}

			if (Chaos || Dungeon.Level is HallLevel) {
				Add(1f, -1, RangedKnight.GetType());
				Add(1f, -1, Knight.GetType());
				Add(1f, -1, Clown.GetType());
				Add(0.6f, -1, Thief.GetType());
			}

			if (Chaos || Dungeon.Level is DesertLevel) {
				Add(1f, -1, Archeologist.GetType());
				Add(1f, -1, Mummy.GetType());
				Add(0.5f, -1, Thief.GetType());
				Add(1f, 1, Skeleton.GetType());
			}

			if (Chaos || Dungeon.Level is ForestLevel) {
				Add(0.5f, 1, Wombat.GetType());
				Add(1f, -1, Hedgehog.GetType());
				Add(1f, 2, Treeman.GetType());
			}

			if (Chaos || Dungeon.Level is LibraryLevel) {
				Add(1f, -1, Mage.GetType());
				Add(1f, -1, Cuok.GetType());
				Add(1f, -1, DiagonalCuok.GetType());
				Add(0.8f, -1, FourSideCuok.GetType());
				Add(0.4f, -1, FourSideCrossCuok.GetType());
				Add(0.2f, 1, SpinningCuok.GetType());
				Add(1.5f, 1, Grandma.GetType());
			}

			if (Chaos || Dungeon.Level is IceLevel) {
				Add(1f, 2, IceElemental.GetType());
				Add(1.5f, -1, Snowball.GetType());
				Add(1f, -1, SnowballFly.GetType());
				Add(0.5f, -1, Snowflake.GetType());
				Add(0.5f, 1, Snowflake.GetType(), Snowflake.GetType(), Snowflake.GetType());
				Add(1f, 1, Roller.GetType());
				Add(1f, 2, Gift.GetType());
			}

			if (Chaos || Dungeon.Level is TechLevel) {
				Add(1f, -1, Vacuum.GetType());
				Add(1f, 2, Factory.GetType());
				Add(0.5f, 1, Factory.GetType(), Repair.GetType());
				Add(0.5f, 1, Vacuum.GetType(), Repair.GetType());
				Add(1f, -1, Batterfly.GetType());
				Add(0.5f, -1, Batterfly.GetType(), Batterfly.GetType(), Batterfly.GetType());
				Add(1f, -1, Tank.GetType());
			}

			if (Chaos || Dungeon.Level is BloodLevel) {
				Add(1f, -1, Zombie.GetType());
				Add(1f, -1, BigZombie.GetType());
				Add(1f, 1, Mother.GetType());
			}
		}
	}
}