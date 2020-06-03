using System;
using BurningKnight.save;

namespace BurningKnight.entity.twitch {
	public static class TwitchBridge {
		public static bool On = false;
		public static Action<string, Action<bool>> TurnOn;
		public static Action<Action> TurnOff;
		public static string LastUsername;
		
		public static Action OnHubEnter;
		public static Action OnNewRun;

		public static string TwitchUsername => GlobalSave.GetString("twitch_username").Replace("_", "\\_");
	}
}