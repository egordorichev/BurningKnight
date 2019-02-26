using System;
using Lens;

namespace BurningKnight.debug {
	public class ZoomCommand : ConsoleCommand {
		public ZoomCommand() {
			_Init();
		}

		protected void _Init() {
			{
				Name = "/zoom";
				ShortName = "/z";
			}
		}

		public override void Run(Console Console, string[] Args) {
			if (Args.Length == 0) return;

			float Zoom = Math.Max(0, Single.Parse(Args[0]));
			Engine.SetWindowed((int) (Display.Width * Zoom), (int) (Display.Height * Zoom));
		}
	}
}