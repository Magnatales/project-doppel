using Code.Service;
using Godot;

namespace Code.Networking.Components;

public partial class UILabelBandwithOut : Label
{
    public override void _Process(double delta)
    {
        float value = -1;
        var networkService = Services.Get<INetworkService>();
        if (networkService == null)
        {
            Text = $"Upload: {value:F0}";
            return;
        }

        if (networkService.IsServer())
        {
            value = networkService.Server.GetLocalConnection().QuickStatus().OutBytesPerSec;
        }
        else
        {
            if (networkService.IsClientConnected())
                value = networkService.Client.Connection.QuickStatus().OutBytesPerSec;
        }

        if (value > 1000)
        {
            Text = $"Upload: {value / 1000:F2} KB/s";
        }
        else
        {
            Text = $"Upload: {value:F0} B/s";
        }
    }
}