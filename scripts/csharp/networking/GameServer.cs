using System.Collections.Generic;
using System.Threading.Tasks;
using Code.References;
using Godot;
using Steamworks;
using Steamworks.Data;

namespace Code.Networking;

public class GameServer : BaseServer
    {
        //public GameState gameState;
        private GameReferences _gameReferences;

        public List<Player> _players = new List<Player>();

        private List<ulong> banned;

        public static ulong SteamId
        {
            get
            {
                if (SteamClient.IsValid == false)
                    return default(ulong);

                return SteamClient.SteamId;
            }
        }
        
        

        public void Init(GameReferences gameReferences)
        {
            base.Initialize();
            _gameReferences = gameReferences;
        }

        public async void CreateNewPlayer(SteamId steamId)
        {
            // ObjectState player = gameState.Server_CreateObjectStateAndPawn((uint)steamId, ResourceId.PawnPlayer, "", steamId, true);
            
            var player = _gameReferences.playerScene.Instantiate<Player>();
            Friend playerSteam = new Friend(steamId);
            await playerSteam.RequestInfoAsync();
            player.SetPawn((uint)playerSteam.Id, playerSteam.Id, playerSteam.Name);
            player.Name = playerSteam.Name;
            _gameReferences.playerSpawnPoint.GetTree().Root.AddChild(player);
            _players.Add(player);
            GD.Print(playerSteam);
            // player.GetPawn().SetName(playerSteam.Name);

            //UINotifications.Instance.PushNotification($"Instantiated Pawn for player {playerSteam.Name}");
        }

        public override void OnConnectionChanged(Connection connection, ConnectionInfo info)
        {
            base.OnConnectionChanged(connection, info);

            //UINotifications.Instance.PushNotification($"{info.identity.SteamId} - {info.State}");

            if (info.State == ConnectionState.Connected)
            {
                connection.UserData = (long)info.identity.SteamId.Value;
                CreateNewPlayer(info.identity.SteamId);
            }

            if (info.state == ConnectionState.None)
            {
                //gameState.RemoveFromGameState((uint)info.identity.SteamId);
            }

            //LobbyManager.Instance.SetPlayersCount(Connected.Count);
        }

        protected override bool CanConnect(Connection connection, ConnectionInfo info)
        {
            var maxPlayers = 4;

            if (Connected.Count >= maxPlayers)
            {
                return false;
            }

            return true;

            if (banned == null)
            {
                // if (SimpleSaveSystem.Load(SimpleSaveSystem.BAN_LIST_PATH, out var banListAsVariant))
                // {
                //     banned = GodotCollectionsUtilities.GetList(banListAsVariant.AsGodotArray<ulong>());
                // }
            }

            if (IsBanned(info.identity.steamid))
            {
                return false;
            }

            return true;
        }

        public void Ban(SteamId steamId)
        {
            if (banned.Contains(steamId))
                return;

            banned.Add(steamId.Value);

            //SimpleSaveSystem.Save(GodotCollectionsUtilities.GetGodotArray(banned), SimpleSaveSystem.BAN_LIST_PATH);

            Kick(steamId);
        }

        public void Unban(SteamId steamId)
        {
            if (banned.Contains(steamId) == false)
                return;

            banned.Remove(steamId.Value);
            //SimpleSaveSystem.Save(GodotCollectionsUtilities.GetGodotArray(banned), SimpleSaveSystem.MUTE_LIST_PATH);
        }

        public bool IsBanned(SteamId steamId)
        {
            if (banned == null)
                return false;

            return banned.Contains(steamId);
        }

        public override void Tick(ulong tick)
        {
            base.Tick(tick);
        }

        public void Kick(SteamId playerId)
        {
            foreach (var connection in Connected)
            {
                if (connection.UserData == (long)playerId.Value)
                {
                    connection.Close();
                }
            }
        }

        public void KickAll()
        {
            foreach (var connection in Connected)
            {
                connection.Close();
            }
        }

        public int GetPing(ulong steamId)
        {
            foreach (var connection in Connected)
            {
                if (connection.UserData == (long)steamId)
                {
                    return connection.QuickStatus().Ping;
                }
            }

            return -1;
        }

        public async Task<string> GetName(ulong steamId)
        {
            var friend = new Friend(steamId);
            await friend.RequestInfoAsync();
            return friend.Name;
        }

        public void Shutdown()
        {
            KickAll();
            //LobbyManager.Instance.LobbyLocalhost.Leave();
        }
    }