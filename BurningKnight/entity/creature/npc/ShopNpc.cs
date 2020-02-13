using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.npc.dungeon;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item.stand;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using Lens.entity;
using Lens.util.math;

namespace BurningKnight.entity.creature.npc {
	public class ShopNpc : Npc {
		public const string AccessoryTrader = "accessory_trader";
		public const string ActiveTrader = "active_trader";
		public const string WeaponTrader = "weapon_trader";
		public const string HatTrader = "hat_trader";
		
		public const string Snek = "snek";
		public const string Boxy = "boxy";
		public const string Roger = "roger";
		public const string Gobetta = "gobetta";
		public const string Vampire = "vampire";
		public const string TrashGoblin = "trash_goblin";
		public const string Duck = "duck";
		public const string Nurse = "nurse";
		public const string Elon = "elon";
	
		private float delay;
		internal bool Hidden;
		private bool saved;
		private bool hided;
		
		protected bool Remove;
		protected bool Flips = true;

		public override void Init() {
			base.Init();
			
			Subscribe<RoomChangedEvent>();
			Subscribe<ItemBoughtEvent>();

			var id = GetId();
			
			saved = id == TrashGoblin || GlobalSave.IsTrue(id);
			AlwaysActive = true;
			Hidden = Run.Depth == 0 && !saved;
		}

		public override void AddComponents() {
			base.AddComponents();
			
			if (Run.Depth == 0) {
				AddComponent(new CloseDialogComponent(GetDialog()) {
					DecideVariant = e => GetDialog(),
					Radius = 72 * 72,
					RadiusMax = 96 * 96
				});
			}
		}
		
		public override void Update(float dt) {
			if (Hidden) {
				if (!hided) {
					hided = true;

					foreach (var item in Area.Tagged[Tags.Item]) {
						if (item is ItemStand stand && OwnsStand(stand)) {
							stand.Done = true;
						}
					}
				}
				
				return;
			}

			if (saved && Remove && !OnScreen) {
				Done = true;
				Hidden = true;

				var stand = new ItemStand();
				Area.Add(stand);
				stand.Center = Center;
				stand.SetItem(Items.CreateAndAdd("bk:emerald", Area), null);
				
				return;
			}

			base.Update(dt);

			if (!Flips) {
				return;
			}
			
			delay -= dt;

			if (delay <= 0) {
				delay = Rnd.Float(1, 10);
				GraphicsComponent.Flipped = !GraphicsComponent.Flipped;
			}
		}

		public override void Render() {
			if (Hidden) {
				return;
			}
			
			base.Render();
		}

		protected override void RenderShadow() {
			if (Hidden) {
				return;
			}
		
			base.RenderShadow();
		}

		protected virtual bool OwnsStand(ItemStand stand) {
			return false;
		}

		protected virtual string GetHiDialog() {
			return null;
		}

		public override bool HandleEvent(Event e) {
			if (Hidden || Done) {
				return false;
			}
			
			if (e is RoomChangedEvent rce) {
				if (rce.Who is Player && rce.New == GetComponent<RoomComponent>().Room) {
					if (Run.Depth > 0 && !saved) {
						GetComponent<AudioEmitterComponent>().EmitRandomized("hi");

						if ((rce.Who.TryGetComponent<ActiveWeaponComponent>(out var a) && a.Item != null && a.Item.Id == "bk:cage_key") ||
						    (rce.Who.TryGetComponent<WeaponComponent>(out var w) && w.Item != null && w.Item.Id == "bk:cage_key")) {

							GetComponent<DialogComponent>().StartAndClose("npc_2", 3);
						} else {
							GetComponent<DialogComponent>().StartAndClose("npc_0", 3);
						}
					} else {
						var s = GetHiDialog();
						
						if (s != null) {
							GetComponent<DialogComponent>().StartAndClose(s, 3);
						}
					}
				}
			} else if (e is ItemBoughtEvent ibe) {
				if (OwnsStand(ibe.Stand) && ibe.Stand.GetComponent<RoomComponent>().Room == GetComponent<RoomComponent>().Room) {
					GetComponent<DialogComponent>().StartAndClose(GetDealDialog(), 3);
					OnItemBought(ibe);
				}
			}
			
			return base.HandleEvent(e);
		}

		protected virtual void OnItemBought(ItemBoughtEvent ibe) {
			
		}

		protected virtual string GetDealDialog() {
			return $"shopkeeper_{Rnd.Int(9, 12)}";
		}

		public void Save() {
			if (Run.Depth > 0 && !saved) {
				saved = true;
				Remove = true;
				GetComponent<DialogComponent>().StartAndClose("npc_1", 6);
				
				GlobalSave.Put(GetId(), true);
				GlobalSave.Put("saved_npc", true);
				
				RemoveComponent<InteractableComponent>();

				HandleEvent(new SavedEvent {
					Npc = this
				});
			}
		}
		
		protected virtual string GetDialog() {
			return $"shopkeeper_{Rnd.Int(6, 9)}";
		}

		public virtual string GetFailDialog() {
			return $"shopkeeper_{Rnd.Int(15, 18)}";
		}
		
		public virtual string GetId() {
			return null;
		}

		public class SavedEvent : Event {
			public ShopNpc Npc;
		}
		
		public static ShopNpc FromId(string id) {
			switch (id) {
				case HatTrader: return new HatTrader();
				case WeaponTrader: return new WeaponTrader();
				case ActiveTrader: return new ActiveTrader();
				case AccessoryTrader: return new AccessoryTrader();
				case Snek: return new Snek();
				case Boxy: return new Boxy();
				case Vampire: return new Vampire();
				case Roger: return new Roger();
				case TrashGoblin: return new TrashGoblin();
				case Duck: return new DungeonDuck();
				case Nurse: return new Nurse();
				case Elon: return new DungeonElon();
				case Gobetta: return new Gobetta();
			}

			return new Boxy();
		}
	}
}