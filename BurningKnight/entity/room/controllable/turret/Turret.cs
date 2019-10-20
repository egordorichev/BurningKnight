using System;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using BurningKnight.level;
using BurningKnight.level.rooms;
using BurningKnight.physics;
using BurningKnight.util;
using Lens.entity;
using Lens.util;
using Lens.util.file;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.room.controllable.turret {
	public class Turret : RoomControllable {
		private float beforeNextBullet;
		protected bool Rotates;

		protected uint Angle {
			get => GetComponent<AnimationComponent>().Animation.Frame;
			set => GetComponent<AnimationComponent>().Animation.Frame = value;
		}

		public uint StartingAngle;
		public bool ReverseDirection;

		public override void Init() {
			Y -= 8;
			base.Init();
		}

		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new RectBodyComponent(4, 4, 8, 6, BodyType.Static));

			var a = new AnimationComponent("turret");
			AddComponent(a);

			a.Animation.Tag = "single";
			a.Animation.Paused = true;
			a.ShadowOffset = 4;
			
			AddComponent(new ShadowComponent());
			AddComponent(new ExplodableComponent());

			AlwaysActive = true;
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
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteByte((byte) StartingAngle);
			stream.WriteBoolean(ReverseDirection);
		}

		public override void PostInit() {
			base.PostInit();
			Angle = StartingAngle;
		}

		public override void TurnOn() {
			base.TurnOn();
			beforeNextBullet = 0;
		}

		public override void Update(float dt) {
			base.Update(dt);

			var room = GetComponent<RoomComponent>().Room;

			if (room != null) {
				var a = room.Type == RoomType.Regular;
				var b = room.Tagged[Tags.MustBeKilled].Count == 0;

				if (a && b) {
					return;
				}
			}

			beforeNextBullet -= dt;

			if (beforeNextBullet <= 0) {
				beforeNextBullet = 2;
				
				var a = GetComponent<AnimationComponent>();

				Tween.To(0.6f, a.Scale.X, x => a.Scale.X = x, 0.2f);
				Tween.To(1.6f, a.Scale.Y, x => a.Scale.Y = x, 0.2f).OnEnd = () => {

					Tween.To(1.8f, a.Scale.X, x => a.Scale.X = x, 0.1f);
					Tween.To(0.2f, a.Scale.Y, x => a.Scale.Y = x, 0.1f).OnEnd = () => {

						Tween.To(1, a.Scale.X, x => a.Scale.X = x, 0.4f);
						var t = Tween.To(1, a.Scale.Y, x => a.Scale.Y = x, 0.4f);

						if (Rotates) {
							t.OnEnd = () => Angle = (uint) ((Angle + (ReverseDirection ? -1 : 1)) % 8);
						}

						var r = GetComponent<RoomComponent>();

						if (r.Room != null && r.Room.Tagged[Tags.Player].Count > 0) {
							Fire(Angle / 4f * Math.PI);
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
	}
}