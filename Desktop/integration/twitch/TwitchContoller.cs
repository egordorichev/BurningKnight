using System;
using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.twitch.happening;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using Lens;
using Lens.assets;
using Lens.graphics;
using Lens.util;
using Lens.util.math;
using Lens.util.timer;
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
		private HappeningOption happeningRn;
		private float voteDelay;
		
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

			if (!(state is InGameState ig) || state.Paused) {
				return;
			}

			if (ig.Died) {
				happeningRn = null;

				if (timeLeft < 1f) {
					timeLeft = 1f;
					GenerateOptions();
				}
				
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
			
			if (happeningRn != null) {
				voteDelay -= dt;
				happeningRn.Happening.Update(dt);
				
				if (voteDelay <= 0) {
					try {
						happeningRn.Happening.End(last);
						last = null;
					} catch (Exception e) {
						Log.Error(e);
					}
					
					voteDelay = 0;
					happeningRn = null;
					GenerateOptions();
				}
			} else {
				timeLeft -= dt / TotalTime;

				if (timeLeft <= 0) {
					ExecuteOrder66();
				}
			}
		}

		public void Render() {
			if (!(Engine.Instance.State is InGameState ig) || ig.Died) {
				return;
			}

			if (happeningRn != null || busy) {
				if (happeningRn != null) {
					var tt = $"{lastName} ({(int) Math.Ceiling(voteDelay)}s)";
					Graphics.Print(tt, Font.Small, new Vector2(Display.UiWidth - Font.Small.MeasureString(tt).Width - 8, 8));
				}
				
				return;
			}

			var text = $"{question}{(busy ? "" : $" ({(int) Math.Ceiling(timeLeft * TotalTime)}s)")}";

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
			if (happeningRn != null || busy) {
				return false;
			}
			
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

		private Player last;
		private bool busy;
		private string lastName;
		
		private void ExecuteOrder66() {
			busy = true;
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

			var name = Locale.Get($"happening_{opt.Id}");
			lastName = name;
			Log.Debug($"Option #{opt.Num} ({name}) has won!");
			
			player.GetComponent<DialogComponent>().StartAndClose($"[cl purple]{name}[cl]", 3);
			
			last = player;
			happeningRn = opt;
			voteDelay = opt.Happening.GetVoteDelay();

			Timer.Add(() => {
				try {
					opt.Happening.Happen(player);
				} catch (Exception e) {
					Log.Error(e);
				}

				busy = false;
			}, 4);
		}
	}
}