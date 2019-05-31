using Lens.input;
using Microsoft.Xna.Framework.Input;

namespace BurningKnight {
	public static class Controls {
		public const string Up = "up";
		public const string Left = "left";
		public const string Down = "down";
		public const string Right = "right";
		
		public const string Active = "active";
		public const string Use = "use";
		public const string Bomb = "bomb";
		public const string Interact = "interact";
		public const string Swap = "swap";

		public const string Roll = "roll";
		
		public const string Pause = "pause";

		public const string UiAccept = "ui_accept";
		public const string Cancel = "cancel";
		
		public const string Fullscreen = "fullscreen";
		public const string Mute = "mute";

		public static void Bind() {
			Input.Bind(Up, Keys.W, Keys.Up);
			Input.Bind(Left, Keys.A, Keys.Left);
			Input.Bind(Down, Keys.S, Keys.Down);
			Input.Bind(Right, Keys.D, Keys.Right);
			
			Input.Bind(Active, Keys.Space);
			Input.Bind(Active, Buttons.B);
			
			Input.Bind(Use, MouseButtons.Left);
			Input.Bind(Use, Buttons.RightTrigger);
			
			Input.Bind(Bomb, Keys.Q);
			Input.Bind(Bomb, Buttons.Y);

			Input.Bind(Interact, Keys.E);
			Input.Bind(Interact, Buttons.A);
			
			Input.Bind(Swap, Keys.LeftShift);
			Input.Bind(Swap, Buttons.X);

			Input.Bind(Roll, MouseButtons.Right);
			Input.Bind(Roll, Buttons.LeftTrigger);
			
			Input.Bind(Pause, Keys.Escape);
			Input.Bind(Pause, Buttons.Back);
			
			Input.Bind(UiAccept, MouseButtons.Left);
			
			Input.Bind("console", Keys.F1);
			
			Input.Bind(Mute, Keys.M);
			Input.Bind(Fullscreen, Keys.F11, Keys.F);
			
			Input.Bind(Cancel, Keys.Escape);
			Input.Bind(Cancel, Buttons.Back);
		}
	}
}