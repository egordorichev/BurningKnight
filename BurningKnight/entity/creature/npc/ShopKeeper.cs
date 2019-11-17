using System;
using System.Numerics;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.entity.item.stand;
using BurningKnight.entity.room;
using BurningKnight.level.entities;
using BurningKnight.level.entities.machine;
using BurningKnight.save;
using BurningKnight.ui.dialog;
using Lens;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.util;
using Lens.util.camera;
using Lens.util.file;
using Lens.util.math;
using Lens.util.timer;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace BurningKnight.entity.creature.npc {
	public class ShopKeeper : Npc {
		private sbyte _mood = 3;
		private Item shotgun;

		public sbyte Mood {
			get => _mood;

			set {
				if (_mood == value) {
					return;
				}
				
				var r = raging;
				
				if (_mood < 0 && value > 0) {
					return;
				}

				var old = _mood;
				_mood = value;

				if (_mood < old && _mood < 3 && _mood > -1) {
					GetComponent<DialogComponent>().StartAndClose($"shopkeeper_{_mood}", 1);
				}

				if (raging) {
					GameSave.Put("sk_enraged", true);
				}

				if (!r && raging) {
					AddTag(Tags.Mob);
					GetComponent<RoomComponent>().Room.Tagged[Tags.Mob].Add(this); // Hacky solution tbh
					
					Become<RunState>();
					GetComponent<DialogComponent>().StartAndClose($"shopkeeper_{Rnd.Int(3, 5)}", 1);

					SetItemsFree();
				}

				Recalc();
			}
		}

		private bool raging => Mood < 0;
		
		private void Recalc() {
			var r = GetComponent<RoomComponent>().Room;

			if (r == null) {
				return;
			}
			
			foreach (var s in r.Tagged[Tags.Item]) {
				if (s is ShopStand ss) {
					ss.Recalculate();
				}
			}
		}
		
		public override void AddComponents() {
			base.AddComponents();
			AlwaysActive = true;
			
			AddComponent(new AimComponent(AimComponent.AimType.AnyPlayer));
			AddComponent(new AnimationComponent("shopkeeper"));

			AddDrops(new SingleDrop("bk:shotgun", 1f));
			
			var h = GetComponent<HealthComponent>();

			h.InitMaxHealth = 15;
			h.Unhittable = false;

			var b = new RectBodyComponent(4, 2, 10, 14);
			AddComponent(b);
			b.Body.LinearDamping = 5;
			
			Subscribe<BombPlacedEvent>();
			Subscribe<GramophoneBrokenEvent>();
			Subscribe<RerollMachine.BrokenEvent>();
			Subscribe<VendingMachine.BrokenEvent>();
			Subscribe<RoomChangedEvent>();
			Subscribe<ItemBoughtEvent>();
			
			AddTag(Tags.ShopKeeper);
			
			Become<IdleState>();
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			Mood = stream.ReadSbyte();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteSbyte(Mood);
		}

		public void Enrage() {
			if (raging) {
				return;
			}

			Mood = -1;
		}

		public override bool HandleEvent(Event e) {
			if (e is BombPlacedEvent bpe) {
				if (bpe.Bomb.GetComponent<RoomComponent>().Room == GetComponent<RoomComponent>().Room) {
					Enrage();
				}
			} else if (e is GramophoneBrokenEvent gbe) {
				if (gbe.Gramophone.GetComponent<RoomComponent>().Room == GetComponent<RoomComponent>().Room) {
					Mood--;
				}
			} else if (e is RerollMachine.BrokenEvent rme) {
				if (rme.Machine.GetComponent<RoomComponent>().Room == GetComponent<RoomComponent>().Room) {
					Mood--;
				}
			} else if (e is VendingMachine.BrokenEvent vme) {
				if (vme.Machine.GetComponent<RoomComponent>().Room == GetComponent<RoomComponent>().Room) {
					Mood--;
				}
			} else if (e is HealthModifiedEvent hme && hme.Amount < 0) {
				if (hme.Amount < 0 && Mood > -2) {
					Mood = (sbyte) Math.Min(2, Mood - 1);
					hme.Amount = -1;
				}
			} else if (e is RoomChangedEvent rce) {
				if (rce.Who is Player && rce.New == GetComponent<RoomComponent>().Room) {
					if (GameSave.IsTrue("sk_enraged")) {
						Enrage();
					}
					
					Recalc();
					
					if (Mood > -1) {
						GetComponent<AudioEmitterComponent>().EmitRandomized("hi");
						GetComponent<DialogComponent>().StartAndClose($"shopkeeper_{Rnd.Int(6, 9)}", 3);
					}
				}
			} else if (e is ItemBoughtEvent ibe) {
				if (ibe.Stand.GetComponent<RoomComponent>().Room == GetComponent<RoomComponent>().Room) {
					Mood++;
					GetComponent<DialogComponent>().StartAndClose($"shopkeeper_{Rnd.Int(9, 12)}", 3);
				}
			}

			return base.HandleEvent(e);
		}

		private void SetItemsFree() {
			var r = GetComponent<RoomComponent>().Room;

			if (r != null) {
				foreach (var s in r.Tagged[Tags.Item]) {
					if (s is ShopStand ss) {
						ss.Free = true;
					}
				}
			}
		}

		protected override bool HandleDeath(DiedEvent d) {
			Items.Unlock("bk:shotgun");
			SetItemsFree();
			
			return base.HandleDeath(d);
		}

		protected override string GetDeadSfx() {
			return "villager6";
		}

		protected override string GetHurtSfx() {
			return "villager5";
		}

		public override bool IsFriendly() {
			return !raging;
		}
		
		private float delay;

		public override void Update(float dt) {
			base.Update(dt);

			if (!raging) {
				delay -= dt;

				if (delay <= 0) {
					delay = Rnd.Float(1, 3f);
					GetComponent<AudioEmitterComponent>().EmitRandomized($"villager{Rnd.Int(1, 5)}");
				}
			}

			shotgun?.Update(dt);
		}

		#region Shopkeeper States
		/*
		 * Peacefull
		 */
		private class IdleState : SmartState<ShopKeeper> {
			private float actionDelay;

			public override void Init() {
				base.Init();
				actionDelay = Rnd.Float(2, 6);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (Self.GetComponent<DialogComponent>().Dialog.Saying) {
					T = 0;
					return;
				}
				
				if (T >= actionDelay) {
					Self.Become<MoveToState>();
				}
			}
		}

		private class MoveToState : SmartState<ShopKeeper> {
			private Vector2 target;
			private bool toPlayer;
			
			public override void Init() {
				base.Init();
				
				Self.GetComponent<AnimationComponent>().Animation.Tag = "run";
				var r = Self.GetComponent<RoomComponent>().Room;

				toPlayer = r.Tagged[Tags.Player].Count > 0 && Rnd.Chance(40);

				if (toPlayer) {
					if (Self.DistanceTo(r.Tagged[Tags.Player][0]) < 64f) {
						toPlayer = false;
					} else {
						return;
					}
				}
				
				var p = r.GetRandomFreeTile();
				target = new Vector2(p.X * 16, p.Y * 16);
			}

			public override void Update(float dt) {
				base.Update(dt);

				var t = target;

				if (toPlayer) {
					var a = Self.GetComponent<RoomComponent>().Room.Tagged[Tags.Player];

					if (a.Count > 0) {
						target = a[0].Center;
					}
				}
				
				var dx = Self.DxTo(t);
				var dy = Self.DyTo(t);
				var d = MathUtils.Distance(dx, dy);
				var s = dt * 300;

				var b = Self.GetComponent<RectBodyComponent>();
				b.Velocity += new Vector2(dx / d * s, dy / d * s);

				if (d <= 24) {
					if ((toPlayer && Rnd.Chance(80)) || Rnd.Chance(30)) {
						if (Self.GetComponent<RoomComponent>().Room.Tagged[Tags.Player].Count > 0) {
							Self.GetComponent<DialogComponent>().StartAndClose($"shopkeeper_{(Rnd.Chance(30) ? 18 : Rnd.Int(12, 15))}", 3);
						}
					}
					
					Self.Become<IdleState>();
				}
			}
		}

		public override void Render() {
			base.Render();
			shotgun?.Renderer.Render(false, Engine.Instance.State.Paused, Engine.Delta, false, 0);
		}

		protected override void RenderShadow() {
			base.RenderShadow();
			shotgun?.Renderer.Render(false, Engine.Instance.State.Paused, Engine.Delta, true, 0);
		}

		/*
		 * Raging
		 */
		private class RunState : SmartState<ShopKeeper> {
			private Vector2 target;
			private float delay;
			private bool set;
			
			public override void Init() {
				base.Init();

				delay = Rnd.Float(0.2f, 0.8f);
				
				Self.GetComponent<AnimationComponent>().Animation.Tag = "run";
				var r = Self.GetComponent<RoomComponent>().Room;

				if (r != null) {
					set = true;
					var p = r.GetRandomFreeTile();
					target = new Vector2(p.X * 16, p.Y * 16);

					if (Self.shotgun == null) {
						Self.shotgun = Items.CreateAndAdd("bk:shotgun", Self.Area);
						Self.shotgun.RemoveDroppedComponents();
						Self.shotgun.AddComponent(new OwnerComponent(Self));
					}
				}
			}
			
			public override void Update(float dt) {
				base.Update(dt);

				if (!set) {
					Init();
				}

				if (Self.shotgun != null && Self.GetComponent<RoomComponent>().Room.Tagged[Tags.Player].Count > 0) {
					Self.shotgun.Use(Self);
				}
				
				var dx = Self.DxTo(target);
				var dy = Self.DyTo(target);
				var d = MathUtils.Distance(dx, dy);
				var s = Math.Min(T * 3, 1f) * dt * 36000;

				var b = Self.GetComponent<RectBodyComponent>();
				b.Velocity = new Vector2(dx / d * s, dy / d * s);

				if (d <= 8 || T >= delay) {
					T = 0;
					Init();
				}
			}
		}
		#endregion

		public override bool ShouldCollide(Entity entity) {
			return !(entity is ItemStand) && base.ShouldCollide(entity);
		}
	}
}