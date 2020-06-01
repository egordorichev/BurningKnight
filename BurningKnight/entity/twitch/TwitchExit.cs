using BurningKnight.level.entities;
using BurningKnight.save;
using BurningKnight.state;
using Lens.assets;
using Lens.entity;

namespace BurningKnight.entity.twitch {
	public class TwitchExit : Exit {
		protected override string GetSlice() {
			return "twitch_exit";
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