using Lens.Entity;
using Lens.Entity.Components.Graphics;
using Lens.Entity.Components.Logic;

namespace TestProject.Entities {
	public class Player : Entity {
		protected override void AddComponents() {
			base.AddComponents();
			
			GraphicsComponent = new ImageComponent("player.png");
			AddComponent(new StateComponent<Player>());
		}
	}
}