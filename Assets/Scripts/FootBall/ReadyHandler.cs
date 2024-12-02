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

    [Networked]
    public bool Ready { get; set; } = false;
    private bool localReady = false;

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
}
