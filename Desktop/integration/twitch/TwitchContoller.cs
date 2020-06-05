using System;
using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.twitch.happening;
using BurningKnight.state;
using Lens;
using Lens.assets;
using Lens.graphics;
using Lens.util;
using Lens.util.math;
using Microsoft.Xna.Framework;
using TwitchLib.Client.Models;

namespace Desktop.integration.twitch {
	public class TwitchContoller {
		private const int TotalTime = 45;
		
		private List<HappeningOption> options = new List<HappeningOption>();
		private List<string> votersCache = new List<string>();
		private float timeLeft = 1f;
		private Player player;
		private string question;
		private string totalVotes;
		
		public void Init() {
			GenerateOptions();
		}

		private void GenerateOptions() {
			question = Locale.Get("twitch_next");
			totalVotes = Locale.Get("total_votes");
			
			votersCache.Clear();
			options.Clear();
			timeLeft = 1f;

			var pool = new List<string>();

			foreach (var h in HappeningRegistry.Defined) {
				pool.Add(h.Key);
			}

			for (var i = 0; i < 3; i++) {
				var j = Rnd.Int(pool.Count);
				var happening = pool[j];
				pool.RemoveAt(j);
				
				AddOption(happening, 3 - i);
			}
		}

		private void AddOption(string id, int i) {
			var happening = HappeningRegistry.Get(id);

			if (happening == null) {
				return;
			}
			
			var option = new HappeningOption(id, i);
			options.Add(option);

			float x = Display.UiWidth;

			foreach (var o in options) {
				x -= o.LabelWidth + 8;
			}

			option.Position = new Vector2(x, 18);
		}

		public void Update(float dt) {
			var state = Engine.Instance.State;

			if (!(state is InGameState) || state.Paused) {
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
			
			timeLeft -= dt / TotalTime;

			if (timeLeft <= 0) {
				ExecuteOrder66();
				GenerateOptions();
			}
		}

		public void Render() {
			if (!(Engine.Instance.State is InGameState)) {
				return;
			}

			var text = $"{question} ({(int) Math.Ceiling(timeLeft * TotalTime)}s)";

			Graphics.Color.A = 200;
			Graphics.Print(text, Font.Small, new Vector2(Display.UiWidth - Font.Small.MeasureString(text).Width - 8, 8));
			Graphics.Color.A = 255;
			
			foreach (var option in options) {
				option.Render();
			}
			
			Graphics.Color.A = 170;
			var t = $"{votersCache.Count} {totalVotes}";
			Graphics.Print(t, Font.Small, new Vector2(Display.UiWidth - Font.Small.MeasureString(t).Width - 8, 28));
			Graphics.Color.A = 255;
		}
		
		private void BalanceVotes() {
			float total = votersCache.Count;

			foreach (var o in options) {
				o.Percent = (int) Math.Floor(o.Votes / total * 100);
			}
		}

		public bool HandleMessage(ChatMessage chatMessage) {
			var message = chatMessage.Message;

			if (!votersCache.Contains(chatMessage.Username)) {
				var m = message.ToLower();

				foreach (var o in options) {
					if (o.Name == m) {
						votersCache.Add(chatMessage.Username);
						o.Votes++;

						BalanceVotes();
						Log.Info($"Voted for {m}");

						return true;
					}
				}
			}

			if (message.StartsWith("#")) {
				message = message.Substring(1, message.Length - 1);
			}
			
			if (int.TryParse(message, out var number)) {
				if (number < 1 || number > options.Count || (chatMessage.Username != TwitchIntegration.DevAccount && votersCache.Contains(chatMessage.Username))) {
					return false;
				}
				
				votersCache.Add(chatMessage.Username);
				options[(options.Count - number)].Votes++;

				BalanceVotes();
				
				Log.Info($"Voted for #{number}");
				return true;
			}

			return false;
		}

		private void ExecuteOrder66() {
			var options = new List<HappeningOption>();
			
			HappeningOption opt = null;
			var mostVotes = 0;

			foreach (var h in this.options) {
				if (opt == null || h.Votes > mostVotes) {
					opt = h;
					mostVotes = h.Votes;
					options.Clear();
					options.Add(h);
				} else if (h.Votes == mostVotes) {
					options.Add(h);
				}
			}

			if (options.Count > 1) {
				Log.Info($"Picking from the same vote candidates of size {options.Count}");
				opt = options[Rnd.Int(options.Count)];
			}

			if (opt == null) {
				Log.Error("The jedi have escaped");
				return;
			}
			
			Log.Debug($"Option #{opt.Num} has won!");
			
			try {
				opt.Happening.Happen(player);
			} catch (Exception e) {
				Log.Error(e);
			}

			TextParticle.Add(player, opt.Name);
		}
	}
}