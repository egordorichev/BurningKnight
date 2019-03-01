using Lens.entity;

namespace BurningKnight.entity.creature.player {
	public class LocalPlayer : Player {
		public static LocalPlayer Locate(Area area) {
			foreach (var player in area.Tags[Tags.Player]) {
				if (player is LocalPlayer) {
					return (LocalPlayer) player;
				}
			}

			return null;
		}
		
		public override void AddComponents() {
			base.AddComponents();

			AddComponent(new PlayerInputComponent());
		}
	}
}