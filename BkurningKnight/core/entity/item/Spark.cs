using BurningKnight.core.util;

namespace BurningKnight.core.entity.item {
	public class Spark : Entity {
		protected void _Init() {
			{
				AlwaysActive = true;
			}
		}

		private static Animation Animations = Animation.Make("fx-spark");
		private AnimationData Animation = Animations.Get("idle");
		public float A;
		public bool InUi;
		private float Val;

		public static Void RandomOn(Entity Entity) {
			RandomOn(Entity.X, Entity.Y, Entity.W, Entity.H);
		}

		public static Void RandomOn(float X, float Y, float W, float H) {
			RandomOn(X, Y, W, H, false);
		}

		public static Void RandomOn(float X, float Y, float W, float H, bool Ui) {
			Spark Spark = new Spark();
			Spark.InUi = Ui;
			Spark.A = Random.NewFloat(360);
			Spark.X = Random.NewFloat(X, X + W) - 3.5f;
			Spark.Y = Random.NewFloat(Y, Y + H) - 3.5f;
			Dungeon.Area.Add(Spark);
		}

		public override Void Init() {
			base.Init();
			this.Depth = 5;
			Tween.To(new Tween.Task(this.A + Random.NewFloat(-90f, 90f), 1f) {
				public override float GetValue() {
					return A;
				}

				public override Void SetValue(float Value) {
					A = Value;
				}
			});
			Val = Random.NewFloat(0.6f, 1f);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (this.Animation.Update(Dt * 0.7f)) {
				this.Done = true;
			} 
		}

		public override Void Render() {
			if (InUi) {
				Graphics.Batch.SetProjectionMatrix(Camera.Ui.Combined);
			} 

			Graphics.Batch.SetColor(Val, Val, Val, 1);
			this.Animation.Render(this.X, this.Y, false, false, 3.5f, 3.5f, this.A);

			if (InUi) {
				Graphics.Batch.SetProjectionMatrix(Camera.Game.Combined);
			} 
		}

		public Spark() {
			_Init();
		}
	}
}
