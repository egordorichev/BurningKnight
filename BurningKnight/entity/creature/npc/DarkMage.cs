using System.Timers;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.item.stand;
using BurningKnight.level.biome;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using Lens.entity;
using Lens.util.math;
using Timer = Lens.util.timer.Timer;

namespace BurningKnight.entity.creature.npc {
	public class DarkMage : Npc {
		public override void AddComponents() {
			base.AddComponents();
			
			Width = 14;
			Height = 17;
			
			AddComponent(new AnimationComponent("dark_mage"));

			if (!(Run.Level?.Biome is LibraryBiome)) {
				AddComponent(new CloseDialogComponent("dm_0"));
			}

			Subscribe<Dialog.EndedEvent>();
			Subscribe<ItemTakenEvent>();
		}

		public override bool HandleEvent(Event e) {
			if (e is Dialog.EndedEvent dee) {
				if (dee.Dialog.Id == "bk_10") {
					Timer.Add(() => {
						// Really, well, thank you, Limpor
						GetComponent<DialogComponent>().StartAndClose("dm_1", 5);
					}, 1f);
				}
			} else if (e is ItemTakenEvent ite) {
				if (ite.Stand is DarkMageStand) {
					// Yes, yes, I need more power!
					// MORE POWER!!!
					// I can feel the energy growing in me!
					GetComponent<DialogComponent>().StartAndClose($"dm_{Rnd.Int(2, 4)}", 5);
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}