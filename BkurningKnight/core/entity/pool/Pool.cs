using BurningKnight.core.util;

namespace BurningKnight.core.entity.pool {
	public class Pool<T>  {
		protected List<Class> Classes = new List<>();
		protected List<Float> Chances = new List<>();

		public T Generate() {
			int I = Random.Chances(Chances.ToArray(new Float[0]));

			if (I == -1) {
				Log.Error("-1 as pool result!");

				return null;
			} 

			Class Type = Classes.Get(I);

			try {
				return (T) Type.NewInstance();
			} catch (InstantiationException | IllegalAccessException) {
				E.PrintStackTrace();
			}

			return null;
		}

		public Void Add(Class Type, float Chance) {
			Classes.Add(Type);
			Chances.Add(Chance);
		}

		public Void Clear() {
			Classes.Clear();
			Chances.Clear();
		}

		public Void AddFrom(Pool Pool) {
			this.Classes.AddAll(Pool.Classes);
			this.Chances.AddAll(Pool.Chances);
		}
	}
}
