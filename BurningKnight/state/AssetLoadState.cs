using System;
using System.Threading;
using BurningKnight.assets;
using BurningKnight.assets.achievements;
using BurningKnight.assets.items;
using BurningKnight.assets.lighting;
using BurningKnight.assets.loot;
using BurningKnight.assets.mod;
using BurningKnight.assets.prefabs;
using BurningKnight.level.tile;
using BurningKnight.physics;
using BurningKnight.save;
using BurningKnight.ui.str;
using BurningKnight.util;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.game;
using Lens.graphics;
using Lens.util;
using Lens.util.math;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.state {
	public class AssetLoadState : GameState {
		private PhotoCard logoCard;

		private TextureRegion pixel;
		// private PhotoCard[] cards = new PhotoCard[3];
		private bool ready;
		private int progress;
		private Area gameArea;
		private float t;
		private bool added;
		private bool removed;
		private bool checkFullscreen;
		private float lastV;
		private UiString tipLabel;
		private bool exitTweenDone;

		public override void Init() {
			base.Init();

			pixel = new TextureRegion(Textures.FastLoad("Content/pixel.png"));
			
			Ui.Add(logoCard = new PhotoCard {
				Region = new TextureRegion(Textures.FastLoad("Content/logo.png")),
				Url = "https://twitter.com/rexcellentgames",
				Name = "Rexcellent Games",
				Position = new Vector2(240, Display.UiHeight + 60),
				Target = new Vector2(240, 115),
				Scale = 1
			});

			Ui.Add(tipLabel = new UiString(Font.Medium));
			
			GenerateNewTip();
			
			logoCard.Start.Y = -90;
			progress = 0;

			var thread = new Thread(() => {
				Log.Info("Starting asset loading thread");
				
				SaveManager.Load(gameArea, SaveType.Global);
				progress++;
				checkFullscreen = true;
				
				Audio.ThreadLoad("Void");
				progress++;
				Assets.Load(ref progress);
				progress++;
				Audio.ThreadLoad("Menu", false);
				progress++;
				
				Dialogs.Load();
				progress++;
				CommonAse.Load();
				progress++;

				try {
					ImGuiHelper.BindTextures();
				} catch (Exception e) {
					Log.Error(e);
				}

				progress++;
				Shaders.Load();
				progress++;
				Prefabs.Load();
				progress++;
				Items.Load();
				progress++;
				LootTables.Load();
				progress++;
				Mods.Load();
				progress++; // Should be 13 here
				
				Log.Info("Done loading assets! Loading level now.");
			
				Lights.Init();
				Physics.Init();
				gameArea = new Area();

				Run.Level = null;
				Tilesets.Load();
				progress++;
				
				Achievements.Load();
				progress++;
				
				SaveManager.Load(gameArea, SaveType.Game);
				progress++;
				
				Rnd.Seed = $"{Run.Seed}_{Run.Depth}"; 
				
				SaveManager.Load(gameArea, SaveType.Level);
				progress++;

				if (Run.Depth > 0) {
					SaveManager.Load(gameArea, SaveType.Player);
				} else {
					SaveManager.Generate(gameArea, SaveType.Player);
				}

				progress++; // Should be 18 here
				Log.Info("Done loading level! Going to menu.");
				
				ready = true;
			});

			thread.Priority = ThreadPriority.BelowNormal;
			thread.Start();
		}

		public override void Update(float dt) {
			base.Update(dt);

			t += dt;

			if (t % 4 < 0.01f) {
				GenerateNewTip();
			}
			
			if (checkFullscreen) {
				checkFullscreen = false;
				
				if (Settings.Fullscreen) {
					Engine.Instance.SetFullscreen();
				} else {
					Engine.Instance.SetWindowed(Display.Width * 3, Display.Height * 3);
				}
			}

			if (!tipLabel.Visible && logoCard.DoneLerping && !added) {
				tipLabel.Visible = true;
			}

			if (added && tipLabel.Visible && exitTweenDone) {
				tipLabel.Visible = false;
			}
			
			if (!added && ready && lastV >= 0.98f) {
				added = true;
				logoCard.GoAway = true;

				/*Timer.Add(() => {
					Ui.Add(cards[0] = new PhotoCard {
						Region = new TextureRegion(Textures.FastLoad("Content/egor.png")),
						Name = "Egor Dorichev",
						Tasks = "Code, Art, Sfx & Design",
						Url = "https://twitter.com/egordorichev",
						Position = new Vector2(-350, 135),
						Target = new Vector2(80, 135),
						Angle = 1f
					});
			
					Ui.Add(cards[1] = new PhotoCard {
						Region = new TextureRegion(Textures.FastLoad("Content/jose.png")),
						Name = "Jose Ramon",
						Tasks = "Music & Sfx",
						Url = "https://twitter.com/BibikiGL",
						Position = new Vector2(240, 360),
						Target = new Vector2(240, 135)
					});
			
					cards[1].Start.Y = -160;
				
					Ui.Add(cards[2] = new PhotoCard {
						Region = new TextureRegion(Textures.FastLoad("Content/mate.png")),
						Name = "Mate Cziner",
						Tasks = "Art & Design",
						Url = "https://twitter.com/MateCziner",
						Position = new Vector2(Display.UiWidth + 350, 135),
						Target = new Vector2(400, 135),
						Angle = -1f
					});
				}, 0.3f);*/
			}

			if (ready && added && logoCard.Done && !removed) {
				removed = true;
				Engine.Instance.StateRenderer.UiEffect = Shaders.Ui;
				Engine.Instance.SetState(new InGameState(gameArea, true));
			}
		}

		public override void RenderUi() {
			base.RenderUi();
			
			var v = Math.Min(1, progress / 21f);
			lastV += (v - lastV) * Engine.Delta;

			var w = Display.UiWidth * 0.5f;
			var h = 16;
			var vl = 0.9f;
			var pos = new Vector2((Display.UiWidth - w) / 2, Display.UiHeight * 0.5f + 96);
			var a = (float) Math.Max(0, 1f - (135 - logoCard.Y) / 135f);
			
			Graphics.Color = new Color(1, 1, 1, a);
			Graphics.Render(pixel, pos, 0, Vector2.Zero, new Vector2(w, h));
			Graphics.Color = ColorUtils.BlackColor;
			Graphics.Render(pixel, pos + new Vector2(1), 0, Vector2.Zero, new Vector2(w - 2, h - 2));
			Graphics.Color = new Color(vl, vl, vl, a);
			Graphics.Render(pixel, pos + new Vector2(2), 0, Vector2.Zero, new Vector2(lastV * (w - 4), h - 4));
			Graphics.Color = ColorUtils.WhiteColor;
		}

		private void GenerateNewTip() {
			exitTweenDone = false;

			Tween.To(Display.UiWidth + 150, tipLabel.CenterX, x => tipLabel.CenterX = x, 0.8f, Ease.QuadIn).OnEnd += () => {
				tipLabel.Label = LoadScreenTitles.Generate();
				tipLabel.FinishTyping();
				tipLabel.Center = new Vector2(-150, Display.UiHeight - 55);

				Tween.To(Display.UiWidth / 2f, tipLabel.CenterX, x => tipLabel.CenterX = x, 0.8f, Ease.QuadIn).OnEnd = () => exitTweenDone = true;
			};
		}
	}
}