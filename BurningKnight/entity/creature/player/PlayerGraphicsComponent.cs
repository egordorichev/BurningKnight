using System;
using BurningKnight.assets;
using BurningKnight.entity.component;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.entity.component.graphics;
using Lens.entity.component.logic;
using Lens.graphics.animation;
using Lens.input;
using Lens.util.camera;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.player {
	public class PlayerGraphicsComponent : GraphicsComponent {
		public Animation Head;
		public Animation Body;

		public override void Init() {
			base.Init();

			Head = Animations.Create("gobbo", "gobbo");
			Body = Animations.Create("gobbo", "body");

			CustomFlip = true;
		}

		public override void Update(float dt) {
			base.Update(dt);
			
			Head.Update(dt);
			Body.Update(dt);
			
			Flipped = Entity.CenterX > Camera.Instance.ScreenToCamera(Input.Mouse.ScreenPosition).X;
		}

		public override void Render() {
			base.Render();

			var weapon = GetComponent<WeaponComponent>();
			var activeWeapon = GetComponent<ActiveWeaponComponent>();

			var shader = Shaders.Entity;			
			Shaders.Begin(shader);
			
			shader.Parameters["flashReplace"].SetValue(1f);
			shader.Parameters["flashColor"].SetValue(new Vector4(1, 0, 0, 1));
			shader.Parameters["flash"].SetValue((float) Math.Max(0, Math.Cos(Engine.Time * 2) * 0.5f + 0.5f));

			weapon.Render();
			
			Head.Render(Entity.Position + Offset, Flipped);
			Body.Render(Entity.Position + Offset, Flipped);

			activeWeapon.Render();
			
			GetComponent<RoomComponent>().Room?.RenderDebug();
			
			Shaders.End();
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