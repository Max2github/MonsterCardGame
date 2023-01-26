using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardGame {
    namespace HTTP {
        // an abstract class for both HTTP requests and responses
        public abstract class Header {
            public String[] Headers;

            protected abstract string GetFirstLine();
            public abstract byte[]? GetData();

            public Header() {
                this.Headers = Array.Empty<string>();
            }

            // public functions
            public byte[] GetSerialized() {
                string allHeader = GetFirstLine();
                if (this.Headers.Length > 0) {
                    allHeader += string.Join("\r\n", this.Headers);
                }
                allHeader += "\r\n\r\n";
                // byte[] res = Encoding.ASCII.GetBytes(allHeader);
                byte[] res = Encoding.ASCII.GetBytes(allHeader);
                byte[]? data = this.GetData();
                if (data == null) { return res; }
                foreach (byte b in data) { res = res.Append(b).ToArray(); }
                return res;
            }

            // set headers
            public void SetHeader(string name, string value) {
                name = name.Replace(" ", null);
                value.Replace(" ", null);
                this.Headers = this.Headers.Append(name + ":" + value).ToArray();
            }
            public string GetHeader(string name) {
                foreach (String header in this.Headers) {
                    String[] spl = header.Split(':', 2);
                    bool comp = String.Equals(spl[0], name, StringComparison.OrdinalIgnoreCase);
                    if (comp) { return spl[1]; }
                }
                return "";
            }
        }

        // parse a HTTP request
        public class Request : Header {
            // private attributes
            public byte[]? Data { get; }

            public override byte[]? GetData() { return Data; }
            protected override string GetFirstLine() {
                return this.Method + " " + this.Path + " " + this.Query;
            }


            // info request
            public string Method { get; }
            public string Path { get; set; } // for redirecting, allow set
            public string Query { get; }

            // constructors / destructors
            // construct from input - for parsing requests
            public Request(ref byte[] data) {
                String dataStr = Encoding.ASCII.GetString(data);
                String[] lines = dataStr.Split("\r\n");

                // separate data and header
                if (lines.Last() == "") {
                    this.Data = null;
                } else {
                    this.Data = Encoding.ASCII.GetBytes(lines.Last());
                }
                lines = lines.SkipLast(1).ToArray();

                // parse first line
                String firstLine = lines[0];
                string[] info = firstLine.Split(" ");
                this.Method = info[0];
                if (info[1].Contains('?')) {
                    String[] pathAndQuery = info[1].Split("?", 2);
                    this.Path = pathAndQuery[0];
                    this.Query = pathAndQuery[1];
                }
                else {
                    this.Path = info[1];
                    this.Query = "";
                }
                lines = lines.Skip(1).ToArray();

                this.Headers = lines;

            }

            // create an invalid request
            public Request() {
                this.Data = null;
                this.Method = "";
                this.Path = "";
                this.Query = "";
            }
        }

        // create a HTTP response
        public class Response : Header {
            public enum Status_e {
                OK_200 = 200,
                CREATED_201 = 201,
                ACCEPTED_202 = 202,
                NO_CONTENT_204 = 204,

                BAD_REQUEST_400 = 400,
                UNAUTHORIZED_401 = 401,
                FORBIDDEN_403 = 403,
                NOT_FOUND_404 = 404,
                CONFLICT_409 = 409,

                INTERNAL_SERVER_ERROR_500 = 500,
                NOT_IMPLEMENTED_501 = 501,
            }
            
            public byte[]? Data { get; set; }
            private string _version = "1.1";
            private string _status = "200 OK";

            public override byte[]? GetData() { return Data; }
            protected override string GetFirstLine() {
                return "HTTP/" + this._version + " " + this._status + "\r\n";
            }

            // conbstructors
            public Response(string version = "1.1", string status = "200 OK") {
                this._version = version;
                this._status = status;
            }

            // private functions
            static private string StatusTable(Status_e status) {
                switch (status) {
                    case Status_e.OK_200: { return "200 OK"; }
                    case Status_e.CREATED_201: { return "201 CREATED"; }
                    case Status_e.ACCEPTED_202: { return "202 ACCEPTED"; }
                    case Status_e.NO_CONTENT_204: { return "204 NO CONTENT"; }

                    case Status_e.BAD_REQUEST_400: { return "400 BAD REQUEST"; }
                    case Status_e.UNAUTHORIZED_401: { return "401 UNAUTHORIZED"; }
                    case Status_e.FORBIDDEN_403: { return "403 FORBIDDEN"; }
                    case Status_e.NOT_FOUND_404: { return "404 NOT FOUND"; }
                    case Status_e.CONFLICT_409: { return "409 CONFLICT"; }

                    case Status_e.INTERNAL_SERVER_ERROR_500: { return "500 INTERNAL_SERVER_ERROR"; }
                    case Status_e.NOT_IMPLEMENTED_501: { return "501 NOT_IMPLEMENTED"; }
                }
                return "";
            }

            // public functions
            public string StatusFull() {
                return this._status;
            }
            public Status_e Status() {
                return (Status_e) UInt16.Parse(this._status.Substring(0, 3));
            }
            public void Status(Status_e status) {
                this._status = Response.StatusTable(status);
            }
            public void Status(ushort status) {
                this.Status((Status_e)status);
            }
        }

        
    }
}
