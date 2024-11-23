using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Steamworks;

namespace Code.Service;

public class SteamService : ISteamService
{
	public event Action<SteamConnectionStatus> OnCheckSteamConnectionStatus = delegate { };
	public SteamConnectionStatus CurrentSteamConnectionStatus { get; private set; } = SteamConnectionStatus.Closed;

	private readonly uint _appId;
	private CancellationTokenSource _cts;
	public SteamService(uint appId)
	{
		_appId = appId;
		_cts = new CancellationTokenSource();
		TryConnectToSteam(1000);
		CheckConnection(1000);
	}

	private async void CheckConnection(int checkIntervalMilliseconds)
	{
		try
		{
			while (!_cts.IsCancellationRequested)
			{
				await Task.Delay(checkIntervalMilliseconds, cancellationToken: _cts.Token);

				if (SteamClient.IsValid == false)
				{
					CurrentSteamConnectionStatus = SteamConnectionStatus.Closed;
				}
				else
				if (SteamClient.IsLoggedOn == false)
				{
					CurrentSteamConnectionStatus = SteamConnectionStatus.NotConnected;
				}
				else
					CurrentSteamConnectionStatus = SteamConnectionStatus.Ok;

				OnCheckSteamConnectionStatus(CurrentSteamConnectionStatus);
			}
		}
		catch(Exception e)
		{
			GD.Print(e);
		}
	}

	public async void TryConnectToSteam(int retryIntervalMilliseconds)
	{
		while (SteamClient.IsValid == false)
		{
			try
			{
				SteamClient.Init(_appId);
				GD.Print("Steam Client Successfully Initialized " + SteamClient.SteamId);
			}
			catch (Exception e)
			{
				GD.Print(e.Message);
			}

			await Task.Delay(retryIntervalMilliseconds);
		}

	}

	public void Dispose()
	{
		_cts?.Cancel();
		_cts?.Dispose();
	}
}
