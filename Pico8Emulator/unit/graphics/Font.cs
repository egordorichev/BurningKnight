using System.Collections.Generic;

namespace Pico8Emulator.unit.graphics {
	/*
	 * TODO: use hex instead of this
	 * 000 = 0x0
	 * 111 = 0x9
	 * etc
	 */
	public static class Font {
		public static Dictionary<char, byte[,]> dictionary;

		#region digit definitions
		private static byte[,] _empty = {
			{0, 0, 0},
			{0, 0, 0},
			{0, 0, 0},
			{0, 0, 0},
			{0, 0, 0},
		};

		private static byte[,] _exclamation = {
			{0, 1, 0},
			{0, 1, 0},
			{0, 1, 0},
			{0, 0, 0},
			{0, 1, 0},
		};

		private static byte[,] _quotes = {
			{1, 0, 1},
			{1, 0, 1},
			{0, 0, 0},
			{0, 0, 0},
			{0, 0, 0},
		};

		private static byte[,] _hashtag = {
			{1, 0, 1},
			{1, 1, 1},
			{1, 0, 1},
			{1, 1, 1},
			{1, 0, 1},
		};

		private static byte[,] _dolar = {
			{1, 1, 1},
			{1, 1, 0},
			{0, 1, 1},
			{1, 1, 1},
			{0, 1, 0},
		};

		private static byte[,] _percentage = {
			{1, 0, 1},
			{0, 0, 1},
			{0, 1, 0},
			{1, 0, 0},
			{1, 0, 1},
		};

		private static byte[,] _ampersand = {
			{1, 1, 0},
			{1, 1, 0},
			{1, 1, 0},
			{1, 0, 1},
			{1, 1, 1},
		};

		private static byte[,] _tone = {
			{0, 1, 0},
			{1, 0, 0},
			{0, 0, 0},
			{0, 0, 0},
			{0, 0, 0},
		};

		private static byte[,] _par_open = {
			{0, 1, 0},
			{1, 0, 0},
			{1, 0, 0},
			{1, 0, 0},
			{0, 1, 0},
		};

		private static byte[,] _par_close = {
			{0, 1, 0},
			{0, 0, 1},
			{0, 0, 1},
			{0, 0, 1},
			{0, 1, 0},
		};

		private static byte[,] _astherisc = {
			{1, 0, 1},
			{0, 1, 0},
			{1, 1, 1},
			{0, 1, 0},
			{1, 0, 1},
		};

		private static byte[,] _plus = {
			{0, 0, 0},
			{0, 1, 0},
			{1, 1, 1},
			{0, 1, 0},
			{0, 0, 0},
		};

		private static byte[,] _comma = {
			{0, 0, 0},
			{0, 0, 0},
			{0, 0, 0},
			{0, 1, 0},
			{1, 0, 0},
		};

		private static byte[,] _dash = {
			{0, 0, 0},
			{0, 0, 0},
			{1, 1, 1},
			{0, 0, 0},
			{0, 0, 0},
		};

		private static byte[,] _dot = {
			{0, 0, 0},
			{0, 0, 0},
			{0, 0, 0},
			{0, 0, 0},
			{0, 1, 0},
		};

		private static byte[,] _slash = {
			{0, 0, 1},
			{0, 1, 0},
			{0, 1, 0},
			{0, 1, 0},
			{1, 0, 0},
		};

		private static byte[,] _digit0 = {
			{1, 1, 1},
			{1, 0, 1},
			{1, 0, 1},
			{1, 0, 1},
			{1, 1, 1},
		};

		private static byte[,] _digit1 = {
			{1, 1, 0},
			{0, 1, 0},
			{0, 1, 0},
			{0, 1, 0},
			{1, 1, 1},
		};

		private static byte[,] _digit2 = {
			{1, 1, 1},
			{0, 0, 1},
			{1, 1, 1},
			{1, 0, 0},
			{1, 1, 1},
		};

