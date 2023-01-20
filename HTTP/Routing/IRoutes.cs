using System;
namespace MonsterCardGame.HTTP.Routing {
	internal interface IRoutes {
		protected struct routeObj {
            public readonly string name;
            public readonly IRoute route;

            public routeObj(string name, IRoute route) {
                this.name = name;
                this.route = route;
            }
        }
	}
}

