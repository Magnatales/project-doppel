using System;

namespace Code.Service;

public interface ISteamService : IDisposable
{
    void TryConnectToSteam(int retryIntervalMilliseconds);
    event Action<SteamConnectionStatus> OnCheckSteamConnectionStatus;
    SteamConnectionStatus CurrentSteamConnectionStatus { get; }
}