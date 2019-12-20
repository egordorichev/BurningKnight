using System;
using BurningKnight.entity.pool;

namespace BurningKnight.level.entities.chest {
	public class ChestRegistry : Pool<Type> {
		public static ChestRegistry Instance = new ChestRegistry();

		static ChestRegistry() {
			Instance.Add(typeof(WoodenChest), 1f);
			Instance.Add(typeof(ScourgedChest), 0.2f);
			Instance.Add(typeof(DoubleChest), 0.5f);
			Instance.Add(typeof(TripleChest), 0.3f);
			Instance.Add(typeof(StoneChest), 1f);
			Instance.Add(typeof(GoldChest), 1f);
			Instance.Add(typeof(RedChest), 0.5f);
		}
	}
}