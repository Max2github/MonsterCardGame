using System;
namespace MonsterCardGame.Helper {
	/**
	 * A list of Arguments
	 * This list must contain be filled correctly,
	 * or you probably have ugly errors.
	 * */
	internal class Arguments {
        private readonly List<object> _args = new List<object>();
		static public object invalidArg = new();

        public void Add(object arg) {
			this._args.Add(arg);
		}
		public Object Get(int index) {
			if (index >= this.Count()) { return Arguments.invalidArg; }
			return this._args[index];
        }
		public int Count() {
			return this._args.Count;
		}

		public static void Error_ArgCount(string msg) {
            Console.WriteLine("The number of passed arguments is not correct: " + msg);
        }
		public static void Error_Wrong_Type(string msg) {
			Console.WriteLine("The passed arguments do not have the correct type: " + msg);
        }
		public static void Error_Intern(string msg) {
            Console.WriteLine("An internal error occured: " + msg);
        }
		public bool IsValid() {
			if (this.Count() == 0) { return false; }
			return true;
        }
        public bool IsValid(int count, params Type[] types) {
			// not enough args
            if (this.Count() != count) { Error_ArgCount("provided: " + this.Count() + " needed: " + count);  return false; }

			// error: IsValid not called correctly
			if (types == null) { Error_Intern("IsValid: argument \"types\" is null!"); return false; }
			if (types.Count() != count) { Error_Intern("IsValid: The count of types passed does not match the count of arguments needed!"); return false; }

			// check if arguments are correct
			for (int i = 0; i < count; i++) {
				object temp = this.Get(i);
                if (temp.GetType() != types[i]) {
					// build string to explain correct types
					string correctTypes = "";
					int j = 0;
					foreach (var t in types) {
						correctTypes += $"[{j}] {t} ";
						j++;
					}
					// print error with correct types
					Error_Wrong_Type(correctTypes);
					return false;
				}
			}

            return true;
        }
    }
}

