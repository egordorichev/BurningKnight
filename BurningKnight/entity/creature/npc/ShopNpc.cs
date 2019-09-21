using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
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

		public override void Init() {
			base.Init();
			Subscribe<RoomChangedEvent>();
		}

		public override void Update(float dt) {
			base.Update(dt);
			delay -= dt;

			if (delay <= 0) {
				delay = Random.Float(1, 4);
				GraphicsComponent.Flipped = !GraphicsComponent.Flipped;
			}
		}

		public override bool HandleEvent(Event e) {
			if (e is RoomChangedEvent rce) {
				if (rce.Who is Player && rce.New == GetComponent<RoomComponent>().Room) {
					GetComponent<AudioEmitterComponent>().EmitRandomized("hi");
					GetComponent<DialogComponent>().StartAndClose(GetDialog(), 3);
				}
			}
			
			return base.HandleEvent(e);
		}

		protected virtual string GetDialog() {
			return $"shopkeeper_{Random.Int(6, 9)}";
		}
	}
}