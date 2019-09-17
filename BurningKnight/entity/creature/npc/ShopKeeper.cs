using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.level.entities;
using BurningKnight.save;
using BurningKnight.ui.dialog;
using Lens;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.util;
using Lens.util.camera;
using Lens.util.file;
using Lens.util.timer;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.creature.npc {
	public class ShopKeeper : Npc {
		private sbyte _mood = 3;

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
					Become<RunState>();
					GetComponent<DialogComponent>().StartAndClose($"shopkeeper_{Random.Int(3, 5)}", 1);

					AddTag(Tags.MustBeKilled);
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
			
			AddComponent(new AnimationComponent("shopkeeper"));

			var h = GetComponent<HealthComponent>();

			h.InitMaxHealth = 20;
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

		public override void PostInit() {
			base.PostInit();

			if (GameSave.IsTrue("sk_enraged")) {
				Enrage();
			}
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
					Recalc();
					
					if (Mood > -1) {
						GetComponent<AudioEmitterComponent>().EmitRandomized("hi");
						GetComponent<DialogComponent>().StartAndClose($"shopkeeper_{Random.Int(6, 9)}", 3);
					}
				}
			} else if (e is ItemBoughtEvent ibe) {
				if (ibe.Stand.GetComponent<RoomComponent>().Room == GetComponent<RoomComponent>().Room) {
					Mood++;
					GetComponent<DialogComponent>().StartAndClose($"shopkeeper_{Random.Int(9, 12)}", 3);
				}
			}

			return base.HandleEvent(e);
		}

		protected override bool HandleDeath(DiedEvent d) {
			foreach (var s in GetComponent<RoomComponent>().Room.Tagged[Tags.Item]) {
				if (s is ShopStand ss) {
					ss.Free = true;
				}
			}
			
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

		#region Shopkeeper States
		/*
		 * Peacefull
		 */
		private class IdleState : SmartState<ShopKeeper> {
			private float delay;
			
			public override void Update(float dt) {
				base.Update(dt);
				delay -= dt;

				if (delay <= 0) {
					delay = Random.Float(2, 6f);
					Self.GetComponent<AudioEmitterComponent>().EmitRandomized($"villager{Random.Int(1, 5)}");
				}
			}
		}
		/*
		 * Raging
		 */
		private class RunState : SmartState<ShopKeeper> {
			public override void Update(float dt) {
				base.Update(dt);
			}
		}
		#endregion
	}
}