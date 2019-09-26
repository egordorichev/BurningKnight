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
	
		private float delay;
		internal bool Hidden;
		private bool saved;

		public override void Init() {
			base.Init();
			
			Subscribe<RoomChangedEvent>();
			Subscribe<ItemBoughtEvent>();

			AlwaysActive = true;
			Hidden = Run.Depth == 0 && GlobalSave.IsFalse(GetId());
		}

		public override void Update(float dt) {
			if (Hidden) {
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
			delay -= dt;

			if (delay <= 0) {
				delay = Random.Float(1, 4);
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

		public override bool HandleEvent(Event e) {
			if (Hidden) {
				return false;
			}
			
			if (e is RoomChangedEvent rce) {
				if (rce.Who is Player && rce.New == GetComponent<RoomComponent>().Room) {
					GetComponent<AudioEmitterComponent>().EmitRandomized("hi");

					if (Run.Depth > 0) {
						if ((rce.Who.TryGetComponent<ActiveWeaponComponent>(out var a) && a.Item != null && a.Item.Id == "bk:cage_key") ||
						    (rce.Who.TryGetComponent<WeaponComponent>(out var w) && w.Item != null && w.Item.Id == "bk:cage_key")) {

							GetComponent<DialogComponent>().StartAndClose( "npc_2", 3);
						} else {
							GetComponent<DialogComponent>().StartAndClose( "npc_0", 3);
						}
					} else {
						GetComponent<DialogComponent>().StartAndClose( GetDialog(), 3);
					}
				}
			} else if (e is ItemBoughtEvent ibe) {
				if (ibe.Stand.GetComponent<RoomComponent>().Room == GetComponent<RoomComponent>().Room) {
					GetComponent<DialogComponent>().StartAndClose($"shopkeeper_{Random.Int(9, 12)}", 3);
				}
			}
			
			return base.HandleEvent(e);
		}

		public void Save() {
			if (Run.Depth > 0 && !saved) {
				saved = true;
				GetComponent<DialogComponent>().StartAndClose("npc_1", 6);
				
				GlobalSave.Put(GetId(), true);
				GlobalSave.Put("saved_npc", true);
			}
		}
		
		protected virtual string GetDialog() {
			return $"shopkeeper_{Random.Int(6, 9)}";
		}

		public virtual string GetId() {
			return null;
		}
	}
}