using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatusText : MonoBehaviour
{
    // Start is called before the first frame update
    TMP_Text txt;
    string str = "";
    void Start()
    {
        txt = GetComponent<TMP_Text>();
    }

    // Update is called once per frame

    public void SetText(string text) {
        lock(str) {
            str = text;
            lock(txt) {
                
            }
        }
    }
  public  void Update()
    {
        lock(txt) {
            if(str != "") {
            txt.text = str;
            txt.color = Color.black;
            str = "";
            }
      
        }
        
          txt.color = Color.Lerp(Color.black,Color.clear,Mathf.PingPong(Time.time,1.0f));
        
    }
}
