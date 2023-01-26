using System.Text;
using MonsterCardGame;
using MonsterCardGame.Helper;
using MonsterCardGame.HTTP;
using MonsterCardGame.HTTP.Routing;
using MonsterCardGame.User;
using MonsterCardGame.Card;
using MonsterCardGame.Card.Package;

// See https://aka.ms/new-console-template for more information

// Collection allUsers = new Collection();

string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=mtcgdb";
IUserManager allUsers = new UserDB(connectionString);
ICardManager allCards = new CardDB(connectionString);
IPackageManager allPackages = new PackageDB(allCards, connectionString);

Admin adminUser = new Admin("admin", "istrator", "Administrator");
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

IRouter fileRouter = new FileRouter("routes.json");
IRouter actionRouter = new ActionRouter(allUsers, allCards, allPackages, "actions.json");

Server serv = new Server(actionRouter);
serv.Start(10001);
