using BurningKnight.entity.component;
using BurningKnight.level.entities;
using BurningKnight.save;
using Lens.assets;

namespace BurningKnight.entity.twitch {
	public class LoginSign : Sign {
		public LoginSign() {
			Region = "sign_twitch";
		}

		public override void AddComponents() {
			base.AddComponents();

			GetComponent<CloseDialogComponent>().DecideVariant = (e) => {
				var id = TwitchBridge.TwitchUsername;

				if (id == null) {
					return Locale.Get("not_logged_in");
				}
				
				return $"{Locale.Get("logged_in_as")}\n[cl purple]@{id}[cl]";
			};
		}
	}
}