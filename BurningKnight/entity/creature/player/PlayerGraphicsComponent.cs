using System;
using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.state;
using BurningKnight.util.geometry;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.graphics;
using Lens.graphics.animation;
using Lens.input;
using Lens.util;
using Lens.util.camera;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace BurningKnight.entity.creature.player {
	public class PlayerGraphicsComponent : AnimationComponent {
		private Vector2 scale = Vector2.One;
		private Animation head;
		public Vector2 Scale = Vector2.One;
		
		public PlayerGraphicsComponent() : base("gobbo", "body") {
			CustomFlip = true;
			ShadowOffset = 8;

			head = Animations.Get("gobbo").CreateAnimation("head");
		}

		public override void Init() {
			base.Init();
			Entity.Area.Add(new RenderTrigger(Entity, RenderPickups, Layers.InGameUi));
		}

		public override void Update(float dt) {
			base.Update(dt);

			head.Update(dt);
			Flipped = Entity.CenterX > Camera.Instance.ScreenToCamera(Input.Mouse.ScreenPosition).X;
		}

		protected override void CallRender(Vector2 pos, bool shadow) {
			var region = Animation.GetCurrentTexture();
			var origin = new Vector2(region.Source.Width / 2f, FlippedVerticaly ? 0 : region.Source.Height);
			var s = scale * Scale;

			if (FlippedVerticaly) {
				pos.Y += region.Source.Height;
			}
			
			Graphics.Render(region, pos + origin, 0, origin, s, Graphics.ParseEffect(Flipped, FlippedVerticaly));
			
			if (GetComponent<StateComponent>().StateInstance is Player.RollState) {
				return;
			}

			var h = GetComponent<HatComponent>();
			var hat = h.Item;

			if (hat != null && !h.DoNotRender) {
				var r = $"{hat.Id}_{(Entity.GetComponent<StateComponent>().StateInstance is Player.DuckState ? "b" : "a")}";
				var m = shadow ? -4 : 4;
				
				region = CommonAse.Items.GetSlice(r);
				origin = new Vector2(region.Width / 2, region.Height + 4);

				Graphics.Render(region, new Vector2(Entity.CenterX, m +
					Entity.Bottom + (shadow ? -1 : 1) * 
					(offsets[Math.Min(offsets.Length - 1, Animation.Frame + Animation.StartFrame)] - 15)), 0, origin, Scale * new Vector2(s.X, s.Y * (shadow ? -1 : 1)), Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
			} else {	
				region = head.GetFrame(Animation.Tag, (int) Animation.Frame);
				origin = new Vector2(region.Source.Width / 2f, FlippedVerticaly ? 0 : region.Source.Height);

				Graphics.Render(region, pos + origin, 0, origin, s, Graphics.ParseEffect(Flipped, FlippedVerticaly));
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
			13, 11, 11, 11, 12, 12, 12, 11, 11, 11, 10, 10, 11, 11, 10, 10, 11
		};

		public void SimpleRender(bool shadow) {
			base.Render(shadow);
		}

		public override void Render(bool shadow) {
			var o = (shadow ? -1 : 1) * (offsets[Math.Min(offsets.Length - 1, Animation.Frame + Animation.StartFrame)] - 11);
			var w = !(GetComponent<StateComponent>().StateInstance is Player.RollState);

			if (w) {
				GetComponent<WeaponComponent>().Render(shadow, o);
			}
			
			
			var stopShader = StartShaders();

			SimpleRender(shadow);

			if (stopShader) {
				Shaders.End();
			}

			if (w) {
				GetComponent<ActiveWeaponComponent>().Render(shadow, o);
			}
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