using Microsoft.AspNetCore.SignalR;

namespace app.Hubs
{
    public class HospitalInfoHub : Hub
    {
        public async Task SendMessage(string hospitalPlaceId, string waitTime, string availableBeds)
        {
            await Clients.All.SendAsync("ReceiveMessage", hospitalPlaceId, waitTime, availableBeds);
        }
    }
}