using System;
using BurningKnight.assets.achievements;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.npc;
using BurningKnight.save;
using BurningKnight.ui.dialog;
using ImGuiNET;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.graphics;
using Lens.util;
using Lens.util.file;
using Lens.util.math;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.entities {
	public class AchievementStatue : Prop {
		private string id = "bk:rip";
		private Achievement achievement;
		private TextureRegion achievementTexture;
		private TextureRegion lockedAchievementTexture;
		private float offset;

		public override void AddComponents() {
			base.AddComponents();

			offset = Rnd.Float(1);
			
			Width = 24;
			Height = 32;
			
			AddComponent(new DialogComponent());
			AddComponent(new InteractableComponent(Interact) {
				// CanInteract = e => achievement.Unlocked
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

			string state;

			if (achievement.Unlocked) {
				state = $"{Locale.Get($"ach_{id}")}\n{Locale.Get($"ach_{id}_desc")}\n{Locale.Get("completed_on")} {achievement.CompletionDate}";
			} else {
				state = Locale.Get($"ach_{id}");

				if (achievement.Max > 0) {
					var p = GlobalSave.GetInt($"ach_{id}", 0);

					state += $"\n{MathUtils.Clamp(0, achievement.Max, p)}/{achievement.Max} {Locale.Get("complete")}";
				}
			}
			
			GetComponent<DialogComponent>().StartAndClose(state, 5);
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
				Graphics.Render(achievementTexture, Position + new Vector2(2, (float) Math.Cos(Engine.Time * 1.5f + offset) * 2.5f - 2.5f));
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