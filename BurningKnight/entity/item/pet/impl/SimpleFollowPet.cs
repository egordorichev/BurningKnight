using BurningKnight.entity.level;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.item.pet.impl {
	public class SimpleFollowPet : PetEntity {
		protected bool BuildPath;
		protected bool DependOnDistance;
		protected float MaxDistance = 32f;
		protected Point Next;
		protected bool NoAdd;
		protected Entity Target;

		public override void Init() {
			base.Init();
			Target = Owner;
		}

		public Point GetCloser(Point Target) {
			var From = (int) (Math.Floor((this.X + W / 2) / 16) + Math.Floor((this.Y + H / 2) / 16) * Level.GetWidth());
			var To = (int) (Math.Floor((Target.X + W / 2) / 16) + Math.Floor((Target.Y + H / 2) / 16) * Level.GetWidth());
			var Step = PathFinder.GetStep(From, To, Dungeon.Level.GetPassable());

			if (Step != -1) {
				var P = new Point();
				P.X = Step % Level.GetWidth() * 16;
				P.Y = (float) (Math.Floor(Step / Level.GetWidth()) * 16);

				return P;
			}

			return null;
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			var Dx = Target.X + Target.W / 2 - this.X - W / 2;
			var Dy = Target.Y + Target.H / 2 - this.Y - H / 2;
			double D = Math.Sqrt(Dx * Dx + Dy * Dy);

			if (BuildPath) {
				if (D > MaxDistance) {
					if (Next == null) {
						Next = GetCloser(Target);
					}
					else {
						Dx = Next.X + 8 - this.X - W / 2;
						Dy = Next.Y + 8 - this.Y - H / 2;
						D = Math.Sqrt(Dx * Dx + Dy * Dy);

						if (D <= 4f) {
							Next = null;
						}
						else {
							D *= 0.1f;
							Velocity.X += Dx / D;
							Velocity.Y += Dy / D;
						}
					}
				}
			}
			else {
				if (D > MaxDistance) {
					if (DependOnDistance) {
						D *= 0.25f;
						Velocity.X += Dx / D;
						Velocity.Y += Dy / D;
					}
					else {
						var S = 10f;
						Velocity.X += Dx / S;
						Velocity.Y += Dy / S;
					}
				}
			}


			if (!NoAdd) {
				this.X += Velocity.X * Dt;
				this.Y += Velocity.Y * Dt;
			}

			Velocity.X -= Velocity.X * Math.Min(1, Dt * 3);
			Velocity.Y -= Velocity.Y * Math.Min(1, Dt * 3);
		}

		public override void RenderShadow() {
			Graphics.Shadow(this.X, this.Y, W, H, Z);
		}
	}
}