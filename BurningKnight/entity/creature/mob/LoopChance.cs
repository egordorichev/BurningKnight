namespace BurningKnight.entity.creature.mob {
	public class LoopChance : SpawnChance {
		public LoopChance(float chance, params string[] areas) : base(chance, areas) {
			LoopOnly = true;
		}
	}
}