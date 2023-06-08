using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using ProductivityKeeperWeb.Domain;
using ProductivityKeeperWeb.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Threading.Tasks;

namespace ProductivityKeeperWeb.Hubs
{
    [Route("chart-hub")]
    [Authorize]
    public class ChartHub : Hub
    {
        public static Dictionary<string, int> UnitConnectionsToUnits = new();
        private readonly IAuthService _authService;

        public ChartHub(IAuthService authService)
        {
            _authService = authService;
        }

        public override Task OnConnectedAsync()
        {
            UnitConnectionsToUnits.Add(Context.ConnectionId, 0);
            return base.OnConnectedAsync();
        }


        public async Task<string> WaitForMessage(string connectionId)
        {
            var message = await Clients.Client(connectionId).InvokeAsync<string>(
                "GetMessage");
            return message;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            UnitConnectionsToUnits.Remove(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
