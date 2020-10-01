using BurningKnight.assets.input;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.ui.editor;
using ImGuiNET;
using Lens;
using Lens.entity;
using Lens.graphics;
using Lens.input;
using Lens.util.camera;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.pc {
	public class Pico : SaveableEntity, PlaceableEntity {
		private bool on;
		private Controller controller;
		// private Emulator emulator;
		// private MonoGameGraphicsBackend backend;
		private const float UpdateTime30 = 1 / 30f;
		private const float UpdateTime60 = 1 / 60f;
		private float deltaUpdate30, deltaUpdate60, deltaDraw;
		private string cart = "ma_puzzle";

		public Entity Entity;

		public override void PostInit() {
			base.PostInit();

			controller = new Controller();
			Area.Add(controller);

			controller.Y = Bottom + 16;
			controller.X = X + 16;
			controller.Pico = this;
			
			Area.Add(new RenderTrigger(this, RenderDisplay, Layers.Console));
		}

		public override void AddComponents() {
			base.AddComponents();

			Width = 138;
			Height = 150 + 5;
			
			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Static, false));
			AddComponent(new SliceComponent("props", "pico"));
			AddComponent(new ShadowComponent());
		}

		public void TurnOn() {
			if (on) {
				return;
			}
			
			/*if (emulator == null) {
				backend = new MonoGameGraphicsBackend(Engine.GraphicsDevice);
				emulator = new Emulator(backend, new MonoGameAudioBackend(), new MonoGameInputBackend());
			}*/
			
			on = true;

			Camera.Instance.Targets.Clear();
			Camera.Instance.Position = Position + new Vector2(Display.Width * 0.25f, 0);// + new Vector2(5 + 64, 16 + 64);

			LoadCart();
		}

		private void LoadCart() {
			/*if (!emulator.CartridgeLoader.Load($"Content/Carts/{cart}.p8")) {
				Log.Error($"Failed to load the cart {cart}");
			}*/
		}

		public void TurnOff() {
			if (!on) {
				return;
			}
			
			Entity.AddComponent(new PlayerInputComponent());
			Entity = null;
			
			Camera.Instance.Targets.Clear();
			((InGameState) Engine.Instance.State).ResetFollowing();

			on = false;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (!on) {
				return;
			}

			if (Input.WasPressed(Controls.Pause, GamepadComponent.Current, true)) {
				TurnOff();

				return;
			}
			
			deltaUpdate60 += dt;
			deltaUpdate30 += dt;

			/*
			while (deltaUpdate30 >= UpdateTime30) {
				deltaUpdate30 -= UpdateTime30;
				emulator.Update30();
			}
			
			while (deltaUpdate60 >= UpdateTime60) {
				deltaUpdate60 -= UpdateTime60;
				emulator.Update60();
			}*/
		}
		
		public void RenderDisplay() {
			/*if (!on) {
				return;
			}
			
			var u = emulator.CartridgeLoader.HighFps ? UpdateTime60 : UpdateTime30;
			
			deltaDraw += Engine.Delta;

			while (deltaDraw >= u) {
				deltaDraw -= u;
				emulator.Graphics.drawCalls = 0;
				emulator.Draw();
			}

			emulator.Graphics.Flip();
			Graphics.Render(backend.Surface, Position + new Vector2(5, 16));*/
		}

		public override void RenderImDebug() {
			base.RenderImDebug();
			ImGui.InputText("Cart", ref cart, 64);
			ImGui.SameLine();
			
			if (ImGui.Button("Load")) {
				LoadCart();
			}
		}
	}
}