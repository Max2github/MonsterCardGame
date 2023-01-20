using System;
using System.Collections;
using System.Text.Json;
using static MonsterCardGame.HTTP.Routing.Action.Schema;

namespace MonsterCardGame.HTTP.Routing.Action {
	internal class Schema {

        // (sub-)structs

        public struct schemaEl {
            public string name;
            public string type;

            public schemaEl() {
                name = "";
                type = "";
            }
            public bool IsValid() {
                return !(this.name == "" || this.type == "");
            }

            public void AddArg(Helper.MyJsonObject json, Helper.Arguments arguments) {
                object invalid;
                object temp = new();
                switch (this.type) {
                    case "string": {
                        invalid = "";
                        temp = json.Element(this.name, (string) invalid);
                        arguments.Add(temp);
                        break;
                    }
                    case "number": {
                        invalid = -575756;
                        temp = json.Element(this.name, (long) invalid);
                        arguments.Add(temp);
                        break;
                    }
                    case "bool": {
                        invalid = false;
                        temp = json.Element(this.name, (bool) invalid);
                        arguments.Add(temp);
                        break;
                    }
                }
            }
        }

        public struct Enumerator : IEnumerable<schemaEl>, IEnumerable, IEnumerator<schemaEl>, IEnumerator, IDisposable {
            private readonly Schema _target;
            private int _position = -1;

            internal Enumerator(Schema schema) {
                this._target = schema;
            }

            private Enumerator GetEnumerator() {
                return this;
            }

            public schemaEl Current {
                get {
                    return this._target.Get(this._position);
                }
            }
            object IEnumerator.Current => this.Current;

            IEnumerator IEnumerable.GetEnumerator() {
                return this.GetEnumerator();
            }

            IEnumerator<schemaEl> IEnumerable<schemaEl>.GetEnumerator() {
                return this.GetEnumerator();
            }

            public bool MoveNext() {
                if (this._position < this._target.Count()) {
                    this._position++;
                    return true;
                }
                return false;
            }

            public void Reset() {
                this._position = -1;
            }

            public void Dispose() {
                this._position = this._target.Count() - 1;
            }
        }

        // attributes

		private readonly List<schemaEl> _schema = new List<schemaEl>();

        public void Add(schemaEl schema) { this._schema.Add(schema); }

        public int Count() { return this._schema.Count; }

        // functions

        public schemaEl Get(int index) {
            int i = 0;
            foreach (var schema in this._schema) {
                if (index == i) { return schema; }
                i++;
            }
            // return invalid
            return new schemaEl();
        }
        public schemaEl GetFirst(string name) {
            foreach (var schema in this._schema) {
                if (schema.name == name) { return schema; }
            }
            // return invalid
            return new schemaEl();
        }

        public Enumerator Enumerate() {
            return new Enumerator(this);
        }

        public override string? ToString() {
            string? res = null;
            if (this._schema != null) {
                res = "{ ";
                foreach (var item in this._schema) {
                    res += item.name + ": " + item.type + ", ";
                }
                res += " }";
            }
            return res;
        }

        private bool ToArgsCheck(object temp, object invalid) {
            return (temp != invalid);
        }

        public Helper.Arguments ToArgs(Helper.MyJsonObject json) {
            Helper.Arguments arguments = new Helper.Arguments();

            foreach (var schema in this.Enumerate()) {
                schema.AddArg(json, arguments);
            }

            return arguments;
        }
    }
}

