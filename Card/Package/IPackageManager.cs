using System;
namespace MonsterCardGame.Card.Package {
	internal interface IPackageManager : DB.IDB<Package> {
        // public Package? Pop();
        public Package? Buy(string username);
    }
}

