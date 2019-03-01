namespace BurningKnight.entity.creature.player {
	public class LocalPlayer : Player {
		public override void AddComponents() {
			base.AddComponents();

			AddComponent(new PlayerInputComponent());
		}
	}
}