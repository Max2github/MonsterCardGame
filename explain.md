# Monster Trading Card Game  (MTCG)

## status

### working
- user management
- packages
- HTTP server in itself (selfmade!!!!!!)

### may / should somehow work
- deck
- battle (dependent on deck)

The code is there, but it may or may not work
perfectly -> needs some debugging

### missing
- trade
- get all cards (from current user)
- scoreboard
- stats

## how to use

### start & stop DB
In the directory "DB" are the scripts
"start.sh" and "stop.sh".
If docker needs sudo on your system, you need to call them with sudo.

`start.sh` will pull a docker image if it is not already downloaded, just to inform you.

### testing
I left the official shell script with the curl commands 
in here (unchanged - MonsterTradingCards.sh).

I also made a nicer, more comfortable script `test.sh`, which just tells you `Ok` or `FAIL` and what HTTP status code would have been expected. Not all curl requests were modified.

## architecture
- HTTP
    - server - listening, receiving, sending
    - HTTP   - parse HTTP protocol
    - Routing/Router - define FileRouter & ActionRouter
    - Routing/Routes - routes for the FileRouter
    - Routing/Action - routes for the ActionRouter
- DB - general Database parts
- Card - card management (DB and in memory)
    - Battle
    - Cards
    - Packages
    - Deck
- User - user management (DB and in memory)
- Helper - some general Helper classes (e.g. Arguments, MyJson)

The Routers both work based on json config files with a specifi syntax.
For the `ActionRouter` this is the `actions.json` and for the `FileRouter` the `routes.json`.

## technical steps
1. own HTTP server
    --- early (before we did our server in class - MessageServer)
    1. server itself (listening, etc.)
    2. "routes" (routes.json) for files -> can deliver files
    3. "actions" (actions.json) for sppecific commands
    --- while we developed our server in class
    4. move "routes" and "actions" in router classes
       (before nearly everything was in the server)
    --- while we finished our server in class + after we finished it
    5. debug + expand parsing and setup from actions.json
    --- after our server in class was finished + a lot very late
    6. add specific commands (API routes / "actions")
2. Models + intern in memory logic
    I did a little bit everywere (Cards, Battle, Users, Battle logic, ...),
    but nothing finished. I began with this rather early, while we were still talking about UMLs, but never spent too much time on it, so I never finished anything.
3. Database
    1. Users (in time)
    2. Cards (late)
    3. Packages (very late)
    4. Deck (now, the day it is due)

## time management
I began very early with the HTTP server, then for a long time I did nearly nothing and only recently I did much.
At the end I had serious time problems (you should be able to  see that from my status)

## unit tests
I chose the classes for the unit test based on the following criteria:
- often used (important)
- in the core of the project (important)
- little & easy (time!!!)

Anyway the unit test do not work on my computer, because Visual Studio on MacOS does not support them and on my virtual Maschine with Windows I get a timeout problem, which tells me that my PC is too slow (Visual Studio in the VM with C# is already slow, so it does not surprise me).

Thus I could neither use or test the unit tests :)

## link to git
https://github.com/Max2github/MonsterCardGame