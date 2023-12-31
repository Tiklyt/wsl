﻿using System.Security.AccessControl;
using Microsoft.Extensions.Logging;

namespace MTCP_WSL2;


/// <summary>
/// Main class of the application that allow to enable MPTCP inside WSL2
/// </summary>
public class MPTCPEnabler
{
    private readonly HyperVManager _hyperVManager;
    private readonly NetworkMonitor _networkMonitor;
    private readonly WslAttacher _wslAttacher;

    /// <summary>
    /// Create an instance of MPTCP class 
    /// </summary>
    /// <param name="refreshDelay">delay in ms between each update of the network interfaces</param>
    public MPTCPEnabler(ILogger logger,CancellationToken token,int refreshDelay)
    {
        _hyperVManager = new HyperVManager(logger);
        _networkMonitor = new NetworkMonitor(logger,token,refreshDelay);
        _wslAttacher = new WslAttacher(logger,token);
        _networkMonitor.OnUpdate += async (sender, collectionUpdateEvent) =>
        {
            if (collectionUpdateEvent.Type == EventType.Add)
                await _hyperVManager.CreateHyperVSwitch(collectionUpdateEvent.InterfaceName);
                Console.WriteLine(collectionUpdateEvent.InterfaceName);
        };
        _hyperVManager.OnAdd += _wslAttacher.AddEvent;
    }
}