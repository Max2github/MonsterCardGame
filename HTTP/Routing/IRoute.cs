using System;
namespace MonsterCardGame.HTTP.Routing {
	public interface IRoute : Helper.IValid {
		bool IsRedirect();
		void Print();
	}
}

