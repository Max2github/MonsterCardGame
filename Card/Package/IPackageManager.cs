using System;
using static MonsterCardGame.Card.Package.PackageDB;

namespace MonsterCardGame.Card.Package {
	internal interface IPackageManager : DB.IDB<Package> {
        // public Package? Pop();
        public Package? Buy(string username);
        public PackageWithID? GetWithID(string? username = null);
        public Package? Get(string? username = null);
        internal int? GetIdByCard(UniqueCard card);
        internal string GetOwner(int id);
    }
}

