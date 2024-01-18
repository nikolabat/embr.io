using StackExchange.Redis;
using Tommy;





string RABBITMQ_IP = "";
string RABBITMQ_USER = "";
string RABBITMQ_PASSWORD = "";

string REDIS_IP = "";




Dictionary<string,int> leaderboard = new Dictionary<string, int>();
#region InitConfig
void InitConfig() {
    try {
    var toml = File.OpenText("conf.toml");
    TomlTable table = TOML.Parse(toml);
    RABBITMQ_IP = table["rabbitmq"]["ip"].AsString;
    RABBITMQ_USER = table["rabbitmq"]["username"].AsString;
    RABBITMQ_PASSWORD = table["rabbitmq"]["password"].AsString;
    REDIS_IP = table["redis"]["ip"].AsString;

} catch(Exception ex) {
    Console.WriteLine($"Greska u ucitavanju konfiguracije! {ex.Message}");

}
}
#endregion

InitConfig();
//var db = new DBKom(REDIS_IP);
//leaderboard = db.VratiLeaderboard();
Console.WriteLine(RABBITMQ_IP);
var rabbit = new RabbitMQKom(RABBITMQ_IP);
DBKom db = new DBKom(REDIS_IP);
AccountManager accountManager = new AccountManager(rabbit,db);
LeaderboardManager leaderboardManager = new LeaderboardManager(rabbit,db);
accountManager.ManageAccounts();
leaderboardManager.ManageLeaderboard();



await Task.Delay(Timeout.Infinite).ConfigureAwait(false);
