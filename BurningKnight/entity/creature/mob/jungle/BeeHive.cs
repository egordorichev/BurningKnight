using System;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using BurningKnight.entity.projectile.controller;
using BurningKnight.level.entities.decor;
using BurningKnight.util;
using Lens.assets;
using Lens.util.file;
using Lens.util.math;
using Lens.util.timer;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.jungle {
	public class BeeHive : Mob {
		private const float ZHeight = 8;
		private bool loaded;
		private Tree tree;
		
		protected override void SetStats() {
			base.SetStats();

			Width = 13;
			Depth = 1;
			
			AddAnimation("beehive");
			SetMaxHp(10);
			Become<IdleState>();

			var body = new SensorBodyComponent(2, 3, 9, 11);
			AddComponent(body);
			body.KnockbackModifier = 0;

			GetComponent<MobAnimationComponent>().ShadowOffset = -ZHeight;
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			loaded = true;
		}

		public override void PostInit() {
			base.PostInit();

			if (!loaded) {
				tree = new Tree();
				tree.Id = 5;
				tree.AlwaysShow = true;
				Area.Add(tree);
			}

			Timer.Add(() => { 
				GetComponent<AudioEmitterComponent>().Emit("mob_hive_static", 0.8f, looped: true, tween: true);
			}, 5f);
		}

		public override void Destroy() {
			base.Destroy();
			GetComponent<AudioEmitterComponent>().StopAll();
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (tree != null) {
				tree.Position = Position - new Vector2(15, 16);
			}
		}

		#region Bee Hive States
		public class IdleState : SmartState<BeeHive> {
			public override void Update(float dt) {
				base.Update(dt);

				if (Self.Target != null) {
					if (T >= 10f) {
						T = 0;
						
						var bee = GenerateBee();
						Self.GetComponent<MobAnimationComponent>().Animate();
						Self.Area.Add(bee);
						bee.BottomCenter = Self.BottomCenter;
						AnimationUtil.Ash(bee.Center);
						
						Self.GetComponent<AudioEmitterComponent>().Emit("mob_hive_pop");
					}
				} else {
					T = 0;
				}
			}
		}

		public class FallingState : SmartState<BeeHive> {
			public override void Init() {
				base.Init();
				
				Self.GetComponent<AudioEmitterComponent>().Emit("mob_hive_breaking");
				Self.tree = null;

				var y = Self.Y;
				var c = Self.GetComponent<MobAnimationComponent>();
				
				Tween.To(0, c.ShadowOffset, x => c.ShadowOffset = x, 0.4f, Ease.QuadIn);
				Tween.To(y + ZHeight, y, x => Self.Y = x, 0.4f, Ease.QuadIn).OnEnd = () => {
					Audio.PlaySfx("mob_hive_release");
					
					Self.AnimateDeath(null);
					Self.GetComponent<AudioEmitterComponent>().StopAll();
					
					var am = 16;
			
					for (var i = 0; i < am; i++) {
						var a = Math.PI * 2 * (((float) i) / am) + Rnd.Float(-1f, 1f);
						var p = Projectile.Make(Self, "circle", a, Rnd.Float(3f, 10f), scale: Rnd.Float(0.4f, 1f));
						p.Color = ProjectileColor.Orange;
						p.BounceLeft = 5;
						p.Controller += SlowdownProjectileController.Make(0.25f, 0.5f, 1f);
					}

					for (var i = 0; i < Rnd.Int(4, 10); i++) {
						var bee = GenerateBee();
						Self.Area.Add(bee);
						bee.Center = Self.Center;
					}
				};
			}
		}
		#endregion

		public static Bee GenerateBee() {
			var r = Rnd.Float();

			if (r < 0.2f) {
				return new Explobee();
			}/* else if (r < 0.3f) {
				return new BigBee();
			}*/

			return new Bee();
		}

		protected override bool HandleDeath(DiedEvent d) {
			Become<FallingState>();
			return true;
		}

		protected override string GetHurtSfx() {
			return "mob_hive_hurt";
		}
	}
}