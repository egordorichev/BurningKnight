using BurningKnight.entity.component;
using BurningKnight.entity.events;
using Lens;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.graphics;
using Lens.input;
using Lens.util;
using Lens.util.camera;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BurningKnight.entity.creature.player {
	public class PlayerGraphicsComponent : AnimationComponent {
		private Vector2 scale = Vector2.One;
		public Vector2 Scale = Vector2.One;
		
		public PlayerGraphicsComponent() : base("gobbo") {
			CustomFlip = true;
			ShadowOffset = 8;
		}

		public override void Init() {
			base.Init();
			Entity.Area.Add(new RenderTrigger(Entity, RenderPickups, Layers.InGameUi));
		}

		public override void Update(float dt) {
			base.Update(dt);
			Flipped = Entity.CenterX > Camera.Instance.ScreenToCamera(Input.Mouse.ScreenPosition).X;
		}

		protected override void CallRender(Vector2 pos, bool shadow) {
			var region = Animation.GetCurrentTexture();
			var origin = new Vector2(region.Source.Width / 2f, FlippedVerticaly ? 0 : region.Source.Height);

			if (FlippedVerticaly) {
				pos.Y += region.Source.Height;
			}
			
			Graphics.Render(region, pos + origin, 0, origin, scale * Scale, Graphics.ParseEffect(Flipped, FlippedVerticaly));
			
			
			if (GetComponent<StateComponent>().StateInstance is Player.RollState) {
				return;
			}
			
			var hat = GetComponent<HatComponent>().Item;

			if (hat != null) {
				region = hat.Region;
				origin = new Vector2(region.Width / 2, region.Height);

				Graphics.Render(region, new Vector2(Entity.CenterX, Entity.Y + 7 + (shadow ? Entity.Height * 2 : 0) + (shadow ? -1 : 1) * offsets[Animation.Frame + Animation.StartFrame]), 0, origin, Scale, Graphics.ParseEffect(Flipped, shadow));
			}
		}

		public override bool HandleEvent(Event e) {
			if (e is WeaponSwappedEvent) {
				scale.Y = 0.3f;
				scale.X = 2f;
				
				Tween.To(1f, scale.X, x => scale.X = x, 0.2f);
				Tween.To(1f, scale.Y, x => scale.Y = x, 0.2f);
			} else if (e is InteractedEvent) {
				scale.Y = 0.5f;
				scale.X = 2f;
				
				Tween.To(1f, scale.X, x => scale.X = x, 0.2f);
				Tween.To(1f, scale.Y, x => scale.Y = x, 0.2f);
			}
			
			return base.HandleEvent(e);
		}

		private static sbyte[] offsets = {
			3, 0, 0, 0, 1, 1, 1, 0, 0, 0, -2, -1, 0, 0, 1, 1, 0
		};

		public void SimpleRender(bool shadow) {
			base.Render(shadow);
		}

		public override void Render(bool shadow) {
			var weapon = GetComponent<WeaponComponent>();
			var activeWeapon = GetComponent<ActiveWeaponComponent>();

			var o = (shadow ? -1 : 1) * offsets[Animation.Frame + Animation.StartFrame];
			
			weapon.Render(shadow, o);
			SimpleRender(shadow);
			activeWeapon.Render(shadow, o);
		}

		public void RenderPickups() {
			var player = (Player) Entity;
			var y = 0f;
			
			if (player.PickedItem != null) {
				var region = player.PickedItem.Region;
				
				Graphics.Render(region, Entity.Center - new Vector2(0, Entity.Height / 2f - 4f + player.Scale.X * 6f), 
					0,  new Vector2(region.Center.X, region.Height), player.Scale);
				
				y += region.Height + 4f;
			}
			
			foreach (var region in player.PickedUp) {
				Graphics.Render(region, Entity.Center - new Vector2(0, y + Entity.Height / 2f + 4f + region.Height / 2f),
					0, region.Center);

				y += region.Height + 4f;
			}
		}
	}
}