namespace BurningKnight.entity.creature.mob.jungle {
	public class BuffedFlower : Flower {
		public override void Init() {
			base.Init();
			ShootAllAtOnce = true;
		}

		protected override string GetAnimation() {
			return "buffed_flower";
		}
	}
}