using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using Lens.entity;
using Lens.util.math;

namespace BurningKnight.entity.creature.npc {
	public class ShopNpc : Npc {
		public static string AccessoryTrader = "accessory_trader";
		public static string ActiveTrader = "accessory_trader";
		public static string WeaponTrader = "accessory_trader";
		public static string HatTrader = "accessory_trader";
	
		private float delay;
		private bool hidden;
		private bool saved;

		public override void Init() {
			base.Init();
			Subscribe<RoomChangedEvent>();

			AlwaysActive = true;
			hidden = Run.Depth == 0 && GlobalSave.IsFalse(GetId());
		}

		public override void Update(float dt) {
			if (hidden) {
				return;
			}

			if (saved && !OnScreen) {
				Done = true;
				hidden = true;
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
			if (hidden) {
				return;
			}
			
			base.Render();
		}

		protected override void RenderShadow() {
			if (hidden) {
				return;
			}
		
			base.RenderShadow();
		}

		public override bool HandleEvent(Event e) {
			if (hidden) {
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
			}
			
			return base.HandleEvent(e);
		}

		public void Save() {
			if (Run.Depth > 0 && !saved) {
				saved = true;
				GetComponent<DialogComponent>().StartAndClose("npc_1", 6);
				GlobalSave.Put(GetId(), true);
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