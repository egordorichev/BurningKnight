using BurningKnight.save;
using BurningKnight.state;
using Lens.assets;
using Lens.entity;

namespace BurningKnight.level.entities.exit {
	public class DailyRunExit : Exit {
		private bool hidden;
		
		protected override void Descend() {
			Run.StartNew(1, RunType.Daily);
			GlobalSave.Put($"daily_{Run.DailyId}", true);
		}

		protected override string GetFxText() {
			return Locale.Get("daily_run");
		}

		public override void Update(float dt) {
			base.Update(dt);
			hidden = GlobalSave.IsTrue($"daily_{Run.CalculateDailyId()}");
		}

		public override void Render() {
			if (hidden) {
				return;
			}
			
			base.Render();
		}

		protected override bool CanInteract(Entity e) {
			return !hidden;
		}
	}
}