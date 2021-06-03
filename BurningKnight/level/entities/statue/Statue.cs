using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.fx;
using BurningKnight.entity.projectile;
using BurningKnight.ui.dialog;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.util.camera;
using Lens.util.file;

namespace BurningKnight.level.entities.statue {
	public class Statue : SolidProp {
		protected bool Broken;

		protected virtual string GetFxText() {
			return "touch";
		}
		
		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new InteractableComponent(Interact) {
				CanInteract = CanInteract,
				OnStart = (e) => {
					if (GetFxText() != null) {
						Engine.Instance.State.Ui.Add(new InteractFx(this, Locale.Get(GetFxText())));
					}
				}
			});

			AddComponent(new AudioEmitterComponent());
			AddComponent(new ShadowComponent());
		}

		protected virtual bool CanInteract(Entity e) {
			return !Broken;
		}

		public override void PostInit() {
			base.PostInit();

			if (Broken) {
				UpdateSprite();
			}

			AddSensor();

			if (TryGetComponent<DialogComponent>(out var c)) {
				c.Dialog.Voice = 30;
			}
		}

		protected virtual void AddSensor() {
			AddComponent(new SensorBodyComponent(-Npc.Padding, -Npc.Padding, Width + Npc.Padding * 2, Height + Npc.Padding * 2));
		}

		protected virtual bool Interact(Entity e) {
			return false;
		}

		protected void Break() {
			if (Broken) {
				return;
			}

			Camera.Instance.Shake(8);
			Broken = true;
			UpdateSprite();

			GetComponent<AudioEmitterComponent>().Emit(GetSfx());
		}

		protected virtual string GetSfx() {
			return "level_statue_break";
		}

		protected virtual void UpdateSprite() {
			GetComponent<InteractableSliceComponent>().Sprite = CommonAse.Props.GetSlice($"broken_{Sprite}");
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			Broken = stream.ReadBoolean();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteBoolean(Broken);
		}

		public override bool ShouldCollide(Entity entity) {
			return base.ShouldCollide(entity) && !(entity is Projectile);
		}
	}
}