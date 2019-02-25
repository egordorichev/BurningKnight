namespace BurningKnight.entity.pool {
	public class MobHub {
		public float Chance;
		public int MaxMatches;
		public int MaxMatchesInitial;
		public bool Once;
		public List<Class> Types;

		public MobHub(float Chance, int Max, params Class[] Classes) {
			Types = new List<>();
			Types.AddAll(Arrays.AsList(Classes));
			this.Chance = Chance;
			MaxMatches = Max;
			MaxMatchesInitial = Max;
		}
	}
}