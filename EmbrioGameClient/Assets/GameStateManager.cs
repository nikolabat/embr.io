using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using System.Linq.Expressions;
using TMPro;


public class GameStateManager : MonoBehaviour
{
   
    public static Color32 Uint2Color(uint aCol)
{   
    Color32 c = new Color32();
    c.b = (byte)((aCol) & 0xFF);
    c.g = (byte)((aCol>>8) & 0xFF);
    c.r = (byte)((aCol>>16) & 0xFF);
    c.a = 255;
    return c;
}
    private SignalR signalR {
        get {
           return SignalRManager.GetSignalR();
        }
    }
    [SerializeField]
    private GameObject krugPrefab;
  
    private string MyToken; 
      
  
      [SerializeField]
    private GameObject kamera;
    [SerializeField]
    private GameObject label;
    private Dictionary<long,GameObject> krugovi = new Dictionary<long, GameObject>();
    private Igra igra = null;
    private object igralock = new object();
    private HashSet<Krug> update =  new HashSet<Krug>();
     private HashSet<Krug> create =  new HashSet<Krug>();
     private HashSet<long> destroy = new HashSet<long>();
     private Krug igrac = null;
     private object igraclock = new object();
     
   
       
    public void Start() {
        
        signalR.On("gameTick",(HashSet<Krug> updated,HashSet<Krug> created,HashSet<long> destroyed) => {
            try {
           lock(update) {update.UnionWith(updated);}
            lock(create) {create.UnionWith(created);}
            lock(destroy) { destroy.UnionWith(destroyed);}
            
           
            
            
           
    
        
        }
        catch (System.Exception e)
        {
            
            Debug.LogException(e);
        }


       });

      

        signalR.On("gameState",(Igra state) => {
           
        try
        {
            lock(igralock) {
            igra = state;
            }
    
        
        }
        catch (System.Exception e)
        {
            
            Debug.LogException(e);
        }

        
       });

       signalR.On("enteredGame", (Krug i) => {
            lock(igraclock) {
                igrac = i;
            }
       });


    }
    private void napraviKrug(Krug create) {
        if(krugovi.ContainsKey(create.EntityID)) {
            return;
        }
          var krug =  Instantiate(krugPrefab,new Vector3(create.Position.X,create.Position.Y,0),Quaternion.identity);
                   
                     krug.GetComponent<SpriteRenderer>().color = Uint2Color(create.Boja);
                    krug.transform.localScale = new Vector3(1,1,0) * create.R;
                    lock(krugovi) {
                    krugovi[create.EntityID] =  krug;
                    }
                    if(create.tip == KrugTip.Igrac) {
                        var txt = Instantiate(label,krug.transform);
                        var lbl = txt.GetComponent<TMP_Text>();
                        lbl.alignment =  TextAlignmentOptions.Center;
                        lbl.text = create.Label;
                    } 
                    krug.name = create.EntityID.ToString();
    }
    public void Update() {

         lock(igralock) {
        if(igra != null) {

            foreach(GameObject krug in krugovi.Values) {
                if(krug.transform.childCount == 2 ) {
                        krug.transform.DetachChildren();
                    }
                Destroy(krug);
            }
            foreach(Krug entity in igra.entities.Values) {
                napraviKrug(entity);
            }
            igra = null;
        }
         }
         lock(create) {
             foreach(var c in create) {
              //  Debug.Log("create");
                napraviKrug(c);
            }
            create.Clear();
         
         }

        lock(destroy) {
            
            foreach(var d in destroy) {
                try {
            //Debug.Log($"destroy {d}");
                 var krug = krugovi[d];
                    if(krug.transform.childCount == 2 ) {
                        kamera.transform.parent = null;
                    }
                    if(krugovi.ContainsKey(d)) {
                    Destroy(krug);
                    krugovi.Remove(d);
                    
                    }
                } catch(Exception e) {
                    Debug.LogException(e);
                }
             

                   
            }
               destroy.Clear();
          
         }

         lock(update) {foreach(var up in update) {
                if(!krugovi.ContainsKey(up.EntityID)) {continue;}
               var krug = krugovi[up.EntityID];
                    krug.transform.localScale = new Vector3(1,1,0) * up.R;
                    krug.transform.position = new Vector3(up.Position.X,up.Position.Y,0);
                    
                   
            }
            update.Clear();
            }
         
           

          
             lock(igraclock) {
                if(igrac != null) {
                  if(krugovi.ContainsKey(igrac.EntityID)) {
                 

                  
                  var krug = krugovi[igrac.EntityID];
                  krug.AddComponent<PlayerControls>().token = SignalRManager.MyToken;
                  kamera.transform.parent = krug.transform;
                  kamera.GetComponent<Camera>().nearClipPlane = -1;
                  kamera.transform.localPosition = new Vector3(0,0,-10);
                  igrac = null;
                  

                }
               
            }
             }
       
       
            
       


}
public void OnApplicationQuit() {
    SignalRManager.GetSignalR().Invoke("LeaveGame",SignalRManager.MyToken);
}
    }
    
