using System;
using Lens.assets;
using Lens.entity;
using Lens.entity.component.graphics;
using Lens.entity.component.logic;
using Lens.graphics.animation;
using Lens.input;
using Lens.util;
using Lens.util.camera;

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
			
			Flipped = Entity.CenterX > Camera.Instance.ScreenToCamera(Input.Mouse.ScreenPosition).X;
		}

		public override void Render() {
			base.Render();

			var weapon = Entity.GetComponent<WeaponComponent>();
			var activeWeapon = Entity.GetComponent<ActiveWeaponComponent>();

			weapon.Render();
			
			Head.Render(Entity.Position + Offset, Flipped);
			Body.Render(Entity.Position + Offset, Flipped);

			activeWeapon.Render();
		}

		public override bool HandleEvent(Event e) {
			if (e is StateChangedEvent ev) {
				Head.Tag = ev.NewState.Name.ToLower().Replace("state", "");
				Body.Tag = Head.Tag;
			}

			return base.HandleEvent(e);
		}
	}
}