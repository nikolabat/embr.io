using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using aipsfaza2.Hubs;
using Microsoft.AspNetCore.SignalR;




public class Igra {
  

       public required Dictionary<string,Igrac> igraci {get;set;}
    private long CreateEntity(Point position, int r, uint boja, KrugTip tip,string label = "") {
        long id = System.Random.Shared.NextInt64();
        Krug k = new Krug(id,position,r,boja,tip,label);
        lock(entities) {
            entities[id] = k;
        }
       lock(created) {
        created.Add(k);

       }
        return id;
    }
        private void DestroyEntity(long entityId) {
       
        lock(entities) {
            entities.Remove(entityId);
        }
        lock(destroyed) {
            destroyed.Add(entityId);
        }
        

    }
    public void UpdateEntity(Krug k) {
        lock(entities) {
            if(!entities.ContainsKey(k.EntityID)) {return;}
            entities[k.EntityID] = k;
            
        }
        lock(updated) {
            updated.Add(k);
        }
       

}

    private const int DUZINA_POLJA = 10000;
    private const int SIRINA_POLJA = 10000;
    private const int KOLICINA_HRANE_MAX = DUZINA_POLJA*SIRINA_POLJA/6000;
    private const int KOLICINA_ZAMKI =  DUZINA_POLJA*SIRINA_POLJA/18400;

    private int hranaRespawnTimer = 0;
    public Dictionary<long,Krug> entities {get;set;}

    private object movelock = new object();
    private int kolicinaHrane = 0;

        private HashSet<Krug> updated;
        private HashSet<Krug> created;
        private HashSet<long> destroyed;

    public (HashSet<Krug>,HashSet<Krug>,HashSet<long>) GetUpdates() {
        HashSet<Krug> update;
        HashSet<Krug> create;
        HashSet<long> destroy;
          lock(created) {create = created.ToHashSet();}
               lock(destroyed) {destroy = destroyed.ToHashSet(); }
        lock(updated) { update    = updated.ToHashSet(); }

          
           
                 
        
        

      
       
                
            
        
       
        return (update,create,destroy);
    }
    public void ClearUpdates(HashSet<long> des, HashSet<Krug> upd, HashSet<Krug> cre) {
        lock(created) {
        lock(destroyed) {
        lock(updated) {
            updated.ExceptWith(upd);
            destroyed.ExceptWith(des);
            created.ExceptWith(cre);
        }
        }
        }
    }
    public HashSet<Krug> obnoviHranu() {
        HashSet<Krug> obnovljeni = new HashSet<Krug>();
        if(hranaRespawnTimer > 0) {
            hranaRespawnTimer--;
            return obnovljeni;
        }

         uint boja = Utils.napraviBoju(0,116,226);
          for(int i = 0;i<KOLICINA_HRANE_MAX-kolicinaHrane;i++) {
            float y = System.Random.Shared.NextSingle()*DUZINA_POLJA;
            float x = System.Random.Shared.NextSingle()*SIRINA_POLJA;
            Point poz = new Point(x,y);
            var id = CreateEntity(poz,1,boja,KrugTip.Hrana);
            obnovljeni.Add(entities[id]);
        }
        kolicinaHrane = KOLICINA_HRANE_MAX;
        hranaRespawnTimer = 300;
        return obnovljeni;
    }
    public void napraviZamke() {
        uint boja = Utils.napraviBoju(0,255,0);
        for(int i = 0;i<KOLICINA_ZAMKI;i++) {
                 float y = System.Random.Shared.NextSingle()*DUZINA_POLJA;
            float x = System.Random.Shared.NextSingle()*SIRINA_POLJA;
           
            Point poz = new Point(x,y);
           CreateEntity(poz,10,boja,KrugTip.Zamka);
        }
    }
    
   
    [SetsRequiredMembers]
    public Igra()
    {  
        
        entities = new Dictionary<long, Krug>();
        igraci = new Dictionary<string, Igrac>();
        updated = new HashSet<Krug>();
        created = new HashSet<Krug>();
        destroyed = new HashSet<long>();
        obnoviHranu();
        napraviZamke();
      
    }
    public void EnterGame(string token,string username,uint boja) {
            igraci[token] = new Igrac(username,boja);
         
            

    }
    public Krug Spawn(string token) {
        Igrac i = null;

        if(igraci.TryGetValue(token, out i)) {
            if(i.celija != -1) {
                return entities[i.celija];
            }
            if(i.RespawnTimer > 0) {
                return null;
            }
            var id = CreateEntity(new Point(System.Random.Shared.NextSingle()*1000,System.Random.Shared.NextSingle()*1000),5,i.Boja,KrugTip.Igrac,i.Username);
            i.celija = id;
            return entities[id];

        }
        return null;

    }
    public long LeaveGame(string token) {
        if(!igraci.ContainsKey(token)) {return -1;}
        var id = igraci[token].celija;
        lock(igraci) {
            
            DestroyEntity(igraci[token].celija);
             
        }
        igraci.Remove(token);
        return id;
       
      
        }
    
   public int MovePlayer(string token,float x,float y) 
   {
    int skor = -1;
     
    
    if(!igraci.ContainsKey(token)) {
        return skor;
    }
    var id = igraci[token].celija;
    if(id == -1) {
        return skor;
    }
    x = Math.Clamp(x,-0.5f,0.5f);
    y = Math.Clamp(y,-0.5f,0.5f);
    var celija =  entities[id];
    float newx = celija.Position.X + x;
    float newy = celija.Position.Y + y;
    
    
    if (newx + celija.R / 2 >= 10000) {
        newx = 10000 - celija.R / 2;
    }
     
    if(newy + celija.R / 2 >= 10000) {
        newy = 10000 - celija.R / 2;
    }
    
     if(newx - celija.R / 2 <= 0) {
        newx = celija.R / 2;
     }

     if(newy - celija.R / 2 <= 0) {
        newy = celija.R / 2;
     }
     
    celija.Position = new Point(newx,newy);
   
    UpdateEntity(celija);
    lock(entities) {
    foreach(Krug k in entities.Values) {
        if(k == celija) {
            continue;
        }
        if(Utils.SeceSe(k,celija)) {

            switch(k.tip) {
            case KrugTip.Hrana:
                celija.R += k.R;
                skor = celija.R;
              
                DestroyEntity(k.EntityID);
                UpdateEntity(celija);
             
                
                break;
            case KrugTip.Igrac:

            if(celija.R > k.R ) {
                celija.R += k.R;
                
                DestroyEntity(k.EntityID);
                lock (igraci) {
                    Igrac i = igraci.Where(i => k.EntityID == i.Value.celija).First().Value;
                    i.celija = -1;
                    i.RespawnTimer = 5;
                }
                skor = celija.R;

                UpdateEntity(celija);
                
            } else {
             
                k.R += celija.R;
                DestroyEntity(celija.EntityID);
                 lock (igraci) {
                    Igrac i = igraci.Where(i => celija.EntityID == i.Value.celija).First().Value;
                    i.celija = -1;
                    i.RespawnTimer = 5;
                }

                UpdateEntity(k);
                
            }
            break;
            case KrugTip.Zamka:
            if(celija.R > k.R) {
              
                celija.R -= 2;
                UpdateEntity(celija);
               
            }
            break;


        }

    }
 
   
   }
   }
  return skor;
}
}
               
            

        
        
    
        
        
      
        
        
    

   
 
