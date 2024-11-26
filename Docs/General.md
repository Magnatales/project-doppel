# General Architecture

## Introduction
Doppel is a game with multiplayer capabilities, to achieve so and ease the single player experience the core will be designed following a client<>server approach.

## Setup

### The Server
1. Data caching: Unless we detect memory issues, we should load all the data at first. This means parsing JSONs (or whatever format we choose) and storing them in memory for: skills, items, monsters, classes, maps, etc...
2. Initialize the Game instance. No need to load maps, monster, players, etc... unless we have a player connection, but the game should be ready to receive them.
3. Initialise networking: Open the socket, create the lobby, await player connections
4. On player connection: Load the map on spawn point if needed, spawn the monsters, spawn the player.
5. On player disconnection: Remove the player from the map, remove the player from the game, dispose the map if needed.


### The Client
1. Initialize Steam. 
2. Data caching: Same as the server, the client should have all the data ready to use. That data should be in a proper abstraction so in the case of the Host is shared across server and client instances.
3. Load the player, which should be stored locally (and synced through Steam Cloud).
4. Connect to the lobby.
5. Await for player spawn (server should spawn the player on the map and authority given to the client).