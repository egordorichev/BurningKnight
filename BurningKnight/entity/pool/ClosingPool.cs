using System;
using System.Collections.Generic;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.pool {
	public class ClosingPool<T> : Pool<T> {
		protected List<float> NewChances = new List<float>();
		protected List<Type> NewClasses = new List<Type>();

		public void Reset() {
			NewClasses = new List<Type>();
			NewClasses.AddRange(Classes);
			NewChances = new List<float>();
			NewChances.AddRange(Chances);
		}

		public override T Generate() {
			if (NewChances.Count == 0) {
				Reset();
			}

			var Type = NewClasses[Random.Chances(NewChances)];
			
			NewChances.Remove(NewClasses.IndexOf(Type));
			NewClasses.Remove(Type);

			return (T) Activator.CreateInstance(Type);
		}
	}
}