		private static byte[,] _digit3 = {
			{1, 1, 1},
			{0, 0, 1},
			{0, 1, 1},
			{0, 0, 1},
			{1, 1, 1},
		};

		private static byte[,] _digit4 = {
			{1, 0, 1},
			{1, 0, 1},
			{1, 1, 1},
			{0, 0, 1},
			{0, 0, 1},
		};

		private static byte[,] _digit5 = {
			{1, 1, 1},
			{1, 0, 0},
			{1, 1, 1},
			{0, 0, 1},
			{1, 1, 1},
		};

		private static byte[,] _digit6 = {
			{1, 0, 0},
			{1, 0, 0},
			{1, 1, 1},
			{1, 0, 1},
			{1, 1, 1},
		};

		private static byte[,] _digit7 = {
			{1, 1, 1},
			{0, 0, 1},
			{0, 0, 1},
			{0, 0, 1},
			{0, 0, 1},
		};

		private static byte[,] _digit8 = {
			{1, 1, 1},
			{1, 0, 1},
			{1, 1, 1},
			{1, 0, 1},
			{1, 1, 1},
		};

		private static byte[,] _digit9 = {
			{1, 1, 1},
			{1, 0, 1},
			{1, 1, 1},
			{0, 0, 1},
			{0, 0, 1},
		};

		private static byte[,] _colon = {
			{0, 0, 0},
			{0, 1, 0},
			{0, 0, 0},
			{0, 1, 0},
			{0, 0, 0},
		};

		private static byte[,] _semicolon = {
			{0, 0, 0},
			{0, 1, 0},
			{0, 0, 0},
			{0, 1, 0},
			{1, 0, 0},
		};

		private static byte[,] _less = {
			{0, 0, 1},
			{0, 1, 0},
			{1, 0, 0},
			{0, 1, 0},
			{0, 0, 1},
		};

		private static byte[,] _equals = {
			{0, 0, 0},
			{1, 1, 1},
			{0, 0, 0},
			{1, 1, 1},
			{0, 0, 0},
		};

		private static byte[,] _greater = {
			{1, 0, 0},
			{0, 1, 0},
			{0, 0, 1},
			{0, 1, 0},
			{1, 0, 0},
		};

		private static byte[,] _question = {
			{1, 1, 1},
			{0, 0, 1},
			{0, 1, 1},
			{0, 0, 0},
			{0, 1, 0},
		};

		private static byte[,] _at = {
			{0, 1, 0},
			{1, 0, 1},
			{1, 0, 1},
			{1, 0, 0},
			{0, 1, 1},
		};

		private static byte[,] _a = {
			{0, 0, 0},
			{1, 1, 1},
			{1, 0, 1},
			{1, 1, 1},
			{1, 0, 1},
		};

		private static byte[,] _b = {
			{0, 0, 0},
			{1, 1, 0},
			{1, 1, 0},
			{1, 0, 1},
			{1, 1, 1},
		};

		private static byte[,] _c = {
			{0, 0, 0},
			{1, 1, 1},
			{1, 0, 0},
			{1, 0, 0},
			{1, 1, 1},
		};

		private static byte[,] _d = {
			{0, 0, 0},
			{1, 1, 0},
			{1, 0, 1},
			{1, 0, 1},
			{1, 1, 0},
		};

		private static byte[,] _e = {
			{0, 0, 0},
			{1, 1, 1},
			{1, 1, 0},
			{1, 0, 0},
			{1, 1, 1},
		};

		private static byte[,] _f = {
			{0, 0, 0},
			{1, 1, 1},
			{1, 1, 0},
			{1, 0, 0},
			{1, 0, 0},
		};

		private static byte[,] _g = {
			{0, 0, 0},
			{1, 1, 1},
			{1, 0, 0},
			{1, 0, 1},
			{1, 1, 1},
		};

		private static byte[,] _h = {
			{0, 0, 0},
			{1, 0, 1},
			{1, 0, 1},
			{1, 1, 1},
			{1, 0, 1},
		};

