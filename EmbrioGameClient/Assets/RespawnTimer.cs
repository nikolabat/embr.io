using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class RespawnTimer : MonoBehaviour
{

    private int timer = 0;
    private object timerlock = new object();
    private TMP_Text txt;
    // Start is called before the first frame update
    void Start()
    {
        SignalRManager.GetSignalR().On("RespawnTick",(Dictionary<string,int> timers) => {
            lock(timerlock) {
                //Debug.Log(timers.Keys);
                timers.TryGetValue(SignalRManager.MyToken,out timer);
            }
        });
        txt = this.gameObject.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        lock(timerlock) {
        if(timer != 0 && !txt.enabled) {
            txt.enabled = true;
        }
        txt.text = $"Ozivljavanje za {timer}";
        if(timer == 0 && txt.enabled) {
            txt.enabled = false;
            SignalRManager.GetSignalR().Invoke("EnterGame",SignalRManager.MyToken);
        }
        }   
    }
}
