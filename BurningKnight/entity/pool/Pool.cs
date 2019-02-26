using BurningKnight.util;

namespace BurningKnight.entity.pool {
	public class Pool<T> {
		protected List<Float> Chances = new List<>();
		protected List<Class> Classes = new List<>();

		public T Generate() {
			var I = Random.Chances(Chances.ToArray(new Float[0]));

			if (I == -1) {
				Log.Error("-1 as pool result!");

				return null;
			}

			Class Type = Classes.Get(I);

			try {
				return (T) Type.NewInstance();
			}
			catch (InstantiationException |

			IllegalAccessException) {
				E.PrintStackTrace();
			}

			return null;
		}

		public void Add(Class Type, float Chance) {
			Classes.Add(Type);
			Chances.Add(Chance);
		}

		public void Clear() {
			Classes.Clear();
			Chances.Clear();
		}

		public void AddFrom(Pool Pool) {
			Classes.AddAll(Pool.Classes);
			Chances.AddAll(Pool.Chances);
		}
	}
}