		private static byte[,] _i = {
			{0, 0, 0},
			{1, 1, 1},
			{0, 1, 0},
			{0, 1, 0},
			{1, 1, 1},
		};

		private static byte[,] _j = {
			{0, 0, 0},
			{1, 1, 1},
			{0, 1, 0},
			{0, 1, 0},
			{1, 1, 0},
		};

		private static byte[,] _k = {
			{0, 0, 0},
			{1, 0, 1},
			{1, 1, 0},
			{1, 0, 1},
			{1, 0, 1},
		};

		private static byte[,] _l = {
			{0, 0, 0},
			{1, 0, 0},
			{1, 0, 0},
			{1, 0, 0},
			{1, 1, 1},
		};

		private static byte[,] _m = {
			{0, 0, 0},
			{1, 1, 1},
			{1, 1, 1},
			{1, 0, 1},
			{1, 0, 1},
		};

		private static byte[,] _n = {
			{0, 0, 0},
			{1, 1, 0},
			{1, 0, 1},
			{1, 0, 1},
			{1, 0, 1},
		};

		private static byte[,] _o = {
			{0, 0, 0},
			{0, 1, 1},
			{1, 0, 1},
			{1, 0, 1},
			{1, 1, 0},
		};

		private static byte[,] _p = {
			{0, 0, 0},
			{1, 1, 1},
			{1, 0, 1},
			{1, 1, 1},
			{1, 0, 0},
		};

		private static byte[,] _q = {
			{0, 0, 0},
			{0, 1, 0},
			{1, 0, 1},
			{1, 1, 0},
			{0, 1, 1},
		};

		private static byte[,] _r = {
			{0, 0, 0},
			{1, 1, 1},
			{1, 0, 1},
			{1, 1, 0},
			{1, 0, 1},
		};

		private static byte[,] _s = {
			{0, 0, 0},
			{0, 1, 1},
			{1, 0, 0},
			{0, 0, 1},
			{1, 1, 0},
		};

		private static byte[,] _t = {
			{0, 0, 0},
			{1, 1, 1},
			{0, 1, 0},
			{0, 1, 0},
			{0, 1, 0},
		};

		private static byte[,] _u = {
			{0, 0, 0},
			{1, 0, 1},
			{1, 0, 1},
			{1, 0, 1},
			{0, 1, 1},
		};

		private static byte[,] _v = {
			{0, 0, 0},
			{1, 0, 1},
			{1, 0, 1},
			{1, 1, 1},
			{0, 1, 0},
		};

		private static byte[,] _w = {
			{0, 0, 0},
			{1, 0, 1},
			{1, 0, 1},
			{1, 1, 1},
			{1, 1, 1},
		};

		private static byte[,] _x = {
			{0, 0, 0},
			{1, 0, 1},
			{0, 1, 0},
			{1, 0, 1},
			{1, 0, 1},
		};

		private static byte[,] _y = {
			{0, 0, 0},
			{1, 0, 1},
			{1, 1, 1},
			{0, 0, 1},
			{1, 1, 1},
		};

		private static byte[,] _z = {
			{0, 0, 0},
			{1, 1, 1},
			{0, 0, 1},
			{1, 0, 0},
			{1, 1, 1},
		};

		private static byte[,] _bracket_open = {
			{1, 1, 0},
			{1, 0, 0},
			{1, 0, 0},
			{1, 0, 0},
			{1, 1, 0},
		};

		private static byte[,] _backslash = {
			{1, 0, 0},
			{0, 1, 0},
			{0, 1, 0},
			{0, 1, 0},
			{0, 0, 1},
		};

		private static byte[,] _bracket_close = {
			{0, 1, 1},
			{0, 0, 1},
			{0, 0, 1},
			{0, 0, 1},
			{0, 1, 1},
		};

		private static byte[,] _carat = {
			{0, 1, 0},
			{1, 0, 1},
			{0, 0, 0},
			{0, 0, 0},
			{0, 0, 0},
		};

