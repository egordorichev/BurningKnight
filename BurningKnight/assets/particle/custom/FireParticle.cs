using System;
using BurningKnight.entity;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.level.biome;
using BurningKnight.state;
using Lens;
using Lens.entity;
using Lens.graphics;
using Lens.util.math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VelcroPhysics.Dynamics;
using MathUtils = Lens.util.MathUtils;

namespace BurningKnight.assets.particle.custom {
	public class FireParticle : Entity {
		public static TextureRegion Region;
		
		public Entity Owner;
		public float Delay;

		public float T;
		public float Scale;
		public float ScaleTar;
		public Vector2 Offset;
		public float Vy;
		public float Vx;
		public bool Growing;
		public float SinOffset;
		public float Mod;

		public float R = 1f;
		public float G = 1f;
		public float B = 1f;
		public float Size = 1;
		public bool Hurts;
		
		public Vector2? Target;

		public override void Init() {
			base.Init();

			if (Region == null) {
				Region = CommonAse.Particles.GetSlice("fire");
			}
			
			AddTag(Tags.FireParticle);

			AlwaysActive = true;
			AlwaysVisible = true;
			
			Growing = true;
			ScaleTar = Rnd.Float(0.5f, 0.9f) * Size;

			if (Mod < 0.01f) {
				Mod = Rnd.Float(0.7f, 1f);
			}

			SinOffset = Rnd.Float(3.2f);

			if (Math.Abs(Offset.X) + Math.Abs(Offset.Y) < 0.1f) {
				Offset = new Vector2(Rnd.Float(-4, 4) * XChange, Rnd.Float(-2, 2));
			}

			Depth = Layers.TileLights + 1;
		}

		public override void AddComponents() {
			base.AddComponents();

			if (Hurts) {
				AddComponent(new CircleBodyComponent(0, 0, 3, BodyType.Dynamic, true, true));
			}
		}

		public override void Update(float dt) {
			if (Delay > 0) {
				Delay -= dt;
				return;
			}
			
			T += dt;

			if (T > 0.3f) {
				if (Run.Level != null && Run.Level.Biome is CaveBiome) {
					R = Math.Max(0, R - dt * 3 * Mod);
					G = Math.Max(0, G - dt * 0.3f * Mod);
					B = Math.Max(0, B - dt * Mod);
				} else {
					R = Math.Max(0, R - dt * 0.3f * Mod);
					G = Math.Max(0, G - dt * Mod);
					B = Math.Max(0, B - dt * 3 * Mod);
				}
			}

			if (Growing) {
				Scale += dt;

				if (Scale >= ScaleTar) {
					Scale = ScaleTar;
					Growing = false;
				}
			} else {
				Scale -= dt * (Target.HasValue ? 0.33f : 0.5f);

				if (Scale <= 0) {
					Done = true;
					return;
				}
			}

			Vy += dt * Mod * 20;

			if (Target.HasValue) {
				var t = Target.Value;

				var dx = t.X - Position.X - Offset.X;
				var dy = t.Y - Position.Y - Offset.Y;
				var d = MathUtils.Distance(Vx, Vy) + dt * 30;
				
				var angle = Math.Atan2(dy, dx);
				var va = Math.Atan2(-Vy, -Vx);

				va = MathUtils.LerpAngle(va, angle, dt * 4);

				Vx = (float) -Math.Cos(va) * d;
				Vy = (float) -Math.Sin(va) * d;
			}
			
			Offset.X -= Vx * dt;
			Offset.Y -= Vy * dt;
			
			if (Owner != null) {
				X = Owner.CenterX; 
				Y = Owner.Bottom;

				if (Owner.TryGetComponent<ZComponent>(out var z)) {
					Y -= z.Z;
				}
			}

			if (Hurts) {
				GetComponent<CircleBodyComponent>().Position = Position + Offset;
			}
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent cse) {
				if (cse.Entity is Player p) {
					p.GetComponent<HealthComponent>().ModifyHealth(-1, this);
				}
			}
			
			return base.HandleEvent(e);
		}

		public float XChange = 1;

		public override void Render() {
			
		}
		
		public void ActuallyRender() {
			if (Delay > 0) {
				return;
			}
			
			var a = (float) Math.Cos(T * 5f + SinOffset) * 0.4f;
			var pos = Position + Offset + Region.Center;

			pos.X += (float) Math.Cos(SinOffset + T * 2.5f) * Scale * 8 * XChange;
		
			Graphics.Color = new Color(R, G, B, 0.5f);
			Graphics.Render(Region, pos, a, Region.Center, new Vector2(Scale * 10));
			Graphics.Color = new Color(R, G, B, 1f);
			Graphics.Render(Region, pos, a, Region.Center, new Vector2(Scale * 5));
		}

		public static void Hook(Area area) {
			area.Add(new RenderTrigger(() => {
				var state = Engine.Instance.StateRenderer;

				state.End();
				var b = state.BlendState;
				state.BlendState = BlendState.Additive;
				state.Begin();
				
				foreach (var e in area.Tagged[Tags.FireParticle]) {
					((FireParticle) e).ActuallyRender();
				}
				
				Graphics.Color = ColorUtils.WhiteColor;

				state.End();
				state.BlendState = b;
				state.Begin();
			}, Layers.WindFx));
		}
	}
}