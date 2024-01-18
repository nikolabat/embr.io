using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using RabbitMQ.Client;



public class Igrac {
    protected string _username;
    public string Username {
        get {
            return _username;
    } set{_username = value;}
    }
   
    [SetsRequiredMembers]
    public Igrac(string username, uint boja) {
        this._username = username;
        this.Boja = boja;
        this.RespawnTimer = 0;
        celija = -1;
        
       
    }
    
  

    public required uint Boja {get;set;}
    public required  long celija;
    

    public required int RespawnTimer {get;set;}

  
}