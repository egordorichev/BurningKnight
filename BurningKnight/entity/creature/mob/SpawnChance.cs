namespace BurningKnight.entity.creature.mob {
	public class SpawnChance {
		public float Chance;
		public string[] Areas;

		public SpawnChance(float chance, params string[] areas) {
			Chance = chance;
			Areas = areas;
		}
	}
}