using System;
namespace MonsterCardGame.Helper {
	public struct Return<T> : IValid {
		public T Value { get; }
		public bool Err { get; }
		public string Msg { get; }

		public Return(T val, bool err = false, string msg = "") {
			this.Value = val;
			this.Err = err;
			this.Msg = msg;
		}

		public bool IsValid() {
			return this.Err;
		}
	}
}