using BurningKnight.assets.achievements;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.npc;
using BurningKnight.ui.dialog;
using ImGuiNET;
using Lens.assets;
using Lens.entity;
using Lens.graphics;
using Lens.util.file;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.entities {
	public class AchievementStatue : Prop {
		private string id = "bk:rip";
		private Achievement achievement;
		private TextureRegion achievementTexture;

		public override void AddComponents() {
			base.AddComponents();
			
			Width = 24;
			Height = 32;
			
			AddComponent(new DialogComponent());
			AddComponent(new InteractableComponent(Interact) {
				CanInteract = e => achievement.Unlocked
			});
			
			AddComponent(new SensorBodyComponent(-Npc.Padding, -Npc.Padding, Width + Npc.Padding * 2, Height + Npc.Padding * 2, BodyType.Static));
			AddComponent(new ShadowComponent());
			AddComponent(new InteractableSliceComponent("props", "achievement_statue"));
			AddComponent(new RectBodyComponent(0, 13, 24, 19, BodyType.Static));

			AddTag(Tags.Statue);
		}

		public override void PostInit() {
			base.PostInit();
			SetupSprite();
			UpdateState();
		}
		
		private bool Interact(Entity e) {
			foreach (var s in Area.Tagged[Tags.Statue]) {
				if (s.TryGetComponent<DialogComponent>(out var d)) {
					d.Close();
				}
			}
		
			GetComponent<DialogComponent>().StartAndClose(achievement.Unlocked ? $"{Locale.Get($"ach_{id}")}\n{Locale.Get($"ach_{id}_desc")}\n{Locale.Get("completed_on")} {achievement.CompletionDate}" : $"ach_{id}", 5);
			return true; 
		}

		private void SetupSprite() {
			achievementTexture = Animations.Get("achievements").GetSlice(id);
		}

		private void UpdateState(string i = null) {
			achievement = Achievements.Get(id);
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

			if (achievement.Unlocked) {
				Graphics.Render(achievementTexture, Position + new Vector2(2, 0));
			}
		}

		public override void RenderImDebug() {
			base.RenderImDebug();

			if (ImGui.InputText("Id", ref id, 128)) {
				SetupSprite();
				UpdateState();
			}
		}
	}
}