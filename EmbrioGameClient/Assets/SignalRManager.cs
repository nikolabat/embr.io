using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public static class SignalRManager {
    private static SignalR signalR = null;
    private static object mutex = new object();
    private static string signalR_url;

    public static string MyToken {get;set;} = "";

    


    private static void Init() {
        signalR_url =  File.ReadAllLines("ip.txt")[0];
        if(signalR == null) {
        lock( mutex) {
            if(signalR == null) {
                signalR = new SignalR();
                signalR.Init(signalR_url);
                signalR.Connect();
            }
        }
        }
        signalR.ConnectionClosed += (SignalR , ConnectionEventArgs) => {
            UnityEngine.Debug.Log("Ponovno povezivanje");
            lock(mutex) {
            signalR.Connect();
            signalR.Invoke("EnterGame",MyToken);
            }

        };
        }
    
    public static SignalR GetSignalR() {
        Init();
    
        return signalR;
}
}