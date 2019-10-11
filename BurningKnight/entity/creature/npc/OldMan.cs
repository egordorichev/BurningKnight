using System.Linq;
using BurningKnight.assets.input;
using BurningKnight.entity.component;
using BurningKnight.level.biome;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using Lens.input;
using Lens.util.math;

namespace BurningKnight.entity.creature.npc {
	public class OldMan : Npc {
		public override void AddComponents() {
			base.AddComponents();

			Width = 20;
			Height = 18;

			AddComponent(new AnimationComponent("old_man"));
			AddComponent(new RectBodyComponent(-Padding, -Padding, Width + Padding * 2, Height + Padding * 2));

			if (Run.Depth == 0) {
				AddComponent(new CloseDialogComponent("old_man_0"));
				AddComponent(new InteractDialogComponent("old_man_1"));
			} else if (Run.Depth == Run.ContentEndDepth) {
				GetComponent<DialogComponent>().Start("old_man_4");
				cycle = true;
			} else if (Run.Depth == 1 && GlobalSave.IsFalse("control_roll")) {
				set = false;
				AddComponent(new CloseDialogComponent("control_1"));
			}

			AlwaysActive = true;
		}

		private bool set = true;
		private bool cycle;
		private float t;
		
		public override void Update(float dt) {
			base.Update(dt);

			if (cycle) {
				t += dt;

				if (t >= 1f) {
					t = 0;

					var all = BiomeRegistry.Defined.Values.ToArray();
					Run.Level.SetBiome(all[Random.Int(all.Length)]);
				}

				return;
			}
			
			if (!set) {
				set = true;
				GetComponent<DialogComponent>().Dialog.Str.SetVariable("ctrl", Controls.Find(Controls.Roll, GamepadComponent.Current != null));
			}
		}
	}
}