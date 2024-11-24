using Code.Service;
using Godot;

namespace Code.Networking.Components;

public partial class UILabelPing : Label
{
    public override void _Process(double delta)
    {
        float value = -1;

        var networkService = Services.Get<INetworkService>();
        if (networkService == null)
        {
            Text = $"Ping: {value:F0}";
            return;
        }

        if (networkService.IsServer())
        {
            value = networkService.Server.GetLocalConnection().QuickStatus().Ping;
        }
        else
        {
            if (networkService.IsClientConnected())
                value = networkService.Client.Connection.QuickStatus().Ping;
        }

        Text = $"Ping: {value} ms";
    }
}