using System;
using BurningKnight.entity.component;
using BurningKnight.entity.events;

namespace BurningKnight.entity.creature {
	public class Decoy : Creature {
		public Action OnDeath;
		
		public override void AddComponents() {
			base.AddComponents();

			Width = 11;
			Height = 10;
			
			AddTag(Tags.PlayerTarget);
			RemoveTag(Tags.LevelSave);
			
			AddComponent(new SensorBodyComponent(0, 0, 11, 10));
			AddComponent(new RectBodyComponent(0, 9, 11, 1));

			GetComponent<RectBodyComponent>().KnockbackModifier = 0.1f;
			
			AddComponent(new SliceComponent("items", "bk:decoy"));
			AddComponent(new ShadowComponent());

			var h = GetComponent<HealthComponent>();

			h.InitMaxHealth = 6;
			h.RenderInvt = true;

			GetComponent<DropsComponent>().Drops.Clear();
		}

		private float t;

		public override void Update(float dt) {
			base.Update(dt);

			var r = GetComponent<RoomComponent>().Room;

			if (r != null && r.Tagged[Tags.MustBeKilled].Count == 0) {
				Kill(this);
				return;
			}
			
			t += dt;

			if (t >= 20f) {
				Kill(this);
			}
		}

		public override void AnimateDeath(DiedEvent d) {
			base.AnimateDeath(d);
			ExplosionMaker.Make(this);
			OnDeath?.Invoke();
		}
	}
}