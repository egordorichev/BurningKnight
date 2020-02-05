using System;
using BurningKnight.assets;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;
using BurningKnight.entity.projectile;
using BurningKnight.entity.projectile.controller;
using BurningKnight.level;
using BurningKnight.level.entities;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.graphics;
using Lens.util.camera;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.boss {
	public class QueenBee : Boss {
		public bool InSecondPhase {
			get {
				var p = GetComponent<HealthComponent>().Percent;
				return p > 0.33f && p <= 0.66f;
			}
		}

		public bool InThirdPhase => GetComponent<HealthComponent>().Percent <= 0.33f;
		public bool InFirstPhase => GetComponent<HealthComponent>().Percent > 0.66f;
		public int Phase => (InThirdPhase ? 3 : (InSecondPhase ? 2 : 1));

		private int lastPhase = 1;
		private float lastParticle;

		public override void AddComponents() {
			base.AddComponents();

			Width = 23;
			Height = 19;
			
			AddComponent(new SensorBodyComponent(1, 4, 21, 14));

			var body = new RectBodyComponent(2, 17, 19, 1);
			AddComponent(body);

			body.Body.LinearDamping = 3;

			AddAnimation("bigbee");
			SetMaxHp(200);

			Flying = true;
			Depth = Layers.FlyingMob;

		}

		protected override void AddPhases() {
			base.AddPhases();
			
			HealthBar.AddPhase(0.33f);
			HealthBar.AddPhase(0.66f);
		}
		
		public override void Update(float dt) {
			base.Update(dt);

			lastParticle -= dt;

			if (lastParticle <= 0) {
				lastParticle = 0.1f;

				if (!IsFriendly()) {
					var s = GraphicsComponent.Flipped ? -1 : 1;
					
					Area.Add(new FireParticle {
						Offset = new Vector2(6 * s, -5.5f),
						Owner = this,
						Size = 0.5f,
						Depth = Depth + 1
					});

					Area.Add(new FireParticle {
						Offset = new Vector2(9 * s, -5.5f),
						Owner = this,
						Size = 0.5f,
						Depth = Depth + 1
					});

					if (Died) {
						return;
					}

					var p = Phase;

					if (lastPhase != p) {
						lastPhase = p;
						Become<SwitchPhaseState>();
					}
				}
			}
		}
		
		#region Queen Bee States
		public override void SelectAttack() {
			base.SelectAttack();
			Become<IdleState>();
		}
		
		public class IdleState : SmartState<QueenBee> {
			public override void Update(float dt) {
				base.Update(dt);

				if (T >= 0.5f) {
					Become<PrepareToPenetrateState>();
				}
			}
		}
		
		public class SwitchPhaseState : SmartState<QueenBee> {
			public override void Update(float dt) {
				base.Update(dt);

				if (T >= 0.5f) {
					Become<IdleState>();
				}
			}
		}

		public class PrepareToPenetrateState : SmartState<QueenBee> {
			private float x;
			private bool locked;
			
			public override void Init() {
				base.Init();

				var r = Self.GetComponent<RoomComponent>().Room;

				if (r.CenterX > Self.CenterX) {
					x = r.X - 32;
				} else {
					x = r.Right + 32;
				}
			}

			public override void Update(float dt) {
				base.Update(dt);
				
				if (locked) {
					if (T >= 0.5f) {
						Become<PenetrateState>();
					}

					return;
				}

				var body = Self.GetComponent<RectBodyComponent>();
				body.Velocity += new Vector2((x > Self.CenterX ? 1 : -1) * dt * 360, 0);

				var py = Self.Target.CenterY;
				var sy = Self.CenterY;
				
				body.Velocity += new Vector2(0, (py > sy ? 1 : -1) * dt * 360);

				if (Math.Abs(py - sy) < 8 && Math.Abs(x - Self.CenterX) < 16) {
					locked = true;
					T = 0;
					body.Velocity -= body.Velocity * (dt * 20);
				}
			}
		}

		public class PenetrateState : SmartState<QueenBee> {
			private float x;
			private float sign;
			private float delay = 1;
			private float lastBullet;
			
			public override void Init() {
				base.Init();

				var r = Self.GetComponent<RoomComponent>().Room;

				if (r.CenterX < Self.CenterX) {
					x = r.X;
					sign = -1;
				} else {
					x = r.Right;
					sign = 1;
				}

				Camera.Instance.Shake(10);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (!Self.InFirstPhase) {
					lastBullet -= dt;

					if (lastBullet <= 0) {
						lastBullet = 0.2f;
						
						var a = (sign < 0 ? Math.PI : 0) + Rnd.Float(-1f, 1f);
						var p = Projectile.Make(Self, "circle", a, Rnd.Float(3f, 10f), scale: Rnd.Float(0.4f, 1f));
						p.Color = ProjectileColor.Orange;
						p.BounceLeft = 5;
						p.Controller += SlowdownProjectileController.Make(0.25f);
						p.AddLight(64f, ProjectileColor.Orange);
					}
				}
				
				var body = Self.GetComponent<RectBodyComponent>();

				if ((Self.CenterX - x) * sign >= 16) {
					body.Velocity -= body.Velocity * dt;
					delay -= dt;

					if (delay <= 0) {
						Become<IdleState>();
					}
					
					return;
				}

				Self.X += sign * dt * 360;
				body.Velocity += new Vector2(sign * dt * 3600, 0);
			}
		}
		#endregion

		public override void Render() {
			base.Render();
			Graphics.Print(GetComponent<StateComponent>().State.Name, Font.Small, BottomCenter);
		}

		public override bool ShouldCollide(Entity entity) {
			if (entity is Prop) {
				return false;
			}
			
			return base.ShouldCollide(entity) && !(entity is Level);
		}
	}
}