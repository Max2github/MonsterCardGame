using System;
namespace MonsterCardGame.Card {
	internal interface ICardManager : DB.IDB<UniqueCard> {
        UniqueCard? Get(Guid id);
    }
}

