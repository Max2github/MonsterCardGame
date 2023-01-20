using System;
namespace MonsterCardGame.User {
	internal interface IUserManager {
		int Count();
		User? Get(string username);
		User? GetByToken(string token);
		User? GetByToken(Credentials.Token token);

		bool Add(User user);
		bool UpdateInfo(Info userInfo, string username);
	}
}

