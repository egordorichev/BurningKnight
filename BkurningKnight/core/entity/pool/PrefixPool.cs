using BurningKnight.core.entity.creature.mob.prefix;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.pool {
	public class PrefixPool {
		public static PrefixPool Instance = new PrefixPool();
		protected List<Prefix> Classes = new List<>();
		protected List<Float> Chances = new List<>();

		public PrefixPool() {
			Add(new DeathShotPrefix(), 1f);
		}

		public Prefix GetModifier(int Id) {
			return Classes.Get(Id);
		}

		protected Void Add(Prefix Type, float Chance) {
			Type.Id = this.Classes.Size();
			Classes.Add(Type);
			Chances.Add(Chance);
		}

		public Prefix Generate() {
			return Classes.Get(Random.Chances(Chances.ToArray(new Float[0])));
		}
	}
}
