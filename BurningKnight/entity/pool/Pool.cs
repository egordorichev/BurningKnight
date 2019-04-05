using System.Collections.Generic;
using Lens.util;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.pool {
	public class Pool<T> {
		protected List<float> Chances = new List<float>();
		protected List<T> Classes = new List<T>();

		public virtual T Generate() {
			var I = Random.Chances(Chances);

			if (I == -1) {
				Log.Error("-1 as pool result!");
				return default(T);
			}

			return Classes[I];
		}

		public void Add(T Type, float Chance) {
			Classes.Add(Type);
			Chances.Add(Chance);
		}

		public void Clear() {
			Classes.Clear();
			Chances.Clear();
		}

		public void AddFrom(Pool<T> Pool) {
			Classes.AddRange(Pool.Classes);
			Chances.AddRange(Pool.Chances);
		}
	}
}