		private static byte[,] _underscore = {
			{0, 0, 0},
			{0, 0, 0},
			{0, 0, 0},
			{0, 0, 0},
			{1, 1, 1},
		};

		private static byte[,] _back_quote = {
			{0, 1, 0},
			{0, 0, 1},
			{0, 0, 0},
			{0, 0, 0},
			{0, 0, 0},
		};

		private static byte[,] _A = {
			{1, 1, 1},
			{1, 0, 1},
			{1, 1, 1},
			{1, 0, 1},
			{1, 0, 1},
		};

		private static byte[,] _B = {
			{1, 1, 1},
			{1, 0, 1},
			{1, 1, 0},
			{1, 0, 1},
			{1, 1, 1},
		};

		private static byte[,] _C = {
			{0, 1, 1},
			{1, 0, 0},
			{1, 0, 0},
			{1, 0, 0},
			{0, 1, 1},
		};

		private static byte[,] _D = {
			{1, 1, 0},
			{1, 0, 1},
			{1, 0, 1},
			{1, 0, 1},
			{1, 1, 1},
		};

		private static byte[,] _E = {
			{1, 1, 1},
			{1, 0, 0},
			{1, 1, 0},
			{1, 0, 0},
			{1, 1, 1},
		};

		private static byte[,] _F = {
			{1, 1, 1},
			{1, 0, 0},
			{1, 1, 0},
			{1, 0, 0},
			{1, 0, 0},
		};

		private static byte[,] _G = {
			{0, 1, 1},
			{1, 0, 0},
			{1, 0, 0},
			{1, 0, 1},
			{1, 1, 1},
		};

		private static byte[,] _H = {
			{1, 0, 1},
			{1, 0, 1},
			{1, 1, 1},
			{1, 0, 1},
			{1, 0, 1},
		};

		private static byte[,] _I = {
			{1, 1, 1},
			{0, 1, 0},
			{0, 1, 0},
			{0, 1, 0},
			{1, 1, 1},
		};

		private static byte[,] J = {
			{1, 1, 1},
			{0, 1, 0},
			{0, 1, 0},
			{0, 1, 0},
			{1, 1, 0},
		};

		private static byte[,] _K = {
			{1, 0, 1},
			{1, 0, 1},
			{1, 1, 0},
			{1, 0, 1},
			{1, 0, 1},
		};

		private static byte[,] _L = {
			{1, 0, 0},
			{1, 0, 0},
			{1, 0, 0},
			{1, 0, 0},
			{1, 1, 1},
		};

		private static byte[,] _M = {
			{1, 1, 1},
			{1, 1, 1},
			{1, 0, 1},
			{1, 0, 1},
			{1, 0, 1},
		};

		private static byte[,] _N = {
			{1, 1, 0},
			{1, 0, 1},
			{1, 0, 1},
			{1, 0, 1},
			{1, 0, 1},
		};

		private static byte[,] _O = {
			{0, 1, 1},
			{1, 0, 1},
			{1, 0, 1},
			{1, 0, 1},
			{1, 1, 0},
		};

		private static byte[,] _P = {
			{1, 1, 1},
			{1, 0, 1},
			{1, 1, 1},
			{1, 0, 0},
			{1, 0, 0},
		};

		private static byte[,] _Q = {
			{0, 1, 0},
			{1, 0, 1},
			{1, 0, 1},
			{1, 1, 0},
			{0, 1, 1},
		};

		private static byte[,] _R = {
			{1, 1, 1},
			{1, 0, 1},
			{1, 1, 0},
			{1, 0, 1},
			{1, 0, 1},
		};

		private static byte[,] _S = {
			{0, 1, 1},
			{1, 0, 0},
			{1, 1, 1},
			{0, 0, 1},
			{1, 1, 0},
		};

		private static byte[,] _T = {
			{1, 1, 1},
			{0, 1, 0},
			{0, 1, 0},
			{0, 1, 0},
			{0, 1, 0},
		};

		private static byte[,] _U = {
			{1, 0, 1},
			{1, 0, 1},
			{1, 0, 1},
			{1, 0, 1},
			{0, 1, 1},
		};

