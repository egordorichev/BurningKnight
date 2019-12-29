using BurningKnight.assets.items;
using BurningKnight.entity.component;
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
		public static string AccessoryTrader = "accessory_trader";
		public static string ActiveTrader = "active_trader";
		public static string WeaponTrader = "weapon_trader";
		public static string HatTrader = "hat_trader";
		public static string Snek = "snek";
		public static string Boxy = "boxy";
		public static string Vampire = "vampire";
		public static string Roger = "roger";
		public static string TrashGoblin = "trash_goblin";
	
		private float delay;
		internal bool Hidden;
		private bool saved;
		private bool hided;

		protected bool Flips = true;

		public override void Init() {
			base.Init();
			
			Subscribe<RoomChangedEvent>();
			Subscribe<ItemBoughtEvent>();

			AlwaysActive = true;
			Hidden = Run.Depth == 0 && GlobalSave.IsFalse(GetId());
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

			if (saved && !OnScreen) {
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

		public override bool HandleEvent(Event e) {
			if (Hidden) {
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
					}
				}
			} else if (e is ItemBoughtEvent ibe) {
				if (OwnsStand(ibe.Stand) && ibe.Stand.GetComponent<RoomComponent>().Room == GetComponent<RoomComponent>().Room) {
					GetComponent<DialogComponent>().StartAndClose(GetDealDialog(), 3);
				}
			}
			
			return base.HandleEvent(e);
		}
		
		protected virtual string GetDealDialog() {
			return $"shopkeeper_{Rnd.Int(9, 12)}";
		}

		public void Save() {
			if (Run.Depth > 0 && !saved) {
				saved = true;
				GetComponent<DialogComponent>().StartAndClose("npc_1", 6);
				
				GlobalSave.Put(GetId(), true);
				GlobalSave.Put("saved_npc", true);

				HandleEvent(new SavedEvent {
					Npc = this
				});
			}
		}
		
		protected virtual string GetDialog() {
			return $"shopkeeper_{Rnd.Int(6, 9)}";
		}

		public virtual string GetId() {
			return null;
		}

		public class SavedEvent : Event {
			public ShopNpc Npc;
		}
	}
}