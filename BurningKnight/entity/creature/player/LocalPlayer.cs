namespace BurningKnight.entity.creature.player {
	public class LocalPlayer : Player {
		protected override void AddComponents() {
			base.AddComponents();

			AddComponent(new PlayerInputComponent());
		}
	}
}