		private static byte[,] _V = {
			{1, 0, 1},
			{1, 0, 1},
			{1, 0, 1},
			{1, 0, 1},
			{0, 1, 0},
		};

		private static byte[,] _W = {
			{1, 0, 1},
			{1, 0, 1},
			{1, 0, 1},
			{1, 1, 1},
			{1, 1, 1},
		};

		private static byte[,] _X = {
			{1, 0, 1},
			{1, 0, 1},
			{0, 1, 0},
			{1, 0, 1},
			{1, 0, 1},
		};

		private static byte[,] _Y = {
			{1, 0, 1},
			{1, 0, 1},
			{1, 1, 1},
			{0, 0, 1},
			{1, 1, 1},
		};

		private static byte[,] _Z = {
			{1, 1, 1},
			{0, 0, 1},
			{0, 1, 0},
			{1, 0, 0},
			{1, 1, 1},
		};

		private static byte[,] _brace_open = {
			{0, 1, 1},
			{0, 1, 0},
			{1, 1, 0},
			{0, 1, 0},
			{0, 1, 1},
		};

		private static byte[,] _pipe = {
			{0, 1, 0},
			{0, 1, 0},
			{0, 1, 0},
			{0, 1, 0},
			{0, 1, 0},
		};

		private static byte[,] _brace_close = {
			{1, 1, 0},
			{0, 1, 0},
			{0, 1, 1},
			{0, 1, 0},
			{1, 1, 0},
		};

		private static byte[,] _tilde = {
			{0, 0, 0},
			{0, 0, 1},
			{1, 1, 1},
			{1, 0, 0},
			{0, 0, 0},
		};

		private static byte[,] _nubbin = {
			{0, 0, 0},
			{0, 1, 0},
			{1, 0, 1},
			{1, 0, 1},
			{1, 1, 1},
		};

		private static byte[,] _block = {
			{1, 1, 1, 1, 1, 1, 1},
			{1, 1, 1, 1, 1, 1, 1},
			{1, 1, 1, 1, 1, 1, 1},
			{1, 1, 1, 1, 1, 1, 1},
			{1, 1, 1, 1, 1, 1, 1},
		};

		private static byte[,] _semi_block = {
			{1, 0, 1, 0, 1, 0, 1},
			{0, 1, 0, 1, 0, 1, 0},
			{1, 0, 1, 0, 1, 0, 1},
			{0, 1, 0, 1, 0, 1, 0},
			{1, 0, 1, 0, 1, 0, 1},
		};

		private static byte[,] _alien = {
			{1, 0, 0, 0, 0, 0, 1},
			{1, 1, 1, 1, 1, 1, 1},
			{1, 0, 1, 1, 1, 0, 1},
			{1, 0, 1, 1, 1, 0, 1},
			{0, 1, 1, 1, 1, 1, 0},
		};

		private static byte[,] _downbutton = {
			{0, 1, 1, 1, 1, 1, 0},
			{1, 1, 0, 0, 0, 1, 1},
			{1, 1, 0, 0, 0, 1, 1},
			{1, 1, 1, 0, 1, 1, 1},
			{0, 1, 1, 1, 1, 1, 0},
		};

		private static byte[,] _quasi_block = {
			{1, 0, 0, 0, 1, 0, 0},
			{0, 0, 1, 0, 0, 0, 1},
			{1, 0, 0, 0, 1, 0, 0},
			{0, 0, 1, 0, 0, 0, 1},
			{1, 0, 0, 0, 1, 0, 0},
		};

		private static byte[,] _shuriken = {
			{0, 0, 1, 0, 0, 0, 0},
			{0, 0, 1, 1, 1, 1, 0},
			{0, 0, 1, 1, 1, 0, 0},
			{0, 1, 1, 1, 1, 0, 0},
			{0, 0, 0, 0, 1, 0, 0},
		};

