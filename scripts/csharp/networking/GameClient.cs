using System;
using Code.Service;
using Steamworks;
using Steamworks.Data;

namespace Code.Networking;

public class GameClient : BaseClient
{
    public static event Action<ulong> OnClientConnected;
    public override void OnConnectionChanged(ConnectionInfo info)
    {
        base.OnConnectionChanged(info);
        
        if (info.State == ConnectionState.Connected)
        {
            OnClientConnected?.Invoke(info.identity.SteamId.Value);
        }
        
        if(info.State == ConnectionState.None || info.State == ConnectionState.ClosedByPeer || info.State == ConnectionState.Dead || info.State == ConnectionState.ProblemDetectedLocally)
        {
            
            // if(info.State == ConnectionState.None)
            //     UINotifications.Instance.PushNotification("Disconnected", UINotifications.NotificationType.Error);
            //
            // if (info.State == ConnectionState.ClosedByPeer)
            //     UINotifications.Instance.PushNotification("You have been kicked", UINotifications.NotificationType.Error);
            //
            // if (info.State == ConnectionState.Dead)
            //     UINotifications.Instance.PushNotification("Server died", UINotifications.NotificationType.Error);
            //
            // if (info.State == ConnectionState.ProblemDetectedLocally)
            //     UINotifications.Instance.PushNotification("ProblemDetectedLocally", UINotifications.NotificationType.Error);
            
            Services.Get<INetworkService>().ClientDisconnect();
        }
    }
}