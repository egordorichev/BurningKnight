namespace BurningKnight.entity.item.stand {
	public class TrashGoblinStand : ItemStand {
		protected override string GetSprite() {
			return "scourge_stand";
		}

		public override ItemPool GetPool() {
			return ItemPool.TrashGoblin;
		}
		
		// todo: reward
	}
}