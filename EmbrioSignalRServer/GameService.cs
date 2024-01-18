using System.Runtime.CompilerServices;
using System.Text.Json;
using aipsfaza2.Hubs;
using Microsoft.AspNetCore.SignalR;


public class GameService : BackgroundService
{
  
    private double ticks = 0;
    private readonly IHubContext<GameHub> _gameHub;

     private readonly RabbitMQKom rabbit;
    

        public GameService(RabbitMQKom rabbit,IHubContext<GameHub> gameHub) {
            
            this.rabbit = rabbit;
            _gameHub = gameHub;
            
            
            //Leaderboard.Set(JsonSerializer.Deserialize<Dictionary<string,int>>((JsonElement)lbs));

        }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
    
            var igra = new Igra();
       
       
      IgraSingleton.Set(igra);

      using var timer = new PeriodicTimer(TimeSpan.FromSeconds(Utils.tickrate));
      

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            ticks += Utils.tickrate;
            var (updated,created,destroyed) = IgraSingleton.Get().GetUpdates();
             if(!(destroyed.Count == 0 && updated.Count == 0 && created.Count == 0)) {
               
                 await _gameHub.Clients.All.SendAsync("gameTick",updated,created,destroyed);
                 IgraSingleton.Get().ClearUpdates(destroyed,updated,created);
            }
          
        
          
          if(ticks < 1f) {
            continue;
          }
            IgraSingleton.Get().obnoviHranu();
           
            
            
            var waitToRespawn = IgraSingleton.Get().igraci.Where(i => i.Value.RespawnTimer > 0);
            var timers = waitToRespawn.ToDictionary(i => i.Key, i=> i.Value.RespawnTimer);
            await _gameHub.Clients.All.SendAsync("RespawnTick",timers);
            foreach(var i in waitToRespawn) {
                i.Value.RespawnTimer--;
       
            }
            ticks = 0;
        
            



            

        }

         await Task.CompletedTask;
    }

 
}
