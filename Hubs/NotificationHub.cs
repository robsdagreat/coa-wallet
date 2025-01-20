using Microsoft.AspNetCore.SignalR;

namespace coa_Wallet.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendBudgetAlert(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveBudgetAlert", message);
        }
    }
}