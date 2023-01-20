using System;
namespace MonsterCardGame.HTTP.Routing.Action {
	internal class route : IRoute {
        internal struct actionArgId {
            public char id;
            public int number;
        }

        public string action;
        public List<actionArgId> actionArgIdList;
        // public bool requiredLogin;
        public Auth auth;
        public bool redirect;
        public string method;

        public Schema schema;
        
        public route() {
            // general info
            // requiredLogin = false;
            auth = new Auth();
            redirect = false;
            method = "";

            // action (+ schema & args for the action)
            action = "";
            schema = new();
            actionArgIdList = new();
        }

        public void Print() {
            Console.WriteLine("{");
            Console.WriteLine("    method: " + this.method + ",");
            Console.WriteLine("    action: " + this.action + ",");
            Console.WriteLine("    schema: " + this.schema + ",");
            // Console.WriteLine("    requiredLogin: " + this.requiredLogin + ",");
            Console.WriteLine("   auth: " + this.auth);
            Console.WriteLine("    redirect: " + this.redirect);
            Console.WriteLine("}");
        }

        public bool IsRedirect() {
            return (redirect != false);
        }
        public bool IsValid() {
            return !(/*action == null &&*/!this.IsRedirect() && action == "" && method == "" && (schema == null || schema.Count() == 0));
        }
	}
}

