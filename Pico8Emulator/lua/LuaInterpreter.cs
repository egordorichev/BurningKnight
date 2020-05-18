namespace Pico8Emulator.lua {
	/// <summary>
	/// Defines the interface for a lua interpreter like Moonsharp or NLua. <see cref="LuaInterpreter" />
	/// </summary>
	public interface LuaInterpreter {
		/// <summary>
		/// Function to add a C# function to the lua interpreter so that lua code can call it.
		/// </summary>
		/// <param name="name">The name <see cref="string"/> of the function.</param>
		/// <param name="func">The function <see cref="object"/> to be added.</param>
		void AddFunction(string name, object func);

		/// <summary>
		/// Calls a function that is present in the interpreter's function list. Can call a non existing function, getting a nullptr exception.
		/// </summary>
		/// <param name="name">The name <see cref="string"/> of the function to be called.</param>
		void CallFunction(string name);

		/// <summary>
		/// Calls a function that is present in the interpreter's function list. Only called if the function exists.
		/// </summary>
		/// <param name="name">The name <see cref="string"/> of the function to be called.</param>
		/// <returns>True if the function exists, false otherwise.</returns>
		bool CallIfDefined(string name);

		/// <summary>
		/// Runs a lua script with the interpreter that implemented this interface.
		/// </summary>
		/// <param name="script">The script <see cref="string"/> to be ran.</param>
		void RunScript(string script);

		/// <summary>
		/// Checks if the function name given is defined inside the interpreter.
		/// </summary>
		/// <param name="name">The name <see cref="string"/> of the function to catch.</param>
		/// <returns>True if the function is defined, false otherwise.<see cref="bool"/>.</returns>
		bool IsDefined(string name);
	}
}
