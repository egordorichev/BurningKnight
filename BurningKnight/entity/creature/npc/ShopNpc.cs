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

		public override void Init() {
			base.Init();
			Subscribe<RoomChangedEvent>();

			hidden = Run.Depth == 0 && GlobalSave.IsFalse(GetId());
		}

		public override void Update(float dt) {
			if (hidden) {
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
					GetComponent<DialogComponent>().StartAndClose(GetDialog(), 3);
				}
			}
			
			return base.HandleEvent(e);
		}

		protected virtual string GetDialog() {
			return $"shopkeeper_{Random.Int(6, 9)}";
		}

		public virtual string GetId() {
			return null;
		}
	}
}