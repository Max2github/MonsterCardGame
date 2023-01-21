using System;
namespace MonsterCardGame.Card {
	internal interface ICardManager : DB.IDB<UniqueCard> {
        //int Count();
        UniqueCard? Get(Guid id);

        //bool Add(UniqueCard card);
    }
}

