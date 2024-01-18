using System.Diagnostics;
using System.Runtime.CompilerServices;
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

           

        }
        public override async Task OnConnectedAsync() {
           Clients.Caller.SendAsync("leaderboardUpdate",Leaderboard.Get());
        Clients.Caller.SendAsync("gameState",IgraSingleton.Get());
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
            if(!error.StartsWith("Greska:")) {
                if(!IgraSingleton.Get().igraci.ContainsKey(error)) {
                IgraSingleton.Get().EnterGame(error,username,(uint)System.Random.Shared.Next());
                }
            }
           
            await Clients.Caller.SendAsync("LoginResult",!error.StartsWith("Greska:"),error);

        }
        public async Task EnterGame(string token) {
           
           var krug = IgraSingleton.Get().Spawn(token);
          await Clients.Caller.SendAsync("enteredGame",krug);
          



        }
        public async Task LeaveGame(string token) {
       IgraSingleton.Get().LeaveGame(token);
            Task.Run(() => rabbit.RpcCall("logout",token));
            
        }
        public async Task Move(string token,float x,float y) {
            var skor = IgraSingleton.Get().MovePlayer(token,x,y);
            if(skor != -1) {
              
            Task.Run(() => { rabbit.CreateAndSendTo("skor",new Dictionary<string,object> {{"token",token},{"skor",skor}});});
            }
            }
           
            }
          

        }
       
        
        
    
