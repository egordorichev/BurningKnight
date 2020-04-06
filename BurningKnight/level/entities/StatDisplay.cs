using BurningKnight.assets.achievements;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.npc;
using BurningKnight.save;
using BurningKnight.state;
using ImGuiNET;
using Lens;
using Lens.entity;
using Lens.util.file;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.entities {
	public class StatDisplay : SolidProp {
		private string board;
		private int challengeId;

		protected override Rectangle GetCollider() {
			return new Rectangle(0, 6, 17, 4);
		}

		public override void PostInit() {
			Sprite = "stat_display";
			base.PostInit();
		}

		public override void AddComponents() {
			base.AddComponents();

			Width = 17;
			Height = 10;
			
			AddComponent(new InteractableComponent(Interact));
			AddComponent(new ShadowComponent());

			AddComponent(new SensorBodyComponent(-2, -Npc.Padding, Width + 4, Height + Npc.Padding * 2, BodyType.Static));
		}

		private bool Interact(Entity e) {
			var s = (InGameState) Engine.Instance.State;

			s.InStats = true;
			s.Paused = true;
			
			s.ReturnFromLeaderboard = () => {
				s.Paused = false;
				s.InStats = false;
			};

			var b = board;

			if (board == "daily") {
				b = $"daily_{Run.CalculateDailyId()}";
			} else if (board == "challenge") {
				b = $"challenge_{challengeId}";
			}

			s.ShowLeaderboard(b);
			return true;
		}

		private bool ch;
		
		public override void Update(float dt) {
			base.Update(dt);

			if (!Engine.EditingLevel && !ch) {
				ch = true;
				
				if (InGameState.SetupLeaderboard == null || (board == "daily" && !GlobalSave.IsTrue($"daily_{Run.CalculateDailyId()}"))) {
					Done = true;
				}
			}
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			
			board = stream.ReadString();

			if (board == "challenge") {
				challengeId = stream.ReadInt32();
			}
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteString(board);

			if (board == "challenge") {
				stream.WriteInt32(challengeId);
			}
		}

		public override void RenderImDebug() {
			base.RenderImDebug();

			if (board == null) {
				board = "";
			}

			ImGui.InputText("Board", ref board, 128);
			
			if (board == "challenge") {
				ImGui.InputInt("Challenge Id", ref challengeId);
			}
		}
	}
}