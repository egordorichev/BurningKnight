using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.level.entities;
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

		private sbyte mood {
			get => _mood;

			set {
				if (_mood == value) {
					return;
				}
				
				var r = raging;
				
				if (_mood < 0 && value > 0) {
					return;
				}
				
				_mood = value;

				if (_mood < 3 && _mood > -1) {
					GetComponent<DialogComponent>().StartAndClose($"shopkeeper_{_mood}", 1);
				}

				if (!r && raging) {
					Become<RunState>();
					GetComponent<DialogComponent>().StartAndClose($"shopkeeper_{Random.Int(3, 5)}", 1);

					AddTag(Tags.MustBeKilled);
				}
			}
		}
		
		private bool raging => mood < 0;
		
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
			
			Become<IdleState>();
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			mood = stream.ReadSbyte();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteSbyte(mood);
		}

		public void Enrage() {
			if (raging) {
				return;
			}

			mood = -1;
		}

		public override bool HandleEvent(Event e) {
			if (e is BombPlacedEvent bpe) {
				if (bpe.Bomb.GetComponent<RoomComponent>().Room == GetComponent<RoomComponent>().Room) {
					Enrage();
				}

				return false;
			} else if (e is GramophoneBrokenEvent gbe) {
				if (gbe.Gramophone.GetComponent<RoomComponent>().Room == GetComponent<RoomComponent>().Room) {
					mood--;
				}

				return false;
			} else if (e is RerollMachine.BrokenEvent rme) {
				if (rme.Machine.GetComponent<RoomComponent>().Room == GetComponent<RoomComponent>().Room) {
					mood--;
				}

				return false;
			} else if (e is VendingMachine.BrokenEvent vme) {
				if (vme.Machine.GetComponent<RoomComponent>().Room == GetComponent<RoomComponent>().Room) {
					mood--;
				}

				return false;
			} else if (e is HealthModifiedEvent hme) {
				if (hme.Amount < 0 && mood > -2) {
					mood--;
					hme.Amount = -1;
				}
			} else if (e is RoomChangedEvent rce) {
				if (mood > -1 && rce.Who is Player && rce.New == GetComponent<RoomComponent>().Room) {
					GetComponent<DialogComponent>().StartAndClose($"shopkeeper_{Random.Int(6, 9)}", 3);
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

		public override bool IsFriendly() {
			return !raging;
		}

		#region Shopkeeper States
		/*
		 * Peacefull
		 */
		private class IdleState : SmartState<ShopKeeper> {
			public override void Update(float dt) {
				base.Update(dt);
				
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