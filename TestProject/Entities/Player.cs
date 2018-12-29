using Lens.Entities;
using Lens.Entities.Components.Graphics;
using Lens.Entities.Components.Logic;

namespace TestProject.Entities {
	public class Player : Entity {
		protected override void AddComponents() {
			base.AddComponents();
			
			SetGraphicsComponent(new ImageComponent("player"));
			AddComponent(new StateComponent<Player>());			
		}
	}
}