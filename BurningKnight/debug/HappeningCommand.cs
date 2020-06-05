using System;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.twitch.happening;
using Lens;
using Lens.util;
using Lens.util.timer;

namespace BurningKnight.debug {
	public class HappeningCommand : ConsoleCommand {
		public HappeningCommand() {
    	Name = "HappeningCommand";
    	ShortName = "hp";
    }
    
    public override void Run(Console Console, string[] Args) {
    	if (Args.Length == 1) {
	      var h = HappeningRegistry.Get(Args[0]);

	      if (h == null) {
		      return;
	      }

	      try {
		      var p = LocalPlayer.Locate(Engine.Instance.State.Area);
					h.Happen(p);

					Timer.Add(() => {
						h.End(p);
					}, h.GetVoteDelay());
	      } catch (Exception e) {
		      Log.Error(e);
	      }
      }
    }
	}
}