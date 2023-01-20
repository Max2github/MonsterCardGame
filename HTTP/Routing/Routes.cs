using System.IO;
using System.Text.Json;

namespace MonsterCardGame {
    namespace HTTP.Routing {
        internal class Routes : IRoutes {
            internal struct route : IRoute {
                public string file;
                public string type;
                public bool requiredLogin;
                public bool redirect;

                public route() {
                    file = "";
                    type = "";
                    requiredLogin = false;
                    redirect = false;
                }

                public bool IsValid() {
                    if (this.file == "" || this.file == null) { return false; }
                    if (!this.redirect && (this.type == "" || this.type == null)) { return false; }
                    if (this.redirect && (this.type != "")) { return false; }
                    return true;
                }
                public bool IsRedirect() { return (redirect != false); }
                public void Print() {
                    Console.WriteLine("{ file: " + file + ", type: " + type + ", requiredLogin: " + requiredLogin + ", redirect: " + redirect + " }");
                }
            }
            /*
            private struct routeObj {
                public readonly string name;
                public readonly route route;

                public routeObj(string name, route route) {
                    this.name = name;
                    this.route = route;
                }
            }
            */
            public static readonly string Dir = Directory.GetCurrentDirectory() + "/";
            private List<IRoutes.routeObj> _routeObj = new();

            public Routes() {} // create dummy / invalid instance
            public Routes(string filePath) {
                string jsonStr = File.ReadAllText(Routes.Dir + filePath);
                JsonElement json = JsonDocument.Parse(jsonStr).RootElement;

                if (json.ValueKind != JsonValueKind.Object) { /* some error */ }

                foreach (var item in json.EnumerateObject()) {
                    route route = ParseRoute(item.Value);
                    IRoutes.routeObj obj = new IRoutes.routeObj(item.Name, route);
                    this._routeObj.Add(obj);
                }
            }

            private route ParseRoute(JsonElement routeEl) {
                route myroute;
                Helper.MyJsonObject myjson = new Helper.MyJsonObject(routeEl);

                // redirect
                if (routeEl.ValueKind == JsonValueKind.String) {
                    myroute = new route();

                    string? erg = routeEl.GetString();
                    if (erg != null) { myroute.file = erg; }
                    else { myroute.file = ""; }

                    myroute.redirect = true;
                    return myroute;
                }
                // invalid
                if (routeEl.ValueKind != JsonValueKind.Object) {
                    return new route();
                }

                // object
                myroute.redirect = false;
                myroute.file = myjson.Element("file", "");
                myroute.type = myjson.Element("type", "");
                myroute.requiredLogin = myjson.Element("requiredLogin", false);

                return myroute;
            }

            public route Get(string path) {
                route? res = (route?) this._routeObj.SingleOrDefault(x => x.name == path).route;
                if (res == null) { return new route(); } // return an invalid route
                return (route) res;
            }
        }
    }
}

