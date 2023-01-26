using System;
using MonsterCardGame.User;
using MonsterCardGame.Card;
using MonsterCardGame.Card.Package;
using MonsterCardGame.Card.Deck;
using MonsterCardGame.Card.Battle;

namespace MonsterCardGame {
	internal class SemiGlobal {
        public IUserManager userManager;
        public ICardManager cardManager;
        public IPackageManager packageManager;
        public IDeckManager deckManager;
        public Lobby battleLobby;

        public SemiGlobal(
            IUserManager userManager,
            ICardManager cardManager,
            IPackageManager packageManager,
            IDeckManager deckManager,
            Lobby lobby
        ) {
            this.userManager = userManager;
            this.cardManager = cardManager;
            this.packageManager = packageManager;
            this.deckManager = deckManager;
            this.battleLobby = lobby;
        }
    }
}

