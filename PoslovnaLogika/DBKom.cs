using System.Security.Cryptography;
using Isopoh.Cryptography.Argon2;
using StackExchange.Redis;

public class DBKom : IDisposable {
    
private IDatabase db;
private ConnectionMultiplexer redis;

public DBKom(string ip) {
    redis = ConnectionMultiplexer.Connect(ip);
    db = redis.GetDatabase();
}

private void DodajToken(string token,string username) {
    db.HashSet("ulogovanikorisnici",token,username);
}

private bool DodajKorisnika(string username,string hash) {
    return db.HashSet("korisnici",username,hash,When.NotExists,CommandFlags.DemandMaster);

}


public string vratiUsernameOdTokena(string token) {
     var korisnik = db.HashGet("ulogovanikorisnici",token,CommandFlags.DemandMaster);
     if(korisnik.IsNullOrEmpty) {
        return "";
   }

   return korisnik.ToString();
}


private string vratiPasswordHash(string username) {
     var korisnik = db.HashGet("korisnici",username,CommandFlags.DemandMaster);
     if(korisnik.IsNullOrEmpty) {
        return "";
   }
   return korisnik.ToString();
}

public string Login(string username, string password) {
    var hash = vratiPasswordHash(username);
    if(hash == "") {
        return "Greska: Nepostojeci korisnik";
    }
   if(!Argon2.Verify(hash,password)) {
        return "Greska: Netacna lozinka";
   }
   string token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
   DodajToken(token,username);
   return token;



    


}

public string Register(string username,string password) {


    if(password.Length < 5) {
        return "Kratka lozinka";
    }
    string hash = Argon2.Hash(password);
   
   if(!DodajKorisnika(username,hash)) {
    return "Korisnik sa tim korisnickim imenom vec postoji";
   }
   return "";

}
public Dictionary<string,int> VratiLeaderboard() {
    var lb = db.HashGetAll("leaderboard",CommandFlags.DemandMaster);
    return lb.ToDictionary(x => x.Name.ToString(), x=> int.Parse(x.Value.ToString()));
}

public void AzurirajLeaderboard(string username,int score) {
    db.HashSet("leaderboard",username,score,When.Always,CommandFlags.DemandMaster);
}

    public void Dispose()
    {
        redis.Dispose();
    }
}