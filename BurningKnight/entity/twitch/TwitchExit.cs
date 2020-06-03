using System.Threading;
using BurningKnight.entity.creature.player;
using BurningKnight.level.entities;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using Lens.assets;
using Lens.entity;

namespace BurningKnight.entity.twitch {
	public class TwitchExit : Exit {
		private LoginSign sign;
		
		protected override string GetSlice() {
			return "twitch_exit";
		}

		private void TurnOn(Entity entity) {
			if (!TwitchBridge.On) {
				entity.RemoveComponent<PlayerInputComponent>();
				var id = TwitchBridge.TwitchUsername;
				
				sign.GetComponent<DialogComponent>().Start("logging_in");
				
				new Thread(() => {
					TwitchBridge.TurnOn(GlobalSave.GetString("twitch_username"), (ok) => {
						if (ok) {
							base.Interact(entity);
						} else {
							sign.GetComponent<DialogComponent>().StartAndClose($"{Locale.Get("failed_to_login")}\n[cl purple]{id}[cl]", 3);
							entity.AddComponent(new PlayerInputComponent());
						}
					});
				}).Start();
			} else {
				base.Interact(entity);
			}
		}

		protected override bool Interact(Entity entity) {
			if (TwitchBridge.On && TwitchBridge.LastUsername != GlobalSave.GetString("twitch_username")) {
				TwitchBridge.TurnOff(() => {
					TurnOn(entity);
				});
			} else {
				TurnOn(entity);
			}
			
			return true;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (sign != null) {
				return;
			}

			sign = (LoginSign) Area.FindClosest(Center, Tags.LevelSave, e => e is LoginSign);
		}

		protected override void Descend() {
			Run.StartNew(1, RunType.Twitch);
		}

		protected override string GetFxText() {
			return Locale.Get("twitch_mode");
		}

		protected override bool CanInteract(Entity e) {
			return GlobalSave.GetString("twitch_username") != null;
		}
	}
}