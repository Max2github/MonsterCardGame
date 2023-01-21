using System.Text;
using MonsterCardGame;
using MonsterCardGame.Helper;
using MonsterCardGame.Card;
using MonsterCardGame.HTTP;
using MonsterCardGame.HTTP.Routing;
using MonsterCardGame.User;

// See https://aka.ms/new-console-template for more information

// Collection allUsers = new Collection();

string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=mtcgdb";
UserDB allUsers = new UserDB(connectionString);

Admin adminUser = new Admin("admin", "admin", "Admin");
allUsers.Add(adminUser);

Console.WriteLine(allUsers.Count());

/**
 * Allow authentification with tokens with the following form:
 * Authorization: Bearer <user>-mtcgToken
 * */
Auth.AllowUnsafeToken = true;
Auth.Header = "Authorization";
Auth.Prefix = "Bearer ";
Auth.Suffix = "";
Auth.UnsafeToken_Prefix = "";
Auth.UnsafeToken_Suffix = "-mtcgToken";

FileRouter fileRouter = new FileRouter("routes.json");
ActionRouter actionRouter = new ActionRouter(allUsers, "actions.json");

Server serv = new Server(actionRouter);
serv.Start(10001);
