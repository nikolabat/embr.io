using System.Runtime.CompilerServices;
using System.Text.Json;
using aipsfaza2.Hubs;
using Microsoft.AspNetCore.SignalR;




public class LeaderboardService : BackgroundService
{
  
    
    private readonly IHubContext<GameHub> _gameHub;

     private readonly RabbitMQKom rabbit;

     private async void LeaderboardUpdates(Dictionary<string,object> novilb) {
            var lbkastovani = novilb.ToDictionary(x => x.Key.ToString(), x=> int.Parse(x.Value.ToString()));
            Leaderboard.Set(lbkastovani);
             await _gameHub.Clients.All.SendAsync("leaderboardUpdate",Leaderboard.Get());
           
           
     }

        public LeaderboardService(RabbitMQKom rabbit,IHubContext<GameHub> gameHub) {
            
            this.rabbit = rabbit;
            _gameHub = gameHub;
            Console.WriteLine("Gettting leaderboards from database");
            var lbs = rabbit.RpcCall("getLeaderboard");

            
            Leaderboard.Set(JsonSerializer.Deserialize<Dictionary<string,int>>((JsonElement)lbs));
            Console.WriteLine("OK");

        }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
       rabbit.CreateAndListen("leaderboardUpdates",LeaderboardUpdates);
      await Task.CompletedTask;
    }
}
