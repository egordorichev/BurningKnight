using System.Linq;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.util;
using Lens.assets;
using Lens.entity;
using Lens.util.math;
using Lens.util.timer;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities.statue {
	public class Well : Statue {
		protected override string GetFxText() {
			return "throw_coin";
		}

		public override void AddComponents() {
			base.AddComponents();

			Sprite = "well";
			Width = 20;
			Height = 30;
			
			AddComponent(new AudioEmitterComponent());
		}

		protected override Rectangle GetCollider() {
			return new Rectangle(0, 15, 20, 15);
		}

		protected override bool Interact(Entity e) {
			var c = e.GetComponent<ConsumablesComponent>();

			if (c.Coins < 1) {
				AnimationUtil.ActionFailed();
				TextParticle.Add(e, Locale.Get("no_coins"));
				
				return true;
			}

			GetComponent<AudioEmitterComponent>().Emit("level_well_coin");

			c.Coins--;
			
			var keys = DiceStatue.Effects.Keys;
			var key = keys.ElementAt(Rnd.Int(keys.Count));
			
			TextParticle.Add(this, Locale.Get(key));

			Timer.Add(() => {
				DiceStatue.Effects[key](this, e);
			}, 1f);

			return true;
		}
	}
}