using System;
using FootBall;
using Fusion;

/// <summary>
/// Script for handling ready confirmation checks for all connected clients. Unique one exists for each client
/// </summary>
public class ReadyHandler : NetworkBehaviour
{
    /// <summary>
    /// Fired when local player ready state changes. bool for new state
    /// </summary>
    public static event Action<bool> OnLocalReadyStateChanged;
    public static event Action OnLocalReadyHandlerSpawned;
    public static event Action OnLocalReadyHandlerDespawned;

    [Networked]
    public bool Ready { get; set; } = false;
    private bool localReady = false;

    public override void Spawned()
    {
        if (HasInputAuthority)
        {
            OnLocalReadyHandlerSpawned?.Invoke();
            OnLocalReadyStateChanged?.Invoke(Ready);
        }
    }

    public override void FixedUpdateNetwork()
    {
        // Change ready state based on input, 
        // but let it only happen on server side because networked vars cannot be changed by clients 
        // and changing these clientside will lead to chaos 
        if (!HasStateAuthority) return;
        if (GetInput(out InputData data))
        {
            if (data.ReadyTriggered)
            {
                Ready = !Ready;
            }
        }
    }

    private void Update()
    {
        // Detect change clientside
        if (HasInputAuthority && localReady != Ready)
        {
            localReady = Ready;
            OnLocalReadyStateChanged?.Invoke(localReady);
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (HasInputAuthority)
            OnLocalReadyHandlerDespawned?.Invoke();
    }
}
