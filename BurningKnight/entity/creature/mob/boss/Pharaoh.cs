using System;
using System.Collections.Generic;
using BurningKnight.assets.achievements;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.desert;
using BurningKnight.entity.projectile;
using BurningKnight.entity.room.controllable;
using BurningKnight.level.tile;
using BurningKnight.state;
using BurningKnight.util.geometry;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.util;
using Lens.util.math;
using Lens.util.timer;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace BurningKnight.entity.creature.mob.boss {
	public class Pharaoh : Boss {
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
		
		public override void AddComponents() {
			base.AddComponents();

			Width = 15;
			Height = 18;
			
			AddComponent(new SensorBodyComponent(1, 1, 13, 17));

			var body = new RectBodyComponent(2, 17, 11, 1);
			AddComponent(body);

			body.KnockbackModifier = 0.1f;
			body.Body.LinearDamping = 4;

			AddAnimation("pharaoh");
			SetMaxHp(300);
		}

		protected override void AddPhases() {
			base.AddPhases();
			
			HealthBar.AddPhase(0.33f);
			HealthBar.AddPhase(0.66f);
		}

		private float lastParticle;
		private float t;

		public override void Update(float dt) {
			base.Update(dt);

			lastParticle -= dt;
			t += dt;

			if (lastParticle <= 0) {
				lastParticle = 0.1f;

				if (!IsFriendly()) {
					Area.Add(new FireParticle {
						Offset = new Vector2(-2, -11),
						Owner = this,
						Size = 0.5f,
						Depth = Depth + 1
					});

					Area.Add(new FireParticle {
						Offset = new Vector2(2, -11),
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

		public override void SelectAttack() {
			base.SelectAttack();
			Become<IdleState>();
		}

		private int counter;

		protected override void OnTargetChange(Entity target) {
			base.OnTargetChange(target);

			if (target == null) {
				Become<FriendlyState>();
			}
		}

		#region Pharaoh States
		public class IdleState : SmartState<Pharaoh> {
			public override void Update(float dt) {
				if (Self.Target == null) {
					return;
				}

				base.Update(dt);
				
				if (T >= (Self.InThirdPhase ? 0.01f : 0.3f)) {
					var v = Self.counter;

					if (v == 0) {
						Become<SimpleSpiralState>();
					} else if (v == 1) {
						if (Self.InThirdPhase) {
							Become<BulletHellState>();
						} else {
							Become<TileMoveState>();
						}
					} else if (v == 2) {
						Become<TeleportState>();
					} else if (v == 3) {
						Become<BulletHellState>();
					} else if (v == 4) {
						Become<TeleportState>();
					} else if (v == 5) {
						Become<AdvancedSpiralState>();
					} else if (v == 6) {
						if (Self.InThirdPhase) {
							Become<TeleportState>();
						} else {
							Become<TileMoveState>();
						}
					}

					Self.counter = (v + 1) % 7;
				}
			}
		}

		public class SimpleSpiralState : SmartState<Pharaoh> {
			private float sinceLast;
			private int count;
		
			public override void Update(float dt) {
				base.Update(dt);
				sinceLast -= dt;

				if (sinceLast <= 0) {
					sinceLast = 0.2f;
					var amount = 2 + (Self.Phase - 1);

					var builder = new ProjectileBuilder(Self, "small");

					builder.RemoveFlags(ProjectileFlags.Reflectable, ProjectileFlags.BreakableByMelee);

					for (var i = 0; i < amount; i++) {
						var a = Math.PI * 2 * ((float) i / amount) + Math.Cos(Self.t * 0.8f) * Math.PI;

						builder.Color = count % 4 == 0 ? ProjectileColor.Orange : ProjectileColor.Red;
						builder.Scale = count % 2 == 0 ? 1.5f : 2f;
						builder.Shoot(a, 8f).Build();
					}

					Self.GetComponent<AudioEmitterComponent>().EmitRandomized("mob_pharaoh_shot");
					count++;
				}

				if (T >= 8f) {
					Become<IdleState>();
				}
			}
		}
		
		public class AdvancedSpiralState : SmartState<Pharaoh> {
			private float sinceLast;
			private int count;
		
			public override void Update(float dt) {
				base.Update(dt);
				sinceLast -= dt;

				if (sinceLast <= 0) {
					sinceLast = Self.InFirstPhase ? 0.6f : 0.4f;
					var amount = 8;

					var builder = new ProjectileBuilder(Self, "small") {
						Scale = 1.5f
					};

					builder.RemoveFlags(ProjectileFlags.Reflectable, ProjectileFlags.BreakableByMelee);

					for (var i = 0; i < amount; i++) {
						var a = Math.PI * 2 * ((float) i / amount) + (Math.Cos(Self.t * 1) * Math.PI * 0.25f) * (i % 2 == 1 ? -1 : 1);

						builder.Color = ProjectileColor.DesertRainbow[count % ProjectileColor.DesertRainbow.Length];

						builder.Shoot(a, 6f + (float) Math.Cos(Self.t * 1) * 2f).Build();
					}

					Self.GetComponent<AudioEmitterComponent>().EmitRandomized("mob_pharaoh_shot_wave");
					count++;
				}
				

				if (T >= 10f) {
					Become<IdleState>();
				}
			}
		}

		public class BulletHellState : SmartState<Pharaoh> {
			private float sinceLast;
		
			public override void Update(float dt) {
				base.Update(dt);
				sinceLast -= dt;

				if (sinceLast <= 0) {
					sinceLast = Self.InFirstPhase ? 0.4f : 0.2f;

					var amount = 4;
					var builder = new ProjectileBuilder(Self, "small") {
						Scale = 1.5f
					};

					builder.RemoveFlags(ProjectileFlags.Reflectable, ProjectileFlags.BreakableByMelee);

					for (var i = 0; i < amount; i++) {
						builder.Color = ProjectileColor.DesertRainbow[Rnd.Int(ProjectileColor.DesertRainbow.Length)];

						var a = Math.PI * 2 * ((float) i / amount) + (Math.Cos(Self.t * 2f) * Math.PI) * (i % 2 == 0 ? -1 : 1);
						builder.Shoot(a, 7f + (float) Math.Cos(Self.t * 2f) * 2f).Build();
					}
				}

				if (T >= 10f) {
					Become<IdleState>();
				}
			}
		}
		
		public class TileMoveState : SmartState<Pharaoh> {
			private Dot PickDot() {
				var room = Self.GetComponent<RoomComponent>().Room;
				var attempt = 0;
				var toCheck = new List<Entity>();

				toCheck.Add(Self);
				toCheck.AddRange(room.Tagged[Tags.Player]);

				Dot spot;

				do {
					spot = new Dot(Rnd.Int(room.MapX + 3, room.MapX + room.MapW - 3),
						Rnd.Int(room.MapY + 3, room.MapY + room.MapH - 3));

					var vector = spot * 16 + new Vector2(8, 8);
					var found = false;
					
					foreach (var e in toCheck) {
						if (e.DistanceTo(vector) <= 48f) {
							found = true;
							break;
						}
					}

					if (!found) {
						return spot;
					}

					if (attempt++ > 30) {
						Become<IdleState>();
						return null;
					}
				} while (true);

				return spot;
			}
			
			public override void Init() {
				base.Init();
				
				var spot = PickDot();
				var to = PickDot();

				if (spot == null || to == null) {
					return;
				}

				var sx = Rnd.Int(1, 4);
				var sy = Rnd.Int(1, 4);

				for (var x = sx > 2 ? -1 : 0; x < sx; x++) {
					for (var y = sy > 2 ? -1 : 0; y < sy; y++) {
						var x1 = x;
						var y1 = y;

						Timer.Add(() => {
							var part = new TileParticle();

							part.FromBottom = true;
							part.Top = Run.Level.Tileset.FloorA[0];
							part.TopTarget = Run.Level.Tileset.WallTopADecor;
							part.Side = Run.Level.Tileset.WallA[0];
							part.Sides = Run.Level.Tileset.WallSidesA[2];
							part.Tile = Tile.WallA;

							part.X = (spot.X + x1) * 16;
							part.Y = (spot.Y + y1) * 16 + 8;
							part.Target.X = (to.X + x1) * 16;
							part.Target.Y = (to.Y + y1) * 16 + 8;

							Self.Area.Add(part);
						}, Rnd.Float(1f));
					}
				}
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (T >= 4f) {
					Become<IdleState>();
				}
			}
		}

		public class SwitchPhaseState : SmartState<Pharaoh> {
			public override void Init() {
				base.Init();

				var am = 24;
				var builder = new ProjectileBuilder(Self, "small");
				builder.RemoveFlags(ProjectileFlags.Reflectable, ProjectileFlags.BreakableByMelee);

				for (var j = 0; j < 2; j++) {
					var j1 = j;

					Timer.Add(() => {
						var z = (j1 == 0 ? am : am / 2);
						
						for (var i = 0; i < z; i++) {
							var a = Math.PI * 2 * ((i + j1 * 0.5f) / z);

							builder.Scale = j1 == 0 ? 1f : 2f;
							builder.Shoot(a, 7f + j1 * 2).Build();
						}

						Self.GetComponent<AudioEmitterComponent>().EmitRandomized("mob_pharaoh_shot");
					}, j);
				}

				Become<IdleState>();
			}

			public override void Update(float dt) {
				base.Update(dt);
					
				if (T >= 2f) {
					Become<SummoningState>();
				}
			}
		}
		
		public class SummoningState : SmartState<Pharaoh> {
			private bool summoned;
			
			public override void Update(float dt) {
				base.Update(dt);

				if (!summoned && T >= 1f) {
					summoned = true;
					
					var amount = Self.InThirdPhase ? 4 : 2;
					var d = 16;

					for (var i = 0; i < amount; i++) {
						var i1 = i;

						Timer.Add(() => {
							Self.GetComponent<AudioEmitterComponent>().EmitRandomized("mob_pharaoh_summon");
							
							var mummy = new Mummy();
							Self.Area.Add(mummy);
							mummy.BottomCenter = Self.BottomCenter + MathUtils.CreateVector(i1 / (float) amount * Math.PI * 2, d);
							mummy.GetComponent<StateComponent>().Become<Mummy.SummonedState>();
						}, i * 0.5f);
					}
				} else if (summoned && T >= 2f) {
					Become<IdleState>();
				}
			}
		}

		public class TeleportState : SmartState<Pharaoh> {
			public override void Init() {
				base.Init();

				Self.GetComponent<AudioEmitterComponent>().EmitRandomized("mob_pharaoh_adidos");

				Tween.To(0, 255, x => Self.GetComponent<MobAnimationComponent>().Tint.A = (byte) x, 0.5f).OnEnd = () => {
					var tile = Self.GetComponent<RoomComponent>().Room.GetRandomWallFreeTile() * 16;

					Self.BottomCenter = tile + new Vector2(8, 8); 
					Self.GetComponent<AudioEmitterComponent>().EmitRandomized("mob_pharaoh_appear");

					Tween.To(255, 0, x => Self.GetComponent<MobAnimationComponent>().Tint.A = (byte) x, 0.5f).OnEnd = () => {
						Become<IdleState>();
					};
				};
			}
		}
		#endregion

		public override void PlaceRewards() {
			base.PlaceRewards();
			Achievements.Unlock("bk:mummified");
		}

		public override string GetScream() {
			return "pharaoh_scream";
		}
	}
}