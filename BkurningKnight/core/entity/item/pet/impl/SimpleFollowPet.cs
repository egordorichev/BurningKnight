using BurningKnight.core.assets;
using BurningKnight.core.entity.level;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.item.pet.impl {
	public class SimpleFollowPet : PetEntity {
		protected float MaxDistance = 32f;
		protected Entity Target;
		protected bool DependOnDistance;
		protected bool BuildPath;
		protected Point Next;
		protected bool NoAdd;

		public override Void Init() {
			base.Init();
			this.Target = this.Owner;
		}

		public Point GetCloser(Point Target) {
			int From = (int) (Math.Floor((this.X + this.W / 2) / 16) + Math.Floor((this.Y + this.H / 2) / 16) * Level.GetWidth());
			int To = (int) (Math.Floor((Target.X + this.W / 2) / 16) + Math.Floor((Target.Y + this.H / 2) / 16) * Level.GetWidth());
			int Step = PathFinder.GetStep(From, To, Dungeon.Level.GetPassable());

			if (Step != -1) {
				Point P = new Point();
				P.X = Step % Level.GetWidth() * 16;
				P.Y = (float) (Math.Floor(Step / Level.GetWidth()) * 16);

				return P;
			} 

			return null;
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			float Dx = this.Target.X + this.Target.W / 2 - this.X - this.W / 2;
			float Dy = this.Target.Y + this.Target.H / 2 - this.Y - this.H / 2;
			double D = Math.Sqrt(Dx * Dx + Dy * Dy);

			if (this.BuildPath) {
				if (D > this.MaxDistance) {
					if (this.Next == null) {
						this.Next = this.GetCloser(this.Target);
					} else {
						Dx = this.Next.X + 8 - this.X - this.W / 2;
						Dy = this.Next.Y + 8 - this.Y - this.H / 2;
						D = Math.Sqrt(Dx * Dx + Dy * Dy);

						if (D <= 4f) {
							this.Next = null;
						} else {
							D *= 0.1f;
							this.Velocity.X += Dx / D;
							this.Velocity.Y += Dy / D;
						}

					}

				} 
			} else {
				if (D > MaxDistance) {
					if (DependOnDistance) {
						D *= 0.25f;
						this.Velocity.X += Dx / D;
						this.Velocity.Y += Dy / D;
					} else {
						float S = 10f;
						this.Velocity.X += Dx / S;
						this.Velocity.Y += Dy / S;
					}

				} 
			}


			if (!this.NoAdd) {
				this.X += this.Velocity.X * Dt;
				this.Y += this.Velocity.Y * Dt;
			} 

			this.Velocity.X -= this.Velocity.X * Math.Min(1, Dt * 3);
			this.Velocity.Y -= this.Velocity.Y * Math.Min(1, Dt * 3);
		}

		public override Void RenderShadow() {
			Graphics.Shadow(this.X, this.Y, this.W, this.H, this.Z);
		}
	}
}