		private static byte[,] _shiny_ball = {
			{0, 0, 1, 1, 1, 0, 0},
			{0, 1, 1, 1, 0, 1, 0},
			{0, 1, 1, 1, 1, 1, 0},
			{0, 1, 1, 1, 1, 1, 0},
			{0, 0, 1, 1, 1, 0, 0},
		};

		private static byte[,] _heart = {
			{0, 1, 1, 0, 1, 1, 0},
			{0, 1, 1, 1, 1, 1, 0},
			{0, 1, 1, 1, 1, 1, 0},
			{0, 0, 1, 1, 1, 0, 0},
			{0, 0, 0, 1, 0, 0, 0},
		};

		private static byte[,] _sauron = {
			{0, 0, 1, 1, 1, 0, 0},
			{0, 1, 1, 0, 1, 1, 0},
			{1, 1, 1, 0, 1, 1, 1},
			{0, 1, 1, 0, 1, 1, 0},
			{0, 0, 1, 1, 1, 0, 0},
		};

		private static byte[,] _human = {
			{0, 0, 1, 1, 1, 0, 0},
			{0, 0, 1, 1, 1, 0, 0},
			{0, 1, 1, 1, 1, 1, 0},
			{0, 0, 1, 1, 1, 0, 0},
			{0, 0, 1, 0, 1, 0, 0},
		};

		private static byte[,] _house = {
			{0, 0, 1, 1, 1, 0, 0},
			{0, 1, 1, 1, 1, 1, 0},
			{1, 1, 1, 1, 1, 1, 1},
			{0, 1, 0, 1, 0, 1, 0},
			{0, 1, 0, 1, 1, 1, 0},
		};

		private static byte[,] _leftbutton = {
			{0, 1, 1, 1, 1, 1, 0},
			{1, 1, 1, 0, 0, 1, 1},
			{1, 1, 0, 0, 0, 1, 1},
			{1, 1, 1, 0, 0, 1, 1},
			{0, 1, 1, 1, 1, 1, 0},
		};

		private static byte[,] _face = {
			{1, 1, 1, 1, 1, 1, 1},
			{1, 0, 1, 1, 1, 0, 1},
			{1, 1, 1, 1, 1, 1, 1},
			{1, 0, 0, 0, 0, 0, 1},
			{1, 1, 1, 1, 1, 1, 1},
		};

		private static byte[,] _note = {
			{0, 0, 0, 1, 1, 1, 0},
			{0, 0, 0, 1, 0, 0, 0},
			{0, 0, 0, 1, 0, 0, 0},
			{0, 1, 1, 1, 0, 0, 0},
			{0, 1, 1, 1, 0, 0, 0},
		};

		private static byte[,] _obutton = {
			{0, 1, 1, 1, 1, 1, 0},
			{1, 1, 0, 0, 0, 1, 1},
			{1, 1, 0, 1, 0, 1, 1},
			{1, 1, 0, 0, 0, 1, 1},
			{0, 1, 1, 1, 1, 1, 0},
		};

		private static byte[,] _diamond = {
			{0, 0, 0, 1, 0, 0, 0},
			{0, 0, 1, 1, 1, 0, 0},
			{0, 1, 1, 1, 1, 1, 0},
			{0, 0, 1, 1, 1, 0, 0},
			{0, 0, 0, 1, 0, 0, 0},
		};

		private static byte[,] _dot_line = {
			{0, 0, 0, 0, 0, 0, 0},
			{0, 0, 0, 0, 0, 0, 0},
			{1, 0, 1, 0, 1, 0, 1},
			{0, 0, 0, 0, 0, 0, 0},
			{0, 0, 0, 0, 0, 0, 0},
		};

		private static byte[,] _rightbutton = {
			{0, 1, 1, 1, 1, 1, 0},
			{1, 1, 0, 0, 1, 1, 1},
			{1, 1, 0, 0, 0, 1, 1},
			{1, 1, 0, 0, 1, 1, 1},
			{0, 1, 1, 1, 1, 1, 0},
		};

