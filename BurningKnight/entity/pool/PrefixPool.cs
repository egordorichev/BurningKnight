using BurningKnight.entity.creature.mob.prefix;
using BurningKnight.util;

namespace BurningKnight.entity.pool {
	public class PrefixPool {
		public static PrefixPool Instance = new PrefixPool();
		protected List<Float> Chances = new List<>();
		protected List<Prefix> Classes = new List<>();

		public PrefixPool() {
			Add(new DeathShotPrefix(), 1f);
		}

		public Prefix GetModifier(int Id) {
			return Classes.Get(Id);
		}

		protected void Add(Prefix Type, float Chance) {
			Type.Id = Classes.Size();
			Classes.Add(Type);
			Chances.Add(Chance);
		}

		public Prefix Generate() {
			return Classes.Get(Random.Chances(Chances.ToArray(new Float[0])));
		}
	}
}