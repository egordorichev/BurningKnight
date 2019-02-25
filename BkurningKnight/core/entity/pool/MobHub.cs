namespace BurningKnight.core.entity.pool {
	public class MobHub {
		public List<Class> Types;
		public float Chance;
		public int MaxMatches;
		public int MaxMatchesInitial;
		public bool Once;

		public MobHub(float Chance, int Max, params Class[] Classes) {
			Types = new List<>();
			Types.AddAll(Arrays.AsList(Classes));
			this.Chance = Chance;
			MaxMatches = Max;
			MaxMatchesInitial = Max;
		}
	}
}
