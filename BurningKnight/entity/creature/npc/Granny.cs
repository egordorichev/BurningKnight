using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.ui.dialog;
using Lens.entity;
using Lens.util.math;

namespace BurningKnight.entity.creature.npc {
	public class Granny : Npc {
		public override void AddComponents() {
			base.AddComponents();
			
			AlwaysActive = true;
			Width = 15;
			
			AddComponent(new AnimationComponent("grandma"));

			var b = new RectBodyComponent(2, 6, Width - 4, Height - 6);
			AddComponent(b);
			b.KnockbackModifier = 0;
		}

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
					
					Items.Unlock("bk:grandma_head");
				}
			}
			
			return base.HandleEvent(e);
		}

		private string GetDialog() {
			return $"granny_{Random.Int(3)}";
		}
	}
}