using System;
using BurningKnight.assets;
using BurningKnight.assets.achievements;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;
using BurningKnight.entity.creature.mob.prefabs;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using BurningKnight.state;
using Lens.entity;
using Lens.graphics;
using Lens.util;
using Lens.util.file;
using Lens.util.math;
using Lens.util.timer;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob {
	public class Mimic : Slime {
		public string Kind;
		public string Pool = "bk:wooden_chest";
		private bool invoked;
		
		protected override void SetStats() {
			base.SetStats();

			Width = 16;
			Height = 13;
			
			SetMaxHp(40 + Run.Depth * 7);

			var body = CreateBodyComponent();
			AddComponent(body);

			body.Body.LinearDamping = 2;
			body.KnockbackModifier = 0.5f;

			AddComponent(CreateSensorBodyComponent());
		}

		public override void PostInit() {
			base.PostInit();

			if (Kind == null) {
				Kind = "wooden_chest";
			}
			
			AddComponent(new InteractableSliceComponent("props", Kind));
			GetComponent<DropsComponent>().Add(Pool);

			if (!invoked) {
				RemoveTag(Tags.MustBeKilled);
				RemoveTag(Tags.Mob);
				Become<FriendlyState>();
				TouchDamage = 0;
				
				AddComponent(new InteractableComponent(Interact) {
					CanInteract = e => !invoked
				});
			}
		}

		private bool Interact(Entity e) {
			if (invoked) {
				return true;
			}

			e.GetComponent<HealthComponent>().ModifyHealth(-2, this, DamageType.Custom);
			Target = e;
			Become<JumpState>();
			
			return true;
		}

		public override bool HandleEvent(Event e) {
			if (!invoked && e is HealthModifiedEvent hme && hme.Amount < 0) {
				Become<JumpState>();
			}
			
			return base.HandleEvent(e);
		}

		protected override TextureRegion GetDeathFrame() {
			return CommonAse.Props.GetSlice($"{Kind}_open");
		}

		protected virtual BodyComponent CreateBodyComponent() {
			return new RectBodyComponent(0, 12, 16, 1);
		}

		protected virtual BodyComponent CreateSensorBodyComponent() {
			return new SensorBodyComponent(0, 0, 16, 13);
		}

		protected override void OnJump() {
			base.OnJump();

			for (var i = 0; i < 3; i++) {
				Timer.Add(() => {
					if (Target == null) {
						return;
					}
				
					GetComponent<AudioEmitterComponent>().EmitRandomized("mob_fire");

					var a = AngleTo(Target) + Rnd.Float(-0.1f, 0.1f);
					var projectile = Projectile.Make(this, "circle", a, 9f);

					projectile.Spectral = true;
					projectile.Center = Center + MathUtils.CreateVector(a, 5f) - new Vector2(0, GetComponent<ZComponent>().Z);
					projectile.AddLight(32f, ProjectileColor.Red);
					projectile.CanBeBroken = false;
					projectile.CanBeReflected = false;
				}, i * 0.3f);
			}
		}

		protected override void OnLand() {
			if (Target == null) {
				return;
			}
			
			var am = 16;
			GetComponent<AudioEmitterComponent>().EmitRandomized("mob_fire");

			for (var i = 0; i < am; i++) {
				var a = Math.PI * 2 * (((float) i) / am);
				var fast = i % 2 == 0;
				var projectile = Projectile.Make(this, fast ? "small" : "circle", a, fast ? 7f : 4f);
					
				projectile.Center = BottomCenter;
				projectile.AddLight(32f, ProjectileColor.Red);
				projectile.CanBeBroken = false;
				projectile.CanBeReflected = false;
			}
		}
		
		protected override void AnimateJump(Action callback) {
			var anim = GetComponent<InteractableSliceComponent>();
				
			Tween.To(2f, anim.Scale.X, x => anim.Scale.X = x, 0.2f);
			Tween.To(0.3f, anim.Scale.Y, x => anim.Scale.Y = x, 0.2f).OnEnd = () => {
				Tween.To(0.5f, anim.Scale.X, x => anim.Scale.X = x, 0.3f);

				Tween.To(2f, anim.Scale.Y, x => anim.Scale.Y = x, 0.3f).OnEnd = () => {
					Tween.To(1, anim.Scale.X, x => anim.Scale.X = x, 0.2f);
					Tween.To(1, anim.Scale.Y, x => anim.Scale.Y = x, 0.2f);
				};

				callback();
			};
		}

		protected override void AnimateLand() {
			var anim = GetComponent<InteractableSliceComponent>();

			anim.Scale.X = 2f;
			anim.Scale.Y = 0.3f;
			Tween.To(1, anim.Scale.X, x => anim.Scale.X = x, 0.3f);
			Tween.To(1, anim.Scale.Y, x => anim.Scale.Y = x, 0.3f);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			
			Kind = stream.ReadString();
			invoked = stream.ReadBoolean();
			Pool = stream.ReadString();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			stream.WriteString(Kind);
			stream.WriteBoolean(invoked);
			stream.WriteString(Pool);
		}

		private class FriendlyState : SmartState<Mimic> {
			public override void Destroy() {
				base.Destroy();

				Self.invoked = true;
				Self.AddTag(Tags.MustBeKilled);
				Self.AddTag(Tags.Mob);
				Self.TouchDamage = 1;

				Achievements.Unlock("bk:mimic");
			}
		}
	}
}