		private static byte[,] _star = {
			{0, 0, 0, 1, 0, 0, 0},
			{0, 0, 1, 1, 1, 0, 0},
			{1, 1, 1, 1, 1, 1, 1},
			{0, 1, 1, 1, 1, 1, 0},
			{0, 1, 0, 0, 0, 1, 0},
		};

		private static byte[,] _hourclass = {
			{0, 1, 1, 1, 1, 1, 0},
			{0, 0, 1, 1, 1, 0, 0},
			{0, 0, 0, 1, 0, 0, 0},
			{0, 0, 1, 1, 1, 0, 0},
			{0, 1, 1, 1, 1, 1, 0},
		};

		private static byte[,] _upbutton = {
			{0, 1, 1, 1, 1, 1, 0},
			{1, 1, 1, 0, 1, 1, 1},
			{1, 1, 0, 0, 0, 1, 1},
			{1, 1, 0, 0, 0, 1, 1},
			{0, 1, 1, 1, 1, 1, 0},
		};

		private static byte[,] _down_arrows = {
			{0, 0, 0, 0, 0, 0, 0},
			{1, 0, 1, 0, 0, 0, 0},
			{0, 1, 0, 0, 1, 0, 1},
			{0, 0, 0, 0, 0, 1, 0},
			{0, 0, 0, 0, 0, 0, 0},
		};

		private static byte[,] _triangle_wave = {
			{0, 0, 0, 0, 0, 0, 0},
			{1, 0, 0, 0, 1, 0, 0},
			{0, 1, 0, 1, 0, 1, 0},
			{0, 0, 1, 0, 0, 0, 1},
			{0, 0, 0, 0, 0, 0, 0},
		};

		private static byte[,] _xbutton = {
			{0, 1, 1, 1, 1, 1, 0},
			{1, 1, 0, 1, 0, 1, 1},
			{1, 1, 1, 0, 1, 1, 1},
			{1, 1, 0, 1, 0, 1, 1},
			{0, 1, 1, 1, 1, 1, 0},
		};

		private static byte[,] _horizontal_lines = {
			{1, 1, 1, 1, 1, 1, 1},
			{0, 0, 0, 0, 0, 0, 0},
			{1, 1, 1, 1, 1, 1, 1},
			{0, 0, 0, 0, 0, 0, 0},
			{1, 1, 1, 1, 1, 1, 1},
		};

		private static byte[,] _vertical_lines = {
			{1, 0, 1, 0, 1, 0, 1},
			{1, 0, 1, 0, 1, 0, 1},
			{1, 0, 1, 0, 1, 0, 1},
			{1, 0, 1, 0, 1, 0, 1},
			{1, 0, 1, 0, 1, 0, 1},
		};
		#endregion

