using System;
using System.Text;
using MonsterCardGame.HTTP.Routing;

namespace MonsterCardGame.HTTP.Routing {
    /**
     * A general Router class, which should handle incoming HTTP requests.
     * */
	internal interface IRouter {
        HTTP.Response HandleRequest(HTTP.Request request);
	}
    /**
     * This Router class should handle different Actions.
     * The shema of incoming data and the action to take should be described in actions.json.
     * */
    internal class ActionRouter : IRouter {
        private readonly User.IUserManager _userCollection;
        private readonly Card.ICardManager _cardCollection;
        private readonly Card.Package.IPackageManager _packageManager;
        private readonly Action.Actions _ACTION;

        public ActionRouter(
            User.IUserManager userCollection,
            Card.ICardManager cardCollection,
            Card.Package.IPackageManager packageManager,
            string file = "actions.json"
        ) {
            Console.Write($"Reading actions from {file}... ");
            this._ACTION = new Action.Actions(file);
            Console.WriteLine("Done.");
            this._userCollection = userCollection;
            this._cardCollection = cardCollection;
            this._packageManager = packageManager;
        }

        public HTTP.Response HandleRequest(HTTP.Request request) {
            Response response = new();
            response.Status(Response.Status_e.BAD_REQUEST_400);

            // get route
            Action.route toDeliver = this._ACTION.Get(request.Method, request.Path);
            // toDeliver.Print();

            // invalid
            if (!toDeliver.IsValid()) {
                response.Status(Response.Status_e.NOT_FOUND_404);
                return response;
            }
            // redirect
            if (toDeliver.redirect) {
                request.Path = toDeliver.action;
                return this.HandleRequest(request);
            }
            // normal request
            // get command
            Action.Command.ICommand? command = Action.Command.ICommand.CreateCommandByName(this._userCollection, this._cardCollection, this._packageManager, toDeliver.action);
            if (command == null) {
                // something went wrong - on config error (in actions.json); command not implemented (yet)
                response.Status(Response.Status_e.NOT_IMPLEMENTED_501);
                return response;
            }

            // get command args
            Action.RequestParser requestParser = new(request, toDeliver);
            Helper.Arguments arguments = new();
            foreach (var argId in toDeliver.actionArgIdList) {
                requestParser.AddArg(argId, arguments);
            }

            // check auth
            toDeliver.auth.SetUser(Auth.GetUser(this._userCollection, request));
            if (!toDeliver.auth.Accept(request)) {
                response.Status(Response.Status_e.UNAUTHORIZED_401);
                return response;
            }

            // execute command
            /*bool success =*/ command.Execute(arguments, response);
            // if (request.Data != null) { }

            return response;
        }
    }
    /**
     * This Router should only handl GET request targeting a file.
     * It needs a routes.json file, which mapps the paths to the files.
     * */
    internal class FileRouter : IRouter {
        private readonly Routes _GET;

        public FileRouter(string file = "routes.json")  {
            Console.Write($"Reading routes from {file}... ");
            this._GET = new Routes(file);
            Console.WriteLine("Done.");
        }

        public HTTP.Response HandleRequest(HTTP.Request request) {
            Response response = new();

            if (request.Method != "GET") {
                // this router can only handle GET requests
                return response;
            }
            
            Routes.route toDeliver = this._GET.Get(request.Path);
            if (!toDeliver.IsValid()) {
                string responseData = $"404 Not Found. Route \"{request.Path}\" is missing or invalid!";
                response.Data = Encoding.ASCII.GetBytes(responseData);
                response.Data = Encoding.Convert(Encoding.ASCII, Encoding.UTF8, response.Data);

                response.SetHeader("Connection", "Close");
                response.SetHeader("Content-Type", "text/plain; charset=UTF-8");

                response.Status(Response.Status_e.NOT_FOUND_404);

                return response;
            }
            if (toDeliver.redirect) {
                request.Path = toDeliver.file;
                return this.HandleRequest(request);
            }
            response.Data = File.ReadAllBytes(Routes.Dir + toDeliver.file);

            response.SetHeader("Connection", "Close");
            response.SetHeader("Content-Type", toDeliver.type + "; charset=UTF-8");

            response.Status(Response.Status_e.OK_200);

            return response;
        }
    }
}

