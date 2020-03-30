using BurningKnight.assets.achievements;
using BurningKnight.entity;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.fx;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.ui.editor;
using BurningKnight.util;
using ImGuiNET;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.util.file;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.entities {
	public class Exit : SaveableEntity, PlaceableEntity {
		public int To;
		
		public override void Init() {
			base.Init();
			Depth = Layers.Entrance;
		}

		protected virtual bool CanUse() {
			return true;
		}
		
		protected virtual bool Interact(Entity entity) {
			if (!CanUse()) {
				AnimationUtil.ActionFailed();
				return true;
			}
			
			entity.RemoveComponent<PlayerInputComponent>();
			entity.GetComponent<HealthComponent>().Unhittable = true;
			
			if (Run.Depth == Run.ContentEndDepth - 1 || (Run.Type == RunType.BossRush && Run.Depth == 5)) {
				Run.Win();
			} else {
				((InGameState) Engine.Instance.State).TransitionToBlack(entity.Center, Descend);
			}

			Audio.PlaySfx("player_descending");			

			return true;
		}
		
		protected virtual void Descend() {
			if (Run.Depth == -2) {
				Achievements.Unlock("bk:tutorial");
				GlobalSave.Put("finished_tutorial", true);
				Run.Depth = 0;
			} else if (To == 1) {
				Run.StartNew();
			} else {
				Run.Depth = To;
			}
		}

		protected virtual string GetFxText() {
			return Locale.Get(Run.Depth == 0 ? "new_run" : "descend");
		}

		public override void AddComponents() {
			base.AddComponents();

			Width = 16;
			Height = 14;
			
			To = Run.Depth + 1;
			
			AddComponent(new InteractableComponent(Interact) {
				OnStart = entity => {
					if (entity is LocalPlayer && Run.Depth != -2) {
						Engine.Instance.State.Ui.Add(new InteractFx(this, GetFxText()));
					}
				},
				
				CanInteract = CanInteract
			});
			
			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Static, true));
		}

		protected virtual bool CanInteract(Entity e) {
			return true;
		}

		public override void PostInit() {
			base.PostInit();
			AddComponent(new InteractableSliceComponent("props", GetSlice()));
		}

		protected virtual string GetSlice() {
			return "exit";
		}
		
		public override void Load(FileReader stream) {
			base.Load(stream);
			To = stream.ReadInt16();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteInt16((short) To);
		}

		public override void RenderImDebug() {
			ImGui.InputInt("To", ref To);
		}
	}
}