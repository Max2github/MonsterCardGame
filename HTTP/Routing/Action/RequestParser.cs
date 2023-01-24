using System;
using System.Text;
using MonsterCardGame.Helper;

namespace MonsterCardGame.HTTP.Routing.Action {
	internal class RequestParser {
		private readonly Request _request;
		private readonly route _route;

		public RequestParser(Request request, route r) {
			this._request = request;
			this._route = r;
		}

		private MyJson? GetJson() {
            if (this._request.Data == null) { return null; } // no data

            string dataAsString = Encoding.UTF8.GetString(this._request.Data);
			return RequestParser.GetJson(dataAsString, this._route.dataType);
        }
		private static MyJson? GetJson(string data, string dataType = "object") {
            MyJson json;
            if (string.Compare(dataType, "array", true) == 0) {
                // special: as array
                json = new MyJsonArray(data);
            } else {
                // default: as object
                json = new MyJsonObject(data);
            }
            return json;
        }

		private bool AddArg_data(Arguments arguments) {
			MyJson? json = this.GetJson();
			if (json == null) { return false; }

			return RequestParser.AddArg_data(json, arguments);
		}
        private static bool AddArg_data(MyJson json, Arguments arguments) {
            arguments.Add(json);
            return true;
        }

        private bool AddArg_schema(int number, Arguments arguments) {
            if (this._request.Data == null) { return false; } // no data

			// get json from data
			MyJson json = this.GetJson()!; // we already checked for "data == null" above

			// get the correct part of the schema
			Schema.schemaEl s = this._route.schema.Get(number);
			if (!s.IsValid()) { return false; } // schema invalid?

			if (json is MyJsonArray) { s.AddArg((MyJsonArray)json, arguments); }
			else if (json is MyJsonObject) { s.AddArg((MyJsonObject)json, arguments); }
			else { return false; }

            return true; ;
        }

		private bool AddArg_path(int number, Arguments arguments) {
			String[] pathParts = this._request.Path.Split("/");
			if (number < 0 || number > pathParts.Length) { return false; }

			arguments.Add(pathParts[number]);
			return true;
		}

		public bool AddArg(route.actionArgId argId, Arguments arguments) {
			switch (argId.id) {
				case 's': { return this.AddArg_schema(argId.number, arguments); }
				case 'p': { return this.AddArg_path(argId.number, arguments); }
                case 'd': { return this.AddArg_data(arguments); }

                default: { return false; }
			}
		}

		public bool IsValid() {
			return this._route.IsValid();
		}
	}
}

