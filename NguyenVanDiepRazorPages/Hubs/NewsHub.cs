using Microsoft.AspNetCore.SignalR;

namespace NguyenVanDiepRazorPages.Hubs
{
    public class NewsHub : Hub
    {
        public async Task SendNewsUpdate(string action, string articleId, string title)
        {
            await Clients.All.SendAsync("ReceiveNewsUpdate", action, articleId, title);
        }

        public async Task RefreshNewsList()
        {
            await Clients.All.SendAsync("ReloadNewsData");
        }
    }
}
