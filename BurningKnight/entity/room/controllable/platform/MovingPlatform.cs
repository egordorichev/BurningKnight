using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.events;
using BurningKnight.level.tile;
using BurningKnight.state;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.util.camera;
using Lens.util.file;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

/*
 * todo: different method of border collision, like chasms on 4 sides, that get disabled
 */

namespace BurningKnight.entity.room.controllable.platform {
	public class MovingPlatform : Platform {
		protected byte tw = 2;
		protected byte th = 2;

		public PlatformController Controller;
		private int step;
		private Vector2 velocity;
		
		public override void AddComponents() {
			base.AddComponents();

			Width = tw * 16;
			Height = th * 16 + 6;

			AddComponent(new StateComponent());
			AddComponent(new AnimationComponent("moving_platform"));
			AddComponent(new RectBodyComponent(0, 0, tw * 16, th * 16, BodyType.Dynamic, true));
		}

		public override void PostInit() {
			base.PostInit();
			Stop();
		}

		protected override bool ShouldMove(Entity e) {
			return base.ShouldMove(e) && e.Bottom < Bottom - 6;
		}

		#region Platform States
		protected class IdleState : SmartState<MovingPlatform> {
			private const float Delay = 1f;

			public override void Update(float dt) {
				base.Update(dt);

				if (T >= Delay) {
					Become<MovingState>();
				}
			}
		}

		protected class MovingState : SmartState<MovingPlatform> {
			private const float Speed = 32;

			public override void Update(float dt) {
				base.Update(dt);
				Self.GetComponent<RectBodyComponent>().Velocity = Self.velocity * Speed;

				var x = Math.Abs(Self.velocity.X) < 0.1f ? (int) Math.Ceiling(Self.X / 16) : (int) (Self.velocity.X < 0 ? Math.Floor(Self.X / 16) : Math.Ceiling(Self.X / 16) + Self.tw - 1);
				var y = Math.Abs(Self.velocity.Y) < 0.1f ? (int) Math.Ceiling(Self.Y / 16) : (int) (Self.velocity.Y < 0 ? Math.Floor(Self.Y / 16) : Math.Ceiling(Self.Y / 16) + Self.th - 1);

				var t = Run.Level.Get(x, y);

				if (t != Tile.Chasm) {
					Self.Stop();
				}
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<RectBodyComponent>().Velocity = Vector2.Zero;
			}
		}
		#endregion

		private static Vector2[] directions = {
			new Vector2(1, 0),
			new Vector2(0, 1),
			new Vector2(-1, 0),
			new Vector2(0, -1)
		};

		protected virtual void Stop() {
			GetComponent<StateComponent>().Become<IdleState>();
			step++;
			
			switch (Controller) {
				case PlatformController.LeftRight: {
					step %= 2;
					velocity.X *= -1;
					break;
				}
				
				case PlatformController.UpDown: {
					step %= 2;
					velocity.Y *= -1;
					break;
				}

				case PlatformController.ClockWise: {
					step %= 4;
					velocity = directions[step];
					break;
				}

				case PlatformController.CounterClockWise: {
					step %= 4;
					velocity = directions[3 - step];
					break;
				}
			}

			if (OnScreen) {
				Camera.Instance.Shake(4);
			}
		}

		public override void TurnOn() {
			base.TurnOn();
			GetComponent<StateComponent>().Become<MovingState>();
		}

		public override void TurnOff() {
			base.TurnOff();
			GetComponent<StateComponent>().Become<IdleState>();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			stream.WriteByte((byte) Controller);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);

			Controller = (PlatformController) stream.ReadByte();
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent ev) {
				if (ev.Entity is MovingPlatform) {
					Stop();
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}