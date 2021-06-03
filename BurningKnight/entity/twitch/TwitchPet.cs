using System;
using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.pet;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.entity.projectile;
using BurningKnight.state;
using Lens;
using Lens.entity;
using Lens.graphics;
using Lens.util;
using Lens.util.camera;
using Lens.util.file;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.twitch {
	public class TwitchPet : AnimatedFollowerPet {
		public string Color = "#ffffff";
		public string Nick = "egordorichev";

		private float nickW;
		private float nickTimeLeft = 5;
		private bool nickOn = true;
		private float a = 1;
		
		public TwitchPet() : base("twitch_pet") {
			
		}

		public override void PostInit() {
			base.PostInit();

			UpdateColor();
			nickW = Nick.Length * 4;

			try {
				nickW = Font.Small.MeasureString(Nick).Width;
			} catch (Exception e) {
				
			}

			AddTag(Tags.PlayerSave);
			Subscribe<ItemUsedEvent>(); 
		}

		public void UpdateColor() {
			GetComponent<AnimationComponent>().Tint = ColorUtils.FromHex(Color);
		}
		
		private void RenderNick() {
			if (a <= 0.01f) {
				return;
			}
			
			Graphics.Color.A = (byte) (a * 255);
			Graphics.Print(Nick, Font.Small, Camera.Instance.CameraToUi(new Vector2(CenterX, Y - 10)) - new Vector2(nickW * 0.5f, 0));
			Graphics.Color.A = 255;
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			
			Color = stream.ReadString();
			Nick = stream.ReadString();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			stream.WriteString(Color);
			stream.WriteString(Nick);
		}

		private float cooldown;

		public override bool HandleEvent(Event e) {
			if (e is ItemUsedEvent iue && iue.Item.Type == ItemType.Weapon && cooldown <= 0.01f) {
				cooldown = 2f;
				Owner.GetComponent<AudioEmitterComponent>().EmitRandomizedPrefixed("item_meatguy", 4, 0.5f);
						
				var a = AngleTo(Owner.GetComponent<AimComponent>().RealAim);
				var an = GetComponent<AnimationComponent>();

				var projectile = new ProjectileBuilder(Owner, "small") {
					Scale = 0.8f,
					LightRadius = 32f,
					Color = an.Tint
				}.Shoot(a, 8f).Build();

				projectile.Center = Center + MathUtils.CreateVector(a, 5f);

				an.Animate();
				
				Owner.HandleEvent(new ProjectileCreatedEvent {
					Projectile = projectile,
					Owner = Owner
				});

				projectile.Owner = this;
			}
			
			return base.HandleEvent(e);
		}
		
		private bool added;

		public override void Update(float dt) {
			base.Update(dt);

			nickTimeLeft -= dt;

			if (nickTimeLeft <= 0) {
				nickOn = !nickOn;

				if (nickOn) {
					nickTimeLeft = Rnd.Float(3, 6);
				} else {
					nickTimeLeft = Rnd.Float(32, 128);
				}
			}

			a += ((nickOn ? 1 : 0) - a) * dt * 4;
			
			if (!added) {
				added = true;
				((InGameState) Engine.Instance.State).Ui.Add(new RenderTrigger(this, RenderNick, 0));
			}

			if (Owner == null && Area.Tagged[Tags.Player].Count > 0) {
				Owner = Area.Tagged[Tags.Player][0];
				Follow();
			}

			if (cooldown > 0) {
				cooldown -= dt;
			}
		}

		protected override void Follow() {
			base.Follow();

			if (Owner != null) {
				var f = GetComponent<FollowerComponent>();

				if (f.Follower == Owner) {
					f.MaxDistance = 48;
					f.FollowSpeed = 2;
				}
			}
		}
	}
}