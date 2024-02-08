using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerControls : MonoBehaviour

{
  
private TMP_Text coords;
[SerializeField]
private float brzina = 35f;
public string token = "";
    // Start is called before the first frame update
    void Start()
    {
     coords = GameObject.Find("coords").GetComponent<TMP_Text>();   
     coords.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        coords.text = $"X:{(int)this.gameObject.transform.position.x},Y:{(int)this.gameObject.transform.position.y},Skor:{(int)this.gameObject.transform.localScale.x}";
            float xdelta = 0;
              float ydelta = 0;
        if(Input.GetKey(KeyCode.W)) {
          ydelta = brzina * Time.deltaTime;
        }
         if(Input.GetKey(KeyCode.S)) {
           
            ydelta = -(brzina * Time.deltaTime);
     
        }
         if(Input.GetKey(KeyCode.A)) {
              xdelta = -(brzina * Time.deltaTime);
        }
         if(Input.GetKey(KeyCode.D)) {
             xdelta = brzina * Time.deltaTime;
       
           
        }
        if(Input.GetKeyUp(KeyCode.F2)) {coords.enabled = !coords.enabled;} 
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) {
        SignalRManager.GetSignalR().Invoke("Move",token,xdelta,ydelta);
        }
    }
}
