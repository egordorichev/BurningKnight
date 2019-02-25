using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.item {
	public class Gold : Item {
		protected void _Init() {
			{
				Name = Locale.Get("gold");
				Stackable = true;
				AutoPickup = true;
				Useable = false;
				Description = Locale.Get("gold_desc");
			}
		}

		private static Animation Gold = Animation.Make("coin", "-gold");
		private static Animation Iron = Animation.Make("coin", "-iron");
		private static Animation Bronze = Animation.Make("coin", "-bronze");
		private AnimationData Animation = Bronze.Get("idle");

		public override Void OnPickup() {
			Audio.PlaySfx("coin");
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			this.Animation.Update(Dt);
		}

		public override TextureRegion GetSprite() {
			return this.Animation.GetCurrent().Frame;
		}

		public override Item SetCount(int Count) {
			if (Count == 1) {
				this.Animation = Bronze.Get("idle");
			} else if (Count >= 5 && Count <= 9) {
				this.Animation = Iron.Get("idle");
			} else if (Count >= 10) {
				this.Animation = Gold.Get("idle");
			} 

			return base.SetCount(Count);
		}

		public override Void Generate() {
			base.Generate();

			switch (Random.NewInt(10)) {
				case 0: 
				case 1: 
				case 2: 
				case 3: 
				case 4: 
				case 5: 
				case 6: 
				case 7: {
					this.SetCount(1);

					break;
				}

				case 9: 
				case 8: {
					this.SetCount(5);

					break;
				}

				case 10: {
					this.SetCount(10);

					break;
				}
			}

			if (Player.Instance != null) {
				this.SetCount((int) (Player.Instance.GoldModifier * this.GetCount()));
			} 
		}

		public Gold() {
			_Init();
		}
	}
}
