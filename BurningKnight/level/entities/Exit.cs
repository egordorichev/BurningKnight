using BurningKnight.assets.achievements;
using BurningKnight.entity;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.fx;
using BurningKnight.level.entities.exit;
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
		public static Exit Instance;
		
		public override void Init() {
			base.Init();
			Depth = Layers.Entrance;
			Instance = this;
		}

		public override void Destroy() {
			base.Destroy();

			if (Instance == this) {
				Instance = null;
			}
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
			
			if (Run.Depth == Run.ContentEndDepth || (Run.Type == RunType.BossRush && Run.Depth == 5)) {
				if (Run.Type == RunType.Regular || Run.Type == RunType.Twitch) {
					Run.ActualDepth = -1;
					Run.Depth = 1;
					Run.Loop++;
					
					Achievements.Unlock("bk:loop");
				} else {
					Run.Win();
				}
			} else {
				((InGameState) Engine.Instance.State).TransitionToBlack(entity.Center, () => {
					Run.NumPlayers = 0;
					Descend();
				});
			}

			Audio.PlaySfx("player_descending");			
			return true;
		}
		
		protected virtual void Descend() {
			if (Run.Depth == -2) {
				Achievements.Unlock("bk:tutorial");
				GlobalSave.Put("finished_tutorial", true);
				Run.Depth = 0;
			} else if (To == 1 || this is BossRushExit) {
				Run.NumPlayers = Area.Tagged[Tags.Player].Count;
				Run.StartNew();
				// Caves secret location
			} else if (Run.Depth == 13) {
				Run.Depth = 4;
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

			if (To != 13) {
				To = Run.Depth + 1;
			}

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
			return To == 13 ? "emerald_exit" : "exit";
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