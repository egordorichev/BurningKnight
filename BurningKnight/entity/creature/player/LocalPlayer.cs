using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.state;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.util;
using Lens.util.camera;
using Lens.util.tween;

namespace BurningKnight.entity.creature.player {
	public class LocalPlayer : Player {
		public static LocalPlayer Locate(Area area) {
			foreach (var player in area.Tagged[Tags.Player]) {
				if (player is LocalPlayer localPlayer) {
					return localPlayer;
				}
			}

			return null;
		}

		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new GamepadComponent());
			AddComponent(new PlayerInputComponent());
			
			AddTag(Tags.LocalPlayer);
		}

		private bool died;

		public override bool HandleEvent(Event e) {
			if (e is DiedEvent ev && ev.Who == this) {
				if (!GetComponent<HealthComponent>().Dead && !died) {
					died = true;
					Done = false;

					Audio.FadeOut();
					
					((InGameState) Engine.Instance.State).HandleDeath();

					Camera.Instance.Targets.Clear();
					Camera.Instance.Follow(this, 1);
					
					Tween.To(0.3f, Engine.Instance.Speed, x => Engine.Instance.Speed = x, 0.5f).OnEnd = () => {
						var t = Tween.To(1, Engine.Instance.Speed, x => Engine.Instance.Speed = x, 0.5f);

						t.Delay = 0.8f;
						t.OnEnd = ((InGameState) Engine.Instance.State).AnimateDeathScreen;

						HandleEvent(e);
						AnimateDeath(ev);
						Done = true;
					};

					return true;
				}
			} else if (e is PostHealthModifiedEvent hp && hp.Amount < 0) {
				Engine.Instance.Split = 1f;
				Engine.Instance.Flash = 1f;
				Engine.Instance.Freeze = 1f;
				
				Camera.Instance.Shake(4);

				if (Camera.Instance != null) {
					Camera.Instance.TextureZoom -= 0.2f;
					Tween.To(1f, Camera.Instance.TextureZoom, x => Camera.Instance.TextureZoom = x, 0.3f);					
				}	
			}
			
			return base.HandleEvent(e);
		}
	}
}