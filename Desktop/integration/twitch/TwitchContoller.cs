using System;
using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.entity.creature.player;
using BurningKnight.state;
using BurningKnight.ui;
using Desktop.integration.twitch.happening;
using Lens;
using Lens.graphics;
using Lens.util;
using Lens.util.math;
using Microsoft.Xna.Framework;
using TwitchLib.Client.Models;

namespace Desktop.integration.twitch {
	public class TwitchContoller {
		private List<HappeningOption> options = new List<HappeningOption>();
		private List<string> votersCache = new List<string>();
		private float timeLeft = 1f;
		private Player player;
		
		public void Init() {
			GenerateOptions();
		}

		private void GenerateOptions() {
			votersCache.Clear();
			options.Clear();
			timeLeft = 1f;
			
			AddOption("bk:hurt");
			AddOption("bk:big_hurt");
			AddOption("bk:omega_hurt");
		}

		private void AddOption(string id) {
			var happening = HappeningRegistry.Get(id);

			if (happening == null) {
				return;
			}
			
			var option = new HappeningOption(id);
			options.Add(option);

			float x = Display.UiWidth;

			foreach (var o in options) {
				x -= o.LabelWidth + 8;
			}

			option.Position = new Vector2(x, 8);
		}

		public void Update(float dt) {
			var state = Engine.Instance.State;

			if (!(state is InGameState)) {
				return;
			}

			if (player == null) {
				player = LocalPlayer.Locate(state.Area);
				
				if (player == null) {
					return;
				}
			}

			if (player.Done) {
				player = null;
				return;
			}
			
			timeLeft -= dt / 60f;

			if (timeLeft <= 0) {
				ExecuteOrder66();
				GenerateOptions();
			}
		}

		public void Render() {
			if (!(Engine.Instance.State is InGameState)) {
				return;
			}
			
			foreach (var option in options) {
				option.Render();
			}

			Graphics.Print($"{timeLeft * 60}", Font.Medium, new Vector2(8));
		}

		public bool HandleMessage(ChatMessage chatMessage) {
			var message = chatMessage.Message;

			if (message.StartsWith("#")) {
				message = message.Substring(1, message.Length - 1);
			}
			
			if (int.TryParse(message, out var number)) {
				if (number < 1 || number > options.Count || votersCache.Contains(chatMessage.Username)) {
					return false;
				}
				
				votersCache.Add(chatMessage.Username);
				options[number - 1].Votes++;

				Log.Info($"Voted for #{number}");
				return true;
			}

			return false;
		}

		private void ExecuteOrder66() {
			Happening topHappening = null;
			var mostVotes = 0;
			var optionId = 0;
			var topOptionId = 0;

			foreach (var h in options) {
				if (h.Votes >= mostVotes && (topHappening == null || Rnd.Chance())) {
					topHappening = h.Happening;
					topOptionId = optionId;
					mostVotes = h.Votes;
				}

				optionId++;
			}

			if (topHappening == null) {
				Log.Error("The jedi have escaped");
				return;
			}
			
			Log.Debug($"Option #{topOptionId + 1} has won!");
			
			try {
				topHappening.Happen(player);
			} catch (Exception e) {
				Log.Error(e);
			}
		}
	}
}