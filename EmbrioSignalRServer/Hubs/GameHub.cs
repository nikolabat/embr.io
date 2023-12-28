using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
namespace aipsfaza2.Hubs
{
    public class GameHub : Hub
    {
        private readonly RabbitMQKom rabbit;
        
        public GameHub(RabbitMQKom rabbit) {
            this.rabbit = rabbit;
            
          
            
        }
        public  override async Task OnDisconnectedAsync(Exception? exception) {
            Console.WriteLine("Disconnect:" + exception);

        }
        public override async Task OnConnectedAsync() {
            await Clients.Caller.SendAsync("leaderboardUpdate",Leaderboard.Get());
            await base.OnConnectedAsync();
        }
        public async Task Register(string username, string password)
        {
            string? error = rabbit.RpcCall("Register",username,password).ToString();
            if(error == null) {
                Console.WriteLine("Greska!");
            }
           
            await Clients.Caller.SendAsync("RegisterResult",error == "",error);
        }
     public async Task Login(string username, string password)
        { 
           
            string? error = rabbit.RpcCall("Login",username,password).ToString();
            if(error == null) {
                Console.WriteLine("Greska!");
                return;
            }
           
           
            await Clients.Caller.SendAsync("LoginResult",!error.StartsWith("Greska:"),error);

        }
        
         public async Task UpdateScore(string token, int score)
        {
          //   Console.WriteLine($"{token}:{score}");
          // Console.WriteLine(score);
                  
            rabbit.CreateAndSendTo("skor",new Dictionary<string,object> {{"token",token},{"skor",score}});
        
          //  Console.WriteLine($"{token}:{score}");
            await Task.FromResult(0);
        }
    }
}