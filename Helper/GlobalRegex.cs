using System;
using System.Text.RegularExpressions;

namespace MonsterCardGame.Helper {
	static internal class GlobalRegex {
		public static readonly Regex user = new Regex("^[a-zA-Z0-9]*$");
	}
}

