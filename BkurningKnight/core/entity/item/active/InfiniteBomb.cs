using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.item.entity;

namespace BurningKnight.core.entity.item.active {
	public class InfiniteBomb : ActiveItem {
		protected void _Init() {
			{
				Sprite = "item-bomb_orbital";
				Stackable = false;
				UseTime = 0.4f;
				Name = Locale.Get("infinite_bomb");
				Description = Locale.Get("infinite_bomb_desc");
			}
		}

		public override Void Use() {
			if (this.Delay > 0) {
				return;
			} 

			base.Use();
			BombEntity E = new BombEntity(this.Owner.X + (this.Owner.W - 16) / 2, this.Owner.Y + (this.Owner.H - 16) / 2).ToMouseVel();
			E.Owner = this.Owner;

			if (this.Owner is Player) {
				Player Player = (Player) this.Owner;
				E.LeaveSmall = Player.LeaveSmall;
			} 

			Dungeon.Area.Add(E);
		}

		public InfiniteBomb() {
			_Init();
		}
	}
}
