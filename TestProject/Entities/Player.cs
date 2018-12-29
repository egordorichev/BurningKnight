using Lens.Entities;
using Lens.Entities.Components.Graphics;
using Lens.Entities.Components.Logic;
using Lens.Pattern.Observer;
using TestProject.Pattern.Observers.Sfx;

namespace TestProject.Entities {
	public class Player : Entity {
		protected override void AddComponents() {
			base.AddComponents();
			
			SetGraphicsComponent(new ImageComponent("player"));
			AddComponent(new StateComponent<Player>());			
			Subjects.Audio.Notify(this, new SfxEvent("test"));
		}
	}
}