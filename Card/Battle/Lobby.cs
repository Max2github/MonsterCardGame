using System;
using MonsterCardGame.Card.Deck;

namespace MonsterCardGame.Card.Battle {
	internal class Lobby {
		private readonly Mutex _mutex = new();

		private string? _first = null;
		private string? _opponent = null;
		private int _result = 2; // 0 -> nobody won, 1 -> first won, -1 -> opponent won, 2 -> unset

		private readonly IDeckManager _deckManager;

		public Lobby(IDeckManager deckManager) {
			this._deckManager = deckManager;
		}

		public void Join(string username) {
            this._mutex.WaitOne();

            if (this._first == null) {
                // add client + wait for opponent
                this._first = username;
                this._mutex.ReleaseMutex();

                this.WaitForOpponent();
                return;
			}
			else if (this._opponent == null) {
                // add opponent + start battle
                this._opponent = username;

                this.StartBattle();
                this._mutex.ReleaseMutex();
                return;
			}
			else { /* should never occur, reset this object */ this.Clear(); }
		}

        // private functions

        /// wait until another client shows up
        private void WaitForOpponent() {
			while(true) {
				Thread.Sleep(1000); // wait for a second
				if (this._first == null && this._opponent == null) { break; }
			}
			while(this._result == 2) {}
			// return this._result
		}

        private void StartBattle() {
			// do something

			Deck.Deck? deck1 = this._deckManager.Get(this._first!);
            Deck.Deck? deck2 = this._deckManager.Get(this._opponent!);
			if (deck1 is null || deck2 is null) { this._result = 0; this.Clear(); return; } // this would be a strange bug, if it were to happen

            var battle = new Battle(deck1, deck2);
			this._result = battle.Start();

			// reset the object
			this.Clear();
		}
			
			void Clear() {
				this._first = null;
				this._opponent = null;
				this._result = 2;
				this._mutex.Close();
			} 
	}
}

