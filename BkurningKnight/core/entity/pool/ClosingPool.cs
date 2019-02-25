using BurningKnight.core.util;

namespace BurningKnight.core.entity.pool {
	public class ClosingPool<T>  : Pool<T>  {
		protected List<Class> NewClasses = new List<>();
		protected List<Float> NewChances = new List<>();

		public Void Reset() {
			NewClasses = new List<>();
			NewClasses.AddAll(Classes);
			NewChances = new List<>();
			NewChances.AddAll(Chances);
		}

		public override T Generate() {
			if (NewChances.Size() == 0) {
				Reset();
			} 

			Class Type = NewClasses.Get(Random.Chances(NewChances.ToArray(new Float[0])));
			NewChances.Remove(NewClasses.IndexOf(Type));
			NewClasses.Remove(Type);

			try {
				return (T) Type.NewInstance();
			} catch (InstantiationException | IllegalAccessException) {
				E.PrintStackTrace();
			}

			return null;
		}
	}
}
