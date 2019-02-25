using BurningKnight.entity.item.autouse;
using BurningKnight.entity.item.pet.impl;
using BurningKnight.entity.level.save;
using BurningKnight.util;

namespace BurningKnight.entity.item.pet {
	public class Pet : Autouse {
		public PetEntity Create() {
			return new PetEntity();
		}

		public override void Generate() {
			SetCount(Random.NewInt(1, 4));
		}

		public override void Use() {
			base.Use();
			var Entity = Create();
			double A = Random.NewFloat() * Math.PI * 2;
			var D = 24f;
			Entity.X = Owner.X + Owner.W / 2 + Math.Cos(A) * D;
			Entity.Y = Owner.Y + Owner.H / 2 + Math.Sin(A) * D;
			Dungeon.Area.Add(Entity);
			PlayerSave.Add(Entity);
		}
	}
}