using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace IdOps
{
    public class OpsHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
    }
}
