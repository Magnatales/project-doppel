using Code.Service;
using Godot;

namespace Code.Networking.Components;

public partial class UILabelBandwithIn : Label
{
    public override void _Process(double delta)
    {
        float value = -1;

        var networkService = Services.Get<INetworkService>();
        if (networkService == null)
        {
            Text = $"Download: {value:F0}";
            return;
        }

        if (networkService.IsServer())
        {
            value = networkService.Server.GetLocalConnection().QuickStatus().InBytesPerSec;
        }
        else
        {
            if(networkService.IsClientConnected())
                value = networkService.Client.Connection.QuickStatus().InBytesPerSec;
        }

        if (value > 1000)
        {
            Text = $"Download: {value /1000:F2} KB/s";
        }
        else
        { 
            Text = $"Download: {value:F0} B/s";
        }

    }
}