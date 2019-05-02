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
		
		public static void Bind() {
			Input.Bind(Up, Keys.W);
			Input.Bind(Left, Keys.A);
			Input.Bind(Down, Keys.S);
			Input.Bind(Right, Keys.D);
			
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
		}
	}
}