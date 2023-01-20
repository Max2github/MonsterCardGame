using System;
using System.Text;

namespace MonsterCardGame.HTTP.Routing.Action {
	internal class RequestParser {
		private readonly Request _request;
		private readonly route _route;

		public RequestParser(Request request, route r) {
			this._request = request;
			this._route = r;
		}

		private bool AddArg_schema(int number, Helper.Arguments arguments) {
            if (this._request.Data == null) { return false; } // no data

			// get json from data
            Helper.MyJsonObject json = new Helper.MyJsonObject(Encoding.UTF8.GetString(this._request.Data));

			// get the correct part of the schema
			Schema.schemaEl s = this._route.schema.Get(number);
			if (!s.IsValid()) { return false; } // schema invalid?

			s.AddArg(json, arguments);
			return true; ;
        }

		private bool AddArg_path(int number, Helper.Arguments arguments) {
			String[] pathParts = this._request.Path.Split("/");
			if (number < 0 || number > pathParts.Length) { return false; }

			arguments.Add(pathParts[number]);
			return true;
		}

		public bool AddArg(route.actionArgId argId, Helper.Arguments arguments) {
			switch (argId.id) {
				case 's': { return this.AddArg_schema(argId.number, arguments); }
				case 'p': { return this.AddArg_path(argId.number, arguments); }

				default: { return false; }
			}
		}

		public bool IsValid() {
			return this._route.IsValid();
		}
	}
}

