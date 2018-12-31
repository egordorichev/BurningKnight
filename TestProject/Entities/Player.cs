using Lens.Entities;
using Lens.Entities.Components.Graphics;
using Lens.Entities.Components.Logic;
using TestProject.Entities.Components;

namespace TestProject.Entities {
	public class Player : Entity {
		protected override void AddComponents() {
			base.AddComponents();
			
			SetGraphicsComponent(new AnimationComponent("test"));
			
			AddComponent(new StateComponent<Player>());
			AddComponent(new PlayerInputComponent());
		}
	}
}