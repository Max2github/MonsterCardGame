using System;
namespace MonsterCardGame.Card {
	internal interface ICardManager {
        int Count();
        UniqueCard? Get();

        bool Add(UniqueCard card);
    }
}

