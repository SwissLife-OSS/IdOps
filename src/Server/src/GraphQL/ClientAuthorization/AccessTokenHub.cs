using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.SignalR;

namespace IdOps;

public class AccessTokenHub : Hub
{
    
    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }
    
    public Task SendToken(string token, string connectionId) =>
        Clients.Client(connectionId).SendAsync("TokenReceived", token);
}
