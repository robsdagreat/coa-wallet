using Microsoft.AspNetCore.SignalR;
using coa_Wallet.Hubs; // Add this namespace for NotificationHub

namespace coa_Wallet.Services
{
    public class NotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendBudgetAlert(string userId, decimal currentAmount, decimal budgetLimit)
        {
            var percentageUsed = (currentAmount / budgetLimit) * 100;
            var message = $"Budget Alert: You've used {percentageUsed:F1}% of your budget!";
            await _hubContext.Clients.User(userId).SendAsync("ReceiveBudgetAlert", message);
        }
    }
}
