using System;
using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.events;
using BurningKnight.level.tile;
using BurningKnight.physics;
using BurningKnight.state;
using BurningKnight.util.geometry;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.graphics;
using Lens.util;
using Lens.util.camera;
using Lens.util.file;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using VelcroPhysics.Dynamics;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.room.controllable.platform {
	public class MovingPlatform : Platform, CollisionFilterEntity {
		protected byte tw = 2;
		protected byte th = 2;

		public PlatformController Controller = PlatformController.LeftRight;
		public int StartingStep;
		
		private int step;
		private Vector2 velocity;

		private PlatformBorder left;
		private PlatformBorder right;
		private PlatformBorder up;
		private PlatformBorder down;

		public override void AddComponents() {
			base.AddComponents();

			Width = tw * 16;
			Height = th * 16 + 6;

			AddComponent(new StateComponent());
			AddComponent(new AnimationComponent("moving_platform"));

			var w = tw * 16;
			var h = th * 16;
			var b = new RectBodyComponent(0.5f, 0.5f, w - 1, h - 1);
			AddComponent(b);

			b.Body.Friction = 0;
			
			Area.Add(left = new PlatformBorder());
			left.Setup(this, -12, 0, 8, th * 16);
			
			Area.Add(right = new PlatformBorder());
			right.Setup(this, tw * 16 + 4, 0, 8, th * 16);
			
			Area.Add(up = new PlatformBorder());
			up.Setup(this, 0, -14, tw * 16, 8);
			up.Upper = true;
			
			Area.Add(down = new PlatformBorder());
			down.Setup(this, 0, th * 16 + 2, tw * 16, 8);
		}

		public override void PostInit() {
			base.PostInit();

			step = StartingStep;
			Stop();
		}

		protected override bool ShouldMove(Entity e) {
			return base.ShouldMove(e) && e.Bottom < Bottom - 6;
		}

		protected void ResetBorders() {
			left.On = true;
			right.On = true;
			up.On = true;
			down.On = true;
		}

		protected void RoundUp() {
			var x = ((int) Math.Round(X / 16)) * 16;
			var y = ((int) Math.Round(Y / 16)) * 16;

			if (MathUtils.Distance(X - x, Y - y) < 3) {
				X = x;
				Y = y;
			}
		}

		#region Platform States
		protected class IdleState : SmartState<MovingPlatform> {
			private const float Delay = 1f;

			public override void Update(float dt) {
				base.Update(dt);

				Self.GetComponent<RectBodyComponent>().Velocity = Vector2.Zero;

				if (T >= Delay) {
					Become<MovingState>();
				}
			}
		}

		protected class MovingState : SmartState<MovingPlatform> {
			private const float Speed = 32;
			private bool first = true;

			public override void Init() {
				base.Init();
				Self.ResetBorders();
				Self.Center -= Self.velocity * 0.1f;
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (Math.Abs(Self.velocity.X) + Math.Abs(Self.velocity.Y) < 0.1f) {
					Self.velocity = directions[Random.Int(2) + 2];
				}

				if (Math.Abs(Self.velocity.X) > 0.1f && Math.Abs(Self.velocity.Y) > 0.1f) {
					Self.velocity.Y = 0;
				}
				
				Self.GetComponent<RectBodyComponent>().Velocity = Self.velocity * Speed;

				if (Math.Abs(Self.velocity.X) > 0.1f) {
					var s = (int) Math.Round(Self.Y / 16);
					var x = (int) (Self.velocity.X > 0 ? (Math.Ceiling(Self.X / 16) + Self.tw - 1) : (Math.Floor(Self.X / 16)));
					
					for (var y = s; y < s + Self.th; y++) {
						var t = Run.Level.Get(x, y);

						if (t != Tile.Chasm) {
							Self.RoundUp();
							Self.ResetBorders();

							if (Self.velocity.X > 0) {
								Self.right.On = false;
							} else {
								Self.left.On = false;
							}
							
							Self.Stop();

							if (first) {
								Become<MovingState>();
							}
							
							break;
						}
					}
				} else {
					var s = (int) Math.Round(Self.X / 16);
					var y = (int) (Self.velocity.Y > 0 ? (Math.Ceiling(Self.Y / 16) + Self.th - 1) : (Math.Floor(Self.Y / 16)));
					
					for (var x = s; x < s + Self.tw; x++) {
						var t = Run.Level.Get(x, y);

						if (t != Tile.Chasm) {
							Self.RoundUp();
							Self.ResetBorders();
							
							if (Self.velocity.Y > 0) {
								Self.down.On = false;
							} else {
								Self.up.On = false;
							}

							Self.Stop();

							if (first) {
								Become<MovingState>();
							}
							
							break;
						}
					}
				}

				first = false;
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
				case PlatformController.LeftRight: default: {
					step %= 2;

					if (Math.Abs(velocity.X) < 0.1f) {
						velocity = directions[0];
					}
					
					velocity.X *= -1;
					break;
				}
				
				case PlatformController.UpDown: {
					step %= 2;

					if (Math.Abs(velocity.X) < 0.1f) {
						velocity = directions[1];
					}
					
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
				Camera.Instance.ShakeMax(4);
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
			stream.WriteByte((byte) StartingStep);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);

			Controller = (PlatformController) stream.ReadByte();
			StartingStep = stream.ReadByte();
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent ev) {
				if (ev.Entity is MovingPlatform m) {
					var rect = new Rectangle((int) X, (int) Y, (int) Width, (int) Height - 6);
					
					rect.X += (int) (velocity.X * Width);
					rect.Y += (int) (velocity.Y * (Height - 6));

					if (new Rectangle((int) m.X, (int) m.Y, (int) m.Width, (int) m.Height - 6).Intersects(rect)) {
						Stop();
					}
				}
			}
			
			return base.HandleEvent(e);
		}

		bool CollisionFilterEntity.ShouldCollide(Entity entity) {
			return entity is MovingPlatform;
		}
	}
}