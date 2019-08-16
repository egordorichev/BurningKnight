using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.level.tile;
using BurningKnight.state;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.util.file;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.room.controllable.platform {
	public class MovingPlatform : Platform {
		protected byte tw = 2;
		protected byte th = 2;
		
		public Vector2 Direction = new Vector2(1, 0);
		private Vector2 startDirection;
		
		public override void AddComponents() {
			base.AddComponents();

			Width = tw * 16;
			Height = th * 16 + 6;

			var s = new StateComponent();
			AddComponent(s);
			
			s.Become<IdleState>();

			AddComponent(new AnimationComponent("moving_platform"));
			AddComponent(new RectBodyComponent(0, 0, tw * 16, th * 16, BodyType.Dynamic, true));
		}

		public override void PostInit() {
			base.PostInit();
			startDirection = Direction;
		}

		protected override bool ShouldMove(Entity e) {
			return e.Bottom < Bottom - 6;
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
				Self.GetComponent<RectBodyComponent>().Velocity = Self.Direction * Speed;

				var x = (int) (Self.Direction.X < 0 ? Math.Floor(Self.X / 16) : Math.Ceiling(Self.X / 16) + Self.tw - 1);
				var y = (int) (Self.Direction.Y < 0 ? Math.Floor(Self.Y / 16) : Math.Ceiling(Self.Y / 16) + Self.th - 1);

				var t = Run.Level.Get(x, y);

				if (t != Tile.Chasm) {
					Self.Direction *= -1;
					Become<IdleState>();
				}
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<RectBodyComponent>().Velocity = Vector2.Zero;
			}
		}
		#endregion

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
			
			stream.WriteFloat(startDirection.X);
			stream.WriteFloat(startDirection.Y);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);

			startDirection = new Vector2(stream.ReadFloat(), stream.ReadFloat());
			Direction = startDirection;
		}
	}
}