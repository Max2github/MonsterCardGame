using System;
namespace MonsterCardGame.DB {
    internal interface IDB<T> where T : notnull {
        int Count();
        // T? Get();
        bool Add(T card);
    }
}

