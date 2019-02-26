using BurningKnight.util;

namespace BurningKnight.entity.item {
	public class Spark : Entity {
		private static Animation Animations = Animation.Make("fx-spark");
		public float A;
		private AnimationData Animation = Animations.Get("idle");
		public bool InUi;
		private float Val;

		protected void _Init() {
			{
				AlwaysActive = true;
			}
		}

		public static void RandomOn(Entity Entity) {
			RandomOn(Entity.X, Entity.Y, Entity.W, Entity.H);
		}

		public static void RandomOn(float X, float Y, float W, float H) {
			RandomOn(X, Y, W, H, false);
		}

		public static void RandomOn(float X, float Y, float W, float H, bool Ui) {
			var Spark = new Spark();
			Spark.InUi = Ui;
			Spark.A = Random.NewFloat(360);
			Spark.X = Random.NewFloat(X, X + W) - 3.5f;
			Spark.Y = Random.NewFloat(Y, Y + H) - 3.5f;
			Dungeon.Area.Add(Spark);
		}

		public override void Init() {
			base.Init();
			Depth = 5;
			Tween.To(new Tween.Task(A + Random.NewFloat(-90f, 90f), 1f) {

		public override float GetValue() {
			return A;
		}

		public override void SetValue(float Value) {
			A = Value;
		}
	});

	Val = Random.NewFloat(0.6f, 1f);
}

public override void Update(float Dt) {
base.Update(Dt);
if (this.Animation.Update(Dt * 0.7f)) {
this.Done = true;
}
}
public override void Render() {
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