		static Font() {
			dictionary = new Dictionary<char, byte[,]>();
			dictionary.Add(' ', _empty);
			dictionary.Add('!', _exclamation);
			dictionary.Add('"', _quotes);
			dictionary.Add('#', _hashtag);
			dictionary.Add('$', _dolar);
			dictionary.Add('%', _percentage);
			dictionary.Add('&', _ampersand);
			dictionary.Add('\'', _tone);
			dictionary.Add('(', _par_open);
			dictionary.Add(')', _par_close);
			dictionary.Add('*', _astherisc);
			dictionary.Add('+', _plus);
			dictionary.Add(',', _comma);
			dictionary.Add('-', _dash);
			dictionary.Add('.', _dot);
			dictionary.Add('/', _slash);
			dictionary.Add('0', _digit0);
			dictionary.Add('1', _digit1);
			dictionary.Add('2', _digit2);
			dictionary.Add('3', _digit3);
			dictionary.Add('4', _digit4);
			dictionary.Add('5', _digit5);
			dictionary.Add('6', _digit6);
			dictionary.Add('7', _digit7);
			dictionary.Add('8', _digit8);
			dictionary.Add('9', _digit9);
			dictionary.Add(':', _colon);
			dictionary.Add(';', _semicolon);
			dictionary.Add('<', _less);
			dictionary.Add('=', _equals);
			dictionary.Add('>', _greater);
			dictionary.Add('?', _question);
			dictionary.Add('@', _at);
			dictionary.Add('a', _a);
			dictionary.Add('b', _b);
			dictionary.Add('c', _c);
			dictionary.Add('d', _d);
			dictionary.Add('e', _e);
			dictionary.Add('f', _f);
			dictionary.Add('g', _g);
			dictionary.Add('h', _h);
			dictionary.Add('i', _i);
			dictionary.Add('j', _j);
			dictionary.Add('k', _k);
			dictionary.Add('l', _l);
			dictionary.Add('m', _m);
			dictionary.Add('n', _n);
			dictionary.Add('o', _o);
			dictionary.Add('p', _p);
			dictionary.Add('q', _q);
			dictionary.Add('r', _r);
			dictionary.Add('s', _s);
			dictionary.Add('t', _t);
			dictionary.Add('u', _u);
			dictionary.Add('v', _v);
			dictionary.Add('w', _w);
			dictionary.Add('x', _x);
			dictionary.Add('y', _y);
			dictionary.Add('z', _z);
			dictionary.Add('[', _bracket_open);
			dictionary.Add('\\', _backslash);
			dictionary.Add(']', _bracket_close);
			dictionary.Add('^', _carat);
			dictionary.Add('_', _underscore);
			dictionary.Add('`', _back_quote);
			dictionary.Add('A', _A);
			dictionary.Add('B', _B);
			dictionary.Add('C', _C);
			dictionary.Add('D', _D);
			dictionary.Add('E', _E);
			dictionary.Add('F', _F);
			dictionary.Add('G', _G);
			dictionary.Add('H', _H);
			dictionary.Add('I', _I);
			dictionary.Add('J', J);
			dictionary.Add('K', _K);
			dictionary.Add('L', _L);
			dictionary.Add('M', _M);
			dictionary.Add('N', _N);
			dictionary.Add('O', _O);
			dictionary.Add('P', _P);
			dictionary.Add('Q', _Q);
			dictionary.Add('R', _R);
			dictionary.Add('S', _S);
			dictionary.Add('T', _T);
			dictionary.Add('U', _U);
			dictionary.Add('V', _V);
			dictionary.Add('W', _W);
			dictionary.Add('X', _X);
			dictionary.Add('Y', _Y);
			dictionary.Add('Z', _Z);
			dictionary.Add('{', _brace_open);
			dictionary.Add('|', _pipe);
			dictionary.Add('}', _brace_close);
			dictionary.Add('~', _tilde);
			dictionary.Add((char)127, _nubbin);
			dictionary.Add((char)128, _block);
			dictionary.Add((char)129, _semi_block);
			dictionary.Add((char)130, _alien);
			dictionary.Add((char)131, _downbutton);
			dictionary.Add((char)132, _quasi_block);
			dictionary.Add((char)133, _shuriken);
			dictionary.Add((char)134, _shiny_ball);
			dictionary.Add((char)135, _heart);
			dictionary.Add((char)136, _sauron);
			dictionary.Add((char)137, _human);
			dictionary.Add((char)138, _house);
			dictionary.Add((char)139, _leftbutton);
			dictionary.Add((char)140, _face);
			dictionary.Add((char)141, _note);
			dictionary.Add((char)142, _obutton);
			dictionary.Add((char)143, _diamond);
			dictionary.Add((char)144, _dot_line);
			dictionary.Add((char)145, _rightbutton);
			dictionary.Add((char)146, _star);
			dictionary.Add((char)147, _hourclass);
			dictionary.Add((char)148, _upbutton);
			dictionary.Add((char)149, _down_arrows);
			dictionary.Add((char)150, _triangle_wave);
			dictionary.Add((char)151, _xbutton);
			dictionary.Add((char)152, _horizontal_lines);
			dictionary.Add((char)153, _vertical_lines);
		}
	}
}