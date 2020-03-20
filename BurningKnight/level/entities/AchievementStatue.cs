using BurningKnight.assets.achievements;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.npc;
using BurningKnight.ui.dialog;
using ImGuiNET;
using Lens.assets;
using Lens.graphics;
using Lens.util.file;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.entities {
	public class AchievementStatue : SolidProp {
		private string id = "bk:rip";
		private bool unlocked;
		private TextureRegion achievementTexture;

		public override void AddComponents() {
			base.AddComponents();
			
			Width = 24;
			Height = 32;
			
			AddComponent(new DialogComponent());
			AddComponent(new InteractableComponent((e) => {
				GetComponent<DialogComponent>().StartAndClose($"ach_{id}_desc", 5);
				return true; 
			}) {
				CanInteract = e => unlocked
			});
			
			AddComponent(new SensorBodyComponent(-Npc.Padding, -Npc.Padding, Width + Npc.Padding * 2, Height + Npc.Padding * 2, BodyType.Static));
			AddComponent(new ShadowComponent());

			Achievements.UnlockedCallback += UpdateState;
			Achievements.LockedCallback += UpdateState;
		}

		public override void Destroy() {
			base.Destroy();
			
			Achievements.UnlockedCallback -= UpdateState;
			Achievements.LockedCallback -= UpdateState;
		}

		protected override Rectangle GetCollider() {
			return new Rectangle(0, 13, 24, 19);
		}

		public override void PostInit() {
			Sprite = "achievement_statue";
			
			base.PostInit();
			UpdateState();
			SetupSprite();
		}

		private void UpdateSprite() {
			if (HasComponent<InteractableComponent>()) {
				RemoveComponent<InteractableSliceComponent>();
			}
			
			InsertGraphics();
			SetupSprite();
		}

		private void SetupSprite() {
			achievementTexture = Animations.Get("achievements").GetSlice(id);
		}

		private void UpdateState(string i = null) {
			unlocked = Achievements.Get(id)?.Unlocked ?? false;
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			id = stream.ReadString();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteString(id);
		}

		public override void Render() {
			base.Render();

			if (unlocked) {
				Graphics.Render(achievementTexture, Position + new Vector2(2, 0));
			}
		}

		public override void RenderImDebug() {
			base.RenderImDebug();

			if (ImGui.InputText("Id", ref id, 128)) {
				UpdateSprite();
				UpdateState();
			}
		}
	}
}