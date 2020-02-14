using System;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using BurningKnight.level;
using BurningKnight.level.rooms;
using BurningKnight.physics;
using BurningKnight.state;
using BurningKnight.util;
using ImGuiNET;
using Lens.entity;
using Lens.util;
using Lens.util.file;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.room.controllable.turret {
	public class Turret : RoomControllable, CollisionFilterEntity {
		private float beforeNextBullet;
		protected bool Rotates;

		protected uint Angle {
			get => GetComponent<AnimationComponent>().Animation.Frame;
			set => GetComponent<AnimationComponent>().Animation.Frame = value;
		}

		public uint StartingAngle;
		public bool ReverseDirection;
		public float TimingOffset;
		public float Speed = 1f;

		public override void Init() {
			Y -= 4;
			base.Init();
		}

		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new RectBodyComponent(3, 4, 10, 11, BodyType.Static));

			var a = new AnimationComponent("turret", "on");
			AddComponent(a);

			a.Animation.Tag = "single";
			a.Animation.Paused = true;
			a.ShadowOffset = 4;
			
			AddComponent(new ShadowComponent(RenderShadow));
			AddComponent(new ExplodableComponent());
			AddComponent(new AudioEmitterComponent());

			AlwaysActive = true;

			if (Run.Depth == -2) {
				On = false;
			}
		}
		
		public override bool HandleEvent(Event e) {
			if (e is ExplodedEvent) {
				Done = true;
				RemoveFromRoom();
			}
			
			return base.HandleEvent(e);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			
			StartingAngle = stream.ReadByte();
			ReverseDirection = stream.ReadBoolean();
			TimingOffset = stream.ReadFloat();
			Speed = stream.ReadFloat();

			if (Math.Abs(Speed) < 0.01f) {
				Speed = 1f;
			}

			if (Run.Depth == -2) {
				TimingOffset = 0;
			}
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			stream.WriteByte((byte) StartingAngle);
			stream.WriteBoolean(ReverseDirection);
			stream.WriteFloat(TimingOffset);
			stream.WriteFloat(Speed);
		}

		public override void PostInit() {
			base.PostInit();
			Angle = StartingAngle;
		}

		public override void TurnOn() {
			base.TurnOn();
			
			beforeNextBullet = 0.2f + TimingOffset % 3;
			
			var a = GetComponent<AnimationComponent>();

			a.Scale.X = 2f;
			a.Scale.Y = 0.4f;
			a.Animation.Layer = "on";

			Tween.To(1f, a.Scale.X, x => a.Scale.X = x, 0.3f);
			Tween.To(1f, a.Scale.Y, x => a.Scale.Y = x, 0.3f);
		}

		public override void TurnOff() {
			base.TurnOff();
			
			var a = GetComponent<AnimationComponent>();

			a.Scale.X = 2f;
			a.Scale.Y = 0.4f;
			a.Animation.Layer = "off";

			Tween.To(1f, a.Scale.X, x => a.Scale.X = x, 0.3f);
			Tween.To(1f, a.Scale.Y, x => a.Scale.Y = x, 0.3f);
		}

		public override void Update(float dt) {
			base.Update(dt);
			
			// Always enabled in tutorial
			if (Run.Depth != -2 && On) {
				var room = GetComponent<RoomComponent>().Room;

				if (room != null && room.Type == RoomType.Regular) {
					if (room.Tagged[Tags.MustBeKilled].Count == 0) {

						TurnOff();
						return;
					}
				}
			}
			
			if (!On) {
				return;
			}

			beforeNextBullet -= dt * Speed;

			if (beforeNextBullet <= 0) {
				beforeNextBullet = 3;
				
				var a = GetComponent<AnimationComponent>();

				Tween.To(0.6f, a.Scale.X, x => a.Scale.X = x, 0.2f);
				Tween.To(1.6f, a.Scale.Y, x => a.Scale.Y = x, 0.2f).OnEnd = () => {

					Tween.To(1.8f, a.Scale.X, x => a.Scale.X = x, 0.1f);
					Tween.To(0.2f, a.Scale.Y, x => a.Scale.Y = x, 0.1f).OnEnd = () => {

						Tween.To(1, a.Scale.X, x => a.Scale.X = x, 0.4f);
						var t = Tween.To(1, a.Scale.Y, x => a.Scale.Y = x, 0.4f);

						if (Rotates) {
							t.OnEnd = () => {
								a.Animate();
								GetComponent<AudioEmitterComponent>().EmitRandomized("level_turret_rotating");
								Angle = (uint) ((Angle + (ReverseDirection ? -1 : 1)) % 8);
							};
						}

						var r = GetComponent<RoomComponent>();

						if (r.Room != null && r.Room.Tagged[Tags.Player].Count > 0) {
							Fire(Angle / 4f * Math.PI);

							if (OnScreen) {
								GetComponent<AudioEmitterComponent>().EmitRandomized("level_turret_fire");
							}
						}
					};
				};
			}
		}

		protected virtual void Fire(double angle) {
			SendProjectile(angle);
		}

		protected void SendProjectile(double angle) {
			var projectile = Projectile.Make(this, "small", angle, 6f);

			projectile.Center += MathUtils.CreateVector(angle, 8f);
			projectile.AddLight(32f, Projectile.RedLight);

			AnimationUtil.Poof(projectile.Center);
		}

		public override void Render() {
			if (Run.Depth != -2 || On) {
				base.Render();
			}
		}

		public void RenderShadow() {
			if (Run.Depth != -2 || On) {
				GraphicsComponent.Render(true);
			}
		}

		public override void RenderImDebug() {
			base.RenderImDebug();
			var u = (int) StartingAngle;

			if (ImGui.InputInt("Starting angle", ref u)) {
				Angle = StartingAngle = (uint) u % 8;
			}

			ImGui.InputFloat("Before next", ref beforeNextBullet);
			ImGui.InputFloat("Speed", ref Speed);
		}
		
		public bool ShouldCollide(Entity entity) {
			return !(entity is Chasm);
		}
	}
}