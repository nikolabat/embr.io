using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class UI : MonoBehaviour
{
    
    public  TMP_InputField txt_username;
       public  TMP_InputField txt_password;

       public TMP_Text txt_leaderboard;
       public Button loginbtn;
        [SerializeField]
        private GameObject canvas;
        private bool destroyUi = false;
       public string token;
       
       private static SignalR signalR;
       private int skor = 0;

     
    // Start is called before the first frame update
    void Start()
    {
        //InvokeRepeating("simulirajSkor", 1.0f,3.0f);
        
        signalR = SignalRManager.GetSignalR();
        
           
             signalR.On("RegisterResult",(bool uspeh,string greska) => {
        if(uspeh) {
            Debug.Log("Registracija je uspela, mozete da se login");
        } else {
            Debug.Log("Registracija nije uspela " + greska);
        }
       });

        signalR.On("LoginResult",(bool uspeh,string token) => {
        if(uspeh) {
            Debug.Log(token + " loginovan");
            SignalRManager.MyToken = token;
            signalR.Invoke("EnterGame",token);

            destroyUi = true;
            
            

        } else {
            Debug.Log($"Login neuspesan: {token}");
        }
       });
       
        
        signalR.ConnectionStarted += (object sender,ConnectionEventArgs e) =>
        { 
            Debug.Log(string.Format("Uspostavljena veza sa serverom! ({0})",e.ConnectionId));
           

       };
       var btn = this.GetComponent<Button>();
       
       btn.onClick.AddListener(Register);
       loginbtn.onClick.AddListener(Login);
      
    }

  

    // Update is called once per frame
    void Update()
    {
    
      
     if(destroyUi) {
            destroyUi = false;
               Destroy(canvas);
        }
    }

    void Register() {
        string username = txt_username.text;
        string password = txt_password.text;

        if(username == string.Empty || password == string.Empty) {
             Debug.Log("Sva polja mora da budu popunjena!");
             return;
        }

        
       signalR.Invoke("Register",username,password);
    }

     void Login() {
        string username = txt_username.text;
        string password = txt_password.text;

        if(username == string.Empty || password == string.Empty) {
             Debug.Log("Sva polja mora da budu popunjena!");
             return;
        }

        
       signalR.Invoke("Login",username,password);
    }
}

