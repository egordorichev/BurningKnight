using Lens.assets;
using Lens.entity;
using Lens.entity.component.graphics;
using Lens.entity.component.logic;
using Lens.graphics.animation;

namespace BurningKnight.entity.creature.player {
	public class PlayerGraphicsComponent : GraphicsComponent {
		public Animation Head;
		public Animation Body;

		public override void Init() {
			base.Init();

			Head = Animations.Create("gobbo", "gobbo");
			Body = Animations.Create("gobbo", "body");
		}

		public override void Update(float dt) {
			base.Update(dt);
			
			Head.Update(dt);
			Body.Update(dt);
		}

		public override void Render() {
			base.Render();
			
			Head.Render(Entity.Position + Offset, Flipped);
			Body.Render(Entity.Position + Offset, Flipped);
		}

		public override bool HandleEvent(Event e) {
			base.HandleEvent(e);

			if (e is StateChangedEvent ev) {
				Head.Tag = ev.NewState.Name.ToLower().Replace("state", "");
				Body.Tag = Head.Tag;
			}

			return false;
		}
	}
}