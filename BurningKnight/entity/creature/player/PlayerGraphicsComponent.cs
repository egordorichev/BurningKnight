using System;
using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.pet;
using BurningKnight.entity.door;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.entity.item.stand;
using BurningKnight.entity.item.util;
using BurningKnight.entity.orbital;
using BurningKnight.entity.projectile;
using BurningKnight.entity.room.controllable.platform;
using BurningKnight.entity.room.controllable.spikes;
using BurningKnight.level;
using BurningKnight.level.entities;
using BurningKnight.physics;
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
		private static Color AimLineColor = new Color(1f, 0f, 0f, 1f);

		private Vector2 scale = Vector2.One;
		private Animation head;
		public Vector2 Scale = Vector2.One;

		private TextureRegion wing;
		
		public PlayerGraphicsComponent() : base("gobbo", "body") {
			CustomFlip = true;
			ShadowOffset = 8;

			head = Animations.Get("gobbo").CreateAnimation("head");

			wing = CommonAse.Items.GetSlice("wing");
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
			
			if (!shadow) {
				pos.Y -= GetComponent<ZComponent>().Z;
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
					Entity.Bottom - (shadow ? 0 : GetComponent<ZComponent>().Z) + (shadow ? -1 : 1) * 
					(offsets[Math.Min(offsets.Length - 1, Animation.Frame + Animation.StartFrame)] - 15)), 0, origin, Scale * new Vector2(s.X, s.Y * (shadow ? -1 : 1)), Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
			} else {	
				region = head.GetFrame(Animation.Tag, (int) Animation.Frame);

				if (region == null) {
					return;
				}
				
				origin = new Vector2(region.Source.Width / 2f, FlippedVerticaly ? 0 : region.Source.Height);

				Graphics.Render(region, pos + origin + Offset, 0, origin, s, Graphics.ParseEffect(Flipped, FlippedVerticaly));
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

		private static bool RayShouldCollide(Entity entity) {
			return entity is SolidProp || !(entity is MeleeArc || entity is Orbital || entity is Pet || entity is Prop || entity is Spikes || entity is Chasm || entity is MovingPlatform || entity is PlatformBorder || entity is Item || entity is Projectile || entity is Bomb);
		}

		public override void Render(bool shadow) {
			var o = (shadow ? -1 : 1) * (offsets[Math.Min(offsets.Length - 1, Animation.Frame + Animation.StartFrame)] - 11);
			var w = !(GetComponent<StateComponent>().StateInstance is Player.RollState);
			var z = GetComponent<ZComponent>();

			// Render wings
			if (((Player) Entity).HasFlight) {
				var a = (float) (Math.Sin(Engine.Time * 5f) * 0.5f) * (shadow ? -1 : 1);

				if (!shadow) {
					z.Z = -a * 3 + 4;
				}

				a -= (float) Math.PI / 4 * (shadow ? -1 : 1);
				var wy = shadow ? Entity.Height : 0;

				wy += GetComponent<ZComponent>().Z * (shadow ? 1 : -1);

				Graphics.Render(wing, Entity.Center + new Vector2(-1, wy), a, new Vector2(8),
					shadow ? MathUtils.InvertY : Vector2.One);

				Graphics.Render(wing, Entity.Center + new Vector2(1, wy), -a, new Vector2(8),
					shadow ? MathUtils.InvertXY : MathUtils.InvertX);
			}

			if (w) {
				GetComponent<WeaponComponent>().Render(shadow, o - (int) z.Z);
			}

			var stopShader = StartShaders();

			SimpleRender(shadow);

			if (stopShader) {
				Shaders.End();
			}

			if (!shadow) {
				var aim = GetComponent<AimComponent>();
				
				if (aim.ShowLaserLine) {
					var from = aim.Center;
					var to = aim.Aim;
					var min = 1f;
					var closest = MathUtils.CreateVector(MathUtils.Angle(to.X - from.X, to.Y - from.Y), Display.UiWidth) + from;

					Physics.World.RayCast((fixture, point, normal, fraction) => {
						if (min > fraction && fixture.Body.UserData is BodyComponent b && (!(b.Entity is Creature) || b is SensorBodyComponent) && RayShouldCollide(b.Entity)) {
							min = fraction;
							closest = point;
						}

						return min;
					}, from, to);
					
					Graphics.Batch.FillRectangle((int) closest.X - 1, (int) closest.Y - 1,3, 3, AimLineColor);
					Graphics.Batch.DrawLine(from, new Vector2((int) closest.X, (int) closest.Y), AimLineColor, 1);
				}
			}

			if (w) {
				GetComponent<ActiveWeaponComponent>().Render(shadow, o - (int) z.Z);
			}
		}
	}
}