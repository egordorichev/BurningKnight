using BurningKnight.util;

namespace BurningKnight.entity.pool {
	public class ClosingPool<T> : Pool<T> {
		protected List<Float> NewChances = new List<>();
		protected List<Class> NewClasses = new List<>();

		public void Reset() {
			NewClasses = new List<>();
			NewClasses.AddAll(Classes);
			NewChances = new List<>();
			NewChances.AddAll(Chances);
		}

		public override T Generate() {
			if (NewChances.Size() == 0) Reset();

			Class Type = NewClasses.Get(Random.Chances(NewChances.ToArray(new Float[0])));
			NewChances.Remove(NewClasses.IndexOf(Type));
			NewClasses.Remove(Type);

			try {
				return (T) Type.NewInstance();
			}
			catch (InstantiationException |

			IllegalAccessException) {
				E.PrintStackTrace();
			}

			return null;
		}
	}
}