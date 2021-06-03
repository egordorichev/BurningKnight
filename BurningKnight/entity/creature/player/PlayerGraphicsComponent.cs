using System;
using BurningKnight.assets;
using BurningKnight.entity.bomb;
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
		public static Color AimLineColor = new Color(1f, 0f, 0f, 1f);

		private Vector2 scale = Vector2.One;
		private Animation head;

		private TextureRegion wing;
		public bool Hidden;
		
		public PlayerGraphicsComponent() : base("gobbo", "body") {
			CustomFlip = true;
			ShadowOffset = 8;

			head = Animations.Get("gobbo").CreateAnimation("head");
			wing = CommonAse.Items.GetSlice("wing");
		}

		public override void Update(float dt) {
			base.Update(dt);
			head.Update(dt);

			if (!(GetComponent<StateComponent>().StateInstance is Player.SleepingState)) {
				Flipped = Entity.CenterX > GetComponent<CursorComponent>().Cursor.GamePosition.X;
			}
		}

		private float angle;

		protected override void CallRender(Vector2 pos, bool shadow) {
			if (Hidden) {
				return;
			}

			var region = Animation.GetCurrentTexture();
			var origin = new Vector2(region.Source.Width / 2f, FlippedVerticaly ? 0 : region.Source.Height);
			var s = scale * Scale;

			var v = GetComponent<RectBodyComponent>().Acceleration;
			var target = (v.Length() > 0.1f ? 1f : 0f) * 0.25f * (Flipped ? -1 : 1);
			angle += (target - angle) * Engine.Delta * 3f;

			var state = GetComponent<StateComponent>().StateInstance;
			var a = 0; // (state is Player.RollState || state is Player.DuckState || state is Player.PostRollState || state is Player.SittingState) ? 0 : angle;

			if (shadow) {
				a *= -1;
			}

			if (FlippedVerticaly) {
				pos.Y += region.Source.Height;
			}
			
			if (!shadow) {
				pos.Y -= GetComponent<ZComponent>().Z;
			}

			var p = pos + origin;
			p.Floor();
			Graphics.Render(region, p, a, origin, s, Graphics.ParseEffect(Flipped, FlippedVerticaly));
			var st = GetComponent<StateComponent>().StateInstance;
			
			if (st is Player.RollState || st is Player.SleepingState) {
				return;
			}

			var h = GetComponent<HatComponent>();
			var hat = h.Item;

			if (hat != null && !h.DoNotRender) {
				var r = $"{hat.Id}_{(Entity.GetComponent<StateComponent>().StateInstance is Player.DuckState ? "b" : "a")}";
				var m = shadow ? -4 : 4;
				
				region = CommonAse.Items.GetSlice(r);
				origin = new Vector2(region.Width / 2, region.Height + 4);

				var pp = new Vector2(Entity.CenterX, m +
					Entity.Bottom - (shadow ? 0 : GetComponent<ZComponent>().Z) + (shadow ? -1 : 1) *
					(offsets[Math.Min(offsets.Length - 1, Animation.Frame + Animation.StartFrame)] - 15));

				pp.Floor();

				Graphics.Render(region, pp, a, origin, Scale * new Vector2(s.X, s.Y * (shadow ? -1 : 1)), Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
			} else {	
				region = head.GetFrame(Animation.Tag, (int) Animation.Frame);

				if (region == null) {
					return;
				}
				
				origin = new Vector2(region.Source.Width / 2f, FlippedVerticaly ? 0 : region.Source.Height);

				var pp = pos + origin;
				pp.Floor();
				Graphics.Render(region, pp, a, origin, s, Graphics.ParseEffect(Flipped, FlippedVerticaly));
			}
		}

		public void AnimateSwap() {
			scale.Y = 0.3f;
			scale.X = 2f;
				
			Tween.To(1f, scale.X, x => scale.X = x, 0.2f);
			Tween.To(1f, scale.Y, x => scale.Y = x, 0.2f);
		}

		public override bool HandleEvent(Event e) {
			if (e is WeaponSwappedEvent) {
				AnimateSwap();
			} else if (e is InteractedEvent) {
				scale.Y = 0.5f;
				scale.X = 2f;
				
				Tween.To(1f, scale.X, x => scale.X = x, 0.2f);
				Tween.To(1f, scale.Y, x => scale.Y = x, 0.2f);
			}
			
			return base.HandleEvent(e);
		}

		private static sbyte[] offsets = {
			13, 12, 12, 12, 12, 0, 0, 0, 0, 11, 11, 11, 12, 12, 12, 11, 11, 11, 10, 10, 11, 11, 10, 10, 11
		};

		public void SimpleRender(bool shadow) {
			base.Render(shadow);
		}

		private static bool RayShouldCollide(Entity entity) {
			return entity is SolidProp || !(entity is Entrance || entity is Exit || entity is Player || entity is MeleeArc || entity is Orbital || entity is Pet || entity is Prop || entity is Spikes || entity is Chasm || entity is MovingPlatform || entity is PlatformBorder || entity is Item || entity is Projectile || entity is Bomb);
		}

		public override void Render(bool shadow) {
			if (Hidden) {
				return;
			}

			var o = (shadow ? -1 : 1) * (offsets[Math.Min(offsets.Length - 1, Animation.Frame + Animation.StartFrame)] - 11);
			var s = GetComponent<StateComponent>().StateInstance;
			var w = !(s is Player.RollState || s is Player.SleepingState);
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

			var g = shadow ? 0 : (int) z.Z;

			if (w) {
				GetComponent<WeaponComponent>().Render(shadow, o - g);
			}

			if (!shadow && InGameState.Multiplayer) {
				var pos = Entity.Position + Offset;
				var shader = Shaders.Entity;
				Shaders.Begin(shader);

				shader.Parameters["flash"].SetValue(1f);
				shader.Parameters["flashReplace"].SetValue(1f);
				shader.Parameters["flashColor"].SetValue(Player.VectorTints[Entity.GetComponent<InputComponent>().Index]);

				foreach (var d in MathUtils.Directions) {
					CallRender(pos + d, false);
				}

				Shaders.End();
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
					var to = aim.RealAim;
					var min = 1f;
					var closest = MathUtils.CreateVector(MathUtils.Angle(to.X - from.X, to.Y - from.Y), Display.UiWidth) + from;

					Physics.World.RayCast((fixture, point, normal, fraction) => {
						if (min > fraction && fixture.Body.UserData is BodyComponent b && RayShouldCollide(b.Entity)) {
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
				GetComponent<ActiveWeaponComponent>().Render(shadow, o - g);
			}
		}
	}
}