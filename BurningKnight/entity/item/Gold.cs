using BurningKnight.entity.creature.player;
using BurningKnight.util;

namespace BurningKnight.entity.item {
	public class Gold : Item {
		private static Animation Gold = Animation.Make("coin", "-gold");
		private static Animation Iron = Animation.Make("coin", "-iron");
		private static Animation Bronze = Animation.Make("coin", "-bronze");
		private AnimationData Animation = Bronze.Get("idle");

		public Gold() {
			_Init();
		}

		protected void _Init() {
			{
				Name = Locale.Get("gold");
				Stackable = true;
				AutoPickup = true;
				Useable = false;
				Description = Locale.Get("gold_desc");
			}
		}

		public override void OnPickup() {
			Audio.PlaySfx("coin");
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			Animation.Update(Dt);
		}

		public override TextureRegion GetSprite() {
			return Animation.GetCurrent().Frame;
		}

		public override Item SetCount(int Count) {
			if (Count == 1)
				Animation = Bronze.Get("idle");
			else if (Count >= 5 && Count <= 9)
				Animation = Iron.Get("idle");
			else if (Count >= 10) Animation = Gold.Get("idle");

			return base.SetCount(Count);
		}

		public override void Generate() {
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
					SetCount(1);

					break;
				}

				case 9:
				case 8: {
					SetCount(5);

					break;
				}

				case 10: {
					SetCount(10);

					break;
				}
			}

			if (Player.Instance != null) SetCount((int) (Player.Instance.GoldModifier * GetCount()));
		}
	}
}