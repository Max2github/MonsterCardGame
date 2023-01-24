using System;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using MonsterCardGame.Helper;
using static MonsterCardGame.HTTP.Routing.Action.route;

namespace MonsterCardGame.HTTP.Routing {
    namespace Action {
        internal class Actions {
            /**
             * save a list of routes with their names / path
             * */
            private struct routeObj {
                public readonly string name;
                public readonly route route;

                public routeObj(string name, route route) {
                    this.name = name;
                    this.route = route;
                }
            }

            public static readonly string Dir = Directory.GetCurrentDirectory() + "/";
            private readonly List<routeObj> _routeObj = new();
            // private readonly User.Collection _userCollection;

            // public Actions() { } // create dummy / invalid instance
            public Actions(string filePath) {
                string jsonStr = File.ReadAllText(Routes.Dir + filePath);
                JsonElement json = JsonDocument.Parse(jsonStr).RootElement;

                if (json.ValueKind != JsonValueKind.Object) { /* some error */ }

                foreach (var item in json.EnumerateObject()) {
                    route route = ParseRoute(item.Value);
                    routeObj obj = new routeObj(item.Name, route);
                    this._routeObj.Add(obj);
                }
            }

            static private route ParseRoute(JsonElement routeEl) {
                route myroute = new route();
                MyJsonObject json = new MyJsonObject(routeEl);

                // redirect
                if (routeEl.ValueKind == JsonValueKind.String) {
                    string? erg = routeEl.GetString();
                    if (erg != null) {
                        myroute.redirect = true;
                        myroute.action = erg;
                    }
                    else {
                        myroute.redirect = false;
                    }

                    return myroute;
                }
                // invalid
                if (routeEl.ValueKind != JsonValueKind.Object) {
                    return myroute;
                }

                // object
                myroute.redirect = false;
                myroute.method = json.Element("method", "");

                MyJsonArray? schemas = json.ArrayElement("schema");
                if (schemas != null) {
                    foreach (var schema in schemas.Enumerate()) {
                        Schema.schemaEl neu = new Schema.schemaEl();
                        MyJsonObject schemaJson = new MyJsonObject(schema);

                        neu.name = schemaJson.Element("name", "");
                        neu.type = schemaJson.Element("type", "");

                        if (neu.name != "" && neu.type != "") { myroute.schema.Add(neu); }
                    }
                } else {
                    // leave schema empty
                }

                myroute.dataType = json.Element("dataType", "object");

                MyJsonArray? actionJson = json.ArrayElement("action");
                // myroute.action = (actionJson == null) ? "" : actionJson.Element(0, "");

                if (actionJson != null) {
                    bool first = true;
                    foreach (var arg in actionJson.Enumerate()) {
                        if (first) {
                            string? erg = arg.GetString();
                            myroute.action = (erg == null) ? "" : erg;
                        } else {
                            string? erg = arg.GetString();
                            if (erg != null && erg.Length >= 2) {
                                // build argId
                                actionArgId argId = new actionArgId();
                                argId.id = erg[0];
                                erg = erg.Substring(1);
                                argId.number = int.Parse(erg);
                                // add argId to list
                                myroute.actionArgIdList.Add(argId);
                            } else if (erg != null && erg.Length == 1) {
                                // may accept single id, for z.B. data 'd'
                            } else { }
                        }

                        first = false;
                    }
                }

                // myroute.requiredLogin = json.Element("requiredLogin", false);
                MyJsonArray? authJson = json.ArrayElement("auth");
                if (authJson != null) {
                    foreach (var arg in authJson.Enumerate()) {
                        string? erg = arg.GetString();
                        switch(erg) {
                            case "all":   { myroute.auth.AddFlags(Auth.Flags.Allow_all); break; }
                            case "own":   { myroute.auth.AddFlags(Auth.Flags.Allow_own_user); break; }
                            case "admin": { myroute.auth.AddFlags(Auth.Flags.Allow_admin); break; }
                            case "users": { myroute.auth.AddFlags(Auth.Flags.Allow_all_users); break; }
                        }
                    }
                }

                return myroute;
            }

            public route Get(string method, string path) {
                // route? res = this._routeObj.SingleOrDefault(x => (x.name == path && x.route.method == method)).route;
                route? res = this._routeObj.SingleOrDefault(x => (Regex.IsMatch(path, "^" + x.name + "$") && String.Compare(x.route.method, method, true) == 0)).route;
                if (res == null) { return new route(); } // return an invalid route
                return (route)res;
            }
        }
    }
}

