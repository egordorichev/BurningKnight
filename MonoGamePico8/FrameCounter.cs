using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoGamePico8 {
	public class FrameCounter {
		public const int MaximumSamples = 30;

		private Queue<float> buffer = new Queue<float>();

		public long TotalFrames { get; private set; }
		public float TotalSeconds { get; private set; }
		public int AverageFramesPerSecond { get; private set; }
		public int CurrentFramesPerSecond { get; private set; }

		public void Update(float deltaTime) {
			CurrentFramesPerSecond = (int) Math.Round(1.0f / deltaTime);

			buffer.Enqueue(CurrentFramesPerSecond);

			if (buffer.Count > MaximumSamples) {
				buffer.Dequeue();
				AverageFramesPerSecond = (int) Math.Round(buffer.Average(i => i));
			} else {
				AverageFramesPerSecond = CurrentFramesPerSecond;
			}

			TotalFrames++;
			TotalSeconds += deltaTime;
		}
	}
}