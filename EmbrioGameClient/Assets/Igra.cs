using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.SignalR;




public class Igra {
  

       public  Dictionary<string,Igrac> igraci {get;set;}
    
    private const int DUZINA_POLJA = 10000;
    private const int SIRINA_POLJA = 10000;
    private const int KOLICINA_HRANE_MAX = DUZINA_POLJA*SIRINA_POLJA/40000;
    private const int KOLICINA_ZAMKI =  DUZINA_POLJA*SIRINA_POLJA/640000;

    private int hranaRespawnTimer = 300;
    public Dictionary<long,Krug> entities {get;set;}

    private object movelock = new object();
    private int kolicinaHrane = 0;

   
}
               
            

        
        
    
        
        
      
        
        
    

   
 
