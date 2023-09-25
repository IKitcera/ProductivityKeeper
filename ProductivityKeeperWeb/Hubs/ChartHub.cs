using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ProductivityKeeperWeb.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductivityKeeperWeb.Hubs
{
    [Authorize]
    public class ChartHub : Hub
    {
        public static Dictionary<string, string> UnitConnectionsToUnits = new();
        private readonly IAuthService _authService;

        public ChartHub(IAuthService authService)
        {
            _authService = authService;
        }

        public override Task OnConnectedAsync()
        {
            // Replacing on the newest connection
            if (UnitConnectionsToUnits.ContainsKey(Context.User.Identity.Name))
            {
                UnitConnectionsToUnits[Context.User.Identity.Name] = Context.ConnectionId;
            }
            else
            {
                UnitConnectionsToUnits.Add(Context.User.Identity.Name, Context.ConnectionId);
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            UnitConnectionsToUnits.Remove(Context.User.Identity.Name);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
