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
    
       public string token;
       
       private static SignalR signalR;
       private int skor = 0;
       private Dictionary<string,int> leaderboard = new Dictionary<string, int>();
       void simulirajSkor() {
        if(token == "") {
            return;
        }
        skor += 5;
            Debug.Log("update");
             signalR.Invoke("UpdateScore",token,skor);
        
       
       }
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("simulirajSkor", 1.0f,3.0f);
        
        signalR = SignalRManager.GetSignalR();
        signalR.On("leaderboardUpdate",(Dictionary<string,int> leaderboard) => {
            lock(leaderboard) {
            this.leaderboard = leaderboard;
            }});
           
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
            this.token = token;
            
            

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
    
       txt_leaderboard.text = "";
            foreach( var highscore in leaderboard) {
                
               txt_leaderboard.text += highscore.Key + ":" + highscore.Value + "\n";
               
            
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

