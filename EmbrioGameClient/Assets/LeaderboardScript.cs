using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardScript : MonoBehaviour
{
    [SerializeField]
    private TMP_Text txt;
     private Dictionary<string,int> leaderboard = new Dictionary<string, int>();
    // Start is called before the first frame update
    void Start()
    {
       
        SignalRManager.GetSignalR().On("leaderboardUpdate",(Dictionary<string,int> leaderboard) => {
            lock(leaderboard) {
            this.leaderboard = leaderboard;
            }});
    }

    // Update is called once per frame
    void Update()
    {
         txt.text = "";
            foreach( var highscore in leaderboard) {
                
               txt.text += highscore.Key + ":" + highscore.Value + "\n";
       
            
               
            
    }
    }
}
