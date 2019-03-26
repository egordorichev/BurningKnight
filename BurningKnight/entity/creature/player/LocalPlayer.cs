using BurningKnight.entity.component;
using BurningKnight.entity.events;
using Lens;
using Lens.entity;
using Lens.util;
using Lens.util.tween;

namespace BurningKnight.entity.creature.player {
	public class LocalPlayer : Player {
		public static LocalPlayer Locate(Area area) {
			foreach (var player in area.Tags[Tags.Player]) {
				if (player is LocalPlayer localPlayer) {
					return localPlayer;
				}
			}

			return null;
		}
		
		public override void AddComponents() {
			base.AddComponents();
			AddComponent(new PlayerInputComponent());
		}

		public override bool HandleEvent(Event e) {
			if (e is DiedEvent ev) {
				if (!GetComponent<HealthComponent>().Dead) {
					Tween.To(0.3f, Engine.Instance.Speed, x => Engine.Instance.Speed = x, 0.1f).OnEnd = () => {
						Tween.To(1f, Engine.Instance.Speed, x => Engine.Instance.Speed = x, 0.2f);
						Kill(ev.From);
					};
				}
			} else if (e is HealthModifiedEvent hp && hp.Amount < 0) {
				Engine.Instance.Split = 1f;
			}
			
			return base.HandleEvent(e);
		}
	}
}