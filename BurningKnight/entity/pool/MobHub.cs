using System;
using System.Collections.Generic;

namespace BurningKnight.entity.pool {
	public class MobHub {
		public float Chance;
		public int MaxMatches;
		public int MaxMatchesInitial;
		public bool Once;
		public List<Type> Types;

		public MobHub(float Chance, int Max, params Type[] Classes) {
			Types = new List<Type>();
			Types.AddRange(Classes);
			this.Chance = Chance;
			MaxMatches = Max;
			MaxMatchesInitial = Max;
		}
	}
}