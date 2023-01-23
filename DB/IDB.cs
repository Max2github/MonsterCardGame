using System;
namespace MonsterCardGame.DB {
    internal interface IDB<T> where T : notnull {
        int Count();
        bool Add(T card);
    }
}

