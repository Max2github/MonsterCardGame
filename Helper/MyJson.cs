using System;
using System.Text.Json;

namespace MonsterCardGame.Helper {
    /**
     * Cannot be used from outside
     * */
    internal class MyJson {
        protected JsonValueKind _type;
        protected JsonElement _json;

        protected MyJson(string jsonStr) {
            try {
                this._json = JsonDocument.Parse(jsonStr).RootElement;
            } catch (JsonException) {
                throw;
            }
            this._type = this._json.ValueKind;

            if (this._type != JsonValueKind.Object) {
                /* some error */
            }
        }
        protected MyJson(JsonElement json) {
            this._json = json;
            this._type = this._json.ValueKind;

            if (this._type != JsonValueKind.Object) {
                /* some error */
            }
        }
    }
    /**
     * Only used for json objects
     * */
    internal class MyJsonObject : MyJson {
        public MyJsonObject(string jsonStr) : base(jsonStr) {

        }
        public MyJsonObject(JsonElement json) : base(json) {

        }

        public string Element(string name, string std = "") {
            JsonElement temp;
            if (this._json.TryGetProperty(name, out temp)) {
                string? erg = temp.GetString();
                if (erg != null) { return erg; }
                else { return std; }
            } else { return std; }
        }
        public string? Element(string name) {
            string ret = this.Element(name, "");
            return (ret == "") ? null : ret;
        }

        public long Element(string name, long std = 0) {
            long? erg = this.Element<long?>(name);
            return (erg == null) ? std : (long) erg;
        }

        public int Element(string name, int std = 0) {
            int? erg = this.Element<int?>(name);
            return (erg == null) ? std : (int) erg;
        }

        public bool Element(string name, bool std = false) {
            JsonElement temp;
            if (this._json.TryGetProperty(name, out temp)) {
                bool erg = temp.GetBoolean();
                return erg;
            } else { return std; }
        }

        public MyJsonArray? ArrayElement(string name) {
            JsonElement temp;
            if (this._json.TryGetProperty(name, out temp)) {
                if (temp.ValueKind == JsonValueKind.Array) {
                    return new MyJsonArray(temp);
                }
                return null;
            } else { return null; }
        }

        public MyJsonObject? ObjectElement(string name) {
            JsonElement temp;
            if (this._json.TryGetProperty(name, out temp)) {
                if (temp.ValueKind == JsonValueKind.Object) {
                    return new MyJsonObject(temp);
                }
                return null;
            } else { return null; }
        }

        public T? Element<T>(string name) {
            if (typeof(T) == typeof(string)) { return (T)(object) this.Element(name, ""); }
            JsonElement temp;
            if (typeof(T) == typeof(long) || typeof(T) == typeof(long?)) {
                if (this._json.TryGetProperty(name, out temp)) {
                    long? erg = temp.GetInt64();
                    return (T?)(object?) erg;
                }
                return (T?)(object?) null;
            }
            if (typeof(T) == typeof(int) || typeof(T) == typeof(int?)) {
                if (this._json.TryGetProperty(name, out temp)) {
                    int? erg = temp.GetInt32();
                    return (T?)(object?)erg;
                }
                return (T?)(object?) null;
            }
            if (typeof(T) == typeof(double) || typeof(T) == typeof(double?)) {
                if (this._json.TryGetProperty(name, out temp)) {
                    double? erg = temp.GetDouble();
                    return (T?)(object?)erg;
                }
                return (T?)(object?)null;
            }
            if (typeof(T) == typeof(bool)) { return (T)(object)this.Element(name, false); }
            return (T?)(object?) null;
        }
    }
    /**
     * Only used for json arrays
     * */
    internal class MyJsonArray : MyJson {
        public MyJsonArray(string jsonStr) : base(jsonStr) {

        }
        public MyJsonArray(JsonElement json) : base(json) {

        }

        private JsonElement? GetJsonElement(int index) {
            int i = 0;
            foreach (var element in this._json.EnumerateArray()) {
                if (i == index) { return element; }
                i++;
            }
            return null;
        }

        public string Element(int index, string std = "") {
            var el = this.GetJsonElement(index);
            if (el == null) { return std; }
            string? erg = el.Value.GetString();
            return (erg == null) ? std : erg;
        }
        public long Element(int index, long std = 0) {
            var el = this.GetJsonElement(index);
            if (el == null) { return std; }
            return el.Value.GetInt64();
        }
        public bool Element(int index, bool std = false) {
            var el = this.GetJsonElement(index);
            if (el == null) { return std; }
            return el.Value.GetBoolean();
        }

        public JsonElement.ArrayEnumerator Enumerate() {
            return this._json.EnumerateArray();
        }
    }
}

