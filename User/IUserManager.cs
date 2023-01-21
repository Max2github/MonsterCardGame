using System;
namespace MonsterCardGame.User {
	internal interface IUserManager : DB.IDB<User> {
		// int Count();
		User? Get(string username);
		User? GetByToken(string token);
		User? GetByToken(Credentials.Token token);

		// bool Add(User user);
		bool UpdateInfo(Info userInfo, string username);
	}
}

