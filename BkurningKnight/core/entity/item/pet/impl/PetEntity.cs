using BurningKnight.core.assets;
using BurningKnight.core.entity.creature;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.level;
using BurningKnight.core.entity.level.entities;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.item.pet.impl {
	public class PetEntity : SaveableEntity {
		protected void _Init() {
			{
				AlwaysActive = true;
			}
		}

		public Player Owner;
		public float Z;
		protected string Sprite = "";
		public TextureRegion Region = Item.Missing;
		protected bool NoTp;

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (!this.OnScreen && !this.NoTp) {
				this.X = this.Owner.X + this.Owner.W / 2;
				this.Y = this.Owner.Y + this.Owner.H / 2;
				this.Tp();
			} 
		}

		protected Void Tp() {

		}

		public override Void Init() {
			base.Init();

			if (!this.Sprite.IsEmpty()) {
				this.Region = Graphics.GetTexture(this.Sprite);
			} 

			this.Owner = Player.Instance;
			double A = Random.NewFloat() * Math.PI * 2;
			float D = 24f;
			this.X = this.Owner.X + this.Owner.W / 2 + (float) (Math.Cos(A) * D);
			this.Y = this.Owner.Y + this.Owner.H / 2 + (float) (Math.Sin(A) * D);
		}

		public override Void Render() {
			Graphics.Render(Region, this.X, this.Y);
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity is Creature || Entity is Door || Entity is SolidProp || (Entity is ItemHolder && !(((ItemHolder) Entity).GetItem() is Gold))) {
				return false;
			} 

			return base.ShouldCollide(Entity, Contact, Fixture);
		}

		public PetEntity() {
			_Init();
		}
	}
}
