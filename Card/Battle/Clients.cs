using System;

namespace MonsterCardGame.Card.Battle {
	internal class Lobby {
		private readonly Mutex _mutex = new();

		private HTTP.ClientHandler? _first = null;
		private HTTP.ClientHandler? _opponent = null;

		public void Join(HTTP.ClientHandler client) {
            this._mutex.WaitOne();

            if (this._first == null) {
                // add client + wait for opponent
                this._first = client;
                this._mutex.ReleaseMutex();

                this.WaitForOpponent();
                return;
			}
			else if (this._opponent == null) {
                // add opponent + start battle
                this._opponent = client;
                this._mutex.ReleaseMutex();

                this.StartBattle();
				return;
			}
			else { /* should never occur, reset this object */ this.Clear(); }
		}

        // private functions

        private
            /// wait until another client shows up
            void WaitForOpponent() {
				while(true) {
					Thread.Sleep(1000); // wait for a second
					if (this._first == null && this._opponent == null) { break; }
				}
                this.StartBattle();
			}
			
			void StartBattle() {
				// do something

				// reset the object
				this.Clear();
			}
			
			void Clear() {
				this._first = null;
				this._opponent = null;
				this._mutex.Close();
			} 
	}
}

