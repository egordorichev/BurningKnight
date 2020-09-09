using BurningKnight.entity.creature.npc;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using BurningKnight.util;
using Lens.assets;
using Lens.entity;

namespace BurningKnight.level.entities.exit {
	public class BossRushExit : Exit {
		protected override void Descend() {
			Run.StartNew(1, RunType.BossRush);
		}

		protected override bool Interact(Entity entity) {
			if (GlobalSave.Emeralds < 3) {
				AnimationUtil.ActionFailed();

				foreach (var n in Area.Tagged[Tags.Npc]) {
					if (n is Mike) {
						n.GetComponent<DialogComponent>().Start("mike_0");
						break;
					}
				}
				
				return true;
			}

			GlobalSave.Emeralds -= 3;
			return base.Interact(entity);
		}

		protected override string GetFxText() {
			return Locale.Get("boss_rush");
		}

		protected override bool CanInteract(Entity e) {
			return !InGameState.Multiplayer;
		}
	}
}