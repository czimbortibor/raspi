using Microsoft.AspNetCore.SignalR;

namespace RaspiInterface.Server.Hubs
{
    public class Interface : Hub
    {
        public async Task NewMessage(string message)
        {
            await Clients.All.SendAsync("NewMessage", message);
        }
    }
}
