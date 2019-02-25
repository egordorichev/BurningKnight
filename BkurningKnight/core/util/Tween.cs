namespace BurningKnight.core.util {
	public class Tween {
		public static class Type {
			public const Type LINEAR = new Type() {
				public override float Get(float P) {
					return P;
				}
			};
			public const Type SINE_IN = new Type() {
				public override float Get(float P) {
					return (float) -Math.Cos(P * (Math.PI / 2)) + 1;
				}
			};
			public const Type QUAD_IN = new Type() {
				public override float Get(float P) {
					return P * P;
				}
			};
			public const Type QUAD_OUT = new Type() {
				public override float Get(float P) {
					return (P -= 1) * P * P + 1;
				}
			};
			public const Type QUAD_IN_OUT = new Type() {
				public override float Get(float P) {
					if ((P *= 2) < 1) {
						return 0.5f * P * P * P;
					} 

					return 0.5f * ((P -= 2) * P * P + 2);
				}
			};
			public const Type ELASTIC_IN = new Type() {
				public override float Get(float T) {
					float A = 1;
					float P = 0.3f;

					if (T == 0) return 0;


					if (T == 1) return 1;


					float S = P / 4;

					return -(A * (float) Math.Pow(2, 10 * (T -= 1)) * (float) Math.Sin((T - S) * (2 * Math.PI) / P));
				}
			};
			public const Type ELASTIC_OUT = new Type() {
				public override float Get(float P) {
					float A = 1;
					float Q = 0.3f;

					if (P == 0) return 0;


					if (P == 1) return 1;


					float S = Q / 4;

					return A * (float) Math.Pow(2, -10 * P) * (float) Math.Sin((P - S) * (2 * Math.PI) / Q) + 1;
				}
			};
			public const Type BACK_IN = new Type() {
				public override float Get(float P) {
					return P * P * ((T + 1) * P - T);
				}
			};
			public const Type BACK_OUT = new Type() {
				public override float Get(float P) {
					return (P -= 1) * P * ((T + 1) * P + T) + 1;
				}
			};
			public const Type BACK_IN_OUT = new Type() {
				public override float Get(float P) {
					float S = T;
					float T = P;

					if ((T *= 2) < 1) return 0.5f * (T * T * (((S *= (1.525f)) + 1) * T - S));


					return 0.5f * ((T -= 2) * T * (((S *= (1.525f)) + 1) * T + S) + 2);
				}
			};

			public float Get(float P) {
				return 0;
			}
		}

		public static class Task {
			public float Start;
			public float End;
			public float Rate;
			public float Progress;
			public float Difference;
			public float Delay;
			public Type Type;
			public bool Done;
			public bool Started;

			public Void DeleteSelf() {
				Done = true;
			}

			public Task(float End, float T) {
				this(End, T, Type.QUAD_IN);
			}

			public Task(float End, float T, Type Type) {
				this.End = End;
				this.Start = this.GetValue();
				this.Rate = 1 / T;
				this.Difference = (this.End - this.Start);
				this.Type = Type;
			}

			public Task Delay(float D) {
				this.Delay = D;

				return this;
			}

			public float GetValue() {
				return 0;
			}

			public Void SetValue(float Value) {

			}

			public Void OnStart() {

			}

			public Void OnEnd() {

			}

			public float Function(float P) {
				return this.Type.Get(P);
			}

			public bool RunWhenPaused() {
				return false;
			}
		}

		private static List<Task> Tasks = new List<>();
		private static float T = 1.70158f;

		public static Task To(Task Task) {
			Tasks.Add(Task);

			return Task;
		}

		public static Void Remove(Task Task) {
			Tasks.Remove(Task);
		}

		public static Void Update(float Dt) {
			for (int I = Tasks.Size() - 1; I >= 0; I--) {
				Task Task = Tasks.Get(I);

				if (Task.Done) {
					Tasks.Remove(I);

					continue;
				} 

				if (!Task.RunWhenPaused() && Dungeon.Game.GetState().IsPaused()) {
					continue;
				} 

				if (Task.Delay > 0) {
					Task.Delay -= Dt;

					continue;
				} 

				if (!Task.Started) {
					Task.OnStart();
					Task.Started = true;
				} 

				Task.Progress += Dt * Task.Rate;
				float X = (Task.Progress >= 1 ? Task.Function(1) : Task.Function(Task.Progress));
				Task.SetValue(Task.Start + X * Task.Difference);

				if (Task.Progress >= 1) {
					Tasks.Remove(I);
					Task.OnEnd();
					Task.Done = true;
				} 
			}
		}
	}
}
