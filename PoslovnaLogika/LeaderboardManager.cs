using System.Text.Json;

public class LeaderboardManager : IDisposable {
    private RabbitMQKom rabbit;
    private DBKom db;

    private Dictionary<string,int> leaderboard;
    private object _getLeaderboard(object[] args) {
        return leaderboard;
    }
   private void updateSkor(Dictionary<string,object> args) {
    
    string? token = args["token"].ToString();
    
    if(token == null) {
        return;
    }
    string username = db.vratiUsernameOdTokena(token);
    
    int skor = JsonSerializer.Deserialize<int>((JsonElement)args["skor"]);
    
    int sacuvani_skor = 0;
    leaderboard.TryGetValue(username,out sacuvani_skor);
        
    
    
    if(sacuvani_skor > skor) {
        return;
    }
    leaderboard[username] = skor;

    db.AzurirajLeaderboard(username,skor);
   
   
    rabbit.CreateAndSendTo("leaderboardUpdates",leaderboard,1);

    
    
        
   }
    public LeaderboardManager(RabbitMQKom r, DBKom d) {
        rabbit = r;
        db = d;
        leaderboard = db.VratiLeaderboard();
        rabbit.CreateAndSendTo("leaderboardUpdates",leaderboard,1);
    }

    public void Dispose()
    {
        rabbit.Dispose();
        db.Dispose();
    }

    public Task ManageLeaderboard() {
        return Task.Run(() => {
           rabbit.CreateAndListen("skor",updateSkor);
           rabbit.RegisterRpc("getLeaderboard",_getLeaderboard);
        });
    }
}