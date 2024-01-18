using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;



public class Igrac {
    protected string _username;
    public string Username {
        get {
            return _username;
    } set{_username = value;}
    }
   
   
  

    public  uint Boja {get;set;}
    public  List<long> celije;
    

  
    public  int RespawnTimer {get;set;}

  
}