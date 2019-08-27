using System;
using System.Threading;
using BurningKnight.assets;
using BurningKnight.assets.items;
using BurningKnight.assets.lighting;
using BurningKnight.assets.prefabs;
using BurningKnight.level.tile;
using BurningKnight.physics;
using BurningKnight.save;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.game;
using Lens.graphics;
using Lens.input;
using Lens.util;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Random = Lens.util.math.Random;

namespace BurningKnight.state {
	public class AssetLoadState : GameState {
		private PhotoCard logoCard;
		private PhotoCard[] cards = new PhotoCard[3];
		private bool ready;
		private int progress;
		private Area gameArea;
		private float t;
		private bool added;
		private bool removed;

		public override void Init() {
			base.Init();

			Ui.Add(logoCard = new PhotoCard {
				Region = new TextureRegion(Textures.FastLoad("Content/logo.png")),
				Url = "https://twitter.com/rexcellentgames",
				Name = "Rexcellent Games",
				Position = new Vector2(240, -360),
				Target = new Vector2(240, 135),
				Scale = 1
			});

			progress = 0;

			var thread = new Thread(() => {
				Log.Info("Starting asset loading thread");
				
				Assets.Load(ref progress);
				Dialogs.Load();
				progress++;
				CommonAse.Load();
				progress++;
				ImGuiHelper.BindTextures();
				progress++;
				Shaders.Load();
				progress++;
				Prefabs.Load();
				progress++;
				Items.Load();
				progress++;
				Mods.Load();
				progress++; // Should be 12 here
				
				Log.Info("Done loading assets! Loading level now.");
			
				Lights.Init();
				Physics.Init();
				gameArea = new Area();

				Run.Level = null;
				Tilesets.Load();
				progress++;
				
				SaveManager.Load(gameArea, SaveType.Game);
				progress++;
				
				Random.Seed = $"{Run.Seed}_{Run.Depth}"; 
				
				SaveManager.Load(gameArea, SaveType.Level);
				progress++;

				if (Run.Depth > 0) {
					SaveManager.Load(gameArea, SaveType.Player);
				} else {
					SaveManager.Generate(gameArea, SaveType.Player);
				}

				progress++; // Should be 16 here
				Log.Info("Done loading level! Going to menu.");
				
				ready = true;
			});

			thread.Priority = ThreadPriority.BelowNormal;
			thread.Start();
		}

		public override void Update(float dt) {
			base.Update(dt);

			t += dt;

			if (t > 5f && !added) {
				added = true;
				logoCard.GoAway = true;
				
				Ui.Add(cards[0] = new PhotoCard {
					Region = new TextureRegion(Textures.FastLoad("Content/egor.png")),
					Name = "Egor Dorichev",
					Tasks = "Code, Art, Sfx & Design",
					Url = "https://twitter.com/egordorichev",
					Position = new Vector2(-200, 135),
					Target = new Vector2(80, 135),
					Angle = 1f
				});
			
				Ui.Add(cards[1] = new PhotoCard {
					Region = new TextureRegion(Textures.FastLoad("Content/jose.png")),
					Name = "Jose Ramon",
					Tasks = "Music & Sfx",
					Url = "https://twitter.com/BibikiGL",
					Position = new Vector2(240, 260),
					Target = new Vector2(240, 135)
				});
			
				Ui.Add(cards[2] = new PhotoCard {
					Region = new TextureRegion(Textures.FastLoad("Content/mate.png")),
					Name = "Mate Cziner",
					Tasks = "Art & Design",
					Url = "https://twitter.com/MateCziner",
					Position = new Vector2(Display.UiWidth + 200, 135),
					Target = new Vector2(400, 135),
					Angle = -1f
				});
			}

			if (Input.WasPressed(Controls.GameStart)) {
				Engine.Instance.SetState(new AssetLoadState());
				return;
			}
			
			if (ready) {
				if (removed) {
					foreach (var c in cards) {
						if (!c.Done) {
							return;
						}
					}

					Engine.Instance.SetState(new InGameState(gameArea, true));
				} else {
					removed = true;

					foreach (var c in cards) {
						c.GoAway = true;
					}
				}
			}
		}

		public override void RenderUi() {
			base.RenderUi();
			var w = Input.Mouse.UiPosition;

			Graphics.Print($"{Math.Floor(progress / 16f * 100f)}%", Font.Small, Vector2.Zero);
			Graphics.Batch.DrawCircle(new CircleF(new Point((int) w.X, (int) w.Y), 4), 12, Color.Red, 4);
		